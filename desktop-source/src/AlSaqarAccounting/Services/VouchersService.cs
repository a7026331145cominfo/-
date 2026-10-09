using System.Data;
using System.Data.SqlClient;
using AlSaqarAccounting.Core;

namespace AlSaqarAccounting.Services;

/// <summary>
/// Operational vouchers service (سندات القبض والصرف). A voucher is written
/// through the original two-step contract of GTSdb2026:
/// 1) dbo.Insert_Tran_Tran — inserts the header and returns the new serial
///    through the @Transn OUTPUT parameter.
/// 2) dbo.INSERT_Tran_TranDetails — inserts every debit/credit line bound to
///    the serial returned by step 1.
/// Deletion goes through dbo.Delete_Tran_Tran, listing through
/// dbo.Select_SearchAccountReceipt. No ad-hoc table writes anywhere.
/// </summary>
public sealed class VouchersService
{
    private readonly DbExecutor _db;
    private readonly AuthorizationService _authorization;

    public VouchersService(DbExecutor db)
    {
        _db = db;
        _authorization = new AuthorizationService(db);
    }

    internal string ConnectionString => _db.ConnectionString;

    public Task<DataTable> ListAsync(int? branchId, CancellationToken cancellationToken = default)
        => ExecuteBranchProcedureAsync("dbo.Select_SearchAccountReceipt", branchId, cancellationToken);

    /// <summary>Types of vouchers (سند قبض / سند صرف ...) from Tran_Type.</summary>
    public Task<DataTable> ListTranTypesAsync(CancellationToken cancellationToken = default)
        => _db.QueryAsync(
            "SELECT ID, Name FROM dbo.Tran_Type ORDER BY ID;",
            cancellationToken: cancellationToken);

    /// <summary>Chart-of-accounts list for the account picker.</summary>
    public Task<DataTable> ListAccountsAsync(CancellationToken cancellationToken = default)
        => _db.ExecuteStoredProcedureAsync("dbo.Select_SearchAccount", cancellationToken: cancellationToken);

    /// <summary>Saves a balanced voucher (سند) through the original procedures
    /// and returns the new voucher serial.</summary>
    public async Task<int> CreateAsync(
        Voucher voucher,
        AppSession session,
        CancellationToken cancellationToken = default)
    {
        if (voucher is null)
            throw new ArgumentNullException(nameof(voucher));
        if (!session.BranchId.HasValue)
            throw new InvalidOperationException("حفظ السندات يتطلب فرعاً فعّالاً.");
        if (voucher.TranTypeId <= 0)
            throw new ArgumentException("يجب اختيار نوع السند.");
        if (!voucher.Lines.Any())
            throw new ArgumentException("لا يمكن حفظ سند بدون تفاصيل.");

        foreach (var line in voucher.Lines)
        {
            if (line.AccountSn <= 0)
                throw new ArgumentException("كل سطر في السند يحتاج إلى حساب صحيح.");
            if (line.Debit < 0 || line.Credit < 0)
                throw new ArgumentException("قيم المدين والدائن لا يمكن أن تكون سالبة.");
            if (line.Debit == 0 && line.Credit == 0)
                throw new ArgumentException("كل سطر يجب أن يحتوي على مبلغ مدين أو دائن.");
            if (line.Debit > 0 && line.Credit > 0)
                throw new ArgumentException("لا يمكن أن يكون السطر مديناً ودائناً في نفس الوقت.");
        }

        var totalDebit = voucher.Lines.Sum(l => l.Debit);
        var totalCredit = voucher.Lines.Sum(l => l.Credit);
        if (totalDebit != totalCredit)
            throw new ArgumentException(
                $"السند غير متوازن: إجمالي المدين {totalDebit:N2} وإجمالي الدائن {totalCredit:N2}.");

        var branchId = session.BranchId.Value;

        // The header and every detail row must commit or roll back together.
        // Keep the reference-code range locked while the header is assigned to
        // avoid duplicate numbering when two users save vouchers concurrently.
        using (var connection = new SqlConnection(_db.ConnectionString))
        {
            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
            using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    int referenceCode;
                    using (var referenceCommand = new SqlCommand(
                        @"SELECT ISNULL(MAX(ReferenceCode), 0) + 1
                          FROM dbo.Tran_Tran WITH (UPDLOCK, HOLDLOCK)
                          WHERE BranchID = @BranchID AND TranTypeID = @TranTypeID;",
                        connection,
                        transaction))
                    {
                        referenceCommand.CommandTimeout = 60;
                        referenceCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;
                        referenceCommand.Parameters.Add("@TranTypeID", SqlDbType.Int).Value = voucher.TranTypeId;
                        var value = await referenceCommand.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                        referenceCode = Convert.ToInt32(value);
                    }

                    int serial;
                    using (var headerCommand = new SqlCommand("dbo.Insert_Tran_Tran", connection, transaction))
                    {
                        headerCommand.CommandType = CommandType.StoredProcedure;
                        headerCommand.CommandTimeout = 60;
                        headerCommand.Parameters.Add("@ReferenceCode", SqlDbType.Int).Value = referenceCode;
                        headerCommand.Parameters.Add("@ProjectId", SqlDbType.Int).Value =
                            (object?)voucher.ProjectId ?? DBNull.Value;
                        var transn = headerCommand.Parameters.Add("@Transn", SqlDbType.Int);
                        transn.Direction = ParameterDirection.Output;
                        headerCommand.Parameters.Add("@TranTypeID", SqlDbType.Int).Value = voucher.TranTypeId;
                        headerCommand.Parameters.Add("@DocCode", SqlDbType.NVarChar, 100).Value =
                            (object?)NullIfEmpty(voucher.DocCode) ?? referenceCode.ToString();
                        headerCommand.Parameters.Add("@TranDate", SqlDbType.DateTime).Value = voucher.VoucherDate;
                        headerCommand.Parameters.Add("@Note", SqlDbType.NVarChar, 400).Value =
                            (object?)NullIfEmpty(voucher.Note) ?? DBNull.Value;
                        headerCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;
                        headerCommand.Parameters.Add("@UserID_Add", SqlDbType.Int).Value = session.UserId;
                        headerCommand.Parameters.Add("@UserBranch_Add", SqlDbType.Int).Value = branchId;
                        headerCommand.Parameters.Add("@UserMacAddress_Add", SqlDbType.NVarChar, 200).Value =
                            Environment.MachineName;

                        await headerCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        if (transn.Value is null or DBNull)
                            throw new InvalidOperationException(
                                "لم يُرجع الإجراء رقم السند الجديد (@Transn).");

                        serial = Convert.ToInt32(transn.Value);
                    }

                    var index = 0;
                    foreach (var line in voucher.Lines)
                    {
                        index++;
                        using (var detailCommand = new SqlCommand(
                            "dbo.INSERT_Tran_TranDetails", connection, transaction))
                        {
                            detailCommand.CommandType = CommandType.StoredProcedure;
                            detailCommand.CommandTimeout = 60;
                            detailCommand.Parameters.Add("@TranSn", SqlDbType.Int).Value = serial;
                            detailCommand.Parameters.Add("@Account_Sn", SqlDbType.Int).Value = line.AccountSn;
                            detailCommand.Parameters.Add("@AccounIindex", SqlDbType.Int).Value = index;
                            detailCommand.Parameters.Add("@TranDesc", SqlDbType.NVarChar, 400).Value =
                                (object?)NullIfEmpty(line.Description) ?? DBNull.Value;
                            detailCommand.Parameters.Add("@Debit", SqlDbType.Decimal).Value = line.Debit;
                            detailCommand.Parameters.Add("@Credit", SqlDbType.Decimal).Value = line.Credit;
                            detailCommand.Parameters.Add("@CostCentersID", SqlDbType.Int).Value =
                                (object?)line.CostCenterId ?? DBNull.Value;
                            detailCommand.Parameters.Add("@BranchID", SqlDbType.Int).Value = branchId;

                            await detailCommand.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                        }
                    }

                    transaction.Commit();
                    return serial;
                }
                catch
                {
                    try { transaction.Rollback(); }
                    catch { /* Preserve the original SQL exception. */ }
                    throw;
                }
            }
        }
    }

    /// <summary>Deletes a voucher through the original delete procedure
    /// (header + details are removed by the procedure itself).</summary>
    public async Task DeleteAsync(
        int referenceCode,
        AppSession session,
        int screenId,
        int tranTypeId,
        CancellationToken cancellationToken = default)
    {
        if (!session.BranchId.HasValue)
            throw new InvalidOperationException("حذف السندات يتطلب فرعاً فعّالاً.");

        await _authorization.RequireAsync(
            session,
            screenId,
            PermissionAction.Delete,
            cancellationToken).ConfigureAwait(false);

        await _db.ExecuteStoredProcedureNonQueryAsync(
            "dbo.Delete_Tran_Tran",
            p =>
            {
                p.Add("@ReferenceCode", SqlDbType.Int).Value = referenceCode;
                p.Add("@BranchID", SqlDbType.Int).Value = session.BranchId.Value;
                p.Add("@TranTypeID", SqlDbType.Int).Value = tranTypeId;
            },
            cancellationToken).ConfigureAwait(false);
    }

    private Task<DataTable> ExecuteBranchProcedureAsync(
        string procedureName,
        int? branchId,
        CancellationToken cancellationToken)
    {
        return _db.ExecuteStoredProcedureAsync(
            procedureName,
            p =>
            {
                if (!branchId.HasValue)
                    throw new InvalidOperationException("هذه الشاشة تحتاج إلى فرع فعّال.");
                p.Add("@BranchID", SqlDbType.Int).Value = branchId.Value;
            },
            cancellationToken);
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

/// <summary>In-memory voucher (سند قبض / صرف) submitted by the entry screen.</summary>
public sealed class Voucher
{
    public int TranTypeId { get; set; }
    public string? DocCode { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.Now;
    public string? Note { get; set; }
    public int? ProjectId { get; set; }

    public List<VoucherLine> Lines { get; } = new();
}

/// <summary>One debit/credit line of a voucher.</summary>
public sealed class VoucherLine
{
    public int AccountSn { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public int? CostCenterId { get; set; }
}


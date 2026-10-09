using System.Data;
using System.Data.SqlClient;
using AlSaqarAccounting.Core;

namespace AlSaqarAccounting.Services;

/// <summary>Live dashboard metrics read from the operational SQL Server tables.</summary>
public sealed class DashboardService
{
    private readonly DbExecutor _db;

    public DashboardService(DbExecutor db) => _db = db;

    public async Task<DashboardMetrics> GetMetricsAsync(
        int? branchId,
        CancellationToken cancellationToken = default)
    {
        const string sql = @"
SELECT
    SalesTotal = ISNULL((SELECT SUM(ISNULL(Net, 0))
        FROM dbo.Order_Orders
        WHERE OrderCashierType = 0
          AND Purchases_Date >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
          AND (@BranchID IS NULL OR BranchID = @BranchID OR BranchID IS NULL)), 0),
    PurchaseTotal = ISNULL((SELECT SUM(ISNULL(Net, 0))
        FROM dbo.Order_Purchases
        WHERE Purchases_Date >= DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)
          AND (@BranchID IS NULL OR BranchID = @BranchID OR BranchID IS NULL)), 0),
    SalesCount = ISNULL((SELECT COUNT(1)
        FROM dbo.Order_Orders
        WHERE OrderCashierType = 0
          AND (@BranchID IS NULL OR BranchID = @BranchID OR BranchID IS NULL)), 0),
    PurchaseCount = ISNULL((SELECT COUNT(1)
        FROM dbo.Order_Purchases
        WHERE (@BranchID IS NULL OR BranchID = @BranchID OR BranchID IS NULL)), 0),
    ItemCount = ISNULL((SELECT COUNT(1) FROM dbo.Item_Items), 0),
    StockQuantity = ISNULL((SELECT SUM(ISNULL(q.CurrentBalance, 0))
        FROM dbo.ItemQuantity AS q
        INNER JOIN dbo.Account_Stores AS s ON s.ID = q.StoreID
        WHERE @BranchID IS NULL OR s.BranchID = @BranchID OR s.BranchID IS NULL), 0),
    CustomerCount = ISNULL((SELECT COUNT(1) FROM dbo.Account_CustSup WHERE IsCustomers = 1), 0),
    SupplierCount = ISNULL((SELECT COUNT(1) FROM dbo.Account_CustSup WHERE IsSuppliers = 1), 0),
    DebitTotal = ISNULL((SELECT SUM(ISNULL(d.Debit, 0))
        FROM dbo.Tran_TranDetails AS d
        INNER JOIN dbo.Tran_Tran AS h ON h.ID = d.TranSn AND h.BranchID = d.BranchID
        WHERE @BranchID IS NULL OR h.BranchID = @BranchID), 0),
    CreditTotal = ISNULL((SELECT SUM(ISNULL(d.Credit, 0))
        FROM dbo.Tran_TranDetails AS d
        INNER JOIN dbo.Tran_Tran AS h ON h.ID = d.TranSn AND h.BranchID = d.BranchID
        WHERE @BranchID IS NULL OR h.BranchID = @BranchID), 0);";

        var table = await _db.QueryAsync(sql, p =>
        {
            p.Add("@BranchID", SqlDbType.Int).Value = (object?)branchId ?? DBNull.Value;
        }, cancellationToken).ConfigureAwait(false);

        if (table.Rows.Count == 0)
            return new DashboardMetrics();

        var row = table.Rows[0];
        return new DashboardMetrics
        {
            SalesTotal = Decimal(row, "SalesTotal"),
            PurchaseTotal = Decimal(row, "PurchaseTotal"),
            SalesCount = Int(row, "SalesCount"),
            PurchaseCount = Int(row, "PurchaseCount"),
            ItemCount = Int(row, "ItemCount"),
            StockQuantity = Decimal(row, "StockQuantity"),
            CustomerCount = Int(row, "CustomerCount"),
            SupplierCount = Int(row, "SupplierCount"),
            DebitTotal = Decimal(row, "DebitTotal"),
            CreditTotal = Decimal(row, "CreditTotal")
        };
    }

    private static decimal Decimal(DataRow row, string column) =>
        row[column] == DBNull.Value ? 0m : Convert.ToDecimal(row[column]);

    private static int Int(DataRow row, string column) =>
        row[column] == DBNull.Value ? 0 : Convert.ToInt32(row[column]);
}

public sealed class DashboardMetrics
{
    public decimal SalesTotal { get; init; }
    public decimal PurchaseTotal { get; init; }
    public int SalesCount { get; init; }
    public int PurchaseCount { get; init; }
    public int ItemCount { get; init; }
    public decimal StockQuantity { get; init; }
    public int CustomerCount { get; init; }
    public int SupplierCount { get; init; }
    public decimal DebitTotal { get; init; }
    public decimal CreditTotal { get; init; }
    public decimal LedgerDifference => DebitTotal - CreditTotal;
}

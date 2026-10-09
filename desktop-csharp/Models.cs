using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AlSaqarERP.Desktop;

public sealed class AppSettings
{
    public string Server { get; set; } = "DESKTOP-KBU5DH6";
    public string Database { get; set; } = "GTSdb2026";
    public bool IntegratedSecurity { get; set; } = true;
    public string UserName { get; set; } = "";
    public string EncryptedPassword { get; set; } = "";

    [JsonIgnore]
    public string Password
    {
        get
        {
            if (string.IsNullOrEmpty(EncryptedPassword)) return "";
            try
            {
                var bytes = Convert.FromBase64String(EncryptedPassword);
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser));
            }
            catch { return ""; }
        }
        set
        {
            if (string.IsNullOrEmpty(value)) { EncryptedPassword = ""; return; }
            EncryptedPassword = Convert.ToBase64String(
                ProtectedData.Protect(Encoding.UTF8.GetBytes(value), null, DataProtectionScope.CurrentUser));
        }
    }

    [JsonIgnore]
    public string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "AlSaqarERP", "desktop-settings.json");

    public string BuildConnectionString()
    {
        var b = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder
        {
            DataSource = string.IsNullOrWhiteSpace(Server) ? "DESKTOP-KBU5DH6" : Server.Trim(),
            InitialCatalog = string.IsNullOrWhiteSpace(Database) ? "GTSdb2026" : Database.Trim(),
            IntegratedSecurity = IntegratedSecurity,
            TrustServerCertificate = true,
            Encrypt = true,
            ConnectTimeout = 8,
            ApplicationName = "AlSaqarERP CSharp Desktop"
        };
        if (!IntegratedSecurity)
        {
            b.UserID = UserName;
            b.Password = Password;
        }
        return b.ConnectionString;
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static AppSettings Load()
    {
        var settings = new AppSettings();
        try
        {
            if (File.Exists(settings.SettingsPath))
                settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(settings.SettingsPath)) ?? settings;
        }
        catch { }
        return settings;
    }
}

public sealed record ErpScreen(
    string Key, string Title, string Category, string Description,
    string[] Tables, bool IsReport = false, bool AllowMasterDataEdit = false);

public static class ErpScreenCatalog
{
    public static readonly IReadOnlyList<ErpScreen> Screens =
    [
        new("dashboard", "لوحة تشغيل الصقر ERP", "الرئيسية", "ملخص مباشر من قاعدة GTSdb2026.", ["dbo.Account_Accounts","dbo.Account_CustSup","dbo.Item_Items","dbo.Order_Orders","dbo.Order_Purchases"]),
        new("accounting", "مركز المحاسبة", "المحاسبة", "دليل الحسابات والقيود وكشوف الحساب.", ["dbo.Account_Accounts","dbo.Tran_Tran","dbo.Account_CustSup"], AllowMasterDataEdit: true),
        new("vouchers", "السندات المالية", "السندات", "سندات القبض والصرف والشيكات.", ["dbo.Account_Receipts","dbo.Account_ReceiptsDetails","dbo.Tran_Tran","dbo.Tran_TranDetails"]),
        new("sales", "فواتير المبيعات", "المبيعات", "عرض فواتير البيع وتفاصيلها من قاعدة البيانات.", ["dbo.Order_Orders","dbo.Order_OrdersDetails","dbo.Order_OrderReturn","dbo.Order_OrderReturnDetails"]),
        new("purchases", "فواتير المشتريات", "المشتريات", "عرض فواتير الشراء وتفاصيلها من قاعدة البيانات.", ["dbo.Order_Purchases","dbo.Order_PurchasesDetails","dbo.Order_PurchasesReturn","dbo.Order_PurchasesReturnDetails"]),
        new("inventory", "مركز المخزون والأصناف", "المخزون والأصناف", "الأصناف والوحدات والمجموعات والأرصدة والتحويلات.", ["dbo.Item_Items","dbo.Item_Unit","dbo.Item_Groups","dbo.Item_Class","dbo.Order_OpenQuantity","dbo.Order_StoreTransfer"], AllowMasterDataEdit: true),
        new("domains", "مراكز وعمليات", "المراكز والعمليات", "مراكز التكلفة والعقود والموظفون.", ["dbo.Account_CostCenters","dbo.Account_Projects","dbo.Contract_Contract","dbo.Emp_Employee"], AllowMasterDataEdit: true),
        new("reports", "التقارير", "التقارير", "تنفيذ إجراءات التقارير المسموح بها واستعراض النتائج.", [], true),
        new("security", "الأمان والصلاحيات", "الأمان والصلاحيات", "مراجعة المستخدمين والمجموعات ومصفوفة الصلاحيات.", ["dbo.User_Login","dbo.User_Groups","dbo.User_Screens","dbo.User_Permission"]),
        new("schema", "مستكشف بنية قاعدة البيانات", "ربط وتشغيل النظام", "استعراض الجداول والحقول والأنواع والمفاتيح.", []),
        new("links", "ربط وتشغيل النظام بالكامل", "ربط وتشغيل النظام", "كتالوج الشاشات والجداول والإجراءات المتاحة.", []),
        new("connection", "إعداد الاتصال", "ربط وتشغيل النظام", "إعداد واختبار الاتصال بخادم SQL Server.", [])
    ];

    public static readonly string[] KnownTables =
    [
        "dbo.Account_Accounts",
        "dbo.Account_AccountType",
        "dbo.Account_Branch",
        "dbo.Account_CostCenters",
        "dbo.Account_CustSup",
        "dbo.Account_CustTailor",
        "dbo.Account_DefualtAccount",
        "dbo.Account_DefualtAccount2",
        "dbo.Account_DefualtCustomer",
        "dbo.Account_Final",
        "dbo.Account_Nature",
        "dbo.Account_Place",
        "dbo.Account_Projects",
        "dbo.Account_ProjectsDetails",
        "dbo.Account_Receipts",
        "dbo.Account_ReceiptsDetails",
        "dbo.Account_SalesMan",
        "dbo.Account_Stores",
        "dbo.Account_Suspended",
        "dbo.Account_Type",
        "dbo.AccountStartBalance",
        "dbo.AccountYearEndClosing",
        "dbo.AccountYears",
        "dbo.AgeingAnalysis",
        "dbo.App_FiscalYearState",
        "dbo.Car_Receive",
        "dbo.CashReceiptsType",
        "dbo.Contract_Bounce",
        "dbo.Contract_Contract",
        "dbo.Contract_ContractDetails",
        "dbo.Contract_EJAR",
        "dbo.Contract_Files",
        "dbo.Contract_Guarantee",
        "dbo.Contract_GuaranteeDetails",
        "dbo.Contract_Store",
        "dbo.Emp_Allowances",
        "dbo.Emp_Employee",
        "dbo.Emp_Entrusted",
        "dbo.Emp_EntrustedEmployee",
        "dbo.Emp_EntrustedEmployeeDetails",
        "dbo.Emp_Payroll",
        "dbo.Emp_PayrollDetails",
        "dbo.Emp_PayrollTemp",
        "dbo.Emp_PayrollTempDetails",
        "dbo.EmployeeAllowances",
        "dbo.Halls",
        "dbo.Item_Add",
        "dbo.Item_Class",
        "dbo.Item_Company",
        "dbo.Item_Country",
        "dbo.Item_Doctor",
        "dbo.Item_Groups",
        "dbo.Item_Guarantee",
        "dbo.Item_ItemComponent",
        "dbo.Item_Items",
        "dbo.Item_Type",
        "dbo.Item_Unit",
        "dbo.ItemQuantity",
        "dbo.Items_ContentAlignment_ForCashier",
        "dbo.LoginLogs",
        "dbo.Order_CheckoutOrders",
        "dbo.Order_CheckoutOrdersDetails",
        "dbo.Order_Extension",
        "dbo.Order_Gard",
        "dbo.Order_GardDetails",
        "dbo.Order_InventorySettlementMinus",
        "dbo.Order_InventorySettlementMinusDetails",
        "dbo.Order_Manufacturing",
        "dbo.Order_ManufacturingDetailsItems",
        "dbo.Order_ManufacturingDetailsNew",
        "dbo.Order_ManufacturingOrder",
        "dbo.Order_ManufacturingOrderDetails",
        "dbo.Order_MoveHall",
        "dbo.Order_OpenQuantity",
        "dbo.Order_OpenQuantityDetails",
        "dbo.Order_OrderReturn",
        "dbo.Order_OrderReturnDetails",
        "dbo.Order_Orders",
        "dbo.Order_OrdersDetails",
        "dbo.Order_OrdersDetailsDraft",
        "dbo.Order_OrdersDraft",
        "dbo.Order_PaymentItem",
        "dbo.Order_PaymentItemDetails",
        "dbo.Order_PriceOffer",
        "dbo.Order_PriceOfferDetails",
        "dbo.Order_Purchases",
        "dbo.Order_PurchasesDetails",
        "dbo.Order_PurchasesOrder",
        "dbo.Order_PurchasesOrderDetails",
        "dbo.Order_PurchasesReturn",
        "dbo.Order_PurchasesReturnDetails",
        "dbo.Order_RecItem",
        "dbo.Order_RecItemDetails",
        "dbo.Order_Reservation",
        "dbo.Order_Reservations",
        "dbo.Order_StoreTransfer",
        "dbo.Order_StoreTransferDetails",
        "dbo.Order_Transfer_Master",
        "dbo.Order_Transfer_MasterDetails",
        "dbo.Order_Transfer_StatusType",
        "dbo.Order_Transfer_TrackingType",
        "dbo.Order_TransferFromBranch",
        "dbo.Order_TransferFromBranchDetails",
        "dbo.Order_TransferToBranch",
        "dbo.Order_TransferToBranchDetails",
        "dbo.Order_TypeElectronicInvoice",
        "dbo.OrderDalala_Dalala",
        "dbo.OrderDalala_DalalaDetails",
        "dbo.OrderDalala_Sending",
        "dbo.OrderDalala_SendingDetails",
        "dbo.OrderRent_Recipt",
        "dbo.OrderRent_Recipt3",
        "dbo.OrderRent_RecpitDetails",
        "dbo.OrderRent_RecpitDetails3",
        "dbo.OrderRent_Rent",
        "dbo.OrderRent_RentDetails",
        "dbo.Orders_PriceShowRental",
        "dbo.Orders_RentalInvoice",
        "dbo.OrdersReturn_RentalInvoice",
        "dbo.Ordert_PriceShowRentalDetails",
        "dbo.Ordert_RentalInvoiceDetails",
        "dbo.OrdertReturn_RentalInvoiceDetails",
        "dbo.PriceOffer_Reservations",
        "dbo.Printers",
        "dbo.PrintersCook",
        "dbo.RebuildAllIndexes",
        "dbo.RentalExpenses",
        "dbo.RentalExpensesDetails",
        "dbo.Repairs_Groups",
        "dbo.Repairs_Malfunctions",
        "dbo.Repairs_Repair",
        "dbo.Repairs_RepairDetails",
        "dbo.Restaurant_Delivery",
        "dbo.Restaurant_DeliveryMan",
        "dbo.Restaurant_Room",
        "dbo.Restaurant_Table",
        "dbo.Restaurant_Type",
        "dbo.ResultElectronicInvoiceXmls",
        "dbo.Scaffold_EntryPermission",
        "dbo.Scaffold_EntryPermissionDetails",
        "dbo.Scaffold_ExitPermission",
        "dbo.Scaffold_ExitPermissionDetails",
        "dbo.Scaffold_PriceOffer",
        "dbo.Scaffold_PriceOfferDetails",
        "dbo.Scaffolds_Contract",
        "dbo.Scaffolds_ContractDetails",
        "dbo.Scaffolds_MinutesEnd",
        "dbo.Scaffolds_MinutesEndDetails",
        "dbo.Scaffolds_MinutesStarted",
        "dbo.Scaffolds_MinutesStartedDetails",
        "dbo.Tafqit",
        "dbo.TblRemember",
        "dbo.TblSetting",
        "dbo.Temp_Accounts",
        "dbo.TempAccounts",
        "dbo.TempAgeingAnalysis",
        "dbo.TempBudget",
        "dbo.TempTranMaterDetails",
        "dbo.TempTrialBalanceByTotal",
        "dbo.Tran_Tran",
        "dbo.Tran_TranDetails",
        "dbo.Tran_TranTemp",
        "dbo.Tran_TranTempDetails",
        "dbo.Tran_Type",
        "dbo.User_DayClose",
        "dbo.User_Groups",
        "dbo.User_Login",
        "dbo.User_Permission",
        "dbo.User_Screens",
        "dbo.User_SellPrice",
        "dbo.Virg_AppType",
        "dbo.Virg_Branch",
        "dbo.Virg_Company",
        "dbo.Virg_Franchise",
        "dbo.Virg_Master",
        "dbo.Virg_MasterDetails",
        "dbo.Virg_Statuse",
        "dbo.Virg_Type",
        "dbo.Virg_Virgins"
    ];

    public static readonly string[] ReportProcedures =
    [
        "GetReport_Orders", "GetReport_OrderDetailsSmall", "GetReport_OrdersReturn", "GetReport_Purches",
        "GetReport_PurchesDetails", "GetReport_VatReport", "Ledger_Account", "Get_ProfitAndLossAccount",
        "GetReport_MovementsDailyReport", "GetQuantityFromItem_ByItemCode_StoreId", "GetAllBranches",
        "GetReport_OrdersBySalesMan", "GetReport_MovementItemBetweenToDate", "Select_SearchAccountTran",
        "Get_ContractCustomer"
    ];

    public static readonly HashSet<string> EditableTables = new(StringComparer.OrdinalIgnoreCase)
    {
        "dbo.Account_Accounts","dbo.Account_CustSup","dbo.Item_Items","dbo.Item_Unit",
        "dbo.Item_Groups","dbo.Item_Class","dbo.Account_CostCenters","dbo.Contract_Contract","dbo.Emp_Employee"
    };
}
using AlSaqarAccounting.Core;
using AlSaqarAccounting.Services;
using AlSaqarAccounting.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace AlSaqarAccounting.UI;

public sealed class ScreenRouter
{
    private readonly string _connectionString;
    private readonly AppSession _session;

    public ScreenRouter(string connectionString, AppSession session)
    { _connectionString = connectionString; _session = session; }

    public async Task<RouteResult> TryOpenAsync(Form owner, ScreenAccess access)
    {
        if (!access.AllowEnter)
            return new RouteResult(false, "لا تملك صلاحية فتح هذه الشاشة.");

        // Re-check the current permission in the database immediately before
        // opening the form. The menu is only a cached view of the user's access;
        // it must never become an authorization bypass after permissions change.
        try
        {
            var authorization = new AuthorizationService(
                new DbExecutor(new SqlConnectionFactory(_connectionString)));
            // Do not block the WinForms UI thread while SQL validates the permission.
            var allowedNow = await authorization.CanAsync(
                _session, access.Id, PermissionAction.Enter);

            if (!allowedNow)
            {
                return new RouteResult(false,
                    "تم تغيير صلاحياتك أو إيقاف هذه الشاشة. أعد تحميل الصلاحيات ثم حاول مرة أخرى.");
            }
        }
        catch (Exception ex)
        {
            return new RouteResult(false,
                "تعذر التحقق من صلاحية الشاشة من قاعدة البيانات: " + ex.GetBaseException().Message);
        }

        var screenName = ScreenAccess.CleanScreenName(access.ScreenName);
        if (string.IsNullOrWhiteSpace(screenName))
        {
            return new RouteResult(false, "اسم الشاشة غير صالح.");
        }

        var db = new DbExecutor(new SqlConnectionFactory(_connectionString));

        if (string.Equals(screenName, "إدارة التراخيص", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(screenName, "التراخيص", StringComparison.OrdinalIgnoreCase))
        {
            using var form = new LicenseManagementForm(_session, new LicenseService(db)) { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        if (IsPasswordScreen(screenName))
        {
            using var form = new ChangePasswordForm(_session, new SecurityAdministrationService(db))
            { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        if (IsScreenCatalogScreen(screenName))
        {
            using var form = new UserScreensForm(_session, access, new SecurityAdministrationService(db))
            { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        if (IsPermissionScreen(screenName))
        {
            using var form = new UserPermissionsForm(_session, access, new SecurityAdministrationService(db))
            { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        // Original GTSErpSystem places user-management screens in the Security
        // module. Keep those separate from item groups.
        if (IsUserScreen(screenName))
        {
            using var form = new UserManagementForm(_session, access, new UserManagementService(db)) { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        if (IsSecurityGroupPermissionScreen(screenName))
        {
            using var form = new UserPermissionsForm(_session, access, new SecurityAdministrationService(db))
            { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        if (IsUserGroupScreen(screenName))
        {
            using var form = new UserGroupsForm(_session, access, new UserGroupsService(db)) { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(owner); return new RouteResult(true, string.Empty);
        }

        if (string.Equals(screenName, "FrmUnit", StringComparison.OrdinalIgnoreCase))
        {
            var form = new ItemUnitForm(_session, access, new ItemUnitService(db));
            return new RouteResult(OpenMdi(owner, form), string.Empty);
        }
        if (TryResolveItemMaster(screenName, out var tableName, out var displayName))
        {
            var form = new ItemMasterForm(_session, access, new ItemMasterService(db), tableName, displayName);
            return new RouteResult(OpenMdi(owner, form), string.Empty);
        }
        if (string.Equals(screenName, "FrmItems", StringComparison.OrdinalIgnoreCase))
        {
            var form = new ItemsForm(_session, access, new ItemsService(db));
            return new RouteResult(OpenMdi(owner, form), string.Empty);
        }
        if (string.Equals(screenName, "InvoicesForm", StringComparison.OrdinalIgnoreCase) || string.Equals(screenName, "الفواتير", StringComparison.OrdinalIgnoreCase))
        {
            var form = new InvoicesForm(_session, access, new InvoiceService(db), new SalesService(db), new StoresService(db), new CustomerService(db), new SupplierService(db), new ItemsService(db), new PurchasesService(db), new CustSupService(db));
            return new RouteResult(OpenMdi(owner, form), string.Empty);
        }
        // Bind verified legacy ERP screens before the generic catalog/fallback.
        // These mappings use the original GTSdb2026 SELECT procedures and therefore
        // show real database data instead of DynamicErpScreenForm placeholders.
        if (LegacyScreenCatalog.TryCreate(screenName, _connectionString, _session, access, out var legacyScreen)
            && legacyScreen is not null)
        {
            return new RouteResult(OpenMdi(owner, legacyScreen), string.Empty);
        }

        if (RealScreenCatalog.TryCreate(screenName, _connectionString, _session, access, out var realScreen) && realScreen is not null)
        {
            return new RouteResult(OpenMdi(owner, realScreen), string.Empty);
        }
        // For names not yet assigned to a specialized form, open a schema-driven
        // database view. It is explicitly read-only until a verified write contract
        // is mapped, so unknown labels can be inspected without unsafe generic CRUD.
        try
        {
            var dynamicForm = new DynamicErpScreenForm(
                _session,
                access,
                new DynamicErpScreenService(db),
                screenName,
                readOnlyMode: true);
            return new RouteResult(OpenMdi(owner, dynamicForm), string.Empty);
        }
        catch (Exception ex)
        {
            return new RouteResult(false,
                $"تعذر تجهيز شاشة «{screenName}»: {ex.GetBaseException().Message}");
        }
    }

    public sealed record RouteResult(bool Opened, string Message);

    private static bool OpenMdi(Form owner, Form form)
    {
        if (!owner.IsMdiContainer)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.Show(owner);
            return true;
        }

        var existing = owner.MdiChildren.FirstOrDefault(x =>
            string.Equals(x.GetType().FullName, form.GetType().FullName, StringComparison.Ordinal) &&
            string.Equals(x.Text, form.Text, StringComparison.CurrentCultureIgnoreCase));

        if (existing is not null)
        {
            form.Dispose();
            existing.Activate();
            existing.BringToFront();
            return true;
        }

        form.MdiParent = owner;
        form.StartPosition = FormStartPosition.CenterScreen;
        form.WindowState = FormWindowState.Maximized;
        form.Show();
        form.BringToFront();
        return true;
    }

    private static bool IsPasswordScreen(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim();
        return n.Equals("FrmPassword", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("كلمة المرور", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("تغيير كلمة المرور", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("ChangePasswordForm", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsScreenCatalogScreen(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim();
        return n.Equals("FrmScreens", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("الشاشات", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("شاشات النظام", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("UserScreensForm", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPermissionScreen(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim();
        return n.Equals("FrmPermission", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("FrmPermissions", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("الصلاحيات", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("صلاحيات الشاشات", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("UserPermissionsForm", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUserScreen(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim();
        return n.Equals("مستخدم جديد", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("إضافة مستخدم", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("تعديل مستخدم", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("المستخدمون", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("المستخدمين", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("المستخدمون النظام", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("FrmUsers", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("FrmEditUser", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("UserManagementForm", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSecurityGroupPermissionScreen(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim();
        return n.Equals("FrmSecurityGroup", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUserGroupScreen(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var n = name.Trim();
        return n.Equals("مجموعات المستخدمين", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("مجموعة المستخدمين", StringComparison.OrdinalIgnoreCase) ||
               n.Equals("UserGroupsForm", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryResolveItemMaster(string screenName, out string tableName, out string displayName)
    {
        tableName = string.Empty; displayName = string.Empty;
        switch (screenName.Trim())
        {
            case "FrmCompany": tableName = "Item_Company"; displayName = "الشركات"; return true;
            case "FrmClass": tableName = "Item_Class"; displayName = "الفئات"; return true;
            case "FrmGroups": tableName = "Item_Groups"; displayName = "المجموعات"; return true;
            case "GroupsForm": tableName = "Item_Groups"; displayName = "المجموعات"; return true;
            case "FrmCountry": tableName = "Item_Country"; displayName = "الدول"; return true;
            case "FrmDoctor": tableName = "Item_Doctor"; displayName = "الأطباء"; return true;
            default: return false;
        }
    }

}

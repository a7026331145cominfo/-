using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using AlSaqarAccounting.Core;
using AlSaqarAccounting.Services;
using AlSaqarAccounting.UI;

namespace AlSaqarAccounting.Forms;

public sealed class MainForm : Form
{
    private readonly AppSession _session;
    private readonly SchemaService _schema;
    private readonly StoredProcedureExecutor _sp;
    private readonly SecurityService _security;
    private readonly ScreenRouter _router;
    private readonly DashboardService _dashboard;
    private readonly string _connectionString;

    private readonly MenuStrip _menu = new();
    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _status = new();
    private readonly ToolStripStatusLabel _screenCount = new();
    private readonly ToolStripStatusLabel _clock = new();

    private readonly Panel _header = new();
    // Keep the dashboard separate from the MDI client so child screens are visible and clickable.
    private readonly Panel _workspace = new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.White,
        Padding = new Padding(12, 10, 12, 8)
    };
    private readonly FlowLayoutPanel _servicesBar = new();
    private readonly FlowLayoutPanel _quickBar = new();
    private readonly FlowLayoutPanel _screenBar = new();
    private readonly FlowLayoutPanel _openTabsBar = new();
    private readonly Panel _home = new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.White
    };
    private readonly TextBox _search = new();

    private List<ScreenAccess> _screens = new();
    private DashboardMetrics _metrics = new();
    private string _selectedService = string.Empty;

    private static readonly string[] PreferredServices =
    {
        "الرئيسية", "المبيعات والمشتريات", "المخزون", "الحسابات",
        "العملاء والموردون", "الأصناف والمخازن", "التقارير", "العقود",
        "التصنيع", "الإيجارات", "المطاعم", "الموارد البشرية",
        "الصيانة", "الأمن والصلاحيات", "النظام والإعدادات"
    };

    private static readonly (string Label, string[] Names, bool RequiresSave, bool Primary)[] QuickActions =
    {
        ("فاتورة مبيعات", new[] { "SalesEntryForm", "FrmSalesEntry", "فاتورة مبيعات جديدة", "SalesInvoiceForm", "RealSalesInvoiceForm" }, true, true),
        ("فاتورة مشتريات", new[] { "PurchasesEntryForm", "FrmPurchasesEntry", "فاتورة مشتريات جديدة" }, true, true),
        ("سند جديد", new[] { "VoucherEntryForm", "FrmVoucherEntry", "سند جديد" }, true, true),
        ("الكاشير", new[] { "CashierForm", "FrmCashier", "الكاشير", "نقطة البيع" }, true, true),
        ("دليل الحسابات", new[] { "FrmAccountTree", "شجرة الحسابات", "الحسابات" }, false, false),
        ("الأصناف", new[] { "FrmItems", "الأصناف", "ItemsForm" }, false, false),
        ("العملاء", new[] { "FrmCustomer", "العملاء" }, false, false),
        ("الموردون", new[] { "FrmSuppliers", "الموردون", "الموردين" }, false, false),
        ("الجرد", new[] { "FrmGard", "الجرد" }, false, false)
    };


    public MainForm(AppSession session, SchemaService schema, StoredProcedureExecutor sp,
        SecurityService security, string connectionString)
    {
        _session = session;
        _schema = schema;
        _sp = sp;
        _security = security;
        _connectionString = connectionString;
        _router = new ScreenRouter(_connectionString, session);
        _dashboard = new DashboardService(new DbExecutor(new SqlConnectionFactory(_connectionString)));

        Text = $"الصقر للمحاسبة ERP — {_session.UserName}";
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1200, 760);
        StartPosition = FormStartPosition.CenterScreen;
        IsMdiContainer = true;
        KeyPreview = true;

        ErpTheme.ApplyForm(this);
        // Keep the shell's DockStyle.Right navigation on the physical right edge.
        // RightToLeft still controls Arabic text; mirroring the whole form reverses the sidebar.
        RightToLeftLayout = false;
        BuildShell();
        WireEvents();
        ShowHome();
    }

    private void BuildShell()
    {
        BuildMenuStrip();
        BuildHeader();
        BuildServicesBar();
        BuildQuickBar();
        BuildScreenBar();
        BuildStatus();

        _workspace.Controls.Add(_home);
        BuildOpenTabsBar();

        // Keep the fill-docked dashboard and the ribbon/tab bars as siblings of
        // the MDI client. Hiding _workspace reveals real MDI forms while both
        // navigation and open-screen tabs remain available.
        Controls.Add(_workspace);
        Controls.Add(_openTabsBar);
        Controls.Add(_screenBar);
        Controls.Add(_quickBar);
        Controls.Add(_servicesBar);
        Controls.Add(_menu);
        Controls.Add(_header);
        Controls.Add(_statusStrip);
        MainMenuStrip = _menu;
    }

    private void BuildMenuStrip()
    {
        _menu.Visible = true;
        _menu.Dock = DockStyle.Top;
        _menu.GripStyle = ToolStripGripStyle.Hidden;
        _menu.AutoSize = false;
        _menu.Height = 28;
        _menu.Padding = new Padding(8, 2, 8, 2);
        _menu.BackColor = Color.FromArgb(248, 250, 252);
        _menu.ForeColor = ErpTheme.Text;
        _menu.RenderMode = ToolStripRenderMode.Professional;
        _menu.RightToLeft = RightToLeft.Yes;
        _menu.Items.Clear();
        _menu.Items.Add(CreateFileMenu());
    }

    private ToolStripMenuItem CreateFileMenu()
    {
        var file = new ToolStripMenuItem("ملف");
        file.DropDownItems.Add("لوحة التشغيل الرئيسية", null, (_, _) => ShowHome());
        file.DropDownItems.Add("تحديث الصلاحيات", null, async (_, _) => await LoadSecurityAsync());
        file.DropDownItems.Add("اختبار اتصال قاعدة البيانات", null, async (_, _) => await CheckConnectionAsync());
        file.DropDownItems.Add(new ToolStripSeparator());
        file.DropDownItems.Add("إغلاق الشاشة الحالية", null, (_, _) => CloseCurrentScreen());
        file.DropDownItems.Add("خروج", null, (_, _) => Close());
        return file;
    }

    private void RebuildMenuStrip()
    {
        _menu.SuspendLayout();
        try
        {
            _menu.Items.Clear();
            _menu.Items.Add(CreateFileMenu());

            var groups = _screens
                .Where(screen => screen.AllowEnter &&
                                 !string.IsNullOrWhiteSpace(screen.ModuleDisplayName))
                .GroupBy(screen => screen.ModuleDisplayName.Trim(),
                    StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(group =>
                {
                    var index = Array.FindIndex(PreferredServices,
                        preferred => string.Equals(preferred, group.Key, StringComparison.OrdinalIgnoreCase));
                    return index < 0 ? int.MaxValue : index;
                })
                .ThenBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase);

            foreach (var group in groups)
            {
                var menu = new ToolStripMenuItem(group.Key);
                foreach (var screen in group
                    .OrderBy(item => item.ScreenNum ?? int.MaxValue)
                    .ThenBy(item => ScreenAccess.CleanScreenName(item.ScreenName),
                        StringComparer.CurrentCultureIgnoreCase))
                {
                    var label = ScreenAccess.CleanScreenName(screen.ScreenName);
                    if (string.IsNullOrWhiteSpace(label))
                        label = $"شاشة #{screen.Id}";

                    var item = new ToolStripMenuItem(label);
                    item.Click += (_, _) => OpenAccessScreen(screen);
                    menu.DropDownItems.Add(item);
                }

                if (menu.DropDownItems.Count > 0)
                    _menu.Items.Add(menu);
            }

            var search = new ToolStripMenuItem("بحث الشاشات");
            search.ShortcutKeys = Keys.Control | Keys.K;
            search.ShowShortcutKeys = true;
            search.Click += (_, _) =>
            {
                _search.Focus();
                _search.SelectAll();
            };
            _menu.Items.Add(search);

            if (_session.GroupId == 1)
                _menu.Items.Add(new ToolStripMenuItem("التراخيص", null,
                    (_, _) => OpenLicenseManagement()));
        }
        finally
        {
            _menu.ResumeLayout(true);
        }
    }

    private void BuildHeader()
    {
        _header.Dock = DockStyle.Top;
        _header.Height = 72;
        _header.BackColor = Color.FromArgb(0, 90, 158);
        _header.Paint += (_, e) =>
        {
            if (_header.ClientSize.Width <= 0 || _header.ClientSize.Height <= 0)
                return;
            using var brush = new LinearGradientBrush(
                _header.ClientRectangle,
                Color.FromArgb(0, 90, 158),
                Color.FromArgb(0, 120, 215),
                LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(brush, _header.ClientRectangle);
        };
        _header.Padding = new Padding(16, 6, 16, 6);
        _header.BorderStyle = BorderStyle.None;

        var brandPanel = new Panel { Dock = DockStyle.Right, Width = 230, Padding = new Padding(4), BackColor = Color.Transparent };
        var brand = new Label
        {
            Text = "الصقر للمحاسبة",
            Dock = DockStyle.Top,
            Height = 34,
            Font = new Font("Tahoma", 17f, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = Color.Transparent
        };
        var subtitle = new Label
        {
            Text = "نظام تخطيط وإدارة موارد المنشأة",
            Dock = DockStyle.Bottom,
            Height = 24,
            Font = new Font("Tahoma", 9f),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = Color.Transparent
        };
        brandPanel.Controls.Add(subtitle);
        brandPanel.Controls.Add(brand);

        var userPanel = new Panel { Dock = DockStyle.Left, Width = 175, Padding = new Padding(6, 2, 6, 2), BackColor = Color.Transparent };
        var userLine = new Label
        {
            Text = $"المستخدم: {_session.UserName}",
            Dock = DockStyle.Top,
            Height = 28,
            Font = new Font("Tahoma", 9.5f, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = Color.Transparent
        };
        var branchLine = new Label
        {
            Text = $"الفرع: {_session.BranchId?.ToString() ?? "-"}    |    المجموعة: {_session.GroupId?.ToString() ?? "-"}",
            Dock = DockStyle.Top,
            Height = 22,
            ForeColor = Color.FromArgb(228, 240, 255),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = Color.Transparent
        };
        userPanel.Controls.Add(branchLine);
        userPanel.Controls.Add(userLine);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(12, 6, 12, 6),
            BackColor = Color.Transparent,
            Margin = new Padding(8, 0, 8, 0)
        };

        _search.Width = 178;
        _search.Height = 28;
        _search.Font = new Font("Tahoma", 9.5f);
        _search.RightToLeft = RightToLeft.Yes;
        _search.BorderStyle = BorderStyle.FixedSingle;
        _search.Margin = new Padding(4, 4, 10, 0);

        var searchLabel = new Label
        {
            Text = "بحث سريع",
            AutoSize = true,
            Font = new Font("Tahoma", 9f, FontStyle.Bold),
            ForeColor = ErpTheme.NavigationMuted,
            Margin = new Padding(4, 10, 4, 0)
        };

        actions.Controls.Add(CreateHeaderButton("الرئيسية", (_, _) => ShowHome(), true));
        actions.Controls.Add(CreateHeaderButton("تحديث", async (_, _) => await LoadSecurityAsync()));
        actions.Controls.Add(CreateHeaderButton("إغلاق", (_, _) => CloseCurrentScreen()));
        actions.Controls.Add(CreateHeaderButton("اتصال", async (_, _) => await CheckConnectionAsync()));
        if (_session.GroupId == 1)
            actions.Controls.Add(CreateHeaderButton("التراخيص", (_, _) => OpenLicenseManagement()));
        actions.Controls.Add(CreateHeaderButton("خروج", (_, _) => Close()));
        actions.Controls.Add(searchLabel);
        actions.Controls.Add(_search);

        _header.Controls.Add(actions);
        _header.Controls.Add(userPanel);
        _header.Controls.Add(brandPanel);
    }

    private void BuildServicesBar()
    {
        _servicesBar.Dock = DockStyle.Right;
        _servicesBar.Width = 250;
        _servicesBar.FlowDirection = FlowDirection.TopDown;
        _servicesBar.WrapContents = false;
        _servicesBar.AutoScroll = true;
        _servicesBar.Padding = new Padding(10, 14, 10, 12);
        _servicesBar.BackColor = Color.FromArgb(248, 250, 252);
        _servicesBar.RightToLeft = RightToLeft.Yes;
        _servicesBar.BorderStyle = BorderStyle.None;
    }

    private void BuildQuickBar()
    {
        _quickBar.Dock = DockStyle.Top;
        _quickBar.Height = 44;
        _quickBar.FlowDirection = FlowDirection.RightToLeft;
        _quickBar.WrapContents = false;
        _quickBar.AutoScroll = true;
        _quickBar.Padding = new Padding(8, 5, 8, 5);
        _quickBar.BackColor = ErpTheme.SurfaceSoft;
        _quickBar.RightToLeft = RightToLeft.Yes;
        _quickBar.BorderStyle = BorderStyle.FixedSingle;
    }

    private void RebuildQuickBar()
    {
        _quickBar.SuspendLayout();
        try
        {
            _quickBar.Controls.Clear();
            var used = new HashSet<int>();

            foreach (var quickAction in QuickActions)
            {
                var screen = _screens.FirstOrDefault(candidate =>
                    candidate.AllowEnter &&
                    (!quickAction.RequiresSave || candidate.AllowSave) &&
                    quickAction.Names.Any(name => string.Equals(
                        ScreenAccess.CleanScreenName(candidate.ScreenName),
                        name,
                        StringComparison.OrdinalIgnoreCase)));

                if (screen is null || !used.Add(screen.Id))
                    continue;

                var button = new Button
                {
                    Text = quickAction.Label,
                    Width = 124,
                    Height = 31,
                    Margin = new Padding(3, 0, 3, 0),
                    Font = new Font("Tahoma", 8.5f, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                ErpTheme.ConfigureToolbarButton(button, quickAction.Primary);
                button.Click += (_, _) => OpenAccessScreen(screen);
                _quickBar.Controls.Add(button);
            }

            if (_quickBar.Controls.Count == 0)
            {
                _quickBar.Controls.Add(new Label
                {
                    Text = "لا توجد اختصارات تشغيلية متاحة لهذا الحساب.",
                    AutoSize = true,
                    ForeColor = ErpTheme.Muted,
                    Padding = new Padding(8, 5, 8, 0)
                });
            }
        }
        finally
        {
            _quickBar.ResumeLayout(true);
        }
    }

    private void BuildScreenBar()
    {
        _screenBar.Dock = DockStyle.Top;
        _screenBar.Height = 52;
        _screenBar.FlowDirection = FlowDirection.RightToLeft;
        _screenBar.WrapContents = false;
        _screenBar.AutoScroll = true;
        _screenBar.Padding = new Padding(12, 7, 12, 7);
        _screenBar.BackColor = ErpTheme.Surface;
        _screenBar.RightToLeft = RightToLeft.Yes;
        _screenBar.BorderStyle = BorderStyle.FixedSingle;
        _screenBar.Visible = false;
    }

    private void BuildOpenTabsBar()
    {
        _openTabsBar.Dock = DockStyle.Top;
        _openTabsBar.Height = 36;
        _openTabsBar.FlowDirection = FlowDirection.RightToLeft;
        _openTabsBar.WrapContents = false;
        _openTabsBar.AutoScroll = true;
        _openTabsBar.Padding = new Padding(8, 3, 8, 3);
        _openTabsBar.BackColor = Color.FromArgb(232, 238, 247);
        _openTabsBar.RightToLeft = RightToLeft.Yes;
        _openTabsBar.Visible = false;
    }

    private void RebuildOpenTabs()
    {
        _openTabsBar.SuspendLayout();
        try
        {
            _openTabsBar.Controls.Clear();
            var children = MdiChildren
                .Where(child => !child.IsDisposed)
                .OrderBy(child => child.Text, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            _openTabsBar.Visible = children.Length > 0;
            foreach (var child in children)
            {
                var active = ReferenceEquals(child, ActiveMdiChild);
                var tab = new Panel
                {
                    Width = Math.Max(150, Math.Min(255, 38 + child.Text.Length * 8)),
                    Height = 28,
                    Margin = new Padding(3, 0, 3, 0),
                    Padding = new Padding(0),
                    BackColor = active ? ErpTheme.AccentSoft : Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                var select = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = child.Text,
                    TextAlign = ContentAlignment.MiddleRight,
                    Padding = new Padding(8, 0, 8, 0),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = active ? ErpTheme.AccentSoft : Color.White,
                    ForeColor = active ? ErpTheme.Accent : ErpTheme.Text,
                    Font = new Font("Tahoma", 8.5f, active ? FontStyle.Bold : FontStyle.Regular),
                    Cursor = Cursors.Hand
                };
                select.FlatAppearance.BorderSize = 0;
                select.Click += (_, _) =>
                {
                    if (!child.IsDisposed)
                    {
                        child.Activate();
                        child.BringToFront();
                    }
                };

                var close = new Button
                {
                    Dock = DockStyle.Right,
                    Width = 26,
                    Text = "×",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = ErpTheme.Muted,
                    Font = new Font("Tahoma", 10f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                close.FlatAppearance.BorderSize = 0;
                close.Click += (_, _) =>
                {
                    if (!child.IsDisposed)
                        child.Close();
                    RebuildOpenTabs();
                };

                tab.Controls.Add(select);
                tab.Controls.Add(close);
                _openTabsBar.Controls.Add(tab);
            }
        }
        finally
        {
            _openTabsBar.ResumeLayout(true);
        }
    }

    private void BuildStatus()
    {
        _statusStrip.Dock = DockStyle.Bottom;
        _status.Text = $"المستخدم: {_session.UserName}";
        _screenCount.Text = "الشاشات: ...";
        _clock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _clock.Spring = true;
        _clock.TextAlign = ContentAlignment.MiddleLeft;
        _statusStrip.BackColor = Color.White;
        _statusStrip.ForeColor = ErpTheme.Text;
        _statusStrip.SizingGrip = false;

        _statusStrip.Items.Add(_status);
        _statusStrip.Items.Add(new ToolStripStatusLabel { Text = string.Empty });
        _statusStrip.Items.Add(_screenCount);
        _statusStrip.Items.Add(new ToolStripStatusLabel { Text = string.Empty });
        _statusStrip.Items.Add(new ToolStripStatusLabel { Text = $"الفرع: {_session.BranchId?.ToString() ?? "-"}", ForeColor = Color.Black });
        _statusStrip.Items.Add(new ToolStripStatusLabel { Text = string.Empty });
        _statusStrip.Items.Add(new ToolStripStatusLabel { Text = $"المجموعة: {_session.GroupId?.ToString() ?? "-"}", ForeColor = Color.Black });
        _statusStrip.Items.Add(new ToolStripStatusLabel { Text = string.Empty });
        _statusStrip.Items.Add(_clock);
    }

    private Button CreateHeaderButton(string text, EventHandler click, bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            Width = primary ? 94 : 75,
            Height = 33,
            Margin = new Padding(4),
            Font = new Font("Tahoma", 8.5f, FontStyle.Bold)
        };
        ErpTheme.ConfigureToolbarButton(button, primary);
        button.Click += click;
        return button;
    }

    private void RebuildServicesBar()
    {
        _servicesBar.SuspendLayout();
        _servicesBar.Controls.Clear();

        _servicesBar.Controls.Add(new Label
        {
            Text = "القائمة الرئيسية",
            Width = 214,
            Height = 34,
            Margin = new Padding(2, 0, 2, 8),
            Font = new Font("Tahoma", 11f, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 90, 158),
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = Color.Transparent
        });
        _servicesBar.Controls.Add(new Label
        {
            Text = "الأقسام والوحدات",
            Width = 214,
            Height = 24,
            Margin = new Padding(2, 0, 2, 8),
            Font = new Font("Tahoma", 8.5f, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 116, 139),
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = Color.Transparent
        });

        var available = _screens.Where(s => s.AllowEnter)
            .Select(s => s.ModuleDisplayName)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.CurrentCultureIgnoreCase).ToList();

        var services = PreferredServices.Where(name => name == "الرئيسية" ||
            available.Any(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase))).ToList();

        foreach (var extra in available.Where(x => !services.Any(s => string.Equals(s, x, StringComparison.OrdinalIgnoreCase)))
                     .OrderBy(x => x, StringComparer.CurrentCultureIgnoreCase))
            services.Add(extra);

        foreach (var service in services)
        {
            var active = string.Equals(service, _selectedService, StringComparison.OrdinalIgnoreCase);
            var button = new Button
            {
                Text = service,
                Tag = service,
                Width = 214,
                Height = 44,
                Margin = new Padding(2, 3, 2, 3),
                Font = new Font("Tahoma", 9.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                RightToLeft = RightToLeft.Yes
            };

            ConfigureServiceButton(button, active);
            button.Click += (_, _) =>
            {
                _selectedService = service;
                _status.Text = $"القسم المحدد: {service}";
                if (service == "الرئيسية") ShowHome();
                else { RebuildScreenBar(); HideHomeForService(); }
                RebuildServicesBar();
            };
            _servicesBar.Controls.Add(button);
        }

        _servicesBar.ResumeLayout(true);
    }

    private void ConfigureServiceButton(Button button, bool active)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = active ? ErpTheme.Accent : ErpTheme.Border;
        button.FlatAppearance.MouseOverBackColor = ErpTheme.AccentSoft;
        button.BackColor = active ? ErpTheme.Accent : Color.White;
        button.ForeColor = active ? Color.White : ErpTheme.Text;
        button.Cursor = Cursors.Hand;
        button.TextAlign = ContentAlignment.MiddleRight;
        button.Padding = new Padding(12, 0, 12, 0);
    }


    private void RebuildScreenBar()
    {
        _screenBar.Controls.Clear();
        if (string.IsNullOrWhiteSpace(_selectedService) || _selectedService == "الرئيسية")
        {
            _screenBar.Visible = false;
            return;
        }

        _screenBar.Visible = true;

        var filter = _search.Text.Trim();
        var screens = _screens.Where(s => s.AllowEnter)
            .Where(s => string.Equals(s.ModuleDisplayName, _selectedService, StringComparison.OrdinalIgnoreCase))
            .Where(s => string.IsNullOrWhiteSpace(filter) ||
                        ScreenAccess.CleanScreenName(s.ScreenName).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        s.ModuleDisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
            .OrderBy(s => s.ScreenNum ?? int.MaxValue).ThenBy(s => s.Id).ToList();

        foreach (var screen in screens)
        {
            var name = string.IsNullOrWhiteSpace(screen.ScreenName) ? $"شاشة #{screen.Id}" : ScreenAccess.CleanScreenName(screen.ScreenName);
            var button = new Button
            {
                Text = name,
                Tag = screen,
                AutoSize = false,
                Width = Math.Max(126, Math.Min(210, 36 + (name.Length * 8))),
                Height = 36,
                Margin = new Padding(4, 1, 4, 1),
                Font = new Font("Tahoma", 9f, FontStyle.Bold)
            };
            ErpTheme.ConfigureToolbarButton(button);
            button.Click += (_, _) => OpenAccessScreen(screen);
            _screenBar.Controls.Add(button);
        }

        if (screens.Count == 0)
            _screenBar.Controls.Add(new Label
            {
                Text = "لا توجد شاشة متاحة لهذه الخدمة.",
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Tahoma", 9f),
                Padding = new Padding(8, 9, 8, 0)
            });
    }

    private void HideHomeForService()
    {
        _home.Visible = false;

        // Hide the whole fill-docked dashboard container. Hiding only _home left
        // an empty panel over the MDI client, making opened forms seem unresponsive.
        _workspace.Visible = false;
    }

    private void WireEvents()
    {
        Shown += async (_, _) => await LoadSecurityAsync();
        MdiChildActivate += (_, _) => RebuildOpenTabs();
        _search.TextChanged += (_, _) =>
        {
            if (_selectedService == "الرئيسية") BuildDashboard();
            else RebuildScreenBar();
        };

        var timer = new System.Windows.Forms.Timer { Interval = 1000 };
        timer.Tick += (_, _) => _clock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        timer.Start();
        FormClosed += (_, _) => timer.Dispose();

        KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.F5) { e.Handled = true; _ = LoadSecurityAsync(); }
            else if (e.KeyCode == Keys.F10) { e.Handled = true; ShowHome(); }
            else if (e.KeyCode == Keys.Escape) { e.Handled = true; CloseCurrentScreen(); }
        };
    }

    private async Task LoadSecurityAsync()
    {
        try
        {
            UseWaitCursor = true;
            _screens = (await _security.GetAccessibleScreensAsync(_session)).ToList();
            var groupName = await _security.GetGroupNameAsync(_session);
            try
            {
                _metrics = await _dashboard.GetMetricsAsync(_session.BranchId);
            }
            catch (Exception metricsError)
            {
                _metrics = new DashboardMetrics();
                _status.ToolTipText = "تعذر تحميل مؤشرات لوحة التحكم: " + metricsError.GetBaseException().Message;
            }

            _status.Text = $"المستخدم: {_session.UserName}";
            _screenCount.Text = $"الشاشات: {_screens.Count:N0}";
            _status.ToolTipText = $"المستخدم: {_session.UserName}\r\n" +
                $"الفرع: {_session.BranchId?.ToString() ?? "-"}\r\n" +
                $"المجموعة: {groupName ?? _session.GroupId?.ToString() ?? "-"}";

            if (string.IsNullOrWhiteSpace(_selectedService)) _selectedService = "الرئيسية";
            RebuildMenuStrip();
            RebuildServicesBar();
            RebuildQuickBar();
            RebuildScreenBar();
            RebuildOpenTabs();
            BuildDashboard();
        }
        catch (Exception ex)
        {
            _screens.Clear();
            _screenCount.Text = "الشاشات: 0";
            MessageBox.Show(this, "تعذر تحميل الصلاحيات:\r\n" + ex.GetBaseException().Message,
                "الصلاحيات", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally { UseWaitCursor = false; }
    }

    private async void OpenAccessScreen(ScreenAccess access)
    {
        if (!access.AllowEnter)
        {
            MessageBox.Show(this, "لا تملك صلاحية فتح هذه الشاشة.", "الصلاحيات",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var fromHome = string.Equals(_selectedService, "الرئيسية", StringComparison.OrdinalIgnoreCase)
            && ActiveMdiChild is null;

        UseWaitCursor = true;
        _status.Text = $"جاري فتح الشاشة: {ScreenAccess.CleanScreenName(access.ScreenName)}";
        _home.Visible = false;
        _workspace.Visible = false;

        try
        {
            var routeResult = await _router.TryOpenAsync(this, access);
            var opened = routeResult.Opened;
            var message = routeResult.Message;

            if (!opened && string.IsNullOrWhiteSpace(message))
                message = $"تعذر فتح الشاشة «{ScreenAccess.CleanScreenName(access.ScreenName)}».";

            if (!string.IsNullOrWhiteSpace(message))
                MessageBox.Show(this, message, "فتح الشاشة",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (fromHome && (!opened || ActiveMdiChild is null))
                ShowHome();
            else if (opened)
                _status.Text = $"تم فتح الشاشة: {ScreenAccess.CleanScreenName(access.ScreenName)}";

            RebuildOpenTabs();
        }
        catch (Exception ex)
        {
            _status.Text = $"فشل فتح الشاشة: {ScreenAccess.CleanScreenName(access.ScreenName)}";
            MessageBox.Show(this,
                "تعذر فتح الشاشة:\r\n" + ex.GetBaseException().Message,
                "فتح الشاشة", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (fromHome)
                ShowHome();
        }
        finally
        {
            UseWaitCursor = false;
        }
    }

    private void CloseCurrentScreen()
    {
        var active = ActiveMdiChild;
        if (active is not null) { active.Close(); return; }
        ShowHome();
    }

    private async Task CheckConnectionAsync()
    {
        try
        {
            var tables = await _schema.GetTablesAsync();
            _status.Text = $"الاتصال سليم — الجداول: {tables.Rows.Count:N0}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.GetBaseException().Message, "اختبار الاتصال",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ShowHome()
    {
        _selectedService = "الرئيسية";
        foreach (var child in MdiChildren) child.Close();
        _openTabsBar.Visible = false;
        _workspace.Visible = true;
        _home.Visible = true;
        _home.BringToFront();
        RebuildServicesBar();
        RebuildScreenBar();
        BuildDashboard();
    }

    private void BuildDashboard()
    {
        _home.Controls.Clear();

        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(20),
            BackColor = ErpTheme.SurfaceSoft
        };

        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 142));
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var title = new Label
        {
            Text = $"مرحباً {_session.UserName} — لوحة التحكم",
            Dock = DockStyle.Fill,
            Font = new Font("Tahoma", 22f, FontStyle.Bold),
            ForeColor = ErpTheme.Text,
            TextAlign = ContentAlignment.MiddleRight
        };

        var subtitle = new Label
        {
            Text = $"نظرة سريعة على أعمال المنشأة   •   آخر تحديث: {DateTime.Now:yyyy/MM/dd HH:mm}",
            Dock = DockStyle.Fill,
            Font = new Font("Tahoma", 10f),
            ForeColor = ErpTheme.Muted,
            TextAlign = ContentAlignment.MiddleRight
        };

        outer.Controls.Add(title, 0, 0);
        outer.Controls.Add(subtitle, 0, 1);

        var cards = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RowCount = 1, BackColor = Color.Transparent };
        for (var i = 0; i < 4; i++) cards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        var moduleCount = _screens.Select(s => s.ModuleDisplayName)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.CurrentCultureIgnoreCase).Count();

        cards.Controls.Add(ErpTheme.CreateCard("مبيعات الشهر", $"{_metrics.SalesTotal:N2}", $"عدد الفواتير: {_metrics.SalesCount:N0}"), 0, 0);
        cards.Controls.Add(ErpTheme.CreateCard("مشتريات الشهر", $"{_metrics.PurchaseTotal:N2}", $"عدد الفواتير: {_metrics.PurchaseCount:N0}"), 1, 0);
        cards.Controls.Add(ErpTheme.CreateCard("المخزون الحالي", $"{_metrics.StockQuantity:N2}", $"الأصناف: {_metrics.ItemCount:N0}"), 2, 0);
        cards.Controls.Add(ErpTheme.CreateCard("فرق القيود", $"{_metrics.LedgerDifference:N2}", $"مدين {_metrics.DebitTotal:N2} | دائن {_metrics.CreditTotal:N2}"), 3, 0);
        outer.Controls.Add(cards, 0, 2);

        var quickTitle = new Label
        {
            Text = "الاختصارات التشغيلية",
            Dock = DockStyle.Fill,
            Font = new Font("Tahoma", 12f, FontStyle.Bold),
            ForeColor = ErpTheme.Text,
            TextAlign = ContentAlignment.MiddleRight
        };
        outer.Controls.Add(quickTitle, 0, 3);

        var quick = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = true,
            AutoScroll = true,
            Padding = new Padding(4),
            BackColor = Color.Transparent
        };

        var quickNames = new[]
        {
            "الكاشير", "الفواتير", "المبيعات", "المشتريات",
            "العملاء", "الموردون", "الأصناف", "المخازن",
            "الجرد", "عروض الأسعار", "شجرة الحسابات", "السندات",
            "الفروع", "العقود"
        };

        foreach (var name in quickNames)
        {
            var screen = _screens.FirstOrDefault(s => string.Equals(ScreenAccess.CleanScreenName(s.ScreenName), name, StringComparison.OrdinalIgnoreCase));
            if (screen is null || !screen.AllowEnter) continue;

            var button = new Button
            {
                Text = name,
                Width = 190,
                Height = 54,
                Margin = new Padding(5),
                Font = new Font("Tahoma", 9.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(12, 0, 12, 0)
            };
            ErpTheme.ConfigureDashboardButton(button);
            button.Click += (_, _) => OpenAccessScreen(screen);
            quick.Controls.Add(button);
        }

        if (quick.Controls.Count == 0)
            quick.Controls.Add(new Label
            {
                Text = $"العملاء: {_metrics.CustomerCount:N0}   |   الموردون: {_metrics.SupplierCount:N0}   |   الشاشات المسموح بها: {_screens.Count:N0}",
                AutoSize = true,
                ForeColor = ErpTheme.Muted,
                Padding = new Padding(8, 12, 8, 0)
            });

        var workspace = new Panel
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = ErpTheme.Surface,
            Padding = new Padding(10)
        };
        workspace.Controls.Add(quick);
        outer.Controls.Add(workspace, 0, 4);
        _home.Controls.Add(outer);
    }

    private void OpenLicenseManagement()
    {
        try
        {
            var service = new LicenseService(new DbExecutor(new SqlConnectionFactory(_connectionString)));
            using var form = new LicenseManagementForm(_session, service) { StartPosition = FormStartPosition.CenterParent };
            form.ShowDialog(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.GetBaseException().Message, "إدارة التراخيص",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

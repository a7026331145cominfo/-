using System.Drawing;

namespace AlSaqarERP.Desktop;

public sealed class ConnectionSettingsForm : Form
{
    private readonly AppSettings _settings;
    private readonly Action _onSaved;
    private readonly TextBox _server = new() { Dock = DockStyle.Fill };
    private readonly TextBox _database = new() { Dock = DockStyle.Fill };< convert-react-ui-to-csharp

    private readonly TextBox _userName = new() { Dock = DockStyle.Fill };
    private readonly TextBox _password = new() { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
  main
    private readonly CheckBox _integrated = new() { Text = "مصادقة Windows", AutoSize = true };
    private readonly Label _status = new() { AutoSize = true, MaximumSize = new Size(540, 70) };

    public ConnectionSettingsForm(AppSettings settings, Action onSaved)
    {
        _settings = settings; _onSaved = onSaved;
        Text = "إعداد الاتصال بقاعدة البيانات";
 convert-react-ui-to-csharp
        Width = 620; Height = 300; StartPosition = FormStartPosition.CenterParent;
        RightToLeft = RightToLeft.Yes; RightToLeftLayout = true;
        _server.Text = settings.Server;
        _database.Text = settings.Database;
        _integrated.Checked = settings.IntegratedSecurity;
        var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5, Padding = new Padding(14) };
 
        Width = 620; Height = 430; MinimumSize = new Size(600, 400); StartPosition = FormStartPosition.CenterParent;
        RightToLeft = RightToLeft.Yes; RightToLeftLayout = true;
        _server.Text = settings.Server;
        _database.Text = settings.Database;
        _userName.Text = settings.UserName;
        _password.Text = settings.Password;
        _integrated.Checked = settings.IntegratedSecurity;
        var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 7, Padding = new Padding(14) };
main
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        AddField(form, 0, "خادم SQL Server", _server);
        AddField(form, 1, "قاعدة البيانات", _database);
        form.Controls.Add(new Label { Text = "المصادقة", AutoSize = true, Anchor = AnchorStyles.Right }, 0, 2);
        form.Controls.Add(_integrated, 1, 2);
convert-react-ui-to-csharp
        form.Controls.Add(_status, 0, 3); form.SetColumnSpan(_status, 2);
=======
        AddField(form, 3, "اسم مستخدم SQL", _userName);
        AddField(form, 4, "كلمة مرور SQL", _password);
        form.Controls.Add(_status, 0, 5); form.SetColumnSpan(_status, 2);
        _integrated.CheckedChanged += (_, _) => UpdateAuthenticationFields();
        UpdateAuthenticationFields();
 main
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
        var test = new Button { Text = "اختبار الاتصال", AutoSize = true };
        test.Click += (_, _) => TestConnection();
        var save = new Button { Text = "حفظ الإعدادات", AutoSize = true };
        save.Click += (_, _) => SaveSettings();
        var cancel = new Button { Text = "إلغاء", AutoSize = true };
        cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        buttons.Controls.Add(test); buttons.Controls.Add(save); buttons.Controls.Add(cancel);
        form.Controls.Add(buttons, 0, 4); form.SetColumnSpan(buttons, 2);
        Controls.Add(form);
    }

    private static void AddField(TableLayoutPanel panel, int row, string label, Control control)
    {
        panel.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Right }, 0, row);
        panel.Controls.Add(control, 1, row);
    }

 convert-react-ui-to-csharp
    private AppSettings Candidate() => new()
    {
        Server = _server.Text.Trim(),
        Database = _database.Text.Trim(),
        IntegratedSecurity = _integrated.Checked,
        UserName = _settings.UserName,
        EncryptedPassword = _settings.EncryptedPassword
    };
=======
    private AppSettings Candidate()
    {
        var server = _server.Text.Trim();
        var database = _database.Text.Trim();
        var userName = _userName.Text.Trim();
        if (server.Length == 0)
            throw new InvalidOperationException("أدخل اسم خادم SQL Server أو اسم المثيل المحلي.");
        if (database.Length == 0)
            throw new InvalidOperationException("أدخل اسم قاعدة البيانات.");
        if (!_integrated.Checked && userName.Length == 0)
            throw new InvalidOperationException("أدخل اسم مستخدم SQL أو اختر مصادقة Windows.");
        return new AppSettings
        {
            Server = server,
            Database = database,
            IntegratedSecurity = _integrated.Checked,
            UserName = userName,
            Password = _password.Text
        };
    }

    private void UpdateAuthenticationFields()
    {
        var useSqlAuthentication = !_integrated.Checked;
        _userName.Enabled = useSqlAuthentication;
        _password.Enabled = useSqlAuthentication;
    }
 main

    private void TestConnection()
    {
        try
        {
            var connected = new SqlServerService(Candidate()).TestConnection();
            _status.ForeColor = Color.DarkGreen;
            _status.Text = "نجح الاتصال: " + connected;
        }
        catch (Exception ex)
        {
            _status.ForeColor = Color.Firebrick;
            _status.Text = "فشل الاتصال: " + ex.Message;
        }
    }

    private void SaveSettings()
    {
        try
        {
            var candidate = Candidate();
            _settings.Server = candidate.Server;
            _settings.Database = candidate.Database;
            _settings.IntegratedSecurity = candidate.IntegratedSecurity;
 convert-react-ui-to-csharp
=======
            _settings.UserName = candidate.UserName;
            _settings.Password = candidate.Password;
 main
            _settings.Save();
            _onSaved();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex) { _status.ForeColor = Color.Firebrick; _status.Text = ex.Message; }
    }
}
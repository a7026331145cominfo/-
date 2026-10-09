using System.Drawing;
using System.Windows.Forms;

namespace AlSaqarAccounting.UI;

public static class ErpTheme
{
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceSoft = Color.FromArgb(243, 247, 253);
    public static readonly Color Border = Color.FromArgb(220, 229, 242);
    public static readonly Color Text = Color.FromArgb(24, 42, 69);
    public static readonly Color Muted = Color.FromArgb(104, 120, 145);
    public static readonly Color Accent = Color.FromArgb(31, 100, 210);
    public static readonly Color AccentSoft = Color.FromArgb(232, 241, 255);
    public static readonly Color Navigation = Color.FromArgb(17, 39, 72);
    public static readonly Color NavigationText = Color.White;
    public static readonly Color NavigationMuted = Color.FromArgb(186, 204, 231);

    public static Font RegularFont => new("Tahoma", 9.5f);
    public static Font TitleFont => new("Tahoma", 18f, FontStyle.Bold);

    public static void ApplyForm(Form form)
    {
        form.BackColor = SurfaceSoft;
        form.ForeColor = Text;
        form.Font = RegularFont;
        form.RightToLeft = RightToLeft.Yes;
        form.RightToLeftLayout = true;
    }

    public static void ConfigureGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Border;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersDefaultCellStyle.BackColor = Navigation;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = AccentSoft;
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.DefaultCellStyle.Padding = new Padding(6, 4, 6, 4);
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 251, 253);
        grid.RowTemplate.Height = 36;
    }

    public static void ConfigureToolbarButton(Button button, bool primary = false)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = primary ? Accent : Border;
        button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(17, 98, 107) : AccentSoft;
        button.BackColor = primary ? Accent : Surface;
        button.ForeColor = primary ? Color.White : Text;
        button.Font = new Font("Tahoma", 9f, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
    }

    public static void ConfigureDashboardButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Border;
        button.FlatAppearance.MouseOverBackColor = AccentSoft;
        button.BackColor = Surface;
        button.ForeColor = Text;
        button.Font = new Font("Tahoma", 9.5f, FontStyle.Bold);
        button.Cursor = Cursors.Hand;
        button.FlatAppearance.CheckedBackColor = AccentSoft;
    }

    public static Button CreateNavigationButton(string text)
    {
        var button = new Button
        {
            Text = text, Width = 228, Height = 42,
            Margin = new Padding(6, 3, 6, 3), FlatStyle = FlatStyle.Flat,
            BackColor = Navigation, ForeColor = NavigationText,
            TextAlign = ContentAlignment.MiddleRight, Padding = new Padding(12, 0, 12, 0),
            Cursor = Cursors.Hand, Font = new Font("Tahoma", 9.5f, FontStyle.Regular)
        };
        button.FlatAppearance.BorderColor = Color.FromArgb(48, 70, 89);
        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(31, 57, 78);
        return button;
    }

    public static Panel CreateCard(string title, string value, string? hint = null)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(7),
            BackColor = Surface,
            Padding = new Padding(14, 10, 14, 10),
            BorderStyle = BorderStyle.FixedSingle
        };
        var valueLabel = new Label
        {
            Text = value,
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Tahoma", 17f, FontStyle.Bold),
            ForeColor = Accent,
            TextAlign = ContentAlignment.MiddleRight
        };
        var titleLabel = new Label
        {
            Text = title,
            Dock = DockStyle.Top,
            Height = 27,
            Font = new Font("Tahoma", 9.5f, FontStyle.Bold),
            ForeColor = Text,
            TextAlign = ContentAlignment.MiddleRight
        };
        card.Controls.Add(valueLabel);
        card.Controls.Add(titleLabel);
        if (!string.IsNullOrWhiteSpace(hint))
            card.Controls.Add(new Label
            {
                Text = hint,
                Dock = DockStyle.Bottom,
                Height = 22,
                ForeColor = Muted,
                TextAlign = ContentAlignment.MiddleRight
            });
        card.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 5, BackColor = Accent });
        return card;
    }

    public static void StyleRecursive(Control root)
    {
        root.BackColor = Surface;
        root.ForeColor = Text;
        foreach (Control control in root.Controls)
        {
            control.BackColor = Surface;
            control.ForeColor = Text;
            if (control is DataGridView grid) ConfigureGrid(grid);
            else if (control is Button button) ConfigureToolbarButton(button);
            if (control.HasChildren) StyleRecursive(control);
        }
    }
}

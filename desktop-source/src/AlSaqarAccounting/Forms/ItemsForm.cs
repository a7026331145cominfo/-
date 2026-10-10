using System.Data;
using AlSaqarAccounting.Core;
using AlSaqarAccounting.Models;
using AlSaqarAccounting.Services;
using AlSaqarAccounting.UI;

namespace AlSaqarAccounting.Forms;

/// <summary>
/// Concrete inventory item screen. It is backed by GTSdb2026 procedures,
/// not the generic OperationalDataScreen.
/// </summary>
public sealed class ItemsForm : Form
{
    private readonly AppSession _session;
    private readonly ScreenAccess _access;
    private readonly ItemsService _service;
    private readonly DataGridView _grid = new();
    private readonly TextBox _search = new();
    private readonly TextBox _code = new();
    private readonly TextBox _name = new();
    private readonly TextBox _englishName = new();
    private readonly ComboBox _unitSmall = new() { DropDownStyle = ComboBoxStyle.DropDownList, RightToLeft = RightToLeft.Yes };
    private readonly ComboBox _unitMedium = new() { DropDownStyle = ComboBoxStyle.DropDownList, RightToLeft = RightToLeft.Yes };
    private readonly ComboBox _unitLarge = new() { DropDownStyle = ComboBoxStyle.DropDownList, RightToLeft = RightToLeft.Yes };
    private readonly TextBox _itemType = new() { RightToLeft = RightToLeft.Yes };
    private readonly NumericUpDown _costPrice = new();
    private readonly NumericUpDown _sellPrice = new();
    private readonly NumericUpDown _sellPriceMedium = new();
    private readonly NumericUpDown _sellPriceLarge = new();
    private readonly CheckBox _taxEnabled = new() { Text = "خاضع للضريبة", AutoSize = true, RightToLeft = RightToLeft.Yes };
    private readonly NumericUpDown _tax = new();
    private readonly TextBox _currentStock = new() { ReadOnly = true, RightToLeft = RightToLeft.Yes };
    private DataTable? _items;
    private DataTable? _units;
    private int? _selectedItemId;
    private int _selectionVersion;
    private bool _changingSelection;

    public ItemsForm(AppSession session, ScreenAccess access, ItemsService service)
    {
        _session = session;
        _access = access;
        _service = service;
        InitializeUi();
    }

    private void InitializeUi()
    {
        ErpTheme.ApplyForm(this);
        Text = "الأصناف";
        Width = 1280;
        Height = 760;
        RightToLeft = RightToLeft.Yes;
        StartPosition = FormStartPosition.CenterParent;

        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 52,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(8)
        };

        AddToolbarButton(toolbar, "جديد", _access.AllowSave, NewItem, true);
        AddToolbarButton(toolbar, "حفظ الصنف", _access.AllowSave, SaveItemAsync);
        AddToolbarButton(toolbar, "تعديل", _access.AllowEdit, EditItemAsync);
        AddToolbarButton(toolbar, "حذف", _access.AllowDelete, DeleteItemAsync);
        AddToolbarButton(toolbar, "تحديث", _access.AllowEnter, LoadItemsAsync);

        toolbar.Controls.Add(new Label { Text = "بحث:", AutoSize = true, Padding = new Padding(8, 8, 2, 0) });
        _search.Width = 260;
        _search.TextChanged += (_, _) => ApplyFilter();
        toolbar.Controls.Add(_search);
        Controls.Add(toolbar);

        var editor = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 228,
            ColumnCount = 4,
            RowCount = 4,
            Padding = new Padding(8)
        };
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        AddField(editor, "كود الصنف", _code, 0, 0);
        AddField(editor, "اسم الصنف", _name, 1, 0);
        AddField(editor, "الاسم الإنجليزي", _englishName, 2, 0);
        AddField(editor, "نوع الصنف (كود)", _itemType, 3, 0);

        AddField(editor, "الوحدة الصغرى", _unitSmall, 0, 1);
        AddField(editor, "الوحدة المتوسطة", _unitMedium, 1, 1);
        AddField(editor, "الوحدة الكبرى", _unitLarge, 2, 1);
        AddField(editor, "الكمية الحالية بالمخازن", _currentStock, 3, 1);

        AddNumericField(editor, "سعر التكلفة", _costPrice, 0, 2, 4);
        AddNumericField(editor, "سعر البيع للوحدة الصغرى", _sellPrice, 1, 2, 4);
        AddNumericField(editor, "سعر البيع للوحدة المتوسطة", _sellPriceMedium, 2, 2, 4);
        AddNumericField(editor, "سعر البيع للوحدة الكبرى", _sellPriceLarge, 3, 2, 4);

        AddField(editor, "الضريبة", _taxEnabled, 0, 3);
        AddNumericField(editor, "قيمة الضريبة %", _tax, 1, 3, 2);
        Controls.Add(editor);

        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AutoGenerateColumns = true;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        _grid.SelectionChanged += async (_, _) => await LoadSelectedItemAsync();
        ErpTheme.ConfigureGrid(_grid);
        Controls.Add(_grid);

        Shown += async (_, _) => await LoadItemsAsync();
    }

    private static void AddField(TableLayoutPanel panel, string label, Control control, int column, int row)
    {
        var box = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        var text = new Label { Text = label, Dock = DockStyle.Top, Height = 24 };
        control.Dock = DockStyle.Fill;
        box.Controls.Add(control);
        box.Controls.Add(text);
        panel.Controls.Add(box, column, row);
    }

    private static void AddNumericField(TableLayoutPanel panel, string label, NumericUpDown control, int column, int row, int decimals = 0)
    {
        control.DecimalPlaces = decimals;
        control.Maximum = 1000000000;
        control.Minimum = 0;
        AddField(panel, label, control, column, row);
    }

    private static void AddToolbarButton(
        FlowLayoutPanel toolbar,
        string text,
        bool enabled,
        Func<Task> action,
        bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            Width = text.Length > 7 ? 120 : 95,
            Height = 34,
            Enabled = enabled,
            Margin = new Padding(4),
            FlatStyle = FlatStyle.Flat
        };
        button.Click += async (_, _) => await action();
        ErpTheme.ConfigureToolbarButton(button, primary);
        toolbar.Controls.Add(button);
    }

    private static void AddToolbarButton(
        FlowLayoutPanel toolbar,
        string text,
        bool enabled,
        Action action,
        bool primary = false)
    {
        var button = new Button
        {
            Text = text,
            Width = text.Length > 7 ? 120 : 95,
            Height = 34,
            Enabled = enabled,
            Margin = new Padding(4),
            FlatStyle = FlatStyle.Flat
        };
        button.Click += (_, _) => action();
        ErpTheme.ConfigureToolbarButton(button, primary);
        toolbar.Controls.Add(button);
    }

    private void NewItem()
    {
        _selectedItemId = null;
        _changingSelection = true;
        try
        {
            ClearEditor();
            _grid.ClearSelection();
        }
        finally
        {
            _changingSelection = false;
        }
        _code.Focus();
    }

    private async Task LoadSelectedItemAsync()
    {
        if (_changingSelection || _grid.CurrentRow?.DataBoundItem is not DataRowView row)
            return;

        var rawId = RowValue(row.Row, "ItemId");
        if (!int.TryParse(Convert.ToString(rawId), out var id) || id <= 0)
            return;

        _selectedItemId = id;
        var version = Interlocked.Increment(ref _selectionVersion);
        _currentStock.Text = "جاري تحميل التفاصيل...";

        try
        {
            // Load the canonical table row instead of depending on which columns
            // the legacy Get_All_Items procedure happens to return.
            var details = await _service.GetByIdAsync(id);
            if (version != _selectionVersion || IsDisposed)
                return;
            if (details.Rows.Count == 0)
            {
                _currentStock.Text = "لم تعد بيانات الصنف موجودة";
                return;
            }

            var item = details.Rows[0];
            _code.Text = StringValue(item, "Item_code");
            _name.Text = StringValue(item, "item_Name");
            _englishName.Text = StringValue(item, "item_Name_English");
            _itemType.Text = StringValue(item, "item_Type");
            SetComboValue(_unitSmall, RowValue(item, "UnitSmall"));
            SetComboValue(_unitMedium, RowValue(item, "UnitMedium"));
            SetComboValue(_unitLarge, RowValue(item, "UnitLarge"));
            SetNumericValue(_costPrice, RowValue(item, "LastCost"));
            SetNumericValue(_sellPrice, RowValue(item, "SellPriceSmall"));
            SetNumericValue(_sellPriceMedium, RowValue(item, "SellPriceMedium"));
            SetNumericValue(_sellPriceLarge, RowValue(item, "SellpriceLarge"));
            _taxEnabled.Checked = ToBooleanValue(RowValue(item, "Is_Tax"));
            SetNumericValue(_tax, RowValue(item, "Tax_Value"));

            var stock = await _service.ListStockAsync(id, _session);
            if (version != _selectionVersion || IsDisposed)
                return;

            decimal quantity = 0m;
            if (stock.Columns.Contains("CurrentBalance"))
            {
                foreach (DataRow stockRow in stock.Rows)
                {
                    if (stockRow["CurrentBalance"] != DBNull.Value)
                        quantity += Convert.ToDecimal(stockRow["CurrentBalance"]);
                }
            }
            _currentStock.Text = quantity.ToString("N3");
        }
        catch (Exception ex)
        {
            if (version == _selectionVersion && !IsDisposed)
                _currentStock.Text = "تعذر التحميل: " + ex.GetBaseException().Message;
        }
    }

    private static object? RowValue(DataRow row, params string[] names)
    {
        foreach (var name in names)
            if (row.Table.Columns.Contains(name))
                return row[name] == DBNull.Value ? null : row[name];
        return null;
    }

    private static string StringValue(DataRow row, params string[] names)
        => Convert.ToString(RowValue(row, names)) ?? string.Empty;

    private static decimal ToDecimalValue(object? value)
    {
        try { return value is null || value == DBNull.Value ? 0m : Math.Max(0m, Convert.ToDecimal(value)); }
        catch { return 0m; }
    }

    private static bool ToBooleanValue(object? value)
    {
        try { return value is not null && value != DBNull.Value && Convert.ToBoolean(value); }
        catch { return false; }
    }

    private static void SetNumericValue(NumericUpDown control, object? value)
    {
        var numeric = ToDecimalValue(value);
        control.Value = Math.Min(control.Maximum, Math.Max(control.Minimum, numeric));
    }

    private static void SetComboValue(ComboBox combo, object? value)
    {
        combo.SelectedIndex = -1;
        if (value is null || value == DBNull.Value)
            return;
        try { combo.SelectedValue = Convert.ToInt32(value); }
        catch { combo.SelectedIndex = -1; }
    }

    private static int? SelectedUnitId(ComboBox combo)
    {
        try { return combo.SelectedValue is null ? null : Convert.ToInt32(combo.SelectedValue); }
        catch { return null; }
    }

    private static int? ParseNullableInt(string? value)
        => int.TryParse(value, out var parsed) ? parsed : null;

    private async Task LoadItemsAsync()
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            _units ??= await _service.ListUnitsAsync();
            BindUnitCombo(_unitSmall);
            BindUnitCombo(_unitMedium);
            BindUnitCombo(_unitLarge);
            _items = await _service.ListAsync();
            ApplyFilter();
            FormatGridColumns();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.GetBaseException().Message, "خطأ في تحميل الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void BindUnitCombo(ComboBox combo)
    {
        if (_units is null) return;
        combo.DataSource = new DataView(_units);
        combo.DisplayMember = "Name";
        combo.ValueMember = "ID";
        combo.DropDownStyle = ComboBoxStyle.DropDownList;
    }

    private void FormatGridColumns()
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["ItemId"] = "رقم الصنف",
            ["Item_code"] = "كود الصنف",
            ["item_Name"] = "اسم الصنف",
            ["item_Name_English"] = "الاسم الإنجليزي",
            ["item_Type"] = "نوع الصنف",
            ["UnitSmall"] = "الوحدة الصغرى",
            ["UnitMedium"] = "الوحدة المتوسطة",
            ["UnitLarge"] = "الوحدة الكبرى",
            ["LastCost"] = "سعر التكلفة",
            ["SellPriceSmall"] = "سعر البيع للصغرى",
            ["SellPriceMedium"] = "سعر البيع للمتوسطة",
            ["SellpriceLarge"] = "سعر البيع للكبرى",
            ["Is_Tax"] = "خاضع للضريبة",
            ["Tax_Value"] = "قيمة الضريبة %",
            ["SmallUnitQuantity"] = "كمية الوحدة الصغرى",
            ["SmallUnitQuantity2"] = "كمية الوحدة المتوسطة",
            ["SmallUnitQuantity3"] = "كمية الوحدة الكبرى"
        };

        var visibleFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "ItemId", "Item_code", "item_Name", "item_Name_English", "item_Type",
            "UnitSmall", "UnitMedium", "UnitLarge", "LastCost", "SellPriceSmall",
            "SellPriceMedium", "SellpriceLarge", "Is_Tax", "Tax_Value",
            "SmallUnitQuantity", "SmallUnitQuantity2", "SmallUnitQuantity3"
        };

        foreach (DataGridViewColumn column in _grid.Columns)
        {
            if (headers.TryGetValue(column.Name, out var caption))
                column.HeaderText = caption;

            // Keep the list focused on useful item data, not audit, barcode and
            // internal integration fields. The selected item's live stock total
            // is shown in the details row above the grid.
            column.Visible = visibleFields.Contains(column.Name);
        }
    }

    private void ApplyFilter()
    {
        if (_items is null) return;
        var view = new DataView(_items);
        var term = _search.Text.Trim()
            .Replace("'", "''")
            .Replace("[", "[[]")
            .Replace("%", "[%]")
            .Replace("*", "[*]");
        if (term.Length > 0)
        {
            var filters = new List<string>();
            foreach (var name in new[] { "ItemId", "Item_code", "item_Name", "item_Name_English" })
            {
                if (_items.Columns.Contains(name))
                    filters.Add($"Convert([{name}], 'System.String') LIKE '%{term}%'");
            }
            if (filters.Count > 0)
                view.RowFilter = string.Join(" OR ", filters);
        }
        _grid.DataSource = view;
        FormatGridColumns();
    }

    private async Task SaveItemAsync()
    {
        try
        {
            if (!_access.AllowSave)
                throw new InvalidOperationException("لا تملك صلاحية حفظ الأصناف.");

            if (_selectedItemId.HasValue)
            {
                await EditItemAsync();
                return;
            }

            Cursor = Cursors.WaitCursor;
            await _service.CreateAsync(BuildEditorItem(), _session, _access.Id);
            ClearEditor();
            _selectedItemId = null;
            await LoadItemsAsync();
            MessageBox.Show(this, "تم حفظ الصنف في قاعدة البيانات.", "الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.GetBaseException().Message, "تعذر حفظ الصنف", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task EditItemAsync()
    {
        if (!_selectedItemId.HasValue)
        {
            MessageBox.Show(this, "حدد الصنف المطلوب تعديله أولاً.", "الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            Cursor = Cursors.WaitCursor;
            await _service.UpdateAsync(_selectedItemId.Value, BuildEditorItem(), _session, _access.Id);
            await LoadItemsAsync();
            MessageBox.Show(this, "تم تعديل الصنف في قاعدة البيانات.", "الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.GetBaseException().Message, "تعذر تعديل الصنف", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private async Task DeleteItemAsync()
    {
        if (!_selectedItemId.HasValue)
        {
            MessageBox.Show(this, "حدد الصنف المطلوب حذفه أولاً.", "الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show(
                this,
                $"هل تريد حذف الصنف «{_name.Text.Trim()}»؟",
                "تأكيد حذف الصنف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            return;

        try
        {
            Cursor = Cursors.WaitCursor;
            await _service.DeleteAsync(_selectedItemId.Value, _session, _access.Id);
            _selectedItemId = null;
            ClearEditor();
            await LoadItemsAsync();
            MessageBox.Show(this, "تم حذف الصنف من قاعدة البيانات.", "الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.GetBaseException().Message, "تعذر حذف الصنف", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private Item_Items BuildEditorItem()
    {
        int? itemType = null;
        if (!string.IsNullOrWhiteSpace(_itemType.Text))
        {
            if (!int.TryParse(_itemType.Text.Trim(), out var parsedType))
                throw new ArgumentException("نوع الصنف يجب أن يكون كوداً رقمياً صحيحاً.");
            itemType = parsedType;
        }

        return new Item_Items
        {
            Item_code = _code.Text.Trim(),
            item_Name = _name.Text.Trim(),
            item_Name_English = _englishName.Text.Trim(),
            item_Type = itemType,
            UnitSmall = SelectedUnitId(_unitSmall),
            UnitMedium = SelectedUnitId(_unitMedium),
            UnitLarge = SelectedUnitId(_unitLarge),
            LastCost = _costPrice.Value,
            SellPriceSmall = _sellPrice.Value,
            SellPriceMedium = _sellPriceMedium.Value,
            SellpriceLarge = _sellPriceLarge.Value,
            Is_Tax = _taxEnabled.Checked,
            Tax_Value = _tax.Value
        };
    }

    private void ClearEditor()
    {
        _code.Clear();
        _name.Clear();
        _englishName.Clear();
        _itemType.Clear();
        _unitSmall.SelectedIndex = -1;
        _unitMedium.SelectedIndex = -1;
        _unitLarge.SelectedIndex = -1;
        _costPrice.Value = 0;
        _sellPrice.Value = 0;
        _sellPriceMedium.Value = 0;
        _sellPriceLarge.Value = 0;
        _taxEnabled.Checked = false;
        _tax.Value = 0;
        _currentStock.Clear();
    }
}

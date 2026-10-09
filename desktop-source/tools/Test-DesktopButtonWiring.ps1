$ErrorActionPreference = 'Stop'

$sourceRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\src\AlSaqarAccounting"))
$checks = @(
    @{ Name = "Main screen buttons route to the screen opener"; File = "Forms\MainForm.cs"; Pattern = 'button\.Click\s*\+=\s*\(_, _\)\s*=> OpenAccessScreen\(screen\);' },
    @{ Name = "Workspace is hidden when opening screens"; File = "Forms\MainForm.cs"; Pattern = '_workspace\.Visible\s*=\s*false\s*;' },
    @{ Name = "Workspace is restored on Home"; File = "Forms\MainForm.cs"; Pattern = '_workspace\.Visible\s*=\s*true\s*;' },
    @{ Name = "Router uses the real screen catalogs"; File = "UI\ScreenRouter.cs"; Pattern = 'RealScreenCatalog\.TryCreate' },
    @{ Name = "Router resolves legacy screens"; File = "UI\ScreenRouter.cs"; Pattern = 'LegacyScreenCatalog\.TryCreate' },
    @{ Name = "Sales invoice button has a click action"; File = "Forms\OrdersForm.cs"; Pattern = 'AddButton\(toolbar,\s*"فاتورة جديدة",\s*Access\.AllowSave' },
    @{ Name = "Sales deletion goes through SalesService"; File = "Forms\OrdersForm.cs"; Pattern = 'await\s+_sales\.DeleteAsync\(' },
    @{ Name = "Purchase edit button checks edit permission"; File = "Forms\PurchasesForm.cs"; Pattern = 'AddButton\(toolbar,\s*"تعديل الفاتورة",\s*Access\.AllowEdit' },
    @{ Name = "Purchase edit opens the invoice editor"; File = "Forms\PurchasesForm.cs"; Pattern = 'await\s+EditSelectedInvoiceAsync\(\)' },
    @{ Name = "Purchase related routes normalize legacy labels"; File = "Forms\PurchasesForm.cs"; Pattern = 'ScreenAccess\.CleanScreenName\(s\.ScreenName\)' },
    @{ Name = "Sales related routes normalize legacy labels"; File = "Forms\OrdersForm.cs"; Pattern = 'ScreenAccess\.CleanScreenName\(s\.ScreenName\)' },
    @{ Name = "Voucher related routes normalize legacy labels"; File = "Forms\ReceiptsForm.cs"; Pattern = 'ScreenAccess\.CleanScreenName\(s\.ScreenName\)' },
    @{ Name = "Item import is gated by save permission"; File = "Forms\AdvancedItemsForm.cs"; Pattern = 'AddToolbarButton\(toolbar,\s*"استيراد",\s*_access\.AllowSave,\s*ImportItems\)' },
    @{ Name = "Item import has a second permission guard"; File = "Forms\AdvancedItemsForm.cs"; Pattern = 'private\s+async\s+void\s+ImportItems\(\)\s*\{\s*if\s*\(!_access\.AllowSave\)' },
    @{ Name = "Cashier sale button is permission gated"; File = "Forms\CashierForm.cs"; Pattern = 'Enabled\s*=\s*_access\.AllowSave\s*\}\s*;\s*completeBtn\.Click\s*\+=' },
    @{ Name = "Cashier completion click calls its handler"; File = "Forms\CashierForm.cs"; Pattern = 'completeBtn\.Click\s*\+=\s*async\s*\(_, _\)\s*=>\s*await\s+CompleteSaleAsync\(\)' },
    @{ Name = "Cashier print button is permission gated"; File = "Forms\CashierForm.cs"; Pattern = 'Text\s*=\s*"طباعة"[^;]*Enabled\s*=\s*_access\.AllowPrint' },
    @{ Name = "Invoice list has wired create/edit/delete actions"; File = "Forms\InvoicesForm.cs"; Pattern = 'AddToolbarButton\(toolbar,\s*"جديد".*CreateNewInvoice.*\r?\n.*"عرض التفاصيل".*\r?\n.*"تعديل".*EditInvoice.*\r?\n.*"حذف".*DeleteInvoice' },
    @{ Name = "Voucher save button calls SaveAsync"; File = "Forms\VoucherEntryForm.cs"; Pattern = 'Action\("حفظ السند".*SaveAsync\(\)' },
    @{ Name = "Places double-click focuses the edit field"; File = "Forms\PlacesForm.cs"; Pattern = '_grid\.CellDoubleClick\s*\+=\s*\(_, e\)\s*=>\s*\{\s*if\s*\(e\.RowIndex\s*<\s*0\s*\|\|\s*!_access\.AllowEdit\)\s*return;\s*LoadSelected\(\);\s*_name\.Focus\(\);\s*_name\.SelectAll\(\);' },
    @{ Name = "Item editor has save/edit/delete/refresh handlers"; File = "Forms\ItemsForm.cs"; Pattern = 'AddToolbarButton\(toolbar,\s*"حفظ الصنف".*SaveItemAsync.*\r?\n.*"تعديل".*EditItemAsync.*\r?\n.*"حذف".*DeleteItemAsync.*\r?\n.*"تحديث".*LoadItemsAsync' },
    @{ Name = "Printer save/delete buttons are wired"; File = "Forms\PrinterSettingsForm.cs"; Pattern = 's\.Click\s*\+=\s*async\s*\(_\s*,\s*_\s*\)\s*=>\s*await\s+SavePrinter\(\).*d\.Click\s*\+=\s*async\s*\(_\s*,\s*_\s*\)\s*=>\s*await\s+DeletePrinter\(\)' },
    @{ Name = "Main navigation uses a right-side vertical module sidebar"; File = "Forms\MainForm.cs"; Pattern = '_servicesBar\.Dock\s*=\s*DockStyle\.Right' },
    @{ Name = "Dashboard shortcuts use the modern button style"; File = "Forms\MainForm.cs"; Pattern = 'ErpTheme\.ConfigureDashboardButton\(button\)' },
    @{ Name = "ERP theme defines dashboard shortcut styling"; File = "UI\ErpTheme.cs"; Pattern = 'ConfigureDashboardButton' },
    @{ Name = "Cashier VAT is loaded from current database settings"; File = "Forms\CashierForm.cs"; Pattern = '_taxRate\s*=\s*settings\.VatEnabled\s*\?\s*settings\.VatRate\s*:\s*0m' },
    @{ Name = "Cashier VAT covers the discounted line total"; File = "Forms\CashierForm.cs"; Pattern = 'VAT\s*=\s*Convert\.ToDecimal\(row\["Total"\]\)\s*\*\s*_taxRate' },
    @{ Name = "Cashier sale receipts can load branch-scoped saved details"; File = "Forms\CashierForm.cs"; Pattern = 'GetSaleDetailsForBranchAsync\(' },
    @{ Name = "Cashier invoices preserve their POS transaction type"; File = "Services\SalesService.cs"; Pattern = '\.Set\("@OrderCashierType",\s*invoice\.OrderCashierType\)' },
    @{ Name = "Cashier service marks POS invoices"; File = "Services\CashierService.cs"; Pattern = 'OrderCashierType\s*=\s*true' },
    @{ Name = "Cashier daily sales includes the full selected day"; File = "Services\CashierService.cs"; Pattern = 'Purchases_Date\s*<\s*DATEADD\(DAY,\s*1,\s*@ToDate\)' },
    @{ Name = "Purchase editor enables update only with edit permission"; File = "Forms\PurchasesEntryForm.cs"; Pattern = '_invoiceId\.HasValue\s*\?\s*_access\.AllowEdit\s*:\s*_access\.AllowSave' },
    @{ Name = "Item editor distinguishes add and edit permissions"; File = "Forms\AdvancedItemsForm.cs"; Pattern = '_selectedItemId\.HasValue\s*\?\s*_access\.AllowEdit\s*:\s*_access\.AllowSave' },
    @{ Name = "Voucher save wraps the header and details in one transaction"; File = "Services\VouchersService.cs"; Pattern = 'BeginTransaction\(IsolationLevel\.Serializable\).*transaction\.Commit\(\).*transaction\.Rollback\(\)' },
    @{ Name = "Voucher numbering is locked against concurrent duplicates"; File = "Services\VouchersService.cs"; Pattern = 'WITH\s*\(UPDLOCK,\s*HOLDLOCK\)' }
)

$failed = [System.Collections.Generic.List[string]]::new()
$checkFailureCount = 0
foreach ($check in $checks) {
    $path = Join-Path $sourceRoot $check.File
    if (-not (Test-Path -LiteralPath $path)) {
        $failed.Add("$($check.Name) — missing file $($check.File)")
        continue
    }

    $content = [System.IO.File]::ReadAllText($path, [System.Text.Encoding]::UTF8)
    if ([regex]::IsMatch($content, $check.Pattern, [System.Text.RegularExpressions.RegexOptions]::Singleline)) {
        Write-Host "PASS: $($check.Name)" -ForegroundColor Green
    }
    else {
        $failed.Add($check.Name)
        $checkFailureCount++
        Write-Host "FAIL: $($check.Name)" -ForegroundColor Red
    }
}

$allForms = Get-ChildItem -LiteralPath (Join-Path $sourceRoot "Forms") -Filter "*.cs" -File
$noOpHandlers = Select-String -Path $allForms.FullName -Pattern 'await\s+Task\.CompletedTask' -ErrorAction SilentlyContinue
if ($noOpHandlers) {
    foreach ($hit in $noOpHandlers) {
        $failed.Add("Suspicious no-op async handler: $($hit.Path):$($hit.LineNumber)")
        Write-Host "FAIL: Suspicious no-op async handler at $($hit.Path):$($hit.LineNumber)" -ForegroundColor Red
    }
}
else {
    Write-Host "PASS: No await Task.CompletedTask no-op handlers remain" -ForegroundColor Green
}

Write-Host ""
Write-Host ("Button-wiring source checks: {0} passed, {1} failed" -f ($checks.Count - $checkFailureCount), $failed.Count)
if ($failed.Count -gt 0) {
    $failed | ForEach-Object { Write-Error $_ }
    exit 1
}

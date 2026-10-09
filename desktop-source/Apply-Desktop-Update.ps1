param(
    [string]$ProjectRoot = "C:\مجلد جديد\AlSaqarAccountingV4-main",
    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

$project = Join-Path $ProjectRoot "src\AlSaqarAccounting"
$branch = "ai-desktop-source-20261009-160047"
$baseUrl = "https://raw.githubusercontent.com/a7026331145cominfo/-/$branch/desktop-source/src/AlSaqarAccounting"
$work = Join-Path $env:TEMP ("AlSaqar-Update-" + [guid]::NewGuid().ToString("N"))
$stamp = Get-Date -Format "yyyyMMdd-HHmmss"
$parent = Split-Path $ProjectRoot -Parent
$backup = Join-Path $parent ("AlSaqar-Backup-" + $stamp)
$packagePath = Join-Path $parent ("AlSaqar-Desktop-Source-Updated-" + $stamp + ".zip")
$exe = Join-Path $project "bin\Release\net48\AlSaqarAccounting.exe"

if (-not (Test-Path -LiteralPath $project)) {
    throw "لم يتم العثور على مجلد المشروع: $project"
}
if (-not (Get-Command git -ErrorAction SilentlyContinue) -and
    -not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "لم يتم العثور على أدوات البناء .NET."
}
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet غير موجود في PATH. لم يتم تغيير أي ملف."
}
if (Get-Process -Name "AlSaqarAccounting" -ErrorAction SilentlyContinue) {
    throw "أغلق تطبيق الصقر قبل التحديث، ثم نفذ الأمر مرة أخرى."
}

$files = @(
    @{ Relative = "Forms\AdvancedItemsForm.cs"; Marker = "_selectedItemId.HasValue ? _access.AllowEdit : _access.AllowSave" },
    @{ Relative = "Forms\CashierForm.cs"; Marker = "GetSaleDetailsForBranchAsync(" },
    @{ Relative = "Forms\MainForm.cs"; Marker = "_servicesBar.Dock = DockStyle.Right;" },
    @{ Relative = "Forms\OrdersForm.cs"; Marker = "ScreenAccess.CleanScreenName(s.ScreenName)" },
    @{ Relative = "Forms\PlacesForm.cs"; Marker = "_name.Focus();" },
    @{ Relative = "Forms\PurchasesEntryForm.cs"; Marker = "_invoiceId.HasValue ? _access.AllowEdit : _access.AllowSave" },
    @{ Relative = "Forms\PurchasesForm.cs"; Marker = 'AddButton(toolbar, "تعديل الفاتورة", Access.AllowEdit' },
    @{ Relative = "Forms\RealSalesInvoiceFormFixed.cs"; Marker = "body.Controls.Add(bottom, 0, 3);" },
    @{ Relative = "Forms\ReceiptsForm.cs"; Marker = "ScreenAccess.CleanScreenName(s.ScreenName)" },
    @{ Relative = "Services\CashierService.cs"; Marker = "OrderCashierType = true," },
    @{ Relative = "Services\SalesService.cs"; Marker = 'Set("@OrderCashierType", invoice.OrderCashierType)' },
    @{ Relative = "Services\VouchersService.cs"; Marker = "BeginTransaction(IsolationLevel.Serializable)" },
    @{ Relative = "UI\ErpTheme.cs"; Marker = "ConfigureDashboardButton" },
    @{ Relative = "UI\RealScreenCatalog.cs"; Marker = '["SalesInvoiceForm"]' },
    @{ Relative = "UI\ScreenRouter.cs"; Marker = "LegacyScreenCatalog.TryCreate" }
)

New-Item -ItemType Directory -Path $work -Force | Out-Null

# Download and validate every requested file before changing the project.
foreach ($file in $files) {
    $file.Target = Join-Path $project $file.Relative
    $file.Download = Join-Path $work $file.Relative
    New-Item -ItemType Directory -Path (Split-Path $file.Download -Parent) -Force | Out-Null

    if (-not (Test-Path -LiteralPath $file.Target)) {
        throw "الملف الأصلي غير موجود: $($file.Relative)"
    }

    $url = "$baseUrl/$($file.Relative.Replace('\', '/'))"
    Invoke-WebRequest -Uri $url -OutFile $file.Download -UseBasicParsing
    $downloaded = [System.IO.File]::ReadAllText($file.Download, [System.Text.Encoding]::UTF8)

    if ($downloaded.Length -lt 500 -or
        $downloaded.IndexOf($file.Marker, [StringComparison]::Ordinal) -lt 0) {
        throw "فشل التحقق من الملف المنزّل: $($file.Relative). لم يتم تعديل المشروع."
    }
}

New-Item -ItemType Directory -Path $backup -Force | Out-Null

# Back up the existing source and compiled application before applying changes.
foreach ($file in $files) {
    $saved = Join-Path $backup $file.Relative
    New-Item -ItemType Directory -Path (Split-Path $saved -Parent) -Force | Out-Null
    Copy-Item -LiteralPath $file.Target -Destination $saved -Force
}

$releaseDir = Join-Path $project "bin\Release\net48"
$releaseBackup = Join-Path $backup "bin\Release\net48"
if (Test-Path -LiteralPath $releaseDir) {
    New-Item -ItemType Directory -Path (Split-Path $releaseBackup -Parent) -Force | Out-Null
    Copy-Item -LiteralPath $releaseDir -Destination $releaseBackup -Recurse -Force
}

try {
    foreach ($file in $files) {
        Copy-Item -LiteralPath $file.Download -Destination $file.Target -Force
    }

    Write-Host "تم تحديث ملفات الواجهة والعمليات؛ جاري بناء التطبيق..." -ForegroundColor Cyan
    & dotnet build (Join-Path $project "AlSaqarAccounting.csproj") -c Release
    if ($LASTEXITCODE -ne 0) {
        throw "فشل بناء المشروع (exit code $LASTEXITCODE)."
    }
    if (-not (Test-Path -LiteralPath $exe)) {
        throw "نجح أمر البناء لكن ملف EXE غير موجود في المسار المتوقع."
    }
}
catch {
    Write-Warning "فشل التحديث أو البناء؛ سأعيد ملفات المصدر والبرنامج السابق."
    foreach ($file in $files) {
        $saved = Join-Path $backup $file.Relative
        if (Test-Path -LiteralPath $saved) {
            Copy-Item -LiteralPath $saved -Destination $file.Target -Force
        }
    }

    if (Test-Path -LiteralPath $releaseDir) {
        Remove-Item -LiteralPath $releaseDir -Recurse -Force
    }
    if (Test-Path -LiteralPath $releaseBackup) {
        New-Item -ItemType Directory -Path (Split-Path $releaseDir -Parent) -Force | Out-Null
        Copy-Item -LiteralPath $releaseBackup -Destination $releaseDir -Recurse -Force
    }

    throw "لم يكتمل التحديث. تم استرجاع النسخة السابقة. السبب: $($_.Exception.Message)"
}

# Create an updated source package without build outputs or local settings/database files.
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$extensions = @(".cs", ".csproj", ".sln", ".resx")
$sourceFiles = Get-ChildItem -LiteralPath $ProjectRoot -Recurse -File |
    Where-Object {
        $extensions -contains $_.Extension.ToLowerInvariant() -and
        $_.FullName -notmatch '\\(bin|obj|node_modules|\.git|\.vs)\\'
    }

if (Test-Path -LiteralPath $packagePath) {
    Remove-Item -LiteralPath $packagePath -Force
}
$archive = [System.IO.Compression.ZipFile]::Open(
    $packagePath,
    [System.IO.Compression.ZipArchiveMode]::Create
)
try {
    foreach ($file in $sourceFiles) {
        $relative = $file.FullName.Substring($ProjectRoot.Length).TrimStart([char]'\')
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
            $archive,
            $file.FullName,
            $relative.Replace('\', '/'),
            [System.IO.Compression.CompressionLevel]::Optimal
        ) | Out-Null
    }
}
finally {
    $archive.Dispose()
}

Write-Host ""
Write-Host "اكتمل تحديث المصدر والبناء بنجاح." -ForegroundColor Green
Write-Host "الملفات المحدثة: $($files.Count)"
Write-Host "نسخة الأمان: $backup"
Write-Host "حزمة المصدر المحدثة: $packagePath"

if (-not $NoLaunch) {
    Start-Process -FilePath $exe -WorkingDirectory $releaseDir
}

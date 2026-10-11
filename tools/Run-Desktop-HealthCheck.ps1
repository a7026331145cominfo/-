param(
    [string]$ProjectRoot = (Get-Location).Path,
    [string]$SqlServer = '.\SQLEXPRESS',
    [string]$Database = 'GtsDb2026',
    [int]$StartupTimeoutSeconds = 15,
    [switch]$SkipLaunch
)

$ErrorActionPreference = 'Stop'
$script:Results = New-Object System.Collections.Generic.List[object]
$script:LogLines = New-Object System.Collections.Generic.List[string]
$script:HadFatalError = $false

function Add-Check {
    param([string]$Name, [string]$Status, [string]$Details)
    $item = [pscustomobject]@{ Check = $Name; Status = $Status; Details = $Details }
    $script:Results.Add($item)
    $line = '[{0}] {1}: {2}' -f $Status, $Name, $Details
    $script:LogLines.Add($line)
    $color = if ($Status -eq 'PASS') { 'Green' } elseif ($Status -eq 'FAIL') { 'Red' } else { 'Yellow' }
    Write-Host $line -ForegroundColor $color
}

function Find-ProjectRoot {
    param([string]$StartPath)
    $start = (Resolve-Path -LiteralPath $StartPath).Path
    $queue = New-Object 'System.Collections.Generic.Queue[string]'
    $seen = New-Object 'System.Collections.Generic.HashSet[string]' ([StringComparer]::OrdinalIgnoreCase)
    $queue.Enqueue($start)
    while ($queue.Count -gt 0 -and $seen.Count -lt 300) {
        $current = $queue.Dequeue()
        if (-not $seen.Add($current)) { continue }
        $knownProject = Join-Path $current 'src\AlSaqarAccounting\AlSaqarAccounting.csproj'
        $directProject = Join-Path $current 'AlSaqarAccounting.csproj'
        if ((Test-Path -LiteralPath $knownProject) -or (Test-Path -LiteralPath $directProject)) { return $current }
        try {
            $children = Get-ChildItem -LiteralPath $current -Directory -ErrorAction Stop |
                Where-Object { $_.Name -notin @('.git', 'node_modules', 'bin', 'obj', 'packages', '.vs') }
            foreach ($child in $children) { $queue.Enqueue($child.FullName) }
        } catch { }
    }
    return $null
}

function Get-MSBuildPath {
    $command = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    $command = Get-Command msbuild -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    $pf86 = [Environment]::GetFolderPath([Environment+SpecialFolder]::ProgramFilesX86)
    $vswhere = Join-Path $pf86 'Microsoft Visual Studio\Installer\vswhere.exe'
    if (Test-Path -LiteralPath $vswhere) {
        $found = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' 2>$null |
            Select-Object -First 1
        if ($found -and (Test-Path -LiteralPath $found)) { return $found }
    }
    return $null
}

Write-Host ''
Write-Host 'AlSaqar Desktop - One-run Health Check' -ForegroundColor Cyan
Write-Host 'Read-only database checks; no business data is changed.' -ForegroundColor Cyan
Write-Host ''

try {
    $resolvedRoot = Find-ProjectRoot -StartPath $ProjectRoot
    if (-not $resolvedRoot) {
        Add-Check 'Project source' 'FAIL' ('Could not find AlSaqarAccounting.csproj under: ' + $ProjectRoot)
        $script:HadFatalError = $true
    } else {
        $ProjectRoot = $resolvedRoot
        Add-Check 'Project source' 'PASS' $ProjectRoot
    }
} catch {
    Add-Check 'Project source' 'FAIL' $_.Exception.Message
    $script:HadFatalError = $true
}

$projectPath = $null
$buildOk = $false
$expectedExe = $null
if (-not $script:HadFatalError) {
    $projectCandidates = @(
        (Join-Path $ProjectRoot 'src\AlSaqarAccounting\AlSaqarAccounting.csproj'),
        (Join-Path $ProjectRoot 'AlSaqarAccounting.csproj')
    )
    $projectPath = $projectCandidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
    if (-not $projectPath) {
        Add-Check 'Project file' 'FAIL' 'The desktop C# project file is missing.'
        $script:HadFatalError = $true
    } else {
        Add-Check 'Project file' 'PASS' $projectPath
    }
}

if (-not $script:HadFatalError) {
    $solution = Get-ChildItem -LiteralPath $ProjectRoot -Recurse -File -Filter '*.sln' -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj|node_modules|\.git|packages)[\\/]' } |
        Select-Object -First 1
    $buildTarget = if ($solution) { $solution.FullName } else { $projectPath }
    Add-Check 'Solution discovery' 'PASS' $buildTarget

    $wiringTest = Join-Path $ProjectRoot 'tools\Test-DesktopButtonWiring.ps1'
    if (Test-Path -LiteralPath $wiringTest) {
        try {
            $testOutput = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $wiringTest 2>&1
            $testExit = $LASTEXITCODE
            $script:LogLines.Add('')
            $script:LogLines.Add('--- Desktop wiring test output ---')
            foreach ($line in $testOutput) { $script:LogLines.Add([string]$line) }
            if ($testExit -eq 0) {
                Add-Check 'Desktop wiring tests' 'PASS' 'Existing repository wiring test completed successfully.'
            } else {
                Add-Check 'Desktop wiring tests' 'FAIL' ('Test returned exit code ' + $testExit + '. See report for full output.')
            }
        } catch {
            Add-Check 'Desktop wiring tests' 'FAIL' $_.Exception.Message
        }
    } else {
        $routeSpecs = @(
            @{ Relative = 'src\AlSaqarAccounting\UI\ScreenRouter.cs'; Pattern = 'TryOpenAsync'; Name = 'Async screen router' },
            @{ Relative = 'src\AlSaqarAccounting\Forms\MainForm.cs'; Pattern = 'TryOpenAsync'; Name = 'Main navigation' },
            @{ Relative = 'src\AlSaqarAccounting\Forms\OrdersForm.cs'; Pattern = 'TryOpenAsync'; Name = 'Orders related navigation' },
            @{ Relative = 'src\AlSaqarAccounting\Forms\PurchasesForm.cs'; Pattern = 'TryOpenAsync'; Name = 'Purchases related navigation' },
            @{ Relative = 'src\AlSaqarAccounting\Forms\ReceiptsForm.cs'; Pattern = 'TryOpenAsync'; Name = 'Receipts related navigation' }
        )
        $routeFailures = @()
        foreach ($spec in $routeSpecs) {
            $file = Join-Path $ProjectRoot $spec.Relative
            if (-not (Test-Path -LiteralPath $file)) {
                $routeFailures += ($spec.Name + ': source file missing')
                continue
            }
            if (-not (Select-String -LiteralPath $file -Pattern $spec.Pattern -Quiet)) {
                $routeFailures += ($spec.Name + ': async route reference missing')
            }
        }
        if ($routeFailures.Count -eq 0) {
            Add-Check 'Desktop wiring tests' 'PASS' 'All required async routing markers were found.'
        } else {
            Add-Check 'Desktop wiring tests' 'FAIL' ($routeFailures -join '; ')
        }
    }

    $msbuild = Get-MSBuildPath
    $buildText = @()
    if ($msbuild) {
        try {
            $buildText = & $msbuild $buildTarget '/m' '/t:Build' '/p:Configuration=Release' '/p:Platform=Any CPU' '/nologo' 2>&1
            $buildExit = $LASTEXITCODE
            $buildOk = ($buildExit -eq 0)
            foreach ($line in $buildText) { $script:LogLines.Add([string]$line) }
            if ($buildOk) {
                Add-Check 'Release build' 'PASS' 'MSBuild completed without errors.'
            } else {
                Add-Check 'Release build' 'FAIL' ('MSBuild exit code ' + $buildExit + '. See report for diagnostics.')
            }
        } catch {
            Add-Check 'Release build' 'FAIL' $_.Exception.Message
        }
    } else {
        $dotnet = Get-Command dotnet.exe -ErrorAction SilentlyContinue
        if (-not $dotnet) { $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue }
        if ($dotnet) {
            try {
                $buildText = & $dotnet.Source build $projectPath '--configuration' 'Release' '--nologo' 2>&1
                $buildExit = $LASTEXITCODE
                $buildOk = ($buildExit -eq 0)
                foreach ($line in $buildText) { $script:LogLines.Add([string]$line) }
                if ($buildOk) {
                    Add-Check 'Release build' 'PASS' 'dotnet build completed without errors.'
                } else {
                    Add-Check 'Release build' 'FAIL' ('dotnet build exit code ' + $buildExit + '. See report for diagnostics.')
                }
            } catch {
                Add-Check 'Release build' 'FAIL' $_.Exception.Message
            }
        } else {
            Add-Check 'Release build' 'FAIL' 'MSBuild and dotnet SDK were not found. Install Visual Studio Build Tools or the .NET SDK.'
        }
    }

    $expectedExe = Join-Path (Split-Path -Parent $projectPath) 'bin\Release\net48\AlSaqarAccounting.exe'
    if (-not (Test-Path -LiteralPath $expectedExe)) {
        $exe = Get-ChildItem -LiteralPath (Split-Path -Parent $projectPath) -Recurse -File -Filter 'AlSaqarAccounting.exe' -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -match '[\\/]bin[\\/]Release[\\/]' } | Select-Object -First 1
        if ($exe) { $expectedExe = $exe.FullName }
    }
    if ($buildOk -and (Test-Path -LiteralPath $expectedExe)) {
        Add-Check 'Desktop executable' 'PASS' $expectedExe
    } elseif (-not $buildOk -and (Test-Path -LiteralPath $expectedExe)) {
        Add-Check 'Desktop executable' 'WARN' 'An executable exists from an earlier build; it is not proof the current source built.'
    } else {
        Add-Check 'Desktop executable' 'FAIL' 'The expected desktop executable was not produced.'
    }
} else {
    Add-Check 'Build and source tests' 'FAIL' 'Skipped because the project root could not be resolved.'
}

try {
    Add-Type -AssemblyName System.Data
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = "Data Source=$SqlServer;Initial Catalog=$Database;Integrated Security=True;Connect Timeout=5;Application Name=AlSaqarHealthCheck"
    $connection.Open()
    try {
        $command = $connection.CreateCommand()
        $command.CommandTimeout = 5
        $command.CommandText = 'SELECT DB_NAME();'
        $actualDatabase = [string]$command.ExecuteScalar()
        $command.CommandText = 'SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped = 0;'
        $tableCount = [int]$command.ExecuteScalar()
        if ($actualDatabase -ne $Database) {
            Add-Check 'SQL database connection' 'FAIL' ('Connected to unexpected database: ' + $actualDatabase)
        } elseif ($tableCount -eq 0) {
            Add-Check 'SQL database connection' 'WARN' ("Connected to $actualDatabase, but no user tables were found.")
        } else {
            Add-Check 'SQL database connection' 'PASS' ("Connected to $actualDatabase; user table count: $tableCount. Read-only queries only.")
        }
    } finally {
        if ($connection.State -ne 'Closed') { $connection.Close() }
        $connection.Dispose()
    }
} catch {
    Add-Check 'SQL database connection' 'FAIL' ($_.Exception.Message)
}

if (-not $SkipLaunch -and $buildOk -and $expectedExe -and (Test-Path -LiteralPath $expectedExe)) {
    try {
        $running = Get-Process -Name 'AlSaqarAccounting' -ErrorAction SilentlyContinue |
            Where-Object { $_.Path -and ([IO.Path]::GetFullPath($_.Path) -ieq [IO.Path]::GetFullPath($expectedExe)) } |
            Select-Object -First 1
        if ($running) {
            Add-Check 'Desktop startup responsiveness' 'WARN' 'The updated executable is already running; close it and rerun this test to check a fresh launch.'
        } elseif (@($script:Results | Where-Object { $_.Check -eq 'SQL database connection' -and $_.Status -eq 'FAIL' }).Count -gt 0) {
            Add-Check 'Desktop startup responsiveness' 'WARN' 'Skipped launching because the local SQL connection failed.'
        } else {
            $app = Start-Process -FilePath $expectedExe -WorkingDirectory (Split-Path -Parent $expectedExe) -PassThru
            $deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
            $handle = [IntPtr]::Zero
            do {
                Start-Sleep -Milliseconds 500
                $app.Refresh()
                if ($app.HasExited) { break }
                $handle = $app.MainWindowHandle
            } while ($handle -eq [IntPtr]::Zero -and (Get-Date) -lt $deadline)

            if ($app.HasExited) {
                Add-Check 'Desktop startup responsiveness' 'FAIL' ("Application exited during startup; exit code: " + $app.ExitCode)
            } elseif ($handle -eq [IntPtr]::Zero) {
                Add-Check 'Desktop startup responsiveness' 'FAIL' 'Application process is running but no main window appeared within the timeout.'
            } else {
                if (-not ('AlSaqarHealthCheckNativeMethods' -as [type])) {
                    Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class AlSaqarHealthCheckNativeMethods {
    [DllImport("user32.dll", SetLastError=true)]
    public static extern IntPtr SendMessageTimeout(IntPtr hWnd, UInt32 Msg, UIntPtr wParam, IntPtr lParam, UInt32 flags, UInt32 timeout, out UIntPtr result);
}
'@ -ErrorAction Stop
                }
                $messageResult = [UIntPtr]::Zero
                $response = [AlSaqarHealthCheckNativeMethods]::SendMessageTimeout($handle, 0, [UIntPtr]::Zero, [IntPtr]::Zero, 2, 2000, [ref]$messageResult)
                if ($response -ne [IntPtr]::Zero) {
                    Add-Check 'Desktop startup responsiveness' 'PASS' 'The app opened a window and responded to a Windows message. The app is left open.'
                } else {
                    Add-Check 'Desktop startup responsiveness' 'FAIL' 'A main window appeared but did not respond to a Windows message within 2 seconds. The app is left open for inspection.'
                }
            }
        }
    } catch {
        Add-Check 'Desktop startup responsiveness' 'FAIL' $_.Exception.Message
    }
} elseif ($SkipLaunch) {
    Add-Check 'Desktop startup responsiveness' 'WARN' 'Skipped by request.'
} else {
    Add-Check 'Desktop startup responsiveness' 'WARN' 'Skipped because the build did not produce a usable executable.'
}

$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$reportDirectory = if ($ProjectRoot -and (Test-Path -LiteralPath $ProjectRoot)) { $ProjectRoot } else { (Get-Location).Path }
$reportPath = Join-Path $reportDirectory ("AlSaqar-HealthCheck-$stamp.txt")
$summary = @()
$summary += 'AlSaqar Desktop - One-run Health Check'
$summary += ('Date: ' + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))
$summary += ('Project: ' + $ProjectRoot)
$summary += ('SQL Server: ' + $SqlServer)
$summary += ('Database: ' + $Database)
$summary += ''
$summary += $script:LogLines
$summary += ''
$summary += ('PASS: ' + @($script:Results | Where-Object Status -eq 'PASS').Count)
$summary += ('FAIL: ' + @($script:Results | Where-Object Status -eq 'FAIL').Count)
$summary += ('WARN: ' + @($script:Results | Where-Object Status -eq 'WARN').Count)
$summary += ''
$summary += 'Scope note: checks local build, source wiring, SQL reachability, and app startup/message responsiveness.'
$summary += 'It does not perform business transactions or claim to validate every screen action against live data.'
$summary | Out-File -LiteralPath $reportPath -Encoding UTF8

Write-Host ''
Write-Host ('Report saved to: ' + $reportPath) -ForegroundColor Cyan
Write-Host ('PASS: ' + @($script:Results | Where-Object Status -eq 'PASS').Count +
    ' | FAIL: ' + @($script:Results | Where-Object Status -eq 'FAIL').Count +
    ' | WARN: ' + @($script:Results | Where-Object Status -eq 'WARN').Count) -ForegroundColor Cyan

if (@($script:Results | Where-Object Status -eq 'FAIL').Count -gt 0) { exit 1 } else { exit 0 }

# Quick Test Script for Simple OmniParser
# Tests model loading and basic inference

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  Testing Simple OmniParser Implementation" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Check if model exists
$modelPaths = @(
    ".\FlowVision\models\icon_detect.onnx",
    ".\FlowVision\bin\Debug\models\icon_detect.onnx",
    ".\FlowVision\bin\Release\models\icon_detect.onnx"
)

$modelFound = $false
foreach ($path in $modelPaths) {
    if (Test-Path $path) {
        Write-Host "[✓] Model found at: $path" -ForegroundColor Green
        $fileSize = (Get-Item $path).Length / 1MB
        Write-Host "    Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
        $modelFound = $true
        break
    }
}

if (-not $modelFound) {
    Write-Host "[✗] Model not found!" -ForegroundColor Red
    Write-Host "    Run: .\download_omniparser_model.ps1" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host ""

# Check if project files exist
Write-Host "Checking implementation files..." -ForegroundColor Cyan
Write-Host ""

$files = @{
    "SimpleOmniParser.cs" = ".\FlowVision\lib\Classes\SimpleOmniParser.cs"
    "ScreenCaptureOmniParserPlugin.cs" = ".\FlowVision\lib\Plugins\ScreenCaptureOmniParserPlugin.cs"
}

$allFilesExist = $true
foreach ($file in $files.GetEnumerator()) {
    if (Test-Path $file.Value) {
        Write-Host "[✓] $($file.Key)" -ForegroundColor Green
    }
    else {
        Write-Host "[✗] $($file.Key) - NOT FOUND" -ForegroundColor Red
        $allFilesExist = $false
    }
}

Write-Host ""

if (-not $allFilesExist) {
    Write-Host "[✗] Some files are missing!" -ForegroundColor Red
    exit 1
}

# Check dependencies
Write-Host "Checking dependencies..." -ForegroundColor Cyan
Write-Host ""

$csprojPath = ".\FlowVision\FlowVision.csproj"
if (Test-Path $csprojPath) {
    $csproj = Get-Content $csprojPath -Raw
    
    $deps = @{
        "Microsoft.ML.OnnxRuntime" = $csproj -match "Microsoft\.ML\.OnnxRuntime"
        "System.Numerics.Tensors" = $csproj -match "System\.Numerics\.Tensors"
    }
    
    foreach ($dep in $deps.GetEnumerator()) {
        if ($dep.Value) {
            Write-Host "[✓] $($dep.Key)" -ForegroundColor Green
        }
        else {
            Write-Host "[✗] $($dep.Key) - NOT INSTALLED" -ForegroundColor Red
        }
    }
}

Write-Host ""

# Check build output
Write-Host "Checking build output..." -ForegroundColor Cyan
Write-Host ""

$exePaths = @(
    ".\FlowVision\bin\Debug\FlowVision.exe",
    ".\FlowVision\bin\Release\FlowVision.exe"
)

$exeFound = $false
foreach ($path in $exePaths) {
    if (Test-Path $path) {
        Write-Host "[✓] Executable found: $path" -ForegroundColor Green
        $fileSize = (Get-Item $path).Length / 1MB
        Write-Host "    Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
        
        $buildTime = (Get-Item $path).LastWriteTime
        $age = (Get-Date) - $buildTime
        Write-Host "    Last build: $($buildTime.ToString('yyyy-MM-dd HH:mm:ss')) ($([math]::Round($age.TotalMinutes, 1)) minutes ago)" -ForegroundColor Gray
        $exeFound = $true
        break
    }
}

if (-not $exeFound) {
    Write-Host "[!] No executable found - project needs to be built" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  Test Results" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

if ($modelFound -and $allFilesExist) {
    Write-Host "[✓] All checks passed!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Build the project in Visual Studio"
    Write-Host "  2. Run FlowVision.exe"
    Write-Host "  3. Test screen capture with OmniParser"
    Write-Host ""
    Write-Host "Expected behavior:" -ForegroundColor Cyan
    Write-Host "  - First capture: ~500ms (model loading)"
    Write-Host "  - Subsequent captures: ~200ms"
    Write-Host "  - No server startup messages"
    Write-Host "  - Direct ONNX inference"
    Write-Host ""
    exit 0
}
else {
    Write-Host "[✗] Some checks failed!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please resolve the issues above before testing." -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

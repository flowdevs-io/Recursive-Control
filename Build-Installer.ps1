<#
.SYNOPSIS
    Build FlowVision installer using Inno Setup

.DESCRIPTION
    This script builds the FlowVision installer. It requires:
    1. Inno Setup 6 to be installed
    2. The FlowVision project to be built in Release mode

.EXAMPLE
    .\Build-Installer.ps1
#>

param(
    [string]$InnoSetupPath = "",
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "FlowVision Installer Builder" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Find Inno Setup
if (-not $InnoSetupPath) {
    $locations = @(
        "C:\Program Files (x86)\Inno Setup 6\ISCC.exe",
        "C:\Program Files\Inno Setup 6\ISCC.exe",
        "C:\Program Files (x86)\Inno Setup 5\ISCC.exe",
        "C:\Program Files\Inno Setup 5\ISCC.exe"
    )
    
    foreach ($loc in $locations) {
        if (Test-Path $loc) {
            $InnoSetupPath = $loc
            break
        }
    }
}

if (-not $InnoSetupPath -or -not (Test-Path $InnoSetupPath)) {
    Write-Host "ERROR: Inno Setup not found!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Please install Inno Setup 6 from: https://jrsoftware.org/isdl.php" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Or specify the path with: .\Build-Installer.ps1 -InnoSetupPath 'C:\path\to\ISCC.exe'" -ForegroundColor Yellow
    exit 1
}

Write-Host "Using Inno Setup: $InnoSetupPath" -ForegroundColor Green

# Build the project first
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "Building FlowVision in Release mode..." -ForegroundColor Yellow
    
    $msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
    if (-not (Test-Path $msbuild)) {
        $msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe"
    }
    if (-not (Test-Path $msbuild)) {
        $msbuild = "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    }
    
    if (Test-Path $msbuild) {
        & $msbuild "FlowVision\FlowVision.csproj" /t:Build /p:Configuration=Release /v:minimal
        if ($LASTEXITCODE -ne 0) {
            Write-Host "ERROR: Build failed!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Build completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "WARNING: MSBuild not found, skipping build step" -ForegroundColor Yellow
    }
}

# Verify required files exist
Write-Host ""
Write-Host "Verifying build output..." -ForegroundColor Yellow

$requiredFiles = @(
    "FlowVision\bin\Release\FlowVision.exe",
    "FlowVision\bin\Release\onnxruntime.dll",
    "FlowVision\bin\Release\tesseract50.dll",
    "FlowVision\bin\Release\tessdata\eng.traineddata"
)

$missing = @()
foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) {
        $missing += $file
    }
}

if ($missing.Count -gt 0) {
    Write-Host "ERROR: Missing required files:" -ForegroundColor Red
    foreach ($file in $missing) {
        Write-Host "  - $file" -ForegroundColor Red
    }
    exit 1
}

Write-Host "All required files present!" -ForegroundColor Green

# Create installer output directory
$installerDir = "installer"
if (-not (Test-Path $installerDir)) {
    New-Item -ItemType Directory -Path $installerDir | Out-Null
}

# Build the installer
Write-Host ""
Write-Host "Building installer..." -ForegroundColor Yellow
Write-Host "This may take several minutes due to the large model files." -ForegroundColor Gray

$issFile = "FlowVision-Installer.iss"
& $InnoSetupPath $issFile

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: Installer build failed!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Green
Write-Host "Installer built successfully!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Green
Write-Host ""

# Show the output
$installer = Get-ChildItem "installer\*.exe" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($installer) {
    Write-Host "Installer: $($installer.FullName)" -ForegroundColor Cyan
    Write-Host "Size: $([math]::Round($installer.Length/1MB, 2)) MB" -ForegroundColor Cyan
}

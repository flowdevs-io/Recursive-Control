# OmniParser Model Downloader
# Downloads icon_detect model from HuggingFace and sets up for FlowVision

param(
    [string]$OutputPath = ".\FlowVision\models",
    [switch]$Embedded = $false
)

$ErrorActionPreference = "Stop"

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  OmniParser Model Downloader - KISS Edition" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Model URL from HuggingFace (PyTorch format - we'll need to convert to ONNX)
$modelUrl = "https://huggingface.co/microsoft/OmniParser-v2.0/resolve/main/icon_detect/model.pt"
$modelName = "model.pt"
$modelNameOnnx = "icon_detect.onnx"

# Create output directory
if (-not (Test-Path $OutputPath)) {
    Write-Host "[+] Creating directory: $OutputPath" -ForegroundColor Green
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

$outputFile = Join-Path $OutputPath $modelName

# Check if model already exists
if (Test-Path $outputFile) {
    $response = Read-Host "Model already exists at $outputFile. Overwrite? (y/N)"
    if ($response -ne 'y' -and $response -ne 'Y') {
        Write-Host "[!] Download cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Download model
Write-Host ""
Write-Host "[+] Downloading OmniParser model from HuggingFace..." -ForegroundColor Green
Write-Host "    URL: $modelUrl" -ForegroundColor Gray
Write-Host "    Destination: $outputFile" -ForegroundColor Gray
Write-Host ""
Write-Host "    This may take a few minutes (~50MB)..." -ForegroundColor Yellow
Write-Host ""

try {
    # Use WebClient for progress display
    $webClient = New-Object System.Net.WebClient
    
    # Register progress event
    Register-ObjectEvent -InputObject $webClient -EventName DownloadProgressChanged -SourceIdentifier WebClient.DownloadProgressChanged -Action {
        $percent = $EventArgs.ProgressPercentage
        Write-Progress -Activity "Downloading model..." -Status "$percent% Complete" -PercentComplete $percent
    } | Out-Null

    # Download
    $webClient.DownloadFile($modelUrl, $outputFile)
    
    # Unregister event
    Unregister-Event -SourceIdentifier WebClient.DownloadProgressChanged
    Write-Progress -Activity "Downloading model..." -Completed
    
    $webClient.Dispose()
    
    Write-Host "[✓] Download complete!" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host "[✗] Download failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Verify file
if (Test-Path $outputFile) {
    $fileSize = (Get-Item $outputFile).Length / 1MB
    Write-Host "[✓] Model file verified" -ForegroundColor Green
    Write-Host "    Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
    Write-Host "    Path: $outputFile" -ForegroundColor Gray
}
else {
    Write-Host "[✗] Model file not found after download!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  Setup Complete!" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

if ($Embedded) {
    Write-Host "Next steps (Embedded Resource Mode):" -ForegroundColor Yellow
    Write-Host "  1. Open FlowVision project in Visual Studio"
    Write-Host "  2. Right-click '$modelName' in models folder"
    Write-Host "  3. Properties → Build Action → Embedded Resource"
    Write-Host "  4. Rebuild project"
    Write-Host ""
    Write-Host "The model will be compiled into FlowVision.exe" -ForegroundColor Green
}
else {
    Write-Host "Next steps (External File Mode):" -ForegroundColor Yellow
    Write-Host "  1. Build FlowVision project"
    Write-Host "  2. Copy models folder to output directory:"
    Write-Host "     .\FlowVision\bin\Debug\models\"
    Write-Host "     .\FlowVision\bin\Release\models\"
    Write-Host ""
    Write-Host "Or run with -Embedded flag to set up embedded mode" -ForegroundColor Green
}

Write-Host ""
Write-Host "Model is ready to use!" -ForegroundColor Cyan
Write-Host ""

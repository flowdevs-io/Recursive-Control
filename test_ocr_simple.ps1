# Simple OCR Test Script
# This tests if Tesseract OCR is properly initialized and can extract text

Write-Host "=== Tesseract OCR Test ===" -ForegroundColor Cyan
Write-Host ""

$debugPath = "FlowVision\bin\Debug"

# Check prerequisites
Write-Host "Checking prerequisites..." -ForegroundColor Yellow

$checks = @{
    "FlowVision.exe" = Test-Path "$debugPath\FlowVision.exe"
    "Tesseract.dll" = Test-Path "$debugPath\Tesseract.dll"
    "tesseract50.dll" = Test-Path "$debugPath\tesseract50.dll"
    "leptonica-1.82.0.dll" = Test-Path "$debugPath\leptonica-1.82.0.dll"
    "tessdata folder" = Test-Path "$debugPath\tessdata"
    "eng.traineddata" = Test-Path "$debugPath\tessdata\eng.traineddata"
}

$allGood = $true
foreach ($check in $checks.GetEnumerator()) {
    if ($check.Value) {
        Write-Host "  ✓ $($check.Key)" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $($check.Key) MISSING!" -ForegroundColor Red
        $allGood = $false
    }
}

Write-Host ""

if ($allGood) {
    Write-Host "✓ All prerequisites satisfied!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To test OCR:" -ForegroundColor Cyan
    Write-Host "1. Run FlowVision.exe" -ForegroundColor White
    Write-Host "2. Use the OmniParser screen capture tool" -ForegroundColor White
    Write-Host "3. Capture a screenshot with text" -ForegroundColor White
    Write-Host "4. Check if element labels contain actual text" -ForegroundColor White
    Write-Host ""
    Write-Host "Expected log message:" -ForegroundColor Cyan
    Write-Host '  "✓ Tesseract OCR initialized successfully. Text extraction is now enabled."' -ForegroundColor White
} else {
    Write-Host "✗ Some prerequisites are missing!" -ForegroundColor Red
    Write-Host "Please build the project first." -ForegroundColor Yellow
}

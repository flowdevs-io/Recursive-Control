# Complete OmniParser Setup - Both Models
# Downloads and converts BOTH detection and captioning models

param(
    [switch]$CaptionModel = $false
)

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  OmniParser Complete Setup" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Check detection model
$detectionModel = ".\FlowVision\models\icon_detect.onnx"
if (Test-Path $detectionModel) {
    Write-Host "[✓] Detection model found (YOLO)" -ForegroundColor Green
    $size = (Get-Item $detectionModel).Length / 1MB
    Write-Host "    Size: $([math]::Round($size, 2)) MB" -ForegroundColor Gray
} else {
    Write-Host "[✗] Detection model NOT found!" -ForegroundColor Red
    Write-Host "    Run: python convert_omniparser_to_onnx.py" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

if ($CaptionModel) {
    Write-Host "📝 Caption Model Setup" -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "⚠️  WARNING: Caption models are LARGE and SLOW!" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Options:" -ForegroundColor White
    Write-Host "  1. BLIP-2: ~7GB, slower, more detailed captions" -ForegroundColor Gray
    Write-Host "  2. Florence: ~2GB, faster, good captions" -ForegroundColor Gray
    Write-Host ""
    Write-Host "For KISS approach, caption model is OPTIONAL." -ForegroundColor Green
    Write-Host "The AI agent can work fine with just bounding boxes!" -ForegroundColor Green
    Write-Host ""
    
    $choice = Read-Host "Download caption model? (1=BLIP-2, 2=Florence, N=Skip)"
    
    if ($choice -eq "1" -or $choice -eq "2") {
        $modelName = if ($choice -eq "1") { "icon_caption_blip2" } else { "icon_caption_florence" }
        
        Write-Host ""
        Write-Host "[+] Downloading $modelName from HuggingFace..." -ForegroundColor Green
        Write-Host ""
        
        # Download using huggingface-cli
        $cmd = "huggingface-cli download microsoft/OmniParser-v2.0 $modelName --local-dir weights"
        Write-Host "    Running: $cmd" -ForegroundColor Gray
        Invoke-Expression $cmd
        
        Write-Host ""
        Write-Host "[!] Note: Caption models are PyTorch format" -ForegroundColor Yellow
        Write-Host "    Converting to ONNX for .NET is complex and may not be worth it." -ForegroundColor Yellow
        Write-Host "    Consider using detection only for best KISS implementation!" -ForegroundColor Green
    }
} else {
    Write-Host "📝 Caption Model: SKIPPED (Recommended)" -ForegroundColor Green
    Write-Host ""
    Write-Host "You're using detection-only mode:" -ForegroundColor White
    Write-Host "  ✓ Faster inference (~200ms)" -ForegroundColor Green
    Write-Host "  ✓ Less memory (~150MB)" -ForegroundColor Green
    Write-Host "  ✓ Simpler codebase" -ForegroundColor Green
    Write-Host "  ✓ AI agent still works great!" -ForegroundColor Green
    Write-Host ""
    Write-Host "To enable captions later, run:" -ForegroundColor Cyan
    Write-Host "  .\setup_omniparser_full.ps1 -CaptionModel" -ForegroundColor Gray
}

Write-Host ""
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  Current Configuration" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Detection Model: " -NoNewline
Write-Host "ENABLED ✓" -ForegroundColor Green
Write-Host "  - Detects UI element bounding boxes" -ForegroundColor Gray
Write-Host "  - ~200ms per screenshot" -ForegroundColor Gray
Write-Host "  - ~150MB memory" -ForegroundColor Gray
Write-Host ""

Write-Host "Caption Model:   " -NoNewline
if ($CaptionModel) {
    Write-Host "ENABLED" -ForegroundColor Yellow
    Write-Host "  - Describes each element's purpose" -ForegroundColor Gray
    Write-Host "  - +500ms per screenshot" -ForegroundColor Gray
    Write-Host "  - +2GB memory" -ForegroundColor Gray
} else {
    Write-Host "DISABLED (Recommended)" -ForegroundColor Green
    Write-Host "  - Keeps it simple and fast" -ForegroundColor Gray
    Write-Host "  - AI uses coordinates + OCR instead" -ForegroundColor Gray
}

Write-Host ""
Write-Host "[✓] Setup complete!" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Build FlowVision project" -ForegroundColor White
Write-Host "  2. Set icon_detect.onnx as Embedded Resource" -ForegroundColor White
Write-Host "  3. Run and test screen capture" -ForegroundColor White
Write-Host ""

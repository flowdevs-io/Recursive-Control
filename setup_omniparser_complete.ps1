# OmniParser Model Setup - Complete Solution
# Downloads PyTorch model and converts to ONNX for .NET use

param(
    [string]$OutputPath = ".\FlowVision\models",
    [switch]$SkipConversion = $false
)

$ErrorActionPreference = "Stop"

Write-Host "===============================================" -ForegroundColor Cyan
Write-Host "  OmniParser Complete Setup - KISS Edition" -ForegroundColor Cyan
Write-Host "===============================================" -ForegroundColor Cyan
Write-Host ""

# Check if we have a pre-converted ONNX model available
$onnxModel = Join-Path $OutputPath "icon_detect.onnx"
if (Test-Path $onnxModel) {
    $response = Read-Host "Found existing ONNX model. Use it? (Y/n)"
    if ($response -eq '' -or $response -eq 'y' -or $response -eq 'Y') {
        Write-Host "[✓] Using existing ONNX model" -ForegroundColor Green
        $fileSize = (Get-Item $onnxModel).Length / 1MB
        Write-Host "    Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
        Write-Host "    Path: $onnxModel" -ForegroundColor Gray
        Write-Host ""
        Write-Host "[✓] Setup complete! Model is ready to use." -ForegroundColor Cyan
        exit 0
    }
}

Write-Host ""
Write-Host "📦 IMPORTANT: Model Format Information" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host ""
Write-Host "The OmniParser model is available in PyTorch format (.pt)" -ForegroundColor White
Write-Host "For .NET/ONNX Runtime, we need to convert it to ONNX format." -ForegroundColor White
Write-Host ""
Write-Host "Options:" -ForegroundColor Cyan
Write-Host "  1. Download pre-converted ONNX model (recommended)" -ForegroundColor Green
Write-Host "  2. Download PyTorch and convert manually" -ForegroundColor Yellow
Write-Host ""

# Option 1: Try to download pre-converted ONNX
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "Option 1: Checking for pre-converted ONNX model..." -ForegroundColor Cyan
Write-Host ""

# Create output directory
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Try common ONNX model locations
$onnxUrls = @(
    "https://huggingface.co/microsoft/OmniParser/resolve/main/icon_detect/model.onnx",
    "https://huggingface.co/microsoft/OmniParser-v2.0/resolve/main/icon_detect/model.onnx"
)

$onnxDownloaded = $false
foreach ($url in $onnxUrls) {
    Write-Host "[*] Trying: $url" -ForegroundColor Gray
    try {
        $webClient = New-Object System.Net.WebClient
        $webClient.DownloadFile($url, $onnxModel)
        $webClient.Dispose()
        
        if (Test-Path $onnxModel) {
            Write-Host "[✓] Successfully downloaded ONNX model!" -ForegroundColor Green
            $onnxDownloaded = $true
            break
        }
    }
    catch {
        Write-Host "[✗] Not available at this location" -ForegroundColor DarkGray
    }
}

if ($onnxDownloaded) {
    $fileSize = (Get-Item $onnxModel).Length / 1MB
    Write-Host ""
    Write-Host "[✓] Model ready!" -ForegroundColor Green
    Write-Host "    Format: ONNX" -ForegroundColor Gray
    Write-Host "    Size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Gray
    Write-Host "    Path: $onnxModel" -ForegroundColor Gray
    Write-Host ""
    Write-Host "===============================================" -ForegroundColor Cyan
    Write-Host "  Setup Complete!" -ForegroundColor Cyan
    Write-Host "===============================================" -ForegroundColor Cyan
    exit 0
}

# Option 2: Manual conversion required
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host "Option 2: Manual Conversion Required" -ForegroundColor Yellow
Write-Host ""
Write-Host "No pre-converted ONNX model found." -ForegroundColor Yellow
Write-Host ""
Write-Host "To convert the PyTorch model to ONNX:" -ForegroundColor White
Write-Host ""
Write-Host "1. Install Python dependencies:" -ForegroundColor Cyan
Write-Host "   pip install torch onnx ultralytics" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Download the PyTorch model:" -ForegroundColor Cyan
Write-Host "   huggingface-cli download microsoft/OmniParser-v2.0 icon_detect/model.pt --local-dir weights" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Convert to ONNX using Python:" -ForegroundColor Cyan
Write-Host ""
Write-Host "   import torch" -ForegroundColor Gray
Write-Host "   from ultralytics import YOLO" -ForegroundColor Gray
Write-Host ""  
Write-Host "   # Load PyTorch model" -ForegroundColor Gray
Write-Host "   model = YOLO('weights/icon_detect/model.pt')" -ForegroundColor Gray
Write-Host "   # Export to ONNX" -ForegroundColor Gray
Write-Host "   model.export(format='onnx', simplify=True)" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Copy the resulting icon_detect.onnx to:" -ForegroundColor Cyan
Write-Host "   $OutputPath\icon_detect.onnx" -ForegroundColor Gray
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Yellow
Write-Host ""

# Offer to create a conversion script
$createScript = Read-Host "Would you like me to create a Python conversion script? (Y/n)"
if ($createScript -eq '' -or $createScript -eq 'y' -or $createScript -eq 'Y') {
    $scriptContent = @"
#!/usr/bin/env python3
"""
OmniParser PyTorch to ONNX Converter
Converts the OmniParser YOLO model from PyTorch (.pt) to ONNX format
for use with .NET ONNX Runtime
"""

import sys
from pathlib import Path

try:
    from ultralytics import YOLO
except ImportError:
    print("[✗] Error: ultralytics not installed")
    print("    Install with: pip install ultralytics")
    sys.exit(1)

def convert_to_onnx(pt_model_path, output_path):
    """Convert PyTorch YOLO model to ONNX"""
    print("=" * 60)
    print("  OmniParser PyTorch → ONNX Converter")
    print("=" * 60)
    print()
    
    pt_path = Path(pt_model_path)
    if not pt_path.exists():
        print(f"[✗] Error: Model not found at {pt_path}")
        print(f"    Download with:")
        print(f"    huggingface-cli download microsoft/OmniParser-v2.0 \\")
        print(f"        icon_detect/model.pt --local-dir weights")
        sys.exit(1)
    
    print(f"[+] Loading PyTorch model from: {pt_path}")
    try:
        model = YOLO(str(pt_path))
    except Exception as e:
        print(f"[✗] Failed to load model: {e}")
        sys.exit(1)
    
    print("[+] Model loaded successfully")
    print(f"[+] Converting to ONNX format...")
    print(f"    Output: {output_path}")
    print()
    
    try:
        # Export with simplification for better performance
        model.export(
            format='onnx',
            simplify=True,
            opset=12,  # Compatible with most ONNX runtimes
            dynamic=False,  # Static shapes for better performance
            imgsz=640  # Fixed input size
        )
        
        # The export creates a file next to the input with .onnx extension
        generated_onnx = pt_path.with_suffix('.onnx')
        
        if generated_onnx.exists():
            # Move to desired location
            import shutil
            shutil.move(str(generated_onnx), output_path)
            
            import os
            file_size = os.path.getsize(output_path) / (1024 * 1024)
            
            print()
            print("[✓] Conversion successful!")
            print(f"    ONNX model: {output_path}")
            print(f"    Size: {file_size:.2f} MB")
            print()
            print("You can now use this model with FlowVision!")
            
        else:
            print("[✗] ONNX file not found after export")
            sys.exit(1)
            
    except Exception as e:
        print(f"[✗] Conversion failed: {e}")
        sys.exit(1)

if __name__ == "__main__":
    pt_model = "weights/icon_detect/model.pt"
    onnx_output = "FlowVision/models/icon_detect.onnx"
    
    if len(sys.argv) > 1:
        pt_model = sys.argv[1]
    if len(sys.argv) > 2:
        onnx_output = sys.argv[2]
    
    convert_to_onnx(pt_model, onnx_output)
"@

    $scriptPath = "convert_omniparser_to_onnx.py"
    $scriptContent | Out-File -FilePath $scriptPath -Encoding UTF8
    Write-Host "[✓] Created conversion script: $scriptPath" -ForegroundColor Green
    Write-Host ""
    Write-Host "Run it with: python $scriptPath" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "Alternative: Use Pre-converted ONNX" -ForegroundColor Cyan
Write-Host ""
Write-Host "If you have access to a pre-converted ONNX model," -ForegroundColor White
Write-Host "simply place it at:" -ForegroundColor White
Write-Host "  $OutputPath\icon_detect.onnx" -ForegroundColor Cyan
Write-Host ""
Write-Host "The SimpleOmniParser will automatically detect and use it!" -ForegroundColor Green
Write-Host ""

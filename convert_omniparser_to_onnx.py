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
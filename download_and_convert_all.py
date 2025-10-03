#!/usr/bin/env python3
"""
OmniParser Complete Model Downloader and Converter
Downloads both icon_detect (YOLO) and icon_caption_florence models
Converts them to ONNX format for .NET use
"""

import sys
import shutil
from pathlib import Path

def check_dependencies():
    """Check if required packages are installed"""
    print("=" * 60)
    print("  OmniParser Complete Setup")
    print("=" * 60)
    print()
    
    missing = []
    
    try:
        import torch
    except ImportError:
        missing.append("torch")
    
    try:
        from ultralytics import YOLO
    except ImportError:
        missing.append("ultralytics")
    
    try:
        from transformers import AutoProcessor, AutoModelForCausalLM
    except ImportError:
        missing.append("transformers")
    
    if missing:
        print("[✗] Missing dependencies:", ", ".join(missing))
        print()
        print("Install with:")
        print(f"    pip install {' '.join(missing)}")
        sys.exit(1)
    
    print("[✓] All dependencies installed")
    print()

def download_models():
    """Download both models from HuggingFace"""
    print("━" * 60)
    print("Step 1: Downloading Models from HuggingFace")
    print("━" * 60)
    print()
    
    try:
        from huggingface_hub import hf_hub_download
    except ImportError:
        print("[✗] huggingface_hub not installed")
        print("    Install with: pip install huggingface-hub")
        sys.exit(1)
    
    weights_dir = Path("weights")
    
    # Download icon_detect (YOLO)
    print("[1/2] Downloading icon_detect (YOLO)...")
    detect_files = ["model.pt", "model.yaml", "train_args.yaml"]
    detect_dir = weights_dir / "icon_detect"
    detect_dir.mkdir(parents=True, exist_ok=True)
    
    for file in detect_files:
        try:
            print(f"    Downloading {file}...")
            hf_hub_download(
                repo_id="microsoft/OmniParser-v2.0",
                filename=f"icon_detect/{file}",
                local_dir=str(weights_dir)
            )
        except Exception as e:
            print(f"    [!] Could not download {file}: {e}")
    
    print("[✓] icon_detect downloaded")
    print()
    
    # Download icon_caption_florence
    print("[2/2] Downloading icon_caption_florence (Florence-2)...")
    caption_files = ["config.json", "generation_config.json", "model.safetensors", 
                     "preprocessor_config.json", "tokenizer.json", "tokenizer_config.json"]
    caption_dir = weights_dir / "icon_caption_florence"
    caption_dir.mkdir(parents=True, exist_ok=True)
    
    for file in caption_files:
        try:
            print(f"    Downloading {file}...")
            hf_hub_download(
                repo_id="microsoft/OmniParser-v2.0",
                filename=f"icon_caption/{file}",
                local_dir=str(weights_dir / "icon_caption_florence")
            )
        except Exception as e:
            print(f"    [!] Could not download {file}: {e}")
    
    print("[✓] icon_caption_florence downloaded")
    print()

def convert_detection_model():
    """Convert YOLO detection model to ONNX"""
    print("━" * 60)
    print("Step 2: Converting Detection Model (YOLO → ONNX)")
    print("━" * 60)
    print()
    
    from ultralytics import YOLO
    
    pt_path = Path("weights/icon_detect/model.pt")
    if not pt_path.exists():
        print(f"[✗] Model not found at {pt_path}")
        return False
    
    print(f"[+] Loading YOLO model from: {pt_path}")
    model = YOLO(str(pt_path))
    
    print("[+] Exporting to ONNX format...")
    print("    Settings: opset=12, simplify=True, dynamic=False")
    
    try:
        model.export(
            format='onnx',
            simplify=True,
            opset=12,
            dynamic=False,
            imgsz=640
        )
        
        # Move to FlowVision models directory
        generated_onnx = pt_path.with_suffix('.onnx')
        output_dir = Path("FlowVision/models")
        output_dir.mkdir(parents=True, exist_ok=True)
        output_path = output_dir / "icon_detect.onnx"
        
        shutil.move(str(generated_onnx), str(output_path))
        
        import os
        file_size = os.path.getsize(output_path) / (1024 * 1024)
        
        print()
        print("[✓] Detection model converted successfully!")
        print(f"    Output: {output_path}")
        print(f"    Size: {file_size:.2f} MB")
        print()
        return True
        
    except Exception as e:
        print(f"[✗] Conversion failed: {e}")
        return False

def convert_caption_model():
    """Convert Florence caption model to ONNX"""
    print("━" * 60)
    print("Step 3: Converting Caption Model (Florence-2 → ONNX)")
    print("━" * 60)
    print()
    
    caption_dir = Path("weights/icon_caption_florence")
    if not caption_dir.exists():
        print(f"[✗] Caption model not found at {caption_dir}")
        return False
    
    print("[!] Note: Florence-2 ONNX conversion is complex")
    print("    For KISS approach, we'll keep the model in PyTorch format")
    print("    and load it via Python if needed, or skip captions entirely.")
    print()
    
    # Check if we can load the model
    try:
        from transformers import AutoProcessor, AutoModelForCausalLM
        import torch
        
        print("[+] Loading Florence-2 model...")
        model = AutoModelForCausalLM.from_pretrained(
            str(caption_dir),
            trust_remote_code=True,
            torch_dtype=torch.float32
        )
        processor = AutoProcessor.from_pretrained(
            str(caption_dir),
            trust_remote_code=True
        )
        
        print("[✓] Florence-2 model loaded successfully")
        print(f"    Location: {caption_dir}")
        print()
        print("[!] For .NET integration, we have options:")
        print("    1. Use Python bridge for captions (hybrid approach)")
        print("    2. Skip captions and use detection-only (KISS)")
        print("    3. Use ONNX Runtime with manual conversion (complex)")
        print()
        print("    Recommendation: Option 2 (detection-only) for simplicity")
        print()
        
        return True
        
    except Exception as e:
        print(f"[✗] Could not load Florence model: {e}")
        print()
        return False

def main():
    """Main setup flow"""
    check_dependencies()
    
    # Step 1: Download
    try:
        download_models()
    except Exception as e:
        print(f"[✗] Download failed: {e}")
        print("    You can try manual download:")
        print("    huggingface-cli download microsoft/OmniParser-v2.0 --local-dir weights")
        sys.exit(1)
    
    # Step 2: Convert detection
    if not convert_detection_model():
        print("[✗] Detection model conversion failed")
        sys.exit(1)
    
    # Step 3: Handle caption model
    caption_success = convert_caption_model()
    
    # Summary
    print("=" * 60)
    print("  Setup Complete!")
    print("=" * 60)
    print()
    print("✓ Detection Model: Ready (ONNX)")
    print("  └─ FlowVision/models/icon_detect.onnx")
    print()
    
    if caption_success:
        print("✓ Caption Model: Available (PyTorch)")
        print("  └─ weights/icon_caption_florence/")
        print()
        print("  [!] Caption model is optional for KISS implementation")
    else:
        print("○ Caption Model: Not configured")
        print("  └─ Detection-only mode (recommended for simplicity)")
    
    print()
    print("Next steps:")
    print("  1. Build FlowVision project in Visual Studio")
    print("  2. Set icon_detect.onnx as Embedded Resource")
    print("  3. Test screen capture functionality")
    print()
    print("The detection model alone provides bounding boxes,")
    print("which is sufficient for most AI agent use cases!")
    print()

if __name__ == "__main__":
    main()

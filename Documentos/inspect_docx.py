import zipfile
import re
import os

files = [
    r"C:\Proyectos\Tecmein\Documentos\CONTRATO NUEVO - M1.docx",
    r"C:\Proyectos\Tecmein\Documentos\PreContrato_COT-80_20260202003605.docx"
]

keywords = ["dias", "entrega", "{{"]

for file_path in files:
    print(f"\n\n==================================================")
    print(f"ANALYZING: {os.path.basename(file_path)}")
    print(f"==================================================")
    
    if not os.path.exists(file_path):
        print("File not found.")
        continue

    try:
        with zipfile.ZipFile(file_path, 'r') as docx:
            xml_content = docx.read('word/document.xml').decode('utf-8')
            
            print(f"XML Size: {len(xml_content)} chars")
            
            for kw in keywords:
                matches = [m.start() for m in re.finditer(re.escape(kw), xml_content, re.IGNORECASE)]
                print(f"\nKeyword '{kw}' found {len(matches)} times.")
                
                for idx in matches[:5]: # Show first 5 matches context
                    start = max(0, idx - 100)
                    end = min(len(xml_content), idx + 300)
                    snippet = xml_content[start:end]
                    print(f"   Context @ {idx}: ...{snippet}...")

    except Exception as e:
        print(f"Error reading docx: {e}")

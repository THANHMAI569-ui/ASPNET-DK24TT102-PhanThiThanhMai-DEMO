#!/usr/bin/env bash
# Ghép ba phần thành báo cáo hoàn chỉnh rồi xuất .docx và .pdf.
# Chạy từ thư mục thesis/doc:  bash build-report.sh
set -euo pipefail

OUT_MD="bao-cao-day-du.md"
OUT_DOCX="bao-cao-day-du.docx"
OUT_PDF="../pdf/bao-cao-day-du.pdf"

# Phần 1: bỏ danh mục tài liệu tham khảo tạm (đã có bản đầy đủ ở Phần 3).
head -n 666 phan-1-tom-tat-mo-dau-chuong-1-2.md > "$OUT_MD"

# Phần 2 và 3: bỏ khối YAML ở đầu tệp (5 dòng đầu).
{ printf '\n\\newpage\n\n'; tail -n +6 phan-2-chuong-3-4.md; } >> "$OUT_MD"
{ printf '\n\\newpage\n\n'; tail -n +6 phan-3-chuong-5-tltk-phu-luc.md; } >> "$OUT_MD"

# Tiêu đề chung cho bản ghép.
perl -0pi -e 's/^subtitle: .*\n/subtitle: "Báo cáo đồ án tốt nghiệp"\n/m' "$OUT_MD"

python3 fit-images.py "$OUT_MD"

pandoc "$OUT_MD" -o "$OUT_DOCX" \
    --toc --toc-depth=2 \
    --metadata toc-title="MỤC LỤC" \
    --resource-path=.:..:../images:../diagrams

soffice --headless --convert-to pdf --outdir ../pdf "$OUT_DOCX" >/dev/null 2>&1
mv -f "../pdf/${OUT_DOCX%.docx}.pdf" "$OUT_PDF" 2>/dev/null || true

echo "Đã xuất: $OUT_DOCX và $OUT_PDF"

#!/usr/bin/env bash
# Ghép front matter + ba phần thành báo cáo hoàn chỉnh theo mẫu của khoa,
# rồi xuất .docx và .pdf. Chạy từ thư mục thesis/doc:  bash build-report.sh
#
# Quy trình hai lượt:
#   Lượt 1: dựng với placeholder mục lục / danh mục hình / danh mục bảng.
#   fill-front.py đọc PDF lượt 1, tính số trang logic của phần thân (phần thân
#   được đánh số lại từ 1 nên việc front matter dài ra không làm lệch số).
#   Lượt 2: dựng lại với các danh mục đã điền số trang.
set -euo pipefail

OUT_MD="bao-cao-day-du.md"
OUT_DOCX="bao-cao-day-du.docx"
OUT_PDF="../pdf/bao-cao-day-du.pdf"
TEMPLATE="truong-template.docx"

build() {
    pandoc "$OUT_MD" -o "$OUT_DOCX" \
        --reference-doc="$TEMPLATE" \
        --resource-path=.:..:../images:../diagrams
    soffice --headless --convert-to pdf --outdir ../pdf "$OUT_DOCX" >/dev/null 2>&1
}

merge() {
    # Front matter (bìa, nhận xét, lời cảm ơn, các danh mục) đứng trước.
    cat front-matter.md > "$OUT_MD"

    # Phần 1: bỏ danh mục tài liệu tham khảo tạm (đã có bản đầy đủ ở Phần 3),
    # bỏ khối YAML (tiêu đề đã nằm trên bìa).
    awk 'NR<=5 {next} /^# TÀI LIỆU THAM KHẢO \(phần 1\)/{exit} {print}' \
        phan-1-tom-tat-mo-dau-chuong-1-2.md >> "$OUT_MD"

    # Phần 2 và 3: bỏ khối YAML ở đầu tệp (5 dòng đầu).
    { tail -n +6 phan-2-chuong-3-4.md; } >> "$OUT_MD"
    { printf '\n\\newpage\n\n'; tail -n +6 phan-3-chuong-5-tltk-phu-luc.md; } >> "$OUT_MD"

    python3 page-breaks.py "$OUT_MD"
    python3 fit-images.py "$OUT_MD"
}

merge
build                       # lượt 1: đo
python3 fill-front.py       # điền mục lục + danh mục theo số trang thật
build                       # lượt 2: bản cuối

echo "Đã xuất: $OUT_DOCX và $OUT_PDF"

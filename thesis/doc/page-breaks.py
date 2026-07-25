#!/usr/bin/env python3
"""Chèn ngắt trang OOXML thật trước mỗi heading cấp 1.

pandoc bỏ qua \\newpage khi xuất docx, nên nếu chỉ dựa vào \\newpage thì các
chương chảy liền nhau. Script xóa mọi dòng \\newpage rồi chèn một đoạn raw
OOXML ngắt trang trước từng dòng bắt đầu bằng "# " (bỏ qua nội dung nằm trong
khối mã ``` để không đụng các dòng chú thích shell).
"""
import sys
from pathlib import Path

BREAK = '```{=openxml}\n<w:p><w:r><w:br w:type="page"/></w:r></w:p>\n```\n'

md = Path(sys.argv[1])
out, fence, first_heading = [], False, True
lines = md.read_text(encoding="utf-8").splitlines(keepends=True)
for line in lines:
    if line.lstrip().startswith("```"):
        fence = not fence
        out.append(line)
        continue
    if not fence and line.strip() == r"\newpage":
        continue  # bỏ, ngắt trang do heading đảm nhiệm
    # TÓM TẮT đứng ngay sau ngắt section của front matter; chèn thêm break ở
    # đây sẽ tạo một trang trắng mang số 1.
    if not fence and line.startswith("# ") and not line.startswith("# TÓM TẮT"):
        if first_heading:
            first_heading = False  # heading đầu (sau bìa raw) đã có break từ khối bìa? Không - bìa kết thúc không break, nên vẫn chèn.
        out.append(BREAK)
    out.append(line)
md.write_text("".join(out), encoding="utf-8")
print(f"  Đã chèn ngắt trang trước {sum(1 for l in lines if l.startswith('# '))} heading cấp 1")

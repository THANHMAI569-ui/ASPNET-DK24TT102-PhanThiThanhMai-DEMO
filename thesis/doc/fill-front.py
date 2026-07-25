#!/usr/bin/env python3
"""Điền MỤC LỤC và DANH MỤC HÌNH/BẢNG với số trang thật.

Cách hoạt động: build-report.sh dựng lượt 1 với placeholder, script này đọc PDF
lượt 1 bằng pdftotext, xác định trang bắt đầu phần thân (TÓM TẮT ĐỒ ÁN) rồi quy
mọi vị trí về số trang LOGIC của phần thân (phần thân được Word đánh lại từ 1).
Nhờ đánh lại số nên việc các trang đầu dài ra sau khi điền không làm lệch số.

Kết quả ghi đè các placeholder <<MUC_LUC>>, <<DANH_MUC_HINH>>,
<<DANH_MUC_BANG>> trong bao-cao-day-du.md; build-report.sh dựng lượt 2.
"""

import re
import subprocess
import sys
from pathlib import Path

MD = Path("bao-cao-day-du.md")
PDF = Path("../pdf/bao-cao-day-du.pdf")
WIDTH = 88  # tổng độ rộng dòng mục lục tính theo ký tự, canh bằng dấu chấm


def pdf_pages() -> list[str]:
    out = subprocess.run(["pdftotext", str(PDF), "-"], capture_output=True, text=True, check=True)
    return out.stdout.split("\f")


def find_page(pages, needle, start=0):
    for i in range(start, len(pages)):
        if needle in pages[i]:
            return i + 1  # 1-based physical
    return None


def leader(label, page, indent=""):
    text = f"{indent}{label}"
    dots = "." * max(3, WIDTH - len(text) - len(str(page)))
    # Escape ký tự markdown trong nhãn: chỉ cần giữ nguyên vì nhãn là chữ thường.
    return f"{text} {dots} {page}"


def main() -> int:
    md = MD.read_text(encoding="utf-8")
    if "<<MUC_LUC>>" not in md:
        sys.exit("Không thấy placeholder; đã điền rồi?")

    pages = pdf_pages()

    body_start = find_page(pages, "TÓM TẮT ĐỒ ÁN")
    if body_start is None:
        sys.exit("Không tìm thấy TÓM TẮT ĐỒ ÁN trong PDF lượt 1")
    front = body_start - 1  # số trang front matter ở lượt 1

    def logical(physical):
        return physical - front

    # --- Mục lục: heading cấp # và ## trong PHẦN THÂN của markdown -----------
    body_md = md[md.index("# TÓM TẮT ĐỒ ÁN"):]
    toc_lines = []
    cursor = 0
    for m in re.finditer(r"^(#{1,2}) (.+)$", body_md, flags=re.M):
        level, title = len(m.group(1)), m.group(2).strip()
        # Trong PDF, heading nằm trên một dòng riêng.
        phys = find_page(pages, title.split("\n")[0][:60], start=max(cursor - 1, 0))
        if phys is None:
            continue
        cursor = phys
        indent = "" if level == 1 else "\\ \\ \\ \\ "
        label = title if level == 1 else title
        toc_lines.append(leader(label, logical(phys), indent) + "\\")
    toc = "\n".join(toc_lines).rstrip("\\")

    # --- Danh mục hình --------------------------------------------------------
    fig_lines = []
    cursor = 0
    for m in re.finditer(r"^\*(Hình \d+\.\d+)\. (.+?)\*$", md, flags=re.M):
        num, cap = m.group(1), m.group(2)
        phys = find_page(pages, f"{num}. {cap}"[:60], start=max(cursor - 1, 0))
        if phys is None:
            continue
        cursor = phys
        fig_lines.append(leader(f"{num}. {cap}", logical(phys)) + "\\")
    figs = "\n".join(fig_lines).rstrip("\\")

    # --- Danh mục bảng --------------------------------------------------------
    tab_lines = []
    cursor = 0
    for m in re.finditer(r"^\*(Bảng \d+\.\d+)\. (.+?)\*$", md, flags=re.M):
        num, cap = m.group(1), m.group(2)
        phys = find_page(pages, f"{num}. {cap}"[:60], start=max(cursor - 1, 0))
        if phys is None:
            continue
        cursor = phys
        tab_lines.append(leader(f"{num}. {cap}", logical(phys)) + "\\")
    tabs = "\n".join(tab_lines).rstrip("\\")

    md = md.replace("<<MUC_LUC>>", toc)
    md = md.replace("<<DANH_MUC_HINH>>", figs)
    md = md.replace("<<DANH_MUC_BANG>>", tabs)
    MD.write_text(md, encoding="utf-8")
    print(f"  Front lượt 1: {front} trang · mục lục {len(toc_lines)} dòng ·"
          f" {len(fig_lines)} hình · {len(tab_lines)} bảng")
    return 0


if __name__ == "__main__":
    sys.exit(main())

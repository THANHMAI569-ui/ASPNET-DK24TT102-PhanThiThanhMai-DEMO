#!/usr/bin/env python3
"""Gán chiều rộng cho từng ảnh trong Markdown để không ảnh nào tràn trang.

Pandoc mặc định kéo ảnh ra hết chiều ngang khung chữ và giữ nguyên tỉ lệ, nên một
ảnh cao (ví dụ lưu đồ tỉ lệ cao/rộng = 2.5) sẽ chiếm hơn hai trang A4. Script đọc
kích thước thật của từng tệp rồi tính chiều rộng sao cho chiều cao không vượt
MAX_H_CM, đồng thời chiều rộng không vượt MAX_W_CM.
"""

import re
import struct
import sys
from pathlib import Path

MAX_W_CM = 12.5   # be ngang khung chu A4 le 2,5cm la 16cm; chua lai cho thoang
MAX_H_CM = 8.5    # cao hon muc nay thi anh chiem gan tron mot trang


def png_size(data: bytes):
    # IHDR luôn là chunk đầu tiên: 8 byte chữ ký + 4 byte length + 4 byte type
    w, h = struct.unpack(">II", data[16:24])
    return w, h


def jpeg_size(data: bytes):
    i = 2
    while i < len(data):
        if data[i] != 0xFF:
            i += 1
            continue
        marker = data[i + 1]
        # SOF0..SOF15, trừ DHT(C4), JPG(C8) và DAC(CC)
        if 0xC0 <= marker <= 0xCF and marker not in (0xC4, 0xC8, 0xCC):
            h, w = struct.unpack(">HH", data[i + 5:i + 9])
            return w, h
        seg_len = struct.unpack(">H", data[i + 2:i + 4])[0]
        i += 2 + seg_len
    raise ValueError("khong doc duoc kich thuoc JPEG")


def image_size(path: Path):
    data = path.read_bytes()
    if data[:8] == b"\x89PNG\r\n\x1a\n":
        return png_size(data)
    return jpeg_size(data)


def main(md_path: str) -> int:
    md = Path(md_path)
    text = md.read_text(encoding="utf-8")
    base = md.parent
    changed = 0

    def repl(match):
        nonlocal changed
        alt, target = match.group(1), match.group(2)
        if "{" in match.group(0).split(")")[-1][:1]:
            return match.group(0)

        path = (base / target).resolve()
        if not path.exists():
            print(f"  CANH BAO: khong tim thay {target}", file=sys.stderr)
            return match.group(0)

        w, h = image_size(path)
        ratio = h / w
        width_cm = min(MAX_W_CM, MAX_H_CM / ratio)
        changed += 1
        return f"![{alt}]({target}){{width={width_cm:.1f}cm}}"

    text = re.sub(r"!\[([^\]]*)\]\(([^)]+)\)(?!\{)", repl, text)
    md.write_text(text, encoding="utf-8")
    print(f"  Da gan chieu rong cho {changed} anh")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1]))

#!/usr/bin/env bash
# ============================================================
#  Bien dich bao cao LaTeX -> thesis/pdf/bao-cao-day-du.pdf
#
#  Yeu cau: TeX Live co xelatex (mac: /Library/TeX/texbin)
#           font he thong "Times New Roman" va "Courier New"
#
#  Cach dung:  ./build.sh          bien dich va copy sang thesis/pdf/
#              ./build.sh --clean  xoa file trung gian roi thoat
# ============================================================
set -euo pipefail

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$HERE"

BUILD_DIR="build"
MAIN="thesis"
OUT="../pdf/bao-cao-day-du.pdf"

if [[ "${1:-}" == "--clean" ]]; then
  rm -rf "$BUILD_DIR"
  echo "Da xoa $BUILD_DIR"
  exit 0
fi

export PATH="/Library/TeX/texbin:$PATH"

if ! command -v xelatex >/dev/null 2>&1; then
  echo "Khong tim thay xelatex. Cai TeX Live hoac MacTeX truoc." >&2
  exit 1
fi

mkdir -p "$BUILD_DIR" ../pdf

run_pass() {
  echo ">>> Luot $1"
  xelatex -interaction=nonstopmode -halt-on-error \
          -output-directory="$BUILD_DIR" "$MAIN.tex" >/dev/null
}

# latexmk tu quyet dinh so luot can thiet; neu khong co thi chay tay 3 luot
# (luot 1 sinh .aux/.toc, luot 2 dien so trang, luot 3 on dinh cross-reference).
if command -v latexmk >/dev/null 2>&1; then
  echo ">>> latexmk (xelatex)"
  latexmk -xelatex -interaction=nonstopmode -halt-on-error \
          -outdir="$BUILD_DIR" "$MAIN.tex" >/dev/null
else
  run_pass 1
  run_pass 2
  run_pass 3
fi

cp "$BUILD_DIR/$MAIN.pdf" "$OUT"
echo "Xong: $(cd .. && pwd)/pdf/bao-cao-day-du.pdf"

# Canh bao con lai trong log, de kiem tra nhanh
echo
echo "--- Kiem tra log ---"
grep -c "undefined" "$BUILD_DIR/$MAIN.log" | sed 's/^/Tham chieu chua xac dinh: /' || true
grep -c "Missing character" "$BUILD_DIR/$MAIN.log" | sed 's/^/Ky tu thieu glyph: /' || true
grep -c "Overfull" "$BUILD_DIR/$MAIN.log" | sed 's/^/Overfull box: /' || true

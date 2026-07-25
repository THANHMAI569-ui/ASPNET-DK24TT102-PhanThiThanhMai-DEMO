# Tài liệu báo cáo

> **Bản chính từ 26/07/2026 là bản LaTeX ở `thesis/latex/`.**
> `thesis/pdf/bao-cao-day-du.pdf` được sinh từ `thesis/latex/thesis.tex`
> (chạy `thesis/latex/build.sh`, dùng XeLaTeX). Toàn bộ pipeline Markdown +
> pandoc mô tả bên dưới được **giữ lại làm lưu trữ**, không còn là nguồn để
> sửa nội dung. Nếu sửa nội dung báo cáo thì sửa trong `thesis/latex/`.

## Bản chính: LaTeX

| Tệp | Nội dung |
|---|---|
| `../latex/thesis.tex` | Tệp chính, chứa preamble và gọi các phần |
| `../latex/frontmatter/` | Bìa chính, bìa phụ, 2 trang nhận xét, lời cảm ơn, mục lục |
| `../latex/chapters/` | Tóm tắt, Mở đầu, Chương 1-5, TLTK, Phụ lục |
| `../latex/figures/` | 17 sơ đồ vẽ bằng TikZ, trắng đen, vector |
| `../latex/build.sh` | Biên dịch và chép sang `../pdf/bao-cao-day-du.pdf` |

```bash
cd thesis/latex && ./build.sh
```

Yêu cầu: TeX Live có `xelatex` và font hệ thống **Times New Roman**,
**Courier New**.

---

## Lưu trữ: pipeline Markdown + pandoc (không còn dùng)

## Bản để nộp

| Tệp | Nội dung |
|---|---|
| `bao-cao-day-du.docx` | **Bản đầy đủ**, ghép cả ba phần, có mục lục tự động |

## Bản nguồn để sửa

Nội dung viết bằng Markdown, chia ba phần cho dễ sửa:

| Tệp nguồn | Nội dung | Bản .docx riêng |
|---|---|---|
| `phan-1-tom-tat-mo-dau-chuong-1-2.md` | Tóm tắt, Mở đầu, Chương 1, Chương 2 | có |
| `phan-2-chuong-3-4.md` | Chương 3, Chương 4 | có |
| `phan-3-chuong-5-tltk-phu-luc.md` | Chương 5, Tài liệu tham khảo, Phụ lục | có |

`bao-cao-day-du.md` là tệp **sinh tự động** khi ghép ba phần, không sửa trực tiếp
vào tệp này vì lần dựng sau sẽ ghi đè.

## Dựng lại bản đầy đủ sau khi sửa

```bash
cd thesis/doc
bash build-report.sh
```

Script làm ba việc: ghép ba phần (bỏ danh mục tài liệu tham khảo tạm ở Phần 1),
gọi `fit-images.py` để gán chiều rộng cho từng ảnh, rồi xuất `.docx` bằng pandoc
và `.pdf` bằng LibreOffice.

Yêu cầu công cụ: `pandoc` và `soffice` (LibreOffice), cùng `python3`.

## Vì sao cần fit-images.py

Pandoc mặc định kéo ảnh ra hết chiều ngang khung chữ và giữ nguyên tỉ lệ. Một số
sơ đồ rất cao (lưu đồ thuật toán sinh thực đơn có tỉ lệ cao trên rộng là 2,49) sẽ
chiếm hơn hai trang A4 nếu để mặc định. Script đọc kích thước thật của từng ảnh
rồi tính chiều rộng sao cho chiều cao không vượt ngưỡng đặt trong hai hằng số
`MAX_W_CM` và `MAX_H_CM` ở đầu tệp.

Đổi hai hằng số đó là cách nhanh nhất để tăng hoặc giảm tổng số trang.

## Tệp phụ trợ

- `danh-muc-hinh-chuong-4.md` - danh mục hình kèm chú thích đề xuất và số liệu
  thật đo được lúc chụp, dùng khi viết lời bình cho Chương 4.

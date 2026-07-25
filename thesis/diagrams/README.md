# Sơ đồ cho báo cáo

Mỗi sơ đồ có hai tệp: `.mmd` là mã nguồn Mermaid, `.png` là ảnh đã render
(nền trắng, tỉ lệ 2x) dùng để chèn vào báo cáo.

Toàn bộ sơ đồ vẽ **trắng đen** theo yêu cầu trình bày của khoa; bảng màu nằm
trong `bw-config.json` và được truyền vào mermaid-cli qua tham số `-c`.

| Tệp | Dùng ở |
|---|---|
| `00-mo-ta-bai-toan` | Hình 3.1 - Sơ đồ mô tả bài toán |
| `03-erd` | Hình 3.2 - Sơ đồ ERD |
| `02-use-case` | Hình 3.3 - Sơ đồ use-case |
| `01-kien-truc` | Hình 3.4 - Kiến trúc phân tầng |
| `04-so-do-lop` | Hình 3.5 - Sơ đồ lớp thực thể |
| `05-so-do-lop-service` | Hình 3.6 - Sơ đồ lớp tầng dịch vụ |
| `06-tuan-tu-goi-y` | Hình 3.7 - Tuần tự: gợi ý theo nguyên liệu |
| `07-tuan-tu-thuc-don` | Hình 3.8 - Tuần tự: sinh thực đơn tuần |
| `08-tuan-tu-di-cho` | Hình 3.9 - Tuần tự: sinh danh sách đi chợ |
| `09-flowchart-goi-y` | Hình 3.10 - Lưu đồ thuật toán gợi ý |
| `10-flowchart-thuc-don` | Hình 3.11 - Lưu đồ thuật toán sinh thực đơn |

## Sửa sơ đồ

Cách 1, dùng draw.io: mở https://app.diagrams.net, chọn
`Extras > Edit Diagram`, đổi định dạng sang Mermaid rồi dán nội dung tệp `.mmd`.

Cách 2, render lại bằng dòng lệnh (cần Node.js và Google Chrome):

```bash
cat > puppeteer.json <<'JSON'
{ "executablePath": "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
  "args": ["--no-sandbox"] }
JSON

npx -y @mermaid-js/mermaid-cli@11 -i 03-erd.mmd -o 03-erd.png \
  -p puppeteer.json -c bw-config.json -b white -s 2 -w 1600
```

Lưu ý cú pháp Mermaid: trong `erDiagram`, khóa ghép viết là `PK, FK` (có dấu
phẩy), không viết `PK-FK`; khóa duy nhất là `UK`, không phải `UQ`.

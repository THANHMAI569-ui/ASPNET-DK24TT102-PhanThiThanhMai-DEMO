# Danh mục hình cho Chương 4 (Kết quả nghiên cứu)

Ảnh nằm trong `thesis/images/`, chụp ngày 25/07/2026 từ hệ thống chạy thực tế
trên dữ liệu mẫu (25 món ăn, 40 nguyên liệu, 10 danh mục). Chụp ở độ phân giải
1440×900 (desktop) và 390×844 (di động), sau đó thu về chiều rộng tối đa 1600 px.

Cột "Dùng cho mục" là gợi ý vị trí chèn trong Chương 4, chỉnh lại nếu bố cục
chương thay đổi.

| Tệp | Chú thích đề xuất | Dùng cho mục |
|---|---|---|
| `h4-01-trang-chu.jpg` | Hình 4.1. Trang chủ với danh mục món và món nổi bật | 4.1 Giao diện chung |
| `h4-02-danh-sach-mon.jpg` | Hình 4.2. Trang danh sách món ăn với bộ lọc và lưới sản phẩm | 4.1 |
| `h4-03-loc-va-sap-xep.jpg` | Hình 4.3. Kết quả lọc theo vùng miền Nam, độ khó Dễ, sắp xếp theo calo tăng dần | 4.2 Tìm kiếm và lọc |
| `h4-04-chi-tiet-mon.jpg` | Hình 4.4. Trang chi tiết món ăn: thông số, nguyên liệu và các bước nấu | 4.1 |
| `h4-05-goi-y-chua-chon.jpg` | Hình 4.5. Màn hình gợi ý khi người dùng chưa chọn nguyên liệu nào | 4.3 Gợi ý theo nguyên liệu |
| `h4-06-goi-y-ket-qua.jpg` | Hình 4.6. Kết quả gợi ý với 8 nguyên liệu đầu vào: 2 món đủ nguyên liệu, 21 món còn thiếu | 4.3 |
| `h4-07-thuc-don-danh-sach.jpg` | Hình 4.7. Biểu mẫu sinh thực đơn và danh sách thực đơn đã lưu | 4.4 Thực đơn tuần |
| `h4-08-thuc-don-tuan.jpg` | Hình 4.8. Lịch thực đơn 7 ngày × 3 bữa kèm tổng calo từng ngày | 4.4 |
| `h4-09-di-cho.jpg` | Hình 4.9. Danh sách đi chợ sinh từ thực đơn: 35 nguyên liệu gộp theo 8 nhóm | 4.5 Danh sách đi chợ |
| `h4-10-yeu-thich.jpg` | Hình 4.10. Danh sách món yêu thích | 4.6 Tài khoản |
| `h4-11-admin-tong-quan.jpg` | Hình 4.11. Bảng điều khiển quản trị | 4.7 Khu vực quản trị |
| `h4-12-admin-mon-an.jpg` | Hình 4.12. Danh sách quản lý món ăn | 4.7 |
| `h4-13-admin-form-mon.jpg` | Hình 4.13. Biểu mẫu sửa món ăn với phần gán nguyên liệu động | 4.7 |
| `h4-14-admin-nguyen-lieu.jpg` | Hình 4.14. Danh sách quản lý nguyên liệu | 4.7 |
| `h4-15-dang-nhap.jpg` | Hình 4.15. Màn hình đăng nhập | 4.6 |
| `h4-16-trang-thai-rong.jpg` | Hình 4.16. Ví dụ trạng thái rỗng khi chưa có dữ liệu | 4.8 Đánh giá trải nghiệm |
| `h4-17-che-do-toi.jpg` | Hình 4.17. Giao diện ở chế độ tối | 4.8 |
| `h4-18-mobile-trang-chu.jpg` | Hình 4.18. Trang chủ trên màn hình 390 px | 4.8 |
| `h4-19-mobile-bo-loc.jpg` | Hình 4.19. Bộ lọc dạng ngăn kéo trên màn hình di động | 4.8 |

## Số liệu thật kèm theo ảnh (dùng khi viết lời bình)

Những con số dưới đây lấy trực tiếp từ hệ thống lúc chụp ảnh, có thể trích thẳng
vào Chương 4:

- **Hình 4.6:** đầu vào 8 nguyên liệu (chanh, dầu ăn, đường, hành lá, muối, nước
  mắm, rau muống, tỏi). Kết quả: 23 món phù hợp, trong đó **2 món đủ nguyên
  liệu** và 21 món còn thiếu. Món đủ nguyên liệu được xếp lên đầu danh sách.
- **Hình 4.8:** thực đơn 21 suất, tổng calo theo ngày lần lượt là 1580, 1270,
  1130, 1270, 1110, 1030 và 820 kcal.
- **Hình 4.9:** danh sách đi chợ gồm **35 dòng nguyên liệu** gộp theo **8 nhóm**
  (Đậu/Đỗ, Gia vị, Hải sản, Nấm, Ngũ cốc/Tinh bột, Rau củ, Thịt, Trứng/Sữa).
- **Hình 4.11:** 25 món ăn, 40 nguyên liệu, 10 danh mục.

## Ảnh còn thiếu, cần bổ sung khi viết Chương 4

- Màn hình đăng ký tài khoản.
- Thông báo lỗi khi nhập sai dữ liệu trong biểu mẫu (minh họa kiểm tra hợp lệ).
- Trang báo không đủ quyền khi người dùng thường truy cập khu vực quản trị.
- Ảnh chụp kết quả chạy `dotnet test` để minh họa 29 ca kiểm thử đạt.

# CookingAdvisor — Website gợi ý nấu ăn & lập thực đơn cho gia đình

Đồ án tốt nghiệp. Ứng dụng web giúp người dùng **gợi ý món ăn theo nguyên liệu
có sẵn** và **lập thực đơn tuần cho gia đình** (tự sinh + chỉnh tay), kèm danh
sách đi chợ và thông tin dinh dưỡng.

## Công nghệ

| Thành phần | Lựa chọn |
|---|---|
| Nền tảng | ASP.NET Core MVC (.NET 10) |
| Cơ sở dữ liệu | SQL Server (EF Core, Code-First) |
| Xác thực | ASP.NET Core Identity (2 vai: Admin / User) |
| Giao diện | Razor Views + Bootstrap 5 |
| Kiểm thử | xUnit |

## Cấu trúc repository

```
CookingAdvisor/
├─ setup/     Hướng dẫn & tài nguyên cài đặt (docker-compose, script seed)
├─ src/       Mã nguồn (solution + project + test)
│  ├─ CookingAdvisor.sln
│  └─ CookingAdvisor/
├─ thesis/    Tài liệu đồ án
│  ├─ doc/    (.docx)
│  └─ pdf/    (.pdf)
└─ README.md
```

## Chạy thử nhanh

> Yêu cầu: .NET SDK 10, và một SQL Server (xem `setup/`).

```bash
# 1. Dựng SQL Server
#    - macOS: dùng OrbStack/Docker  ->  xem setup/README.md
#    - Windows: cài SQL Server Express/Developer
# 2. Cấu hình connection string (User Secrets hoặc appsettings)
# 3. Tạo database từ migrations
dotnet ef database update --project src/CookingAdvisor
# 4. Chạy
dotnet run --project src/CookingAdvisor
```

Chi tiết cài đặt cho từng hệ điều hành xem trong thư mục [`setup/`](./setup).

## Chức năng chính

- Gợi ý món ăn theo nguyên liệu có sẵn (xếp hạng theo độ khớp).
- Lập thực đơn tuần (7 ngày × 3 bữa): tự sinh + chỉnh tay.
- Tự sinh danh sách đi chợ từ thực đơn.
- Thông tin dinh dưỡng / calo theo món và theo ngày.
- Tìm kiếm & lọc nâng cao (loại món, vùng miền, độ khó, thời gian nấu).
- Tài khoản, món yêu thích, trang quản trị (Admin).

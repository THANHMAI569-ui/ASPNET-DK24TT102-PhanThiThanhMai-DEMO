# Hướng dẫn cài đặt CookingAdvisor

> Phần chi tiết (docker-compose, script seed) sẽ được bổ sung ở Phase 1.
> Tài liệu này tóm tắt cách dựng môi trường chạy trên **macOS** và **Windows**.

## Yêu cầu chung

- .NET SDK **10.0** trở lên — https://dotnet.microsoft.com/download
- Công cụ EF Core: `dotnet tool install --global dotnet-ef`
- Một instance **SQL Server** (xem bên dưới theo hệ điều hành)

## macOS (dùng OrbStack / Docker)

SQL Server không chạy native trên macOS nên ta chạy trong container Linux:

```bash
# Từ thư mục setup/ (docker-compose.yml sẽ có ở Phase 1)
docker compose up -d
# SQL Server sẽ lắng nghe ở localhost:1433
```

Connection string mẫu:
```
Server=localhost,1433;Database=CookingAdvisor;User Id=sa;Password=Your_password123;TrustServerCertificate=True;
```

## Windows

Cài **SQL Server Express** hoặc **Developer Edition**, rồi dùng connection string:
```
Server=localhost\SQLEXPRESS;Database=CookingAdvisor;Trusted_Connection=True;TrustServerCertificate=True;
```

## Cấu hình mật khẩu an toàn (không commit lên GitHub)

```bash
cd src/CookingAdvisor
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<chuỗi kết nối của bạn>"
```

## Tạo database & seed dữ liệu

```bash
dotnet ef database update --project src/CookingAdvisor
# Dữ liệu mẫu (danh mục, nguyên liệu, ~25 món ăn, tài khoản admin)
# được nạp tự động khi chạy lần đầu qua DbInitializer.
```

## Tài khoản mặc định (sau khi seed)

| Vai | Email | Mật khẩu |
|---|---|---|
| Admin | admin@cookingadvisor.local | *(ghi ở Phase 1)* |

# Hướng dẫn cài đặt CookingAdvisor

Yêu cầu: **.NET SDK 10** và một **SQL Server**. Dự án dùng **EF Core Code-First** nên
DB được dựng tự động từ migrations — không cần import file `.bak`.

Cùng một mã nguồn chạy **giống hệt** trên macOS và Windows; chỉ khác **chuỗi kết
nối** (nằm trong cấu hình, không nằm trong code).

---

## 1. Chuẩn bị công cụ

```bash
dotnet --version          # cần >= 10.0
dotnet tool install --global dotnet-ef   # nếu chưa có
```

## 2. Dựng SQL Server

Chọn 1 trong 3 cách — cách nào cũng ra cùng kết quả:

### Cách A — Docker (khuyến nghị, giống nhau trên macOS & Windows)

macOS dùng **OrbStack** hoặc Docker Desktop; Windows dùng **Docker Desktop**. Cùng
một file `docker-compose.yml`:

```bash
cd setup
docker compose up -d
# SQL Server lắng nghe ở localhost:1433, tài khoản sa / CookAdvisor@2026
```

> Mật khẩu SA mặc định `CookAdvisor@2026` chỉ dùng cho DEV cục bộ. Đổi bằng biến
> môi trường `MSSQL_SA_PASSWORD` trước khi `docker compose up` nếu muốn.
> Trên Apple Silicon, image chạy dưới giả lập amd64 (đã cấu hình sẵn trong compose).

### Cách B — SQL Server native trên Windows

Cài **SQL Server Express/Developer**. Dùng SQL authentication (bật mixed mode) với
tài khoản `sa`, hoặc tạo login riêng. Chuỗi kết nối ở mục 3.

### Cách C — LocalDB (Windows, nhẹ nhất cho demo)

Có sẵn khi cài Visual Studio. Chuỗi kết nối ở mục 3.

## 3. Cấu hình chuỗi kết nối (KHÔNG commit mật khẩu)

Đặt qua **User Secrets** (không nằm trong repo):

```bash
cd src/CookingAdvisor
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<chuỗi phù hợp bên dưới>"
```

| Môi trường | Chuỗi kết nối |
|---|---|
| **Docker (Mac/Win)** | `Server=localhost,1433;Database=CookingAdvisor;User Id=sa;Password=CookAdvisor@2026;TrustServerCertificate=True` |
| **SQL Server native (Win)** | `Server=localhost;Database=CookingAdvisor;User Id=sa;Password=<mật khẩu của bạn>;TrustServerCertificate=True` |
| **LocalDB (Win)** | `Server=(localdb)\\MSSQLLocalDB;Database=CookingAdvisor;Trusted_Connection=True;TrustServerCertificate=True` |

> `TrustServerCertificate=True` cần cho cả hai (chứng chỉ dev tự ký).
> Tiếng Việt được lưu bằng `nvarchar` (Unicode) nên hiển thị đúng trên mọi môi trường.

## 4. Tạo database & chạy

```bash
# từ thư mục gốc repo
dotnet ef database update --project src/CookingAdvisor   # dựng schema từ migrations
dotnet run --project src/CookingAdvisor                  # chạy app
```

Lần chạy đầu, `DbInitializer` nạp dữ liệu mẫu (danh mục, nguyên liệu, ~25 món ăn,
tài khoản admin).

### Cách khác cho Windows: import trực tiếp bằng SSMS (không cần cài `dotnet-ef`)

Nếu máy Windows (Visual Studio) chưa cài `dotnet-ef` CLI, có thể dựng schema bằng
script T-SQL xuất sẵn từ migration thay vì bước `dotnet ef database update` ở trên:

```
setup/sql/InitialCreate.sql
```

Mở file này bằng **SQL Server Management Studio** (hoặc `sqlcmd`), kết nối tới
instance SQL Server, chọn/tạo database `CookingAdvisor`, rồi **Execute**. Script là
**idempotent** (tự kiểm tra `__EFMigrationsHistory`, chạy lại nhiều lần không lỗi),
sinh ra từ đúng migration `InitialCreate` nên schema giống hệt bản chạy qua EF Core
— không lệch pha giữa 2 hệ điều hành. Nếu sau này có thêm migration mới, chạy lại
`dotnet ef migrations script --idempotent --project src/CookingAdvisor -o setup/sql/InitialCreate.sql`
để cập nhật file này.

## 5. Tài khoản mặc định (sau khi seed)

| Vai | Email | Mật khẩu |
|---|---|---|
| Admin | `admin@cookingadvisor.local` | `Admin@2026!Cook` |

> Tài khoản seed cho môi trường DEV/demo (đồ án), không dùng cho production.
> Được tạo tự động bởi `DbInitializer` khi chạy app lần đầu trên DB trống.

## Ghi chú cross-platform

Engine trong container Docker chính là **SQL Server for Linux của Microsoft** — cùng
T-SQL, cùng provider EF Core với SQL Server trên Windows. Vì vậy code, migration,
truy vấn **giống hệt** nhau; chỉ chuỗi kết nối thay đổi theo môi trường.

---
title: "Xây dựng website gợi ý nấu ăn và lập thực đơn cho gia đình"
subtitle: "Phần 3: Chương 5, Tài liệu tham khảo và Phụ lục"
lang: vi
---

# CHƯƠNG 5. KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

## 5.1. Kết luận

Đồ án đã hoàn thành việc xây dựng **CookingAdvisor**, một ứng dụng web hỗ trợ
người nấu ăn trong gia đình ở ba khâu: tìm món từ nguyên liệu sẵn có, lập thực
đơn cho cả tuần và tổng hợp danh sách đi chợ.

### 5.1.1. Kết quả đạt được

**Về mặt chức năng,** hệ thống cung cấp 20 chức năng phân thành ba nhóm theo mức quyền
truy cập, chạy hoàn chỉnh đầu-cuối trên cơ sở dữ liệu mẫu gồm 26 món ăn Việt Nam, 40
nguyên liệu và 10 danh mục. Toàn bộ sáu mục tiêu đặt ra ở phần Mở đầu đều đã đạt,
với kết quả kiểm chứng cụ thể trình bày ở mục 4.6.1.

**Về mặt thuật toán,** ba thuật toán lõi đã được thiết kế, cài đặt và kiểm chứng:

- *Thuật toán gợi ý theo nguyên liệu* sử dụng độ phủ tập hợp kết hợp xếp hạng bốn
  tiêu chí. Đóng góp đáng chú ý ở đây là lập luận chọn độ phủ thay cho hệ số
  Jaccard: do Jaccard là độ đo đối xứng nên nó phạt cả phần nguyên liệu dư của
  người dùng, dẫn tới việc một món nấu được ngay có thể bị xếp hạng rất thấp.
- *Thuật toán sinh thực đơn tuần* theo hướng tham lam có ràng buộc, với thứ tự
  nới lỏng ràng buộc được quy định rõ ràng và ràng buộc vùng miền được giữ cứng.
- *Thuật toán sinh danh sách đi chợ* gộp nhóm theo khóa ghép (nguyên liệu, đơn
  vị), bảo đảm không cộng dồn nhầm hai giá trị khác đơn vị.

**Về mặt kiểm chứng,** bộ 29 ca kiểm thử đơn vị đã phát hiện được hai khiếm
khuyết mà việc quan sát bằng mắt thường khó nhận ra:

1. *Suy thoái tính đa dạng của thực đơn.* Khi thư viện món không đủ lớn, tiêu chí
   phụ theo năng lượng khiến thuật toán chọn lại đúng một món cho hầu hết các suất
   còn trống, đo được là một món chiếm 12 suất. Sau khi bổ sung tiêu chí rải đều
   theo số lần đã dùng, thực đơn đạt 17 món khác nhau trên 21 suất.
2. *Thứ tự không xác định khi phân trang.* Việc sắp xếp chỉ theo một khóa duy nhất
   khiến các bản ghi có giá trị bằng nhau không có thứ tự ổn định, dẫn tới nguy cơ
   một món xuất hiện ở hai trang. Đã khắc phục bằng khóa sắp xếp phụ theo tên món.

Việc phát hiện được hai khiếm khuyết này là minh chứng cho giá trị thực tế của
kiểm thử tự động, đồng thời cho thấy quyết định kiến trúc tách tầng Service khỏi
tầng Controller (mục 3.2) đã phát huy tác dụng: nếu logic nằm trong Controller,
việc viết các ca kiểm thử này sẽ khó khăn hơn nhiều.

**Về mặt giao diện,** hệ thống hiển thị đúng trên dải màn hình từ 375 px tới
1440 px, không xuất hiện cuộn ngang, hỗ trợ chế độ tối và đạt chuẩn tương phản
WCAG 2.1 mức AA với cặp màu thấp nhất đo được là 4,69:1.

### 5.1.2. Đóng góp của đồ án

So với các ứng dụng đã khảo sát ở mục 1.2, đồ án có ba điểm khác biệt:

**Thứ nhất, tích hợp trọn vẹn ba chức năng trong một luồng công việc liền mạch.**
Các sản phẩm khảo sát thường mạnh ở một khâu và thiếu ở khâu khác. Mealime lập
được thực đơn nhưng không hỗ trợ truy vấn ngược từ nguyên liệu. Paprika quản lý
công thức và danh sách đi chợ tốt nhưng không tự động hóa việc lập thực đơn. Các
trang trong nước chủ yếu dừng ở việc cung cấp nội dung.

**Thứ hai, dữ liệu món ăn Việt Nam được chuẩn hóa về định lượng.** Bảng trung
gian giữa món ăn và nguyên liệu mang thêm thuộc tính khối lượng và đơn vị. Đây là
điều kiện tiên quyết để danh sách đi chợ có thể cộng dồn chính xác, điều mà các
nền tảng có nội dung do cộng đồng đóng góp khó bảo đảm.

**Thứ ba, thuật toán minh bạch và kiểm chứng được.** Toàn bộ tiêu chí xếp hạng và
thứ tự nới lỏng ràng buộc đều được phát biểu tường minh, có mã giả, có lưu đồ và
có ca kiểm thử tương ứng.

### 5.1.3. Hạn chế

Đồ án còn tám hạn chế đã nêu chi tiết ở mục 4.6.2. Trong đó, ba hạn chế có ảnh
hưởng lớn nhất tới trải nghiệm người dùng là:

- Thuật toán gợi ý mới xét việc **có hay không có** một nguyên liệu, chưa xét
  người dùng có **đủ khối lượng** hay không.
- Kết quả gợi ý **giống nhau với mọi người dùng**, chưa khai thác lịch sử nấu ăn
  để cá nhân hóa.
- Thông tin dinh dưỡng mới dừng ở **năng lượng theo khẩu phần**, chưa bóc tách
  các thành phần dinh dưỡng chi tiết.

## 5.2. Hướng phát triển

### 5.2.1. Hoàn thiện các thuật toán hiện có

**Gợi ý có xét định lượng.** Mở rộng mô hình dữ liệu để người dùng khai báo được
khối lượng nguyên liệu đang có, không chỉ khai báo có hay không. Khi đó điều kiện
"nấu được ngay" trở thành: với mọi nguyên liệu $i$ của món, khối lượng người dùng
có phải lớn hơn hoặc bằng khối lượng công thức yêu cầu. Thay đổi này cũng cho
phép danh sách đi chợ **trừ đi phần đã có**, chỉ liệt kê phần còn thiếu.

**Lập thực đơn theo hướng tối ưu.** Thay chiến lược tham lam bằng một thuật toán
tìm kiếm cục bộ hoặc quy hoạch ràng buộc, để tổng năng lượng theo ngày bám sát
mục tiêu hơn. Có thể bổ sung các ràng buộc mới như cân đối nhóm thực phẩm trong
tuần hoặc tránh lặp cùng một nguyên liệu chính trong hai ngày liên tiếp.

**Cá nhân hóa kết quả gợi ý.** Khi hệ thống đã tích lũy đủ dữ liệu về lịch sử nấu
ăn và món yêu thích, có thể áp dụng kỹ thuật lọc cộng tác để bổ sung một tiêu chí
xếp hạng dựa trên mức độ phù hợp với khẩu vị của từng người dùng. Cần lưu ý đây
là hướng chỉ khả thi khi đã có người dùng thực, do các phương pháp này gặp vấn đề
khởi đầu nguội trên hệ thống mới.

### 5.2.2. Mở rộng chức năng

| Hướng mở rộng | Nội dung |
|---|---|
| Dinh dưỡng chi tiết | Bổ sung đạm, béo, tinh bột, chất xơ; cảnh báo theo chế độ ăn hoặc bệnh lý |
| Quản lý tài khoản | Đổi mật khẩu, quên mật khẩu, xác thực email, trang hồ sơ cá nhân |
| Tải ảnh lên | Thay việc nhập đường dẫn ảnh bằng chức năng tải tệp và tự tạo ảnh thu nhỏ |
| Cộng đồng | Cho phép người dùng đóng góp công thức, đánh giá và bình luận |
| Chia sẻ thực đơn | Xuất thực đơn tuần ra tệp hoặc chia sẻ qua đường dẫn công khai |
| In danh sách đi chợ | Xuất bản in hoặc tệp PDF để mang theo khi đi chợ |

*Bảng 5.1. Các hướng mở rộng chức năng*

### 5.2.3. Cải thiện kỹ thuật

**Phân trang và tìm kiếm cho khu quản trị.** Hiện các danh sách trong khu quản
trị hiển thị toàn bộ bản ghi. Với quy mô dữ liệu mẫu thì chưa gây trở ngại, nhưng
sẽ trở thành hạn chế khi số món tăng lên.

**Bổ sung kiểm thử tích hợp và kiểm thử giao diện tự động.** Hiện phần giao diện
mới được kiểm tra thủ công. Có thể bổ sung kiểm thử tích hợp cho các luồng
Controller và kiểm thử giao diện tự động cho các kịch bản chính.

**Ứng dụng di động.** Bối cảnh sử dụng thực tế của chức năng danh sách đi chợ là
khi người dùng đang ở chợ hoặc siêu thị. Một ứng dụng di động có khả năng hoạt
động ngoại tuyến sẽ phù hợp hơn giao diện web.

\newpage

# DANH MỤC TÀI LIỆU THAM KHẢO

## Tài liệu kỹ thuật

[1] Microsoft Learn. "Overview of ASP.NET Core MVC".
https://learn.microsoft.com/en-us/aspnet/core/mvc/overview. Truy cập ngày
25/07/2026.

[2] Microsoft Learn. "Migrations Overview - EF Core".
https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/. Truy cập
ngày 25/07/2026.

[3] Microsoft Learn. "Introduction to Identity on ASP.NET Core".
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity.
Truy cập ngày 25/07/2026.

## Ứng dụng khảo sát

[4] Apple App Store. "Cookpad Recipes".
https://apps.apple.com/us/app/cookpad-recipes/id340368403. Truy cập ngày
25/07/2026.

[5] Cookpad. "Cookpad Việt Nam". https://cookpad.com/vn. Truy cập ngày
25/07/2026.

[6] Plan to Eat. "Yummly is Closing: Discover the Best Meal Planning
Alternative".
https://www.plantoeat.com/blog/2024/12/yummly-is-closing-discover-the-best-meal-planning-alternative/.
Truy cập ngày 25/07/2026.

[7] Kiểm tra trực tiếp trong quá trình khảo sát: truy vấn https://www.yummly.com/
trả về mã chuyển hướng HTTP 301 tới https://www.kitchenaid.com/recipes; tên miền
help.yummly.com không phân giải được. Ngày kiểm tra 25/07/2026.

[8] CafeF. "Thị trường quá khốc liệt, Cooky - startup đi chợ online của Founder
ShopeeFood rời thị trường Hà Nội, chỉ còn hoạt động tại TPHCM".
https://cafef.vn/thi-truong-qua-khoc-liet-cooky-startup-di-cho-online-cua-founder-shopeefood-roi-thi-truong-ha-noi-chi-con-hoat-dong-tai-tphcm-188231205162148255.chn.
Truy cập ngày 25/07/2026.

[9] Esheep Kitchen. Trang chủ. https://www.esheepkitchen.com/. Truy cập ngày
25/07/2026.

[10] Mealime. Trang chủ. https://www.mealime.com/. Truy cập ngày 25/07/2026.

[11] Apple App Store. "Mealime Meal Plans & Recipes".
https://apps.apple.com/us/app/mealime-meal-plans-recipes/id1079999103. Truy cập
ngày 25/07/2026.

[12] Paprika App. Trang chủ. https://www.paprikaapp.com/. Truy cập ngày
25/07/2026.

[13] Apple App Store. "Paprika Recipe Manager 3".
https://apps.apple.com/us/app/paprika-recipe-manager-3/id1303222628. Truy cập
ngày 25/07/2026.

## Sách

[14] T. H. Cormen, C. E. Leiserson, R. L. Rivest, C. Stein. *Introduction to
Algorithms*, 4th ed. MIT Press, 2022.

[15] C. D. Manning, P. Raghavan, H. Schütze. *Introduction to Information
Retrieval*. Cambridge University Press, 2008.

\newpage

# PHỤ LỤC

## Phụ lục A. Hướng dẫn cài đặt và chạy hệ thống

Hệ thống dùng EF Core theo hướng Code-First nên cơ sở dữ liệu được dựng tự động
từ migrations, **không cần nhập tệp sao lưu `.bak`**. Cùng một mã nguồn chạy giống
hệt nhau trên macOS và Windows; điểm khác biệt duy nhất là **chuỗi kết nối**, vốn
nằm trong cấu hình chứ không nằm trong mã nguồn.

### A.1. Chuẩn bị công cụ

```bash
dotnet --version                          # cần từ 10.0 trở lên
dotnet tool install --global dotnet-ef    # nếu chưa có
```

### A.2. Dựng SQL Server

Chọn một trong ba cách, kết quả như nhau.

**Cách A - Docker (khuyến nghị, giống nhau trên macOS và Windows)**

macOS dùng OrbStack hoặc Docker Desktop; Windows dùng Docker Desktop. Cùng một
tệp `docker-compose.yml`:

```bash
cd setup
docker compose up -d
# SQL Server lắng nghe ở localhost:1433, tài khoản sa / CookAdvisor@2026
```

Mật khẩu SA mặc định chỉ dùng cho môi trường phát triển cục bộ, có thể đổi bằng
biến môi trường `MSSQL_SA_PASSWORD` trước khi chạy lệnh trên. Trên máy Apple
Silicon, ảnh container chạy dưới giả lập amd64, cấu hình đã có sẵn trong tệp
compose.

**Cách B - SQL Server cài trực tiếp trên Windows**

Cài bản Express hoặc Developer, bật chế độ xác thực hỗn hợp và dùng tài khoản
`sa` hoặc tạo login riêng.

**Cách C - LocalDB (Windows, nhẹ nhất cho demo)**

Có sẵn khi cài Visual Studio.

### A.3. Cấu hình chuỗi kết nối

Chuỗi kết nối chứa mật khẩu nên **không được đưa vào mã nguồn**. Đặt qua User
Secrets:

```bash
cd src/CookingAdvisor
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<chuỗi phù hợp>"
```

| Môi trường | Chuỗi kết nối |
|---|---|
| Docker (macOS/Windows) | `Server=localhost,1433;Database=CookingAdvisor;User Id=sa;Password=CookAdvisor@2026;TrustServerCertificate=True` |
| SQL Server cài trực tiếp (Windows) | `Server=localhost;Database=CookingAdvisor;User Id=sa;Password=<mật khẩu của bạn>;TrustServerCertificate=True` |
| LocalDB (Windows) | `Server=(localdb)\MSSQLLocalDB;Database=CookingAdvisor;Trusted_Connection=True;TrustServerCertificate=True` |

Tham số `TrustServerCertificate=True` cần thiết vì chứng chỉ của môi trường phát
triển là chứng chỉ tự ký. Tiếng Việt được lưu bằng kiểu `nvarchar` (Unicode) nên
hiển thị đúng trên mọi môi trường.

### A.4. Tạo cơ sở dữ liệu và chạy ứng dụng

```bash
# từ thư mục gốc của repository
dotnet ef database update --project src/CookingAdvisor   # dựng lược đồ
dotnet run --project src/CookingAdvisor                  # chạy ứng dụng
```

Ở lần chạy đầu tiên, lớp `DbInitializer` nạp dữ liệu mẫu gồm danh mục, nguyên
liệu, 26 món ăn và tài khoản quản trị.

### A.5. Cách thay thế cho Windows: dùng script T-SQL

Nếu máy Windows chưa cài công cụ dòng lệnh `dotnet-ef`, có thể dựng lược đồ bằng
script T-SQL đã xuất sẵn tại `setup/sql/InitialCreate.sql`. Mở tệp bằng SQL Server
Management Studio hoặc `sqlcmd`, kết nối tới máy chủ, chọn hoặc tạo cơ sở dữ liệu
`CookingAdvisor` rồi thực thi.

Script này có tính lũy đẳng: nó tự kiểm tra bảng `__EFMigrationsHistory` nên chạy
lại nhiều lần không gây lỗi. Script được sinh ra từ toàn bộ chuỗi migrations hiện có
nên lược đồ giống hệt bản dựng qua EF Core; khi có migration mới cần sinh lại
script theo lệnh trong `setup/README.md`.

Lưu ý: script chỉ tạo **lược đồ** (bảng, cột, khóa ngoại), không chứa dữ liệu
mẫu. Sau khi thực thi xong vẫn cần chạy ứng dụng **một lần** để `DbInitializer`
nạp dữ liệu, vì phần nạp dữ liệu nằm trong mã C# chứ không nằm trong tệp SQL.

### A.6. Tài khoản mặc định

| Vai trò | Email | Mật khẩu |
|---|---|---|
| Quản trị viên | `admin@cookingadvisor.local` | `Admin@2026!Cook` |

Tài khoản này được tạo tự động khi chạy ứng dụng lần đầu trên cơ sở dữ liệu
trống, chỉ dùng cho môi trường demo của đồ án.

### A.7. Chạy kiểm thử

```bash
dotnet test src/CookingAdvisor.sln
```

Bộ kiểm thử dùng nhà cung cấp EF Core In-Memory nên chạy được mà không cần khởi
động SQL Server.

## Phụ lục B. Cấu trúc mã nguồn

```
CookingAdvisor/
├── setup/                       Tài nguyên cài đặt
│   ├── docker-compose.yml       Cấu hình container SQL Server
│   ├── sql/InitialCreate.sql    Script T-SQL dựng lược đồ
│   └── README.md                Hướng dẫn cài đặt
├── src/
│   ├── CookingAdvisor.sln
│   ├── CookingAdvisor/          Project web
│   │   ├── Controllers/         Tầng điều khiển
│   │   ├── Areas/Admin/         Khu vực quản trị
│   │   ├── Services/            Tầng nghiệp vụ, chứa ba thuật toán lõi
│   │   ├── Models/              Thực thể EF Core và kiểu liệt kê
│   │   ├── ViewModels/          Dữ liệu truyền cho View
│   │   ├── Data/                AppDbContext và DbInitializer
│   │   ├── Views/               Giao diện Razor
│   │   ├── TagHelpers/          Tag helper cho hệ thống biểu tượng
│   │   ├── Migrations/          Lịch sử thay đổi lược đồ
│   │   └── wwwroot/             CSS, JavaScript, phông chữ, ảnh
│   └── CookingAdvisor.Tests/    Dự án kiểm thử xUnit
└── thesis/                      Tài liệu đồ án
    ├── doc/                     Bản .docx của báo cáo
    ├── pdf/                     Bản .pdf để nộp
    ├── images/                  Ảnh chụp giao diện
    └── diagrams/                Sơ đồ (mã nguồn Mermaid và ảnh)
```

## Phụ lục C. Nguồn ảnh minh họa món ăn

Toàn bộ 26 ảnh món ăn sử dụng trong hệ thống được tải từ **Wikimedia Commons**,
đều thuộc các giấy phép cho phép sử dụng tự do (CC0, Public Domain, CC BY, CC
BY-SA). Ảnh đã được giảm kích thước về 900 px chiều ngang.

Danh sách đầy đủ gồm tên món, tên tệp, tác giả, giấy phép và đường dẫn tới trang
nguồn được lưu tại:

```
src/CookingAdvisor/wwwroot/images/recipes/CREDITS.md
```

Bảng dưới đây trích một số mục tiêu biểu:

| Món ăn | Tác giả | Giấy phép |
|---|---|---|
| Phở bò | Andy Li | CC0 |
| Bún chả | Phương Huy | Public Domain |
| Bún bò Huế | Baoothersks | CC BY-SA 4.0 |
| Cơm tấm thịt nướng | Tokeisan | Public Domain |
| Gỏi cuốn | Tran Hai Duong | CC0 |
| Chả giò | Satdeep Gill | CC BY-SA 4.0 |
| Bánh xèo | Orderinchaos | CC BY-SA 4.0 |
| Rau muống xào tỏi | Andy Wright | CC BY 2.0 |

## Phụ lục D. Mã nguồn tiêu biểu

### D.1. Thuật toán gợi ý theo nguyên liệu

Trích từ `src/CookingAdvisor/Services/SuggestionService.cs`:

```csharp
public async Task<List<RecipeSuggestionViewModel>> SuggestAsync(
    IReadOnlyCollection<int> ownedIngredientIds)
{
    var owned = ownedIngredientIds.ToHashSet();
    if (owned.Count == 0)
        return [];

    // Recipes sharing no ingredient with the owned set are not worth suggesting.
    var recipes = await db.Recipes
        .Where(r => r.RecipeIngredients.Any(ri => owned.Contains(ri.IngredientId)))
        .Select(r => new { /* ... */ })
        .ToListAsync();

    return recipes
        .Select(r =>
        {
            var missing = r.Ingredients
                .Where(i => !owned.Contains(i.IngredientId))
                .Select(i => i.Name)
                .OrderBy(name => name)
                .ToList();

            return new RecipeSuggestionViewModel
            {
                TotalIngredientCount = r.Ingredients.Count,
                MatchedCount = r.Ingredients.Count - missing.Count,
                MissingIngredients = missing
                /* ... */
            };
        })
        .OrderByDescending(s => s.CanCookNow)
        .ThenByDescending(s => s.Coverage)
        .ThenBy(s => s.MissingIngredients.Count)
        .ThenBy(s => s.Name)
        .ToList();
}
```

### D.2. Xử lý trường hợp phải lặp món trong thuật toán lập thực đơn

Trích từ `src/CookingAdvisor/Services/MenuPlannerService.cs`. Đây là đoạn khắc
phục khiếm khuyết suy thoái tính đa dạng đã trình bày ở mục 4.4.4:

```csharp
var suitable = candidates.Where(c => c.SuitableMealTypes.HasFlag(flag)).ToList();
var pool = suitable.Where(c => !used.Contains(c.Id)).ToList();

// Repeats only become necessary once every suitable dish has been
// used; meal-type suitability is relaxed only after that.
var repeating = pool.Count == 0;
if (repeating) pool = suitable;
if (pool.Count == 0) pool = candidates;

var ordered = repeating
    // Variety beats preference once repeats are unavoidable: without
    // this key the calorie tie-break returns the same dish for every
    // remaining slot, so one dish fills the week and the rest go unused.
    ? pool.OrderBy(c => useCounts.GetValueOrDefault(c.Id))
        .ThenByDescending(c => favoriteIds.Contains(c.Id))
    : pool.OrderByDescending(c => favoriteIds.Contains(c.Id));

var chosen = ordered
    .ThenBy(c => Math.Abs(dayCalories + c.CaloriesPerServing - mealTarget))
    .ThenBy(c => c.Id)
    .First();
```

### D.3. Gộp nhóm sinh danh sách đi chợ

Trích từ `src/CookingAdvisor/Services/ShoppingListService.cs`:

```csharp
var plan = await db.MenuPlans
    .Include(p => p.Items).ThenInclude(i => i.Recipe).ThenInclude(r => r.RecipeIngredients)
    .FirstOrDefaultAsync(p => p.Id == menuPlanId && p.UserId == userId);

if (plan is null)
    throw new InvalidOperationException("Menu plan not found.");

var aggregated = plan.Items
    .SelectMany(i => i.Recipe.RecipeIngredients)
    .GroupBy(ri => (ri.IngredientId, ri.Unit))
    .Select(g => new ShoppingListItem
    {
        IngredientId = g.Key.IngredientId,
        Unit = g.Key.Unit,
        Quantity = g.Sum(ri => ri.Quantity)
    })
    .ToList();
```

### D.4. Ca kiểm thử tiêu biểu

Trích từ `src/CookingAdvisor.Tests/Services/RecipeServiceSortTests.cs`. Ca kiểm
thử này đã được xác nhận là có hiệu lực bằng cách tạm gỡ khóa sắp xếp phụ khỏi mã
nguồn và quan sát ca kiểm thử chuyển sang trạng thái không đạt:

```csharp
[Fact]
public async Task Breaks_ties_by_name_so_paging_cannot_repeat_a_dish()
{
    using var db = CreateDb();
    // Every dish shares one calorie value, so the tie-break is the only thing
    // deciding the order. Seeded in reverse name order on purpose: if the
    // tie-break were missing, the provider would fall back to insertion order
    // and page 1 would start at "Món 12" instead of "Món 01".
    var recipes = Enumerable.Range(1, RecipeService.PageSize + 3)
        .Select(i => BuildRecipe(i, $"Món {i:D2}", 10, 10, 500))
        .Reverse()
        .ToArray();
    await SeedAsync(db, recipes);

    var service = new RecipeService(db);
    var firstPage = await service.SearchRecipesAsync(
        new RecipeFilterViewModel { Sort = RecipeSortOrder.CaloriesDesc, Page = 1 });
    var secondPage = await service.SearchRecipesAsync(
        new RecipeFilterViewModel { Sort = RecipeSortOrder.CaloriesDesc, Page = 2 });

    Assert.Equal(
        Enumerable.Range(1, RecipeService.PageSize).Select(i => $"Món {i:D2}"),
        firstPage.Recipes.Select(r => r.Name));

    var seen = firstPage.Recipes.Select(r => r.Id)
        .Concat(secondPage.Recipes.Select(r => r.Id))
        .ToList();

    Assert.Equal(seen.Count, seen.Distinct().Count());
    Assert.Equal(recipes.Length, seen.Count);
}
```

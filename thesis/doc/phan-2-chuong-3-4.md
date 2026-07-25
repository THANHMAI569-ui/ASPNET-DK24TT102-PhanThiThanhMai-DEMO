---
title: "Xây dựng website gợi ý nấu ăn và lập thực đơn cho gia đình"
subtitle: "Phần 2: Chương 3 và Chương 4"
lang: vi
---

# CHƯƠNG 3. HIỆN THỰC HÓA NGHIÊN CỨU

Chương này trình bày quá trình hiện thực hóa hệ thống, đi từ đặc tả yêu cầu tới
thiết kế cơ sở dữ liệu, thiết kế lớp, thiết kế luồng xử lý và cuối cùng là chi
tiết cài đặt ba thuật toán lõi.

## 3.1. Mô tả bài toán

Bài toán của đồ án xoay quanh chuỗi công việc hằng ngày của người nội trợ:
quyết định nấu món gì từ nguyên liệu đang có, sắp xếp các bữa ăn cho cả tuần,
và tổng hợp danh sách cần mua. Hình 3.1 mô tả bài toán ở mức tổng quát: người
nội trợ tương tác với hệ thống qua ba chức năng nối tiếp nhau, cả ba cùng khai
thác một kho dữ liệu món ăn đã chuẩn hóa về định lượng nguyên liệu.

![Sơ đồ mô tả bài toán](../diagrams/00-mo-ta-bai-toan.png)

*Hình 3.1. Sơ đồ mô tả bài toán ở mức tổng quát*

Luồng nghiệp vụ chính diễn ra như sau. Người dùng khai báo các nguyên liệu
đang có; hệ thống đối chiếu với kho công thức và trả về danh sách món được xếp
hạng theo mức độ phù hợp, ghi rõ món nào nấu được ngay và món nào còn thiếu
nguyên liệu gì. Khi cần lên kế hoạch cho cả tuần, hệ thống sinh tự động lịch
7 ngày với 3 bữa mỗi ngày, người dùng chỉnh tay từng ô nếu muốn. Cuối cùng, từ
thực đơn đã chốt, hệ thống gộp toàn bộ nguyên liệu của 21 suất ăn thành danh
sách đi chợ, cộng dồn khối lượng theo từng nguyên liệu và đơn vị.

Ba chức năng này tương ứng với ba bài toán con đã phân tích ở mục 1.1: truy
vấn ngược từ nguyên liệu, xếp lịch có ràng buộc, và gộp nhóm - tổng hợp. Phần
còn lại của chương trình bày yêu cầu, thiết kế dữ liệu và cách cài đặt từng
bài toán con đó.

## 3.2. Đặc tả yêu cầu

### 3.2.1. Xác định tác nhân

Hệ thống có ba tác nhân, phân biệt theo mức quyền truy cập tăng dần:

| Tác nhân | Mô tả | Cơ chế nhận diện |
|---|---|---|
| **Khách** | Người truy cập chưa đăng nhập | Không có phiên đăng nhập |
| **Người dùng** | Tài khoản đã đăng nhập, vai trò `User` | Thuộc tính `[Authorize]` |
| **Quản trị viên** | Tài khoản thuộc vai trò `Admin` | `[Authorize(Roles = "Admin")]` |

*Bảng 3.1. Các tác nhân của hệ thống*

Quan hệ giữa ba tác nhân là quan hệ kế thừa quyền: Người dùng có toàn bộ quyền
của Khách, Quản trị viên có toàn bộ quyền của Người dùng.

### 3.2.2. Yêu cầu chức năng

**Nhóm A - Chức năng dành cho Khách (không cần đăng nhập)**

| Mã | Tên chức năng | Mô tả |
|---|---|---|
| A1 | Xem trang chủ | Hiển thị danh mục, món nổi bật, số liệu tổng quan |
| A2 | Duyệt danh sách món ăn | Hiển thị dạng lưới, phân trang 9 món mỗi trang |
| A3 | Tìm kiếm theo tên món | Tìm theo chuỗi con trong tên |
| A4 | Lọc nâng cao | Theo danh mục, vùng miền, độ khó, thời gian nấu tối đa |
| A5 | Sắp xếp kết quả | Theo tên, thời gian nấu, năng lượng tăng hoặc giảm |
| A6 | Xem chi tiết món ăn | Thông số, nguyên liệu định lượng, các bước nấu |
| A7 | Gợi ý theo nguyên liệu | Chọn nguyên liệu đang có, nhận danh sách món xếp hạng |
| A8 | Đăng ký tài khoản | Tạo tài khoản mới, tự động gán vai trò `User` |
| A9 | Đăng nhập, đăng xuất | Xác thực bằng email và mật khẩu |

*Bảng 3.2. Yêu cầu chức năng dành cho Khách*

**Nhóm B - Chức năng dành cho Người dùng đã đăng nhập**

| Mã | Tên chức năng | Mô tả |
|---|---|---|
| B1 | Sinh thực đơn tuần | Tự động lấp 21 suất ăn, có tùy chọn vùng miền |
| B2 | Xem lịch thực đơn | Bảng 7 ngày × 3 bữa kèm tổng năng lượng theo ngày |
| B3 | Sửa thủ công từng bữa | Đổi món hoặc xóa món khỏi một ô |
| B4 | Xem danh sách thực đơn đã lưu | Liệt kê các thực đơn của riêng người dùng |
| B5 | Tạo danh sách đi chợ | Sinh từ một thực đơn, gộp và cộng dồn nguyên liệu |
| B6 | Đánh dấu đã mua | Tick từng nguyên liệu, hiển thị tiến độ |
| B7 | Quản lý món yêu thích | Thêm, bỏ và xem danh sách |

*Bảng 3.3. Yêu cầu chức năng dành cho Người dùng*

**Nhóm C - Chức năng dành cho Quản trị viên**

| Mã | Tên chức năng | Mô tả |
|---|---|---|
| C1 | Xem bảng điều khiển | Thống kê số món, nguyên liệu, danh mục |
| C2 | Quản lý món ăn | Thêm, sửa, xóa; gán nguyên liệu kèm định lượng |
| C3 | Quản lý nguyên liệu | Thêm, sửa, xóa; có kiểm tra ràng buộc tham chiếu |
| C4 | Quản lý danh mục | Thêm, sửa, xóa; có kiểm tra ràng buộc tham chiếu |

*Bảng 3.4. Yêu cầu chức năng dành cho Quản trị viên*

### 3.2.3. Yêu cầu phi chức năng

| Mã | Yêu cầu | Tiêu chí nghiệm thu |
|---|---|---|
| N1 | Bảo mật mật khẩu | Mật khẩu lưu dưới dạng băm, tối thiểu 8 ký tự |
| N2 | Phân quyền | Người dùng thường không truy cập được khu quản trị |
| N3 | Cách ly dữ liệu | Người dùng chỉ thao tác được trên thực đơn của mình |
| N4 | Tương thích màn hình | Hiển thị đúng từ 375 px tới 1440 px, không cuộn ngang |
| N5 | Khả năng tiếp cận | Tương phản màu đạt WCAG 2.1 mức AA |
| N6 | Tính đúng đắn thuật toán | Có kiểm thử đơn vị tự động cho ba thuật toán lõi |
| N7 | Khả năng tái lập | Dựng lại cơ sở dữ liệu bằng một lệnh trên máy khác |

*Bảng 3.5. Yêu cầu phi chức năng*

## 3.3. Mô hình cơ sở dữ liệu

### 3.3.1. Sơ đồ quan hệ thực thể

![Sơ đồ quan hệ thực thể của hệ thống](../diagrams/03-erd.png)

*Hình 3.2. Sơ đồ ERD*

### 3.3.2. Mô tả các bảng

**Bảng `Recipe` (Món ăn)** - bảng trung tâm của hệ thống.

| Trường | Kiểu | Mô tả |
|---|---|---|
| `Id` | int | Khóa chính, tự tăng |
| `Name` | nvarchar | Tên món, bắt buộc |
| `Description` | nvarchar | Mô tả ngắn |
| `Instructions` | nvarchar | Các bước nấu, lưu dạng văn xuôi |
| `Servings` | int | Số khẩu phần |
| `PrepMinutes` | int | Thời gian chuẩn bị |
| `CookMinutes` | int | Thời gian nấu |
| `Difficulty` | int | Độ khó: 0 Dễ, 1 Trung bình, 2 Khó |
| `Region` | int | Vùng miền: 0 Bắc, 1 Trung, 2 Nam |
| `CaloriesPerServing` | int | Năng lượng mỗi khẩu phần |
| `ImageUrl` | nvarchar | Đường dẫn ảnh minh họa |
| `SuitableMealTypes` | int | Cờ nhị phân các bữa phù hợp |
| `CategoryId` | int | Khóa ngoại tới `Category` |

*Bảng 3.6. Cấu trúc bảng Recipe*

Trường `SuitableMealTypes` sử dụng kiểu liệt kê dạng cờ (`[Flags]`) với các giá
trị Breakfast = 1, Lunch = 2, Dinner = 4. Cách biểu diễn này cho phép lưu tổ hợp
nhiều bữa trong một số nguyên duy nhất, ví dụ giá trị 6 nghĩa là món phù hợp cho
cả bữa trưa và bữa tối. Phép kiểm tra một món có hợp bữa hay không quy về phép
`AND` bit, có chi phí hằng số.

**Bảng `RecipeIngredient` (Nguyên liệu của món)** - bảng trung gian, là mấu chốt
của cả ba thuật toán.

| Trường | Kiểu | Mô tả |
|---|---|---|
| `RecipeId` | int | Khóa chính thành phần, khóa ngoại tới `Recipe` |
| `IngredientId` | int | Khóa chính thành phần, khóa ngoại tới `Ingredient` |
| `Quantity` | decimal(10,2) | Khối lượng cần dùng |
| `Unit` | nvarchar | Đơn vị đo |

*Bảng 3.7. Cấu trúc bảng RecipeIngredient*

Bảng này dùng **khóa chính ghép** gồm hai khóa ngoại, bảo đảm một nguyên liệu chỉ
xuất hiện một lần trong mỗi món. Điểm thiết kế quan trọng là bảng mang thêm hai
thuộc tính `Quantity` và `Unit`: nếu chỉ lưu quan hệ nhiều - nhiều thuần túy thì
chức năng danh sách đi chợ không thể cộng dồn khối lượng, như đã phân tích ở mục
2.5.3.

**Bảng `MenuPlan` và `MenuPlanItem` (Thực đơn tuần)**

`MenuPlan` lưu thông tin chung của một thực đơn: người tạo, tên, tuần bắt đầu và
thời điểm tạo. `MenuPlanItem` lưu từng ô của lưới thực đơn, gồm `DayOfWeek` nhận
giá trị từ 0 tới 6 và `MealType` nhận giá trị 0 Sáng, 1 Trưa, 2 Tối. Một thực đơn
đầy đủ gồm 21 bản ghi `MenuPlanItem`.

**Bảng `ShoppingList` và `ShoppingListItem` (Danh sách đi chợ)**

`ShoppingList` liên kết với `MenuPlan` qua khóa ngoại `MenuPlanId` có **ràng buộc
duy nhất**, thể hiện quan hệ một - một: mỗi thực đơn chỉ có tối đa một danh sách
đi chợ. Đây là căn cứ để thao tác tạo lại danh sách được cài đặt theo hướng ghi
đè thay vì tạo bản ghi mới.

**Bảng `Favorite` (Món yêu thích)** dùng khóa chính ghép `(UserId, RecipeId)`,
bảo đảm một người dùng không thể đánh dấu trùng một món.

### 3.3.3. Thiết kế hành vi xóa

Hành vi xóa được cấu hình có chủ đích cho từng quan hệ, chia làm hai nhóm:

| Quan hệ | Hành vi | Lý do |
|---|---|---|
| `Recipe` → `RecipeIngredient` | Cascade | Xóa món thì các dòng nguyên liệu của nó không còn ý nghĩa |
| `MenuPlan` → `MenuPlanItem` | Cascade | Tương tự, các ô thuộc về thực đơn |
| `MenuPlan` → `ShoppingList` | Cascade | Danh sách đi chợ phụ thuộc hoàn toàn vào thực đơn |
| `ShoppingList` → `ShoppingListItem` | Cascade | Các dòng thuộc về danh sách |
| `ApplicationUser` → `MenuPlan`, `Favorite` | Cascade | Xóa tài khoản thì dữ liệu cá nhân đi theo |
| `Recipe` → `Favorite` | Cascade | Món bị xóa thì dấu yêu thích trên món đó không còn ý nghĩa |
| `Category` → `Recipe` | **Restrict** | Không cho xóa danh mục đang có món sử dụng |
| `Ingredient` → `RecipeIngredient` | **Restrict** | Không cho xóa nguyên liệu đang được dùng |
| `Ingredient` → `ShoppingListItem` | **Restrict** | Không cho xóa nguyên liệu đang nằm trong danh sách đi chợ |
| `ApplicationUser` → `ShoppingList` | **Restrict** | Danh sách đi chợ đã xóa lan truyền theo thực đơn, không cần thêm một đường Cascade thứ hai từ tài khoản |
| `Recipe` → `MenuPlanItem` | **Restrict** | Không cho xóa món đang nằm trong thực đơn của người dùng |

*Bảng 3.8. Hành vi xóa của các quan hệ trong cơ sở dữ liệu*

Nguyên tắc phân biệt là: dữ liệu **phụ thuộc sự tồn tại** thì xóa lan truyền, còn
dữ liệu **được tham chiếu** thì chặn xóa để tránh phá vỡ dữ liệu của người dùng
khác. Nếu để `Recipe` → `MenuPlanItem` là Cascade, quản trị viên xóa một món sẽ
âm thầm phá vỡ tính toàn vẹn của thực đơn đã lưu của mọi người dùng.

Hai quan hệ cùng trỏ tới `Recipe` được cấu hình khác nhau một cách có chủ đích:
`MenuPlanItem` là Restrict còn `Favorite` là Cascade. Lý do nằm ở bản chất dữ
liệu. Một suất ăn trong thực đơn là **sản phẩm có cấu trúc** mà người dùng đã bỏ
công lập và chỉnh sửa; mất một suất là thực đơn thủng một ô. Ngược lại, dấu yêu
thích chỉ là **liên kết đánh dấu** tới món; khi món không còn tồn tại thì dấu đó
không còn đối tượng để trỏ tới, giữ lại cũng không hiển thị được gì. Nếu cấu hình
Favorite là Restrict, quản trị viên sẽ không thể xóa một món chỉ vì có người từng
đánh dấu yêu thích nó, một ràng buộc gây phiền mà không bảo vệ được giá trị nào.

### 3.3.4. Khởi tạo dữ liệu mẫu

Lớp `DbInitializer` thực hiện ba việc khi ứng dụng khởi động: áp dụng các
migration còn thiếu, tạo hai vai trò `Admin` và `User` cùng tài khoản quản trị
mặc định, và nạp dữ liệu mẫu nếu bảng `Recipes` còn rỗng. Dữ liệu mẫu gồm 26 món
ăn Việt Nam, 40 nguyên liệu và 10 danh mục.

Điều kiện "chỉ nạp khi bảng còn rỗng" bảo đảm thao tác khởi động là **lũy đẳng**:
chạy lại ứng dụng nhiều lần không tạo ra dữ liệu trùng lặp.

## 3.4. Lược đồ use case

Hình 3.3 tổng hợp các ca sử dụng của ba tác nhân đã xác định ở mục 3.5.1; quan hệ kế thừa quyền giữa các tác nhân được thể hiện bằng việc tác nhân cấp cao dùng được mọi ca của cấp thấp hơn.

![Sơ đồ use-case tổng quát của hệ thống](../diagrams/02-use-case.png)

*Hình 3.3. Sơ đồ use-case tổng quát*

## 3.5. Kiến trúc hệ thống

Hệ thống được tổ chức theo mô hình phân tầng, cụ thể hóa mẫu MVC đã trình bày ở
mục 2.1. Điểm khác biệt so với mẫu MVC cơ bản là việc bổ sung **tầng Service**
nằm giữa Controller và tầng truy cập dữ liệu.

![Kiến trúc phân tầng của hệ thống](../diagrams/01-kien-truc.png)

*Hình 3.4. Kiến trúc phân tầng và luồng xử lý một yêu cầu*

Lý do tách tầng Service, như đã lập luận ở mục 2.1.1, là để logic nghiệp vụ không
phụ thuộc vào hạ tầng web. Nhờ đó ba thuật toán lõi có thể được kiểm thử bằng
kiểm thử đơn vị mà không cần khởi động máy chủ web hay mô phỏng đối tượng
`HttpContext`. Kết quả cụ thể của lựa chọn này được trình bày ở mục 4.4.

Cấu trúc thư mục của ứng dụng web như sau:

```
CookingAdvisor/
├── Controllers/      Tiếp nhận yêu cầu, gọi Service, trả View
├── Areas/Admin/      Khu vực quản trị, tách riêng bằng cơ chế Area
├── Services/         Logic nghiệp vụ, nơi đặt ba thuật toán lõi
├── Models/           Thực thể EF Core và các kiểu liệt kê
├── ViewModels/       Dữ liệu truyền giữa Controller và View
├── Data/             AppDbContext và DbInitializer
├── Views/            Giao diện Razor
├── TagHelpers/       Tag helper tự viết cho hệ thống biểu tượng
├── Migrations/       Lịch sử thay đổi lược đồ cơ sở dữ liệu
└── wwwroot/          Tài nguyên tĩnh: CSS, JavaScript, phông chữ, ảnh
```

## 3.6. Thiết kế lớp

### 3.6.1. Lớp thực thể

![Sơ đồ lớp các thực thể](../diagrams/04-so-do-lop.png)

*Hình 3.5. Sơ đồ lớp tầng thực thể*

### 3.6.2. Lớp dịch vụ và lớp điều khiển

![Sơ đồ lớp tầng dịch vụ](../diagrams/05-so-do-lop-service.png)

*Hình 3.6. Sơ đồ lớp tầng dịch vụ và quan hệ với tầng điều khiển*

Bảng sau tóm tắt trách nhiệm của từng lớp dịch vụ:

| Lớp | Trách nhiệm chính |
|---|---|
| `RecipeService` | Tìm kiếm, lọc, sắp xếp, phân trang; lấy chi tiết món; dữ liệu trang chủ |
| `SuggestionService` | Thuật toán gợi ý theo nguyên liệu |
| `MenuPlannerService` | Thuật toán sinh thực đơn tuần |
| `ShoppingListService` | Thuật toán gộp nhóm sinh danh sách đi chợ |
| `FavoriteService` | Thêm, bỏ, kiểm tra và liệt kê món yêu thích |

*Bảng 3.9. Trách nhiệm của các lớp dịch vụ*

Các lớp dịch vụ được đăng ký vào bộ chứa tiêm phụ thuộc với vòng đời `Scoped`,
tương ứng với vòng đời của `AppDbContext`.

## 3.7. Thiết kế luồng xử lý

### 3.7.1. Luồng gợi ý theo nguyên liệu

![Sơ đồ tuần tự chức năng gợi ý theo nguyên liệu](../diagrams/06-tuan-tu-goi-y.png)

*Hình 3.7. Sơ đồ tuần tự chức năng gợi ý theo nguyên liệu*

### 3.7.2. Luồng sinh thực đơn tuần

![Sơ đồ tuần tự chức năng sinh thực đơn tuần](../diagrams/07-tuan-tu-thuc-don.png)

*Hình 3.8. Sơ đồ tuần tự chức năng sinh thực đơn tuần*

### 3.7.3. Luồng sinh danh sách đi chợ

![Sơ đồ tuần tự chức năng sinh danh sách đi chợ](../diagrams/08-tuan-tu-di-cho.png)

*Hình 3.9. Sơ đồ tuần tự chức năng sinh danh sách đi chợ*

Điểm đáng chú ý ở luồng này là bước nạp dữ liệu có kèm điều kiện lọc theo
`UserId`. Nhờ đó, nếu người dùng cố tình gửi mã thực đơn của người khác, truy vấn
trả về rỗng và hệ thống ném ngoại lệ, thay vì sinh danh sách đi chợ từ dữ liệu
không thuộc quyền của họ. Đây là cách hiện thực yêu cầu phi chức năng N3 đã nêu ở
mục 3.2.3.

## 3.8. Cài đặt các thuật toán lõi

### 3.8.1. Thuật toán gợi ý theo nguyên liệu

![Lưu đồ thuật toán gợi ý theo nguyên liệu](../diagrams/09-flowchart-goi-y.png)

*Hình 3.10. Lưu đồ thuật toán gợi ý theo nguyên liệu*

**Mã giả**

```
THUẬT TOÁN GoiYTheoNguyenLieu
Đầu vào : A - tập mã nguyên liệu người dùng đang có
Đầu ra  : danh sách món đã xếp hạng, kèm nhãn và nguyên liệu còn thiếu

 1  nếu A rỗng thì
 2      trả về danh sách rỗng
 3  hết nếu
 4
 5  A ← chuyển A thành bảng băm            // tra cứu O(1)
 6  R ← truy vấn các món có ít nhất một nguyên liệu thuộc A
 7  KetQua ← danh sách rỗng
 8
 9  với mỗi món r thuộc R lặp
10      I_r      ← tập nguyên liệu của r
11      missing  ← { tên nguyên liệu i | i thuộc I_r và i không thuộc A }
12      missing  ← sắp xếp missing theo thứ tự bảng chữ cái
13      matched  ← |I_r| - |missing|
14      coverage ← matched / |I_r|
15      canCook  ← (missing rỗng)
16      thêm (r, matched, missing, coverage, canCook) vào KetQua
17  hết lặp
18
19  sắp xếp KetQua theo thứ tự từ điển:
20      khóa 1: canCook            giảm dần   // nấu được ngay lên trước
21      khóa 2: coverage           giảm dần
22      khóa 3: |missing|          tăng dần
23      khóa 4: tên món            tăng dần   // bảo đảm thứ tự tất định
24
25  trả về KetQua
```

**Giải thích các quyết định cài đặt**

*Dòng 6 - lọc sớm tại tầng cơ sở dữ liệu.* Điều kiện "có ít nhất một nguyên liệu
thuộc A" được đẩy xuống câu truy vấn thay vì lọc trong bộ nhớ. Những món không
chung nguyên liệu nào với tập đã chọn chắc chắn có `coverage = 0`, không đáng
được gợi ý, nên việc loại bỏ sớm giảm khối lượng dữ liệu phải chuyển lên tầng ứng
dụng.

*Dòng 5 - dùng bảng băm.* Phép kiểm tra thuộc tập ở dòng 11 được thực hiện
$|I_r|$ lần cho mỗi món. Nếu dùng danh sách tuyến tính, chi phí mỗi phép kiểm tra
là $O(|A|)$; với bảng băm, chi phí trung bình là $O(1)$.

*Dòng 12 - sắp xếp tên nguyên liệu thiếu.* Không ảnh hưởng tới thứ hạng, nhưng
làm cho nhãn "Thiếu: X, Y" hiển thị ổn định giữa các lần chạy, thuận tiện cho
kiểm thử tự động.

*Dòng 23 - khóa sắp xếp cuối cùng.* Như đã lập luận ở mục 2.5.1, khóa này bảo đảm
kết quả có thứ tự tất định.

**Độ phức tạp:** $O(nk + n\log n)$ với $n$ là số món thỏa điều kiện lọc và $k$ là
số nguyên liệu trung bình mỗi món.

### 3.8.2. Thuật toán sinh thực đơn tuần

![Lưu đồ thuật toán sinh thực đơn tuần](../diagrams/10-flowchart-thuc-don.png)

*Hình 3.11. Lưu đồ thuật toán sinh thực đơn tuần*

**Mã giả**

```
THUẬT TOÁN SinhThucDonTuan
Đầu vào : userId, tenThucDon, tuanBatDau, vungMien (tuỳ chọn)
Đầu ra  : đối tượng MenuPlan chứa 21 MenuPlanItem
Hằng số : MUC_TIEU_CALO_NGAY = 2000
          BUA[] = [ (Sáng, cờ Breakfast), (Trưa, cờ Lunch), (Tối, cờ Dinner) ]

 1  candidates ← các món thoả bộ lọc vùng miền        // ràng buộc CỨNG
 2  nếu candidates rỗng thì ném ngoại lệ
 3
 4  favoriteIds ← tập mã món yêu thích của userId
 5  used        ← tập rỗng          // các món đã dùng trong tuần
 6  useCounts   ← từ điển rỗng      // số lần mỗi món đã được dùng
 7  items       ← danh sách rỗng
 8
 9  với d ← 0 đến 6 lặp                               // 7 ngày
10      dayCalories ← 0
11      với m ← 0 đến 2 lặp                           // 3 bữa
12          (bua, co) ← BUA[m]
13          mealTarget ← MUC_TIEU_CALO_NGAY × (m + 1) / 3
14
15          suitable ← { c thuộc candidates | c.SuitableMealTypes CÓ cờ co }
16          pool     ← suitable \ used
17          repeating ← (pool rỗng)
18
19          nếu repeating thì pool ← suitable          // nới ràng buộc KHÔNG LẶP
20          nếu pool rỗng   thì pool ← candidates      // nới ràng buộc HỢP BỮA
21
22          nếu repeating thì
23              ordered ← sắp pool theo: useCounts[c] tăng dần,
24                                       rồi (c thuộc favoriteIds) giảm dần
25          ngược lại
26              ordered ← sắp pool theo: (c thuộc favoriteIds) giảm dần
27          hết nếu
28
29          chosen ← phần tử đầu của ordered sau khi sắp tiếp theo:
30                       |dayCalories + c.Calo - mealTarget| tăng dần,
31                       rồi c.Id tăng dần
32
33          dayCalories ← dayCalories + chosen.Calo
34          thêm chosen vào used
35          useCounts[chosen] ← useCounts[chosen] + 1
36          thêm MenuPlanItem(d, bua, chosen) vào items
37      hết lặp
38  hết lặp
39
40  lưu MenuPlan(userId, tenThucDon, tuanBatDau, items)
41  trả về MenuPlan
```

**Giải thích các quyết định cài đặt**

*Dòng 1 và 19-20 - thứ tự nới lỏng ràng buộc.* Ràng buộc vùng miền được áp ngay
từ đầu và không bao giờ được nới. Hai ràng buộc còn lại được nới theo thứ tự:
"không lặp" trước, "hợp bữa" sau. Lý do là việc lặp lại một món trong tuần gây
khó chịu ít hơn so với việc đề xuất một món hoàn toàn không hợp bữa, chẳng hạn đề
xuất món tráng miệng cho bữa sáng.

*Dòng 13 - mốc năng lượng lũy tiến theo bữa.* Giá trị `mealTarget` được tính lũy
tiến theo chỉ số bữa, bằng $\frac{2000 \times (m+1)}{3}$, cho các mốc lần lượt là
667, 1333 và 2000 kcal. Cách tính này khiến tiêu chí ở dòng 30 so sánh **tổng
năng lượng tích lũy tới hết bữa hiện tại** với mốc mong đợi, thay vì so sánh riêng
từng bữa một cách độc lập.

*Dòng 22-27 - hai chế độ sắp xếp khác nhau.* Đây là điểm cài đặt quan trọng nhất
và cũng là nơi phát sinh khiếm khuyết đã nêu ở mục 2.5.2. Khi còn món chưa dùng
(dòng 26), tiêu chí đầu tiên là ưu tiên món yêu thích. Nhưng khi buộc phải lặp
(dòng 23-24), tiêu chí đầu tiên phải chuyển thành **số lần đã dùng ít nhất**.

Nếu bỏ qua sự phân biệt này và luôn dùng nhánh dòng 26, thì trong nhánh lặp mọi
món đều "chưa từng bị loại", nên tiêu chí quyết định thực tế lùi về dòng 30, tức
là khoảng cách năng lượng. Do trạng thái `dayCalories` lặp lại theo chu kỳ giữa
các ngày, cùng một món luôn cho khoảng cách nhỏ nhất và được chọn lại liên tục.
Kết quả đo được trên bộ dữ liệu thử là một món chiếm 12 trong số các suất còn
trống trong khi nhiều món khác không được dùng lần nào.

*Dòng 31 - khóa sắp xếp cuối.* Sắp theo `Id` tăng dần để bảo đảm với cùng đầu
vào, thuật toán luôn sinh ra cùng một thực đơn. Tính chất tất định này là điều
kiện cần để viết kiểm thử tự động.

**Độ phức tạp:** $O(S \cdot n \log n)$ với $S = 21$ suất và $n$ số món ứng viên.

### 3.8.3. Thuật toán sinh danh sách đi chợ

**Mã giả**

```
THUẬT TOÁN SinhDanhSachDiCho
Đầu vào : userId, menuPlanId
Đầu ra  : đối tượng ShoppingList

 1  plan ← nạp MenuPlan kèm Items → Recipe → RecipeIngredients
 2          với điều kiện Id = menuPlanId VÀ UserId = userId
 3  nếu plan là null thì ném ngoại lệ          // chặn truy cập chéo người dùng
 4
 5  tatCa ← danh sách rỗng
 6  với mỗi suất s thuộc plan.Items lặp        // duyệt theo SUẤT, không theo MÓN
 7      thêm toàn bộ s.Recipe.RecipeIngredients vào tatCa
 8  hết lặp
 9
10  nhom ← gộp nhóm tatCa theo khóa ghép (IngredientId, Unit)
11  ketQua ← danh sách rỗng
12  với mỗi nhóm g thuộc nhom lặp
13      thêm ShoppingListItem(g.IngredientId, g.Unit, tổng g.Quantity) vào ketQua
14  hết lặp
15
16  list ← tìm ShoppingList có MenuPlanId = menuPlanId
17  nếu list tồn tại thì
18      xóa toàn bộ dòng cũ của list           // ghi đè, không tạo bản ghi mới
19  ngược lại
20      list ← tạo ShoppingList mới
21  hết nếu
22
23  gán ketQua vào list.Items và lưu
24  trả về list
```

**Giải thích các quyết định cài đặt**

*Dòng 2 - lọc theo `UserId` ngay trong truy vấn.* Việc kiểm tra quyền sở hữu được
thực hiện ở tầng truy vấn chứ không phải bằng câu lệnh `if` sau khi đã nạp dữ
liệu. Cách này an toàn hơn vì không tồn tại nhánh nào có thể vô tình bỏ qua bước
kiểm tra.

*Dòng 6 - duyệt theo suất ăn.* Nếu duyệt theo tập món khác nhau, một món xuất
hiện ba lần trong tuần chỉ được tính nguyên liệu một lần, dẫn tới mua thiếu.

*Dòng 10 - khóa gộp nhóm ghép cặp.* Như đã phân tích ở mục 2.5.3, đơn vị đo phải
nằm trong khóa gộp nhóm để không cộng dồn hai giá trị khác đơn vị.

*Dòng 16-21 - ghi đè thay vì tạo mới.* Do ràng buộc duy nhất trên `MenuPlanId`
(mục 3.3.2), mỗi thực đơn chỉ có tối đa một danh sách đi chợ. Khi người dùng sửa
thực đơn rồi tạo lại danh sách, hệ thống xóa các dòng cũ và ghi lại từ đầu.

**Độ phức tạp:** $O(S \cdot k)$ với $S$ là số suất ăn và $k$ là số nguyên liệu
trung bình mỗi món.

## 3.9. Một số điểm cài đặt khác

### 3.9.1. Tìm kiếm, lọc và sắp xếp

Toàn bộ tham số tìm kiếm được gom vào một lớp `RecipeFilterViewModel` và ánh xạ
tự động nhờ cơ chế model binding. Các điều kiện lọc được ghép dần vào đối tượng
`IQueryable`, do đó chỉ một câu lệnh SQL duy nhất được sinh ra dù người dùng chọn
bao nhiêu điều kiện.

Tùy chọn sắp xếp được biểu diễn bằng kiểu liệt kê `RecipeSortOrder`. Việc dùng
kiểu liệt kê thay vì chuỗi ký tự có ý nghĩa về an toàn: **bản thân kiểu liệt kê
đóng vai trò danh sách trắng**, giá trị không hợp lệ trên thanh địa chỉ sẽ được
ánh xạ về giá trị mặc định, nên không có chuỗi nào do người dùng nhập vào có thể
đi tới biểu thức sắp xếp.

Mọi phương án sắp xếp đều kết thúc bằng khóa phụ là tên món. Thiếu khóa này, các
món có cùng giá trị sắp xếp sẽ không có thứ tự tất định và một món có thể xuất
hiện ở hai trang khác nhau khi phân trang.

### 3.9.2. Bảo mật

| Nguy cơ | Biện pháp trong hệ thống |
|---|---|
| Chèn mã SQL | Toàn bộ truy vấn qua LINQ của EF Core, tham số hóa tự động |
| Lộ mật khẩu | Identity lưu giá trị băm có muối, không lưu mật khẩu gốc |
| Giả mạo yêu cầu liên trang | Token chống giả mạo trên mọi biểu mẫu POST |
| Truy cập vượt quyền chức năng | `[Authorize(Roles = "Admin")]` trên toàn bộ khu quản trị |
| Truy cập vượt quyền dữ liệu | Lọc theo `UserId` ngay trong truy vấn |
| Lộ chuỗi kết nối | Lưu trong User Secrets, không đưa vào mã nguồn |

*Bảng 3.10. Nguy cơ bảo mật và biện pháp áp dụng*

\newpage

# CHƯƠNG 4. KẾT QUẢ NGHIÊN CỨU

## 4.1. Môi trường triển khai và dữ liệu thử nghiệm

Hệ thống được chạy thử trên môi trường sau:

| Thành phần | Cấu hình |
|---|---|
| Nền tảng | .NET 10, ASP.NET Core MVC |
| Cơ sở dữ liệu | SQL Server 2022 chạy trong container |
| Trình duyệt kiểm thử | Google Chrome |
| Độ phân giải kiểm thử | 1440 × 900 và 390 × 844 |

*Bảng 4.1. Môi trường thử nghiệm*

Dữ liệu thử nghiệm do `DbInitializer` nạp tự động:

| Loại dữ liệu | Số lượng |
|---|---|
| Món ăn | 26 |
| Nguyên liệu | 40 |
| Danh mục | 10 |
| Tài khoản mặc định | 1 quản trị viên |

*Bảng 4.2. Dữ liệu thử nghiệm*

## 4.2. Giao diện các chức năng

### 4.2.1. Trang chủ và duyệt món ăn

![Trang chủ](../images/h4-01-trang-chu.jpg)

*Hình 4.1. Trang chủ*

![Danh sách món ăn](../images/h4-02-danh-sach-mon.jpg)

*Hình 4.2. Trang danh sách món ăn với bộ lọc bên trái và lưới kết quả*

![Chi tiết món ăn](../images/h4-04-chi-tiet-mon.jpg)

*Hình 4.3. Trang chi tiết món ăn*

### 4.2.2. Tìm kiếm, lọc và sắp xếp

![Kết quả lọc và sắp xếp](../images/h4-03-loc-va-sap-xep.jpg)

*Hình 4.4. Kết quả khi lọc theo vùng miền Nam, độ khó Dễ và sắp xếp theo năng
lượng tăng dần*

Kết quả cho thấy bộ lọc và tùy chọn sắp xếp hoạt động đồng thời: số món giảm từ
25 xuống còn tập con thỏa mãn cả hai điều kiện, đồng thời thứ tự hiển thị tuân
theo tiêu chí đã chọn. Các liên kết sắp xếp và phân trang đều giữ nguyên trạng
thái lọc hiện tại.

### 4.2.3. Gợi ý theo nguyên liệu

![Màn hình gợi ý khi chưa chọn nguyên liệu](../images/h4-05-goi-y-chua-chon.jpg)

*Hình 4.5. Trạng thái ban đầu khi người dùng chưa chọn nguyên liệu nào*

![Kết quả gợi ý](../images/h4-06-goi-y-ket-qua.jpg)

*Hình 4.6. Kết quả gợi ý với 8 nguyên liệu đầu vào*

Với đầu vào gồm 8 nguyên liệu (chanh, dầu ăn, đường, hành lá, muối, nước mắm, rau
muống, tỏi), hệ thống trả về **24 món phù hợp**, trong đó **2 món nấu được ngay**
và 22 món còn thiếu nguyên liệu. Hai món nấu được ngay được xếp lên đầu danh sách
và mang nhãn màu xanh "Đủ nguyên liệu"; các món còn lại mang nhãn màu hổ phách
ghi rõ số nguyên liệu còn thiếu, kèm danh sách tên nguyên liệu đó ngay dưới tên
món. Kết quả này thể hiện đúng thứ tự ưu tiên đã thiết kế ở mục 3.8.1.

### 4.2.4. Thực đơn tuần

![Màn hình sinh thực đơn](../images/h4-07-thuc-don-danh-sach.jpg)

*Hình 4.7. Biểu mẫu sinh thực đơn và danh sách thực đơn đã lưu*

![Lịch thực đơn tuần](../images/h4-08-thuc-don-tuan.jpg)

*Hình 4.8. Lịch thực đơn 7 ngày × 3 bữa*

Lưới thực đơn hiển thị đủ 21 suất ăn. Mỗi ô gồm tên món, năng lượng của món, một
hộp chọn để đổi sang món khác và một nút xóa. Dòng cuối cùng hiển thị tổng năng
lượng theo từng ngày, lần lượt là 1580, 1270, 1130, 1270, 1110, 1030 và 820 kcal.

### 4.2.5. Danh sách đi chợ

![Danh sách đi chợ](../images/h4-09-di-cho.jpg)

*Hình 4.9. Danh sách đi chợ sinh từ thực đơn ở Hình 4.8*

Từ thực đơn 21 suất ăn, hệ thống sinh ra danh sách gồm **35 dòng nguyên liệu**
được gộp theo **8 nhóm**: Đậu/Đỗ, Gia vị, Hải sản, Nấm, Ngũ cốc/Tinh bột, Rau củ,
Thịt và Trứng/Sữa. Thanh tiến độ ở đầu trang cập nhật theo số nguyên liệu đã
đánh dấu mua.

### 4.2.6. Tài khoản và phân quyền

![Màn hình đăng nhập](../images/h4-15-dang-nhap.jpg)

*Hình 4.10. Màn hình đăng nhập*

![Màn hình đăng ký](../images/h4-20-dang-ky.jpg)

*Hình 4.11. Màn hình đăng ký tài khoản*

![Thông báo lỗi kiểm tra hợp lệ](../images/h4-21-loi-kiem-tra-hop-le.jpg)

*Hình 4.12. Thông báo lỗi khi nhập dữ liệu không hợp lệ*

Hình 4.12 minh họa cơ chế kiểm tra hợp lệ hoạt động đồng thời trên ba trường: sai
định dạng email, mật khẩu ngắn hơn 8 ký tự và mật khẩu xác nhận không khớp. Thông
báo lỗi hiển thị bằng tiếng Việt ngay dưới ô nhập tương ứng, viền ô nhập chuyển
sang màu đỏ.

![Màn hình chặn quyền truy cập](../images/h4-22-chan-quyen.jpg)

*Hình 4.13. Kết quả khi tài khoản không phải quản trị viên truy cập khu vực quản trị*

Hình 4.13 xác nhận yêu cầu phi chức năng N2: một tài khoản vừa đăng ký với vai
trò `User` khi truy cập đường dẫn `/Admin/Recipes` bị chuyển hướng sang trang
thông báo không đủ quyền, kèm tham số `ReturnUrl` ghi nhận trang đã bị chặn.

![Danh sách món yêu thích](../images/h4-10-yeu-thich.jpg)

*Hình 4.14. Danh sách món yêu thích*

### 4.2.7. Khu vực quản trị

![Bảng điều khiển quản trị](../images/h4-11-admin-tong-quan.jpg)

*Hình 4.15. Bảng điều khiển quản trị*

![Danh sách quản lý món ăn](../images/h4-12-admin-mon-an.jpg)

*Hình 4.16. Danh sách quản lý món ăn*

![Biểu mẫu sửa món ăn](../images/h4-13-admin-form-mon.jpg)

*Hình 4.17. Biểu mẫu sửa món ăn với phần gán nguyên liệu*

![Danh sách quản lý nguyên liệu](../images/h4-14-admin-nguyen-lieu.jpg)

*Hình 4.18. Danh sách quản lý nguyên liệu*

### 4.2.8. Giao diện đáp ứng và chế độ tối

![Trang chủ trên màn hình di động](../images/h4-18-mobile-trang-chu.jpg)

*Hình 4.19. Trang chủ trên màn hình rộng 390 px*

![Bộ lọc dạng ngăn kéo](../images/h4-19-mobile-bo-loc.jpg)

*Hình 4.20. Bộ lọc chuyển thành ngăn kéo trượt trên màn hình hẹp*

![Giao diện chế độ tối](../images/h4-17-che-do-toi.jpg)

*Hình 4.21. Giao diện ở chế độ tối*

![Trạng thái rỗng](../images/h4-16-trang-thai-rong.jpg)

*Hình 4.22. Ví dụ trạng thái khi chưa có dữ liệu*

## 4.3. Kịch bản demo

Kịch bản sau minh họa luồng sử dụng xuyên suốt ba chức năng lõi.

**Bước 1 - Tìm món từ nguyên liệu sẵn có.** Người dùng mở mục "Gợi ý nguyên
liệu", tick 8 nguyên liệu đang có trong bếp rồi bấm "Gợi ý món". Hệ thống trả về
24 món xếp hạng, trong đó 2 món nấu được ngay nằm ở đầu danh sách (Hình 4.6).

**Bước 2 - Đánh dấu món yêu thích.** Người dùng mở chi tiết một vài món và bấm
"Thêm yêu thích". Các món này sẽ được ưu tiên khi sinh thực đơn ở bước sau.

**Bước 3 - Sinh thực đơn tuần.** Người dùng chuyển sang mục "Thực đơn tuần", nhập
tên thực đơn, chọn tuần bắt đầu và bấm "Tạo thực đơn". Hệ thống sinh đủ 21 suất
ăn trong một thao tác (Hình 4.8).

**Bước 4 - Chỉnh tay từng bữa.** Với ô nào chưa vừa ý, người dùng chọn món khác
từ hộp chọn ngay trong ô đó. Tổng năng lượng của ngày tương ứng được tính lại.

**Bước 5 - Xuất danh sách đi chợ.** Người dùng bấm "Tạo danh sách". Hệ thống gộp
nguyên liệu của toàn bộ 21 suất thành 35 dòng theo 8 nhóm (Hình 4.9).

**Bước 6 - Đi chợ.** Khi mua xong từng nguyên liệu, người dùng tick vào ô tương
ứng; dòng đó bị gạch ngang và thanh tiến độ tăng lên.

## 4.4. Kết quả kiểm thử

### 4.4.1. Phương pháp kiểm thử

Kiểm thử được thực hiện ở mức **kiểm thử đơn vị** bằng thư viện xUnit, tập trung
vào ba lớp dịch vụ chứa thuật toán lõi cùng chức năng sắp xếp danh sách món. Cơ
sở dữ liệu được thay bằng nhà cung cấp **EF Core In-Memory**, nhờ đó các ca kiểm
thử chạy độc lập, không phụ thuộc vào SQL Server và không để lại dữ liệu thừa.
Mỗi ca kiểm thử tạo một cơ sở dữ liệu riêng mang tên ngẫu nhiên, bảo đảm các ca
không ảnh hưởng lẫn nhau.

Việc kiểm thử được ở mức này là kết quả trực tiếp của quyết định kiến trúc đã nêu
ở mục 3.5: vì logic nghiệp vụ nằm ở tầng Service chứ không nằm trong Controller,
nên có thể khởi tạo và kiểm thử trực tiếp mà không cần hạ tầng web.

Cần lưu ý một giới hạn của cách tiếp cận này: nhà cung cấp In-Memory **không
thực thi** ràng buộc duy nhất, khóa ngoại hay quy tắc so sánh chuỗi của SQL
Server. Vì vậy các ca kiểm thử ở đây kiểm chứng logic của thuật toán, còn các
hành vi phụ thuộc vào cơ sở dữ liệu thật (tranh chấp ghi đồng thời, vi phạm
khóa ngoại, thứ tự sắp xếp theo bảng mã tiếng Việt) nằm ngoài vùng phủ và được
kiểm tra thủ công trên môi trường chạy thật.

### 4.4.2. Bảng ca kiểm thử

**Nhóm 1 - `SuggestionService` (thuật toán gợi ý theo nguyên liệu)**

| Mã | Tên ca kiểm thử | Mục tiêu kiểm chứng | Kết quả |
|---|---|---|---|
| TC-01 | `SuggestAsync_EmptyIngredientSet_ReturnsEmpty` | Tập nguyên liệu rỗng trả về danh sách rỗng | Đạt |
| TC-02 | `SuggestAsync_AllIngredientsOwned_IsCookableWithNoMissing` | Có đủ nguyên liệu thì món được đánh dấu nấu được ngay, danh sách thiếu rỗng | Đạt |
| TC-03 | `SuggestAsync_SomeIngredientsMissing_ListsMissingNamesAndCoverage` | Liệt kê đúng tên nguyên liệu thiếu và tính đúng độ phủ | Đạt |
| TC-04 | `SuggestAsync_NoMatchedIngredient_RecipeIsExcluded` | Món không chung nguyên liệu nào bị loại khỏi kết quả | Đạt |
| TC-05 | `SuggestAsync_RanksCookableFirstThenCoverageThenFewerMissing` | Thứ tự xếp hạng đúng theo bốn tiêu chí | Đạt |
| TC-06 | `GetIngredientOptionsAsync_ReturnsAllIngredientsOrderedByName` | Danh sách nguyên liệu trả về đầy đủ và sắp theo tên | Đạt |

*Bảng 4.3. Ca kiểm thử thuật toán gợi ý theo nguyên liệu*

**Nhóm 2 - `MenuPlannerService` (thuật toán sinh thực đơn tuần)**

| Mã | Tên ca kiểm thử | Mục tiêu kiểm chứng | Kết quả |
|---|---|---|---|
| TC-07 | `GenerateWeeklyPlanAsync_FillsAll21Slots` | Sinh đủ 21 suất ăn | Đạt |
| TC-08 | `GenerateWeeklyPlanAsync_EnoughRecipes_NoRepeatsWithinWeek` | Thư viện đủ lớn thì không lặp món trong tuần | Đạt |
| TC-09 | `GenerateWeeklyPlanAsync_FewRecipes_AllowsRepeatsButStillFills21` | Thư viện nhỏ vẫn lấp đủ 21 suất bằng cách cho lặp | Đạt |
| TC-10 | `GenerateWeeklyPlanAsync_FewRecipes_SpreadsRepeatsEvenly` | Khi buộc lặp thì rải đều, không dồn vào một món | Đạt |
| TC-11 | `GenerateWeeklyPlanAsync_ScarceBreakfastRecipes_VariesBreakfastAcrossWeek` | Bữa sáng thiếu món vẫn đa dạng qua các ngày | Đạt |
| TC-12 | `GenerateWeeklyPlanAsync_RespectsRegionFilter` | Ràng buộc vùng miền không bị nới | Đạt |
| TC-13 | `GenerateWeeklyPlanAsync_PrefersFavoriteRecipe` | Món yêu thích được ưu tiên chọn | Đạt |
| TC-14 | `GenerateWeeklyPlanAsync_NoRecipesAvailable_Throws` | Không có món nào thì ném ngoại lệ | Đạt |
| TC-15 | `GenerateWeeklyPlanAsync_PersistsMenuPlanAndItems` | Thực đơn và các suất được lưu xuống cơ sở dữ liệu | Đạt |

*Bảng 4.4. Ca kiểm thử thuật toán sinh thực đơn tuần*

**Nhóm 3 - `ShoppingListService` (thuật toán sinh danh sách đi chợ)**

| Mã | Tên ca kiểm thử | Mục tiêu kiểm chứng | Kết quả |
|---|---|---|---|
| TC-16 | `GenerateFromMenuPlanAsync_AggregatesSameIngredientAndUnitAcrossRecipes` | Cùng nguyên liệu, cùng đơn vị ở nhiều món được cộng dồn | Đạt |
| TC-17 | `GenerateFromMenuPlanAsync_KeepsSeparateRowsForDifferentUnits` | Cùng nguyên liệu khác đơn vị được tách thành hai dòng | Đạt |
| TC-18 | `GenerateFromMenuPlanAsync_RepeatedRecipeOccurrences_MultipliesQuantity` | Món lặp trong tuần thì khối lượng nhân lên tương ứng | Đạt |
| TC-19 | `GenerateFromMenuPlanAsync_Regenerate_ReplacesOldItems` | Tạo lại thì ghi đè, không nhân đôi dữ liệu | Đạt |
| TC-20 | `GenerateFromMenuPlanAsync_EmptyPlan_CreatesShoppingListWithNoItems` | Thực đơn rỗng vẫn tạo được danh sách rỗng hợp lệ | Đạt |
| TC-21 | `GenerateFromMenuPlanAsync_NonexistentPlan_Throws` | Thực đơn không tồn tại thì ném ngoại lệ | Đạt |
| TC-22 | `GenerateFromMenuPlanAsync_PlanOwnedByDifferentUser_Throws` | Không truy cập được thực đơn của người dùng khác | Đạt |
| TC-23 | `GenerateFromMenuPlanAsync_PersistsMenuPlanIdAndUserId` | Lưu đúng liên kết tới thực đơn và người dùng | Đạt |

*Bảng 4.5. Ca kiểm thử thuật toán sinh danh sách đi chợ*

**Nhóm 4 - `RecipeService` (tìm kiếm và sắp xếp)**

| Mã | Tên ca kiểm thử | Mục tiêu kiểm chứng | Kết quả |
|---|---|---|---|
| TC-24 | `Defaults_to_alphabetical_order` | Mặc định sắp theo tên món | Đạt |
| TC-25 | `Sorts_by_total_time_using_prep_plus_cook` | Sắp theo tổng thời gian chuẩn bị cộng nấu | Đạt |
| TC-26 | `Sorts_by_calories_in_both_directions` (calo tăng dần) | Sắp đúng chiều tăng | Đạt |
| TC-27 | `Sorts_by_calories_in_both_directions` (calo giảm dần) | Sắp đúng chiều giảm | Đạt |
| TC-28 | `Breaks_ties_by_name_so_paging_cannot_repeat_a_dish` | Có khóa phá vỡ thế cân bằng, phân trang không lặp món | Đạt |
| TC-29 | `Sort_survives_alongside_a_filter_and_is_echoed_back` | Sắp xếp hoạt động cùng bộ lọc và được phản hồi lại giao diện | Đạt |

*Bảng 4.6. Ca kiểm thử chức năng tìm kiếm và sắp xếp*

### 4.4.3. Kết quả chạy kiểm thử

Lệnh `dotnet test src/CookingAdvisor.sln` cho kết quả:

```
Test run for .../CookingAdvisor.Tests.dll (.NETCoreApp,Version=v10.0)
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    29, Skipped:     0, Total:    29,
Duration: 337 ms - CookingAdvisor.Tests.dll (net10.0)
```

Tổng hợp:

| Nhóm | Số ca | Đạt | Không đạt |
|---|---|---|---|
| `SuggestionService` | 6 | 6 | 0 |
| `MenuPlannerService` | 9 | 9 | 0 |
| `ShoppingListService` | 8 | 8 | 0 |
| `RecipeService` | 6 | 6 | 0 |
| **Tổng** | **29** | **29** | **0** |

*Bảng 4.7. Tổng hợp kết quả kiểm thử theo nhóm*

### 4.4.4. Khiếm khuyết phát hiện qua kiểm thử

Quá trình kiểm thử đã phát hiện hai khiếm khuyết mà quan sát bằng mắt thường khó
nhận ra. Đây là kết quả có giá trị nhất của phần kiểm thử.

**Khiếm khuyết 1 - Suy thoái tính đa dạng của thực đơn.**

*Hiện tượng.* Khi thư viện món không đủ lấp 21 suất mà không lặp, thuật toán chọn
lại đúng một món cho hầu hết các suất còn trống.

*Đo lường.* Trên bộ dữ liệu thử, một món xuất hiện 12 lần trong khi nhiều món
khác dùng được lại không được chọn lần nào.

*Nguyên nhân.* Như đã phân tích ở mục 3.8.2, trong nhánh phải lặp, tiêu chí quyết
định lùi về khoảng cách năng lượng. Do trạng thái năng lượng tích lũy lặp lại
theo chu kỳ, cùng một món luôn cho khoảng cách nhỏ nhất.

*Khắc phục.* Bổ sung tiêu chí ưu tiên món có số lần đã dùng ít nhất, đặt trước
tiêu chí năng lượng và chỉ áp dụng trong nhánh lặp.

*Kết quả sau khắc phục.* Thực đơn sinh ra đạt **17 món khác nhau trên 21 suất**.
Hai ca kiểm thử hồi quy TC-10 và TC-11 được bổ sung để ngăn khiếm khuyết tái
diễn.

**Khiếm khuyết 2 - Thứ tự không xác định khi phân trang.**

*Hiện tượng.* Khi sắp xếp theo năng lượng, các món có cùng giá trị năng lượng
không có thứ tự tất định giữa hai lần truy vấn, dẫn tới nguy cơ một món xuất hiện
ở cả trang trước và trang sau.

*Nguyên nhân.* Câu lệnh sắp xếp chỉ có một khóa duy nhất. Với các bản ghi có khóa
bằng nhau, hệ quản trị cơ sở dữ liệu được tự do trả về theo thứ tự bất kỳ.

*Khắc phục.* Bổ sung khóa sắp xếp phụ là tên món cho mọi phương án sắp xếp.

*Kiểm chứng.* Ca kiểm thử TC-28 dựng 12 món có cùng giá trị năng lượng, **nạp
theo thứ tự tên giảm dần** để loại trừ khả năng kết quả đúng chỉ nhờ trùng với
thứ tự chèn dữ liệu, sau đó yêu cầu hai trang liên tiếp và kiểm tra không có món
nào lặp lại. Ca kiểm thử này đã được xác nhận là **có hiệu lực**: khi tạm gỡ khóa
sắp xếp phụ khỏi mã nguồn, ca kiểm thử chuyển sang trạng thái không đạt; khi khôi
phục, ca kiểm thử đạt trở lại.

## 4.5. Kiểm thử giao diện

Ngoài kiểm thử tự động ở mức đơn vị, giao diện được kiểm tra thủ công trên trình
duyệt theo các tiêu chí phi chức năng đã nêu ở mục 3.5.3.

| Tiêu chí | Cách kiểm tra | Kết quả |
|---|---|---|
| N4 - Không cuộn ngang | Đo `scrollWidth` so với `clientWidth` ở 375 px trên các trang chính | Đạt, không trang nào cuộn ngang |
| N5 - Tương phản WCAG AA | Đo tỉ lệ tương phản của các cặp màu chữ và nền trên trang thật | Đạt, cặp thấp nhất 4,69:1 so với ngưỡng 4,5:1 |
| Chế độ tối | Chuyển chế độ và đo lại toàn bộ cặp màu | Đạt ở cả hai chế độ |
| Điều hướng bàn phím | Kiểm tra bộ lọc dạng ngăn kéo: mở, giữ tiêu điểm, đóng bằng phím Esc | Đạt |
| Nhật ký lỗi trình duyệt | Theo dõi bảng điều khiển của trình duyệt khi duyệt các trang | Không có lỗi |

*Bảng 4.8. Kết quả kiểm tra giao diện theo tiêu chí phi chức năng*

## 4.6. Đánh giá

### 4.6.1. Kết quả đạt được

Đối chiếu với sáu mục tiêu đề ra ở phần Mở đầu:

| Mục tiêu | Kết quả |
|---|---|
| 1. Quản lý kho dữ liệu món ăn | Đạt, 26 món với đầy đủ thuộc tính và định lượng nguyên liệu |
| 2. Thuật toán gợi ý theo nguyên liệu | Đạt, 6 ca kiểm thử đều đạt |
| 3. Thuật toán sinh thực đơn tuần | Đạt, 9 ca kiểm thử đều đạt |
| 4. Sinh danh sách đi chợ | Đạt, 8 ca kiểm thử đều đạt |
| 5. Xác thực và phân quyền | Đạt, kiểm chứng bằng Hình 4.13 |
| 6. Kiểm chứng bằng kiểm thử tự động | Đạt, 29 ca kiểm thử, phát hiện 2 khiếm khuyết thực tế |

*Bảng 4.9. Đối chiếu kết quả đạt được với mục tiêu đề ra*

### 4.6.2. Hạn chế

Hệ thống còn các hạn chế sau, được nêu đầy đủ để làm cơ sở cho phần Hướng phát
triển ở Chương 5:

1. **Gợi ý chưa xét định lượng.** Thuật toán hiện chỉ xét việc **có hay không có**
   một nguyên liệu, chưa xét người dùng có **đủ khối lượng** hay không. Một người
   có 50 gam thịt vẫn được gợi ý món cần 500 gam.
2. **Chưa cá nhân hóa.** Kết quả gợi ý chỉ phụ thuộc tập nguyên liệu đầu vào,
   giống nhau với mọi người dùng. Lịch sử nấu ăn chưa được khai thác.
3. **Dinh dưỡng ở mức thô.** Mới dừng ở năng lượng theo khẩu phần, chưa bóc tách
   đạm, béo, tinh bột hay vi chất.
4. **Ảnh minh họa nhập bằng đường dẫn,** chưa có chức năng tải ảnh lên.
5. **Chưa có đổi mật khẩu, quên mật khẩu và xác thực email.**
6. **Khu quản trị chưa phân trang và chưa có tìm kiếm.** Với quy mô dữ liệu mẫu
   hiện tại thì chưa gây trở ngại, nhưng sẽ là hạn chế khi số món tăng lên.
7. **Thuật toán lập thực đơn là tham lam,** không bảo đảm nghiệm tối ưu toàn cục
   về cân đối năng lượng.
8. **Chưa kiểm thử tích hợp và kiểm thử giao diện tự động.** Phần giao diện mới
   được kiểm tra thủ công.

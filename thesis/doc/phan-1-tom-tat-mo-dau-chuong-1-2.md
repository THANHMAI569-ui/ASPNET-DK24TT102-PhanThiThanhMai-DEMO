---
title: "Xây dựng website gợi ý nấu ăn và lập thực đơn cho gia đình"
subtitle: "Phần 1: Tóm tắt, Mở đầu, Chương 1, Chương 2"
lang: vi
---

# TÓM TẮT ĐỒ ÁN

Việc quyết định "hôm nay ăn gì" là một công việc lặp lại hằng ngày của mỗi gia
đình. Công việc này có quy mô nhỏ nhưng tốn thời gian và dễ dẫn tới hai hệ quả:
thực đơn lặp đi lặp lại quanh vài món quen thuộc, và nguyên liệu đã mua bị bỏ phí
do không được sử dụng kịp thời. Người nội trợ thường phải tự ghi nhớ mình đang có
những nguyên liệu gì, tự nghĩ ra món phù hợp, rồi tự lập danh sách đi chợ cho cả
tuần. Ba việc này hiện chưa được hỗ trợ một cách liền mạch bởi các công cụ phổ
biến tại Việt Nam.

Đồ án xây dựng **CookingAdvisor**, một ứng dụng web hỗ trợ người nấu ăn trong gia
đình giải quyết đồng thời ba việc nêu trên. Hệ thống được phát triển trên nền
tảng **ASP.NET Core MVC (.NET 10)**, sử dụng **Entity Framework Core** theo hướng
Code-First với hệ quản trị cơ sở dữ liệu **SQL Server**, và **ASP.NET Core
Identity** cho xác thực, phân quyền theo hai vai trò Quản trị viên và Người dùng.

Đóng góp về mặt kỹ thuật của đồ án nằm ở ba thuật toán được thiết kế và cài đặt
trong tầng nghiệp vụ. Thứ nhất, **thuật toán gợi ý theo nguyên liệu sẵn có** đối
chiếu tập nguyên liệu người dùng đang có với tập nguyên liệu của từng món, tính
độ phủ và xếp hạng theo thứ tự ưu tiên: món nấu được ngay, độ phủ giảm dần, số
nguyên liệu còn thiếu ít nhất. Thứ hai, **thuật toán sinh thực đơn tuần** theo
hướng tham lam có ràng buộc, lấp đầy 21 suất ăn (7 ngày × 3 bữa) với mục tiêu
tránh lặp món, tôn trọng ràng buộc vùng miền và cân đối tổng năng lượng theo
ngày. Thứ ba, **thuật toán sinh danh sách đi chợ** gộp toàn bộ nguyên liệu của
thực đơn theo cặp (nguyên liệu, đơn vị) và cộng dồn khối lượng.

Kết quả đạt được là một hệ thống hoàn chỉnh, chạy được đầu-cuối với cơ sở dữ liệu
mẫu gồm 26 món ăn Việt Nam, 40 nguyên liệu và 10 danh mục. Hệ thống cung cấp 20
chức năng, phân thành ba nhóm theo mức quyền truy cập. Phần kiểm thử gồm **29 ca kiểm thử đơn
vị** viết bằng xUnit, tập trung vào ba thuật toán lõi, toàn bộ đạt. Quá trình
kiểm thử đã phát hiện và khắc phục một khiếm khuyết thực tế của thuật toán sinh
thực đơn. Cụ thể, khi thư viện món không đủ lớn, tiêu chí phụ theo năng lượng
khiến hệ thống chọn lặp lại đúng một món cho mọi suất còn trống. Sau khi bổ sung tiêu chí
rải đều theo số lần đã sử dụng, thực đơn sinh ra đạt 17 món khác nhau trên 21
suất. Giao diện được thiết kế theo hướng thương mại điện tử, đáp ứng trên dải màn
hình từ 375 px đến 1440 px và đạt chuẩn tương phản WCAG 2.1 mức AA.

**Từ khóa:** gợi ý món ăn, lập thực đơn, độ phủ tập hợp, thuật toán tham lam,
ASP.NET Core MVC, Entity Framework Core.

\newpage

# MỞ ĐẦU

## 1. Lý do chọn đề tài

Bữa cơm gia đình là một hoạt động diễn ra hằng ngày và liên tục. Đi kèm với nó là
một chuỗi quyết định nhỏ nhưng lặp lại: nấu món gì, cần mua thêm nguyên liệu nào,
làm sao để cả tuần không bị trùng món và vẫn cân đối về dinh dưỡng. Người đảm
nhiệm việc bếp núc trong gia đình phải xử lý chuỗi quyết định này bằng trí nhớ và
kinh nghiệm cá nhân.

Thực tế đó dẫn tới ba khó khăn cụ thể.

**Thứ nhất, khó tận dụng nguyên liệu sẵn có.** Người dùng thường ở trong tình
huống ngược với các công cụ hiện có: họ không tìm kiếm theo tên món, mà xuất phát
từ những nguyên liệu đang có trong tủ lạnh và cần biết có thể nấu được món gì.
Việc tra cứu theo chiều này đòi hỏi phải đối chiếu thủ công từng công thức, một
việc gần như không khả thi khi số lượng công thức lớn.

**Thứ hai, khó lập thực đơn cho cả tuần.** Lập thực đơn tuần là bài toán có ràng
buộc: phải tránh lặp món, phải phù hợp với từng bữa trong ngày, và nên cân đối
năng lượng. Khi làm thủ công, người dùng có xu hướng quay lại một nhóm nhỏ các
món quen thuộc, làm bữa ăn trở nên đơn điệu.

**Thứ ba, khó lập danh sách đi chợ chính xác.** Sau khi đã có thực đơn, việc tổng
hợp nguyên liệu cần mua đòi hỏi phải cộng dồn khối lượng của cùng một nguyên liệu
xuất hiện ở nhiều món khác nhau. Sai sót ở bước này dẫn tới mua thiếu, phải đi
chợ nhiều lần, hoặc mua thừa gây lãng phí.

Ba khó khăn trên có bản chất là các bài toán xử lý dữ liệu có cấu trúc, hoàn toàn
có thể tự động hóa bằng phần mềm. Đây chính là lý do đề tài được lựa chọn: xây
dựng một hệ thống giải quyết trọn vẹn cả ba khâu trong một luồng công việc liền
mạch, thay vì chỉ giải quyết riêng lẻ từng khâu.

## 2. Mục tiêu của đề tài

Đồ án đặt ra các mục tiêu cụ thể sau:

1. Xây dựng ứng dụng web quản lý được kho dữ liệu món ăn Việt Nam, bao gồm công
   thức, nguyên liệu, danh mục, vùng miền, độ khó và thông tin năng lượng.
2. Thiết kế và cài đặt thuật toán **gợi ý món ăn theo tập nguyên liệu sẵn có**,
   có khả năng xếp hạng kết quả theo mức độ phù hợp và chỉ rõ những nguyên liệu
   còn thiếu của từng món.
3. Thiết kế và cài đặt thuật toán **sinh thực đơn tuần tự động** với các ràng
   buộc về tính đa dạng, sự phù hợp theo bữa và cân đối năng lượng, đồng thời cho
   phép người dùng chỉnh sửa thủ công từng suất ăn.
4. Xây dựng chức năng **sinh danh sách đi chợ** tự động từ thực đơn đã lập, có
   gộp nhóm và cộng dồn khối lượng nguyên liệu.
5. Triển khai cơ chế xác thực và phân quyền theo vai trò, tách bạch chức năng
   dành cho khách, người dùng đã đăng nhập và quản trị viên.
6. Kiểm chứng tính đúng đắn của các thuật toán lõi bằng kiểm thử đơn vị tự động.

## 3. Đối tượng và phạm vi nghiên cứu

**Đối tượng nghiên cứu.** Đối tượng của đề tài gồm hai nhóm. Về mặt người dùng,
đó là người trực tiếp nấu ăn trong gia đình Việt Nam, có nhu cầu lên thực đơn và
đi chợ định kỳ. Về mặt kỹ thuật, đó là các phương pháp biểu diễn quan hệ nhiều -
nhiều giữa món ăn và nguyên liệu, các độ đo tương đồng tập hợp phục vụ xếp hạng,
và các thuật toán tham lam có ràng buộc phục vụ bài toán xếp lịch.

**Phạm vi nghiên cứu.** Đồ án giới hạn trong các nội dung sau:

- Hình thức triển khai là **ứng dụng web** chạy trên trình duyệt, không phát
  triển ứng dụng di động gốc.
- Thuật toán gợi ý dựa trên **đối sánh tập hợp nguyên liệu**, không sử dụng các
  mô hình học máy hay lọc cộng tác. Lựa chọn này xuất phát từ đặc thù bài toán:
  hệ thống mới, chưa có dữ liệu hành vi người dùng để huấn luyện mô hình.
- Thuật toán lập thực đơn theo hướng **tham lam có ràng buộc**, không giải bài
  toán tối ưu toàn cục.
- Thông tin dinh dưỡng giới hạn ở mức **năng lượng (kcal) theo khẩu phần**, chưa
  bóc tách các thành phần đạm, béo, tinh bột hay vi chất.
- Dữ liệu mẫu gồm 26 món ăn Việt Nam phổ biến, đủ để minh họa và kiểm chứng thuật
  toán, không nhằm mục tiêu xây dựng kho công thức quy mô lớn.

## 4. Phương pháp nghiên cứu

Đồ án kết hợp bốn phương pháp:

- **Phương pháp khảo sát:** tìm hiểu các ứng dụng nấu ăn và lập thực đơn hiện có
  trong và ngoài nước, xác định những chức năng đã được đáp ứng và những khoảng
  trống còn lại.
- **Phương pháp phân tích và thiết kế hệ thống:** mô hình hóa yêu cầu bằng sơ đồ
  use-case, thiết kế cơ sở dữ liệu quan hệ, thiết kế lớp và sơ đồ tuần tự.
- **Phương pháp thực nghiệm:** cài đặt hệ thống, chạy thử trên dữ liệu mẫu và
  quan sát kết quả đầu ra của các thuật toán.
- **Phương pháp kiểm thử:** viết kiểm thử đơn vị tự động cho các thuật toán lõi,
  sử dụng kiểm thử để phát hiện khiếm khuyết và xác nhận hiệu quả sau khi sửa.

## 5. Bố cục báo cáo

Ngoài phần Mở đầu và Kết luận, báo cáo được tổ chức thành năm chương:

- **Chương 1. Tổng quan:** trình bày bối cảnh thực tiễn, khảo sát các ứng dụng
  tương tự và xác định vấn đề mà đồ án tập trung giải quyết.
- **Chương 2. Nghiên cứu lý thuyết:** trình bày cơ sở lý thuyết về kiến trúc MVC,
  ASP.NET Core, Entity Framework Core, ASP.NET Core Identity, hệ quản trị cơ sở
  dữ liệu SQL Server và nền tảng thuật toán được sử dụng.
- **Chương 3. Hiện thực hóa nghiên cứu:** đặc tả yêu cầu, thiết kế cơ sở dữ liệu,
  thiết kế lớp, sơ đồ tuần tự và chi tiết cài đặt các thuật toán.
- **Chương 4. Kết quả nghiên cứu:** giới thiệu giao diện các chức năng, kịch bản
  demo và kết quả kiểm thử.
- **Chương 5. Kết luận và hướng phát triển.**

\newpage

# CHƯƠNG 1. TỔNG QUAN

## 1.1. Bối cảnh và nhu cầu thực tiễn

Nấu ăn tại nhà là hoạt động thường nhật, nhưng khác với nhiều hoạt động thường
nhật khác, nó đòi hỏi một chuỗi quyết định có ràng buộc lẫn nhau. Quyết định nấu
món gì phụ thuộc vào nguyên liệu đang có. Nguyên liệu cần mua lại phụ thuộc vào
các món dự định nấu. Và thực đơn của một ngày nên xét trong tương quan với cả
tuần để tránh trùng lặp và bảo đảm đa dạng.

Xét theo góc độ tin học, chuỗi quyết định này có thể quy về ba bài toán có cấu
trúc rõ ràng:

**Bài toán 1 - Truy vấn ngược từ nguyên liệu.** Cho trước tập nguyên liệu $A$ mà
người dùng đang có và một kho công thức, cần tìm và xếp hạng những món có thể nấu
được. Điểm đặc biệt của bài toán này là chiều truy vấn ngược với cách tổ chức dữ
liệu thông thường: các trang công thức thường được tổ chức để tra cứu *từ tên món
ra nguyên liệu*, trong khi nhu cầu thực tế của người nội trợ là tra cứu *từ
nguyên liệu ra tên món*.

**Bài toán 2 - Xếp lịch có ràng buộc.** Cần gán món ăn vào 21 ô của một lưới 7
ngày × 3 bữa, thỏa mãn đồng thời nhiều ràng buộc: hạn chế lặp món, phù hợp với
từng bữa trong ngày, tôn trọng lựa chọn về vùng miền và cân đối năng lượng theo
ngày. Đây là một biến thể của bài toán xếp lịch (scheduling) có ràng buộc.

**Bài toán 3 - Gộp nhóm và tổng hợp.** Từ thực đơn đã lập, cần tổng hợp nguyên
liệu cần mua. Cùng một nguyên liệu có thể xuất hiện ở nhiều món, và cùng một món
có thể xuất hiện nhiều lần trong tuần, do đó phải gộp nhóm theo nguyên liệu và
cộng dồn khối lượng.

Cả ba bài toán đều thao tác trên dữ liệu có cấu trúc và đều có lời giải thuật
toán xác định, không đòi hỏi kỹ thuật trí tuệ nhân tạo phức tạp. Đây là cơ sở để
khẳng định tính khả thi của đề tài.

## 1.2. Khảo sát các ứng dụng tương tự

Để xác định khoảng trống mà đồ án hướng tới, tác giả đã khảo sát các ứng dụng
nấu ăn và lập thực đơn tiêu biểu, cả quốc tế lẫn trong nước. Toàn bộ thông tin
dưới đây được tra cứu trực tiếp từ trang chủ hoặc trang phân phối chính thức của
từng sản phẩm trong ngày 25/07/2026.

### 1.2.1. Cookpad

Cookpad là nền tảng chia sẻ công thức nấu ăn theo mô hình cộng đồng, trong đó nội
dung do chính người dùng đăng tải thay vì do một đội ngũ biên tập tạo ra [4].
Cookpad có phiên bản tiếng Việt riêng tại địa chỉ cookpad.com/vn, nằm trong hệ
thống hơn ba mươi phiên bản theo quốc gia [5].

Về chức năng, mô tả chính thức của ứng dụng cho biết Cookpad hỗ trợ **tìm công
thức theo nguyên liệu** với thông điệp "nấu những bữa ăn ngon từ những gì bạn đã
có sẵn trong tủ lạnh", đồng thời có khả năng **tạo danh sách đi chợ tự động từ
công thức** và cho phép người dùng **xây dựng thực đơn theo tuần** [4].

*Nhận xét.* Cookpad là sản phẩm gần với đề tài nhất về mặt chức năng. Tuy nhiên
mô tả chính thức không nêu rõ việc xây dựng thực đơn tuần là *sinh tự động* hay
chỉ là công cụ để người dùng *tự sắp xếp* công thức vào lịch, nên không thể khẳng
định Cookpad có thuật toán sinh thực đơn tự động. Hạn chế đáng kể hơn nằm ở mô
hình nội dung: vì công thức do người dùng tự đăng nên chất lượng và độ chính xác
về định lượng nguyên liệu không đồng đều, điều này ảnh hưởng trực tiếp tới độ tin
cậy của danh sách đi chợ được sinh ra.

### 1.2.2. Yummly

Yummly từng là một dịch vụ gợi ý công thức được nhắc tới nhiều trong các nghiên
cứu về hệ khuyến nghị ẩm thực. Tuy nhiên, kết quả kiểm tra trực tiếp trong quá
trình khảo sát cho thấy **dịch vụ này đã ngừng hoạt động**: truy cập
https://www.yummly.com/ hiện bị chuyển hướng vĩnh viễn (HTTP 301) sang trang công
thức của KitchenAid, và tên miền hỗ trợ help.yummly.com không còn phân giải được
[7]. Theo nguồn thứ cấp trích lại thông báo của công ty chủ quản, website và ứng
dụng di động của Yummly đã đóng cửa từ ngày 20/12/2024 [6].

*Nhận xét.* Trường hợp Yummly có giá trị tham chiếu quan trọng đối với đề tài
theo hai hướng. Thứ nhất, nó cho thấy người dùng Việt Nam không thể trông cậy vào
tính sẵn sàng lâu dài của các dịch vụ gợi ý ẩm thực quốc tế, vốn phụ thuộc vào
quyết định kinh doanh của doanh nghiệp nước ngoài. Thứ hai, do dịch vụ đã đóng,
mọi mô tả chi tiết về tính năng của Yummly đều không thể kiểm chứng lại ở thời
điểm hiện tại; báo cáo này vì vậy không đưa ra bất kỳ khẳng định nào về các chức
năng cụ thể của sản phẩm.

### 1.2.3. Mealime

Mealime là ứng dụng chuyên biệt cho việc **lập thực đơn**, không phải nền tảng
chia sẻ công thức. Trang chủ của sản phẩm nêu khả năng lập kế hoạch bữa ăn cho cả
tuần dựa trên tùy chọn cá nhân về khẩu phần, chế độ ăn và dị ứng thực phẩm, đồng
thời **tự động tổng hợp nguyên liệu thành danh sách đi chợ theo danh mục** [10].
Sản phẩm áp dụng mô hình freemium với gói Mealime Pro giá 2,99 USD mỗi tháng
[11].

*Nhận xét.* Mealime giải quyết tốt bài toán lập thực đơn và danh sách đi chợ,
nhưng chức năng tìm món **xuất phát từ nguyên liệu người dùng đang có** không
được nêu trong mô tả chính thức; trọng tâm của sản phẩm là sinh thực đơn từ sở
thích và chế độ ăn. Ngoài ra, Mealime không có nội dung tiếng Việt và không có
món ăn Việt Nam, nên khó áp dụng trực tiếp cho bữa cơm gia đình Việt.

### 1.2.4. Paprika Recipe Manager

Paprika là ứng dụng quản lý công thức cá nhân đa nền tảng, cho phép lưu công thức
từ web, lập kế hoạch bữa ăn và tạo danh sách đi chợ với khả năng gộp nguyên liệu
trùng nhau và sắp xếp theo khu vực gian hàng [12]. Ứng dụng hỗ trợ tìm kiếm theo
tên và theo nguyên liệu [13].

*Nhận xét.* Điểm khác biệt căn bản so với đề tài là Paprika **không sinh thực đơn
tự động**: người dùng phải tự kéo thả từng công thức vào lịch [12]. Nói cách
khác, Paprika số hóa thao tác lập thực đơn thủ công chứ không thay người dùng
đưa ra quyết định.

### 1.2.5. Các website nấu ăn trong nước

Ở trong nước, Esheep Kitchen là một blog công thức nấu ăn tiếng Việt có lượng
người theo dõi lớn, với nội dung xoay quanh món Việt, bánh và đồ uống [9]. Kiểm
tra trực tiếp cho thấy đây thuần túy là nền tảng đăng tải nội dung: trang không
có chức năng tìm món theo nguyên liệu sẵn có, không sinh thực đơn tuần và không
xuất danh sách đi chợ [9].

Một trường hợp đáng chú ý khác là Cooky.vn. Trang này khởi đầu năm 2015 như một
website công thức nấu ăn, sau đó từ năm 2020 chuyển hướng sang mô hình đi chợ hộ
và giao thực phẩm tươi. Đến tháng 11/2023, doanh nghiệp đã thu hẹp hoạt động, rút
khỏi thị trường Hà Nội và chỉ còn vận hành tại Thành phố Hồ Chí Minh [8]. Tại
thời điểm khảo sát, tác giả **không truy cập được** trang cooky.vn do chứng chỉ
bảo mật TLS của trang đã hết hạn, vì vậy báo cáo này không đưa ra kết luận nào về
các chức năng hiện có của sản phẩm.

### 1.2.6. Bảng tổng hợp kết quả khảo sát

Bảng 1.1 tổng hợp kết quả khảo sát. Các ô ghi "Không xác minh được" phản ánh việc
tác giả không tìm được nguồn chính thức để khẳng định, chứ không đồng nghĩa với
việc chức năng đó chắc chắn không tồn tại.

| Ứng dụng | Gợi ý theo nguyên liệu sẵn có | Sinh thực đơn tuần tự động | Xuất danh sách đi chợ | Nội dung tiếng Việt | Mô hình giá |
|---|---|---|---|---|---|
| Cookpad | Có [4] | Có, chưa rõ mức độ tự động [4] | Có [4] | Có [5] | Miễn phí kèm gói Premium [4] |
| Yummly | Đã ngừng hoạt động từ 20/12/2024 [6][7] | Đã ngừng hoạt động | Đã ngừng hoạt động | Đã ngừng hoạt động | Đã ngừng hoạt động |
| Mealime | Không nêu trong mô tả chính thức [10][11] | Có [10] | Có [10] | Không xác minh được | Freemium, Pro 2,99 USD/tháng [11] |
| Paprika | Có [13] | Không, lập kế hoạch thủ công [12] | Có [12] | Không xác minh được | Trả phí một lần [12] |
| Esheep Kitchen | Không [9] | Không [9] | Không [9] | Có [9] | Miễn phí [9] |
| Cooky.vn | Không xác minh được | Không xác minh được | Không xác minh được | Có [8] | Không xác minh được |
| **CookingAdvisor** | **Có** | **Có** | **Có** | **Có** | **Miễn phí** |

*Bảng 1.1. So sánh chức năng giữa các ứng dụng khảo sát và đề tài*

## 1.3. Nhận xét chung và khoảng trống nghiên cứu

Từ kết quả khảo sát, có thể rút ra bốn nhận xét.

**Một là, ba chức năng cốt lõi hiếm khi xuất hiện đồng thời trong một sản phẩm.**
Các sản phẩm khảo sát có xu hướng mạnh ở một khâu và yếu ở khâu còn lại: Mealime
mạnh về lập thực đơn nhưng không hỗ trợ truy vấn ngược từ nguyên liệu; Paprika
mạnh về quản lý công thức và danh sách đi chợ nhưng không tự động hóa việc lập
thực đơn; các blog trong nước chỉ dừng ở việc cung cấp nội dung.

**Hai là, các sản phẩm quốc tế không phù hợp với bối cảnh ẩm thực Việt Nam.**
Mealime và Paprika không có nội dung tiếng Việt và không có dữ liệu món ăn Việt.
Ngay cả khi giao diện được dịch, cơ sở dữ liệu nguyên liệu và công thức vẫn xây
dựng theo khẩu vị Âu - Mỹ, khiến kết quả gợi ý không sát với thực tế bữa cơm gia
đình Việt.

**Ba là, tính sẵn sàng lâu dài của dịch vụ nước ngoài là một rủi ro thực tế.**
Việc Yummly ngừng hoạt động hoàn toàn vào cuối năm 2024 là minh chứng cụ thể cho
rủi ro này [6][7].

**Bốn là, mô hình nội dung do cộng đồng đóng góp ảnh hưởng tới chất lượng dữ liệu
định lượng.** Với các nền tảng như Cookpad, công thức do người dùng tự đăng nên
định lượng nguyên liệu không được chuẩn hóa. Điều này không gây trở ngại khi
người dùng chỉ đọc công thức, nhưng trở thành vấn đề khi hệ thống cần *tính toán*
trên dữ liệu đó, ví dụ khi cộng dồn khối lượng để sinh danh sách đi chợ.

Khoảng trống mà đồ án hướng tới, vì vậy, là một hệ thống **tích hợp cả ba chức
năng trong một luồng công việc liền mạch**, xây dựng trên **dữ liệu món ăn Việt
Nam được chuẩn hóa về định lượng**, với thuật toán xử lý minh bạch và có thể kiểm
chứng được.

## 1.4. Vấn đề tập trung giải quyết và đóng góp của đồ án

Trên cơ sở phân tích trên, đồ án tập trung giải quyết bài toán sau:

> *Cho một kho dữ liệu món ăn Việt Nam được chuẩn hóa, hãy xây dựng một hệ thống
> web cho phép:*
>
> *(a) xếp hạng các món có thể nấu từ một tập nguyên liệu cho trước;*
>
> *(b) sinh tự động thực đơn cho bảy ngày với các ràng buộc về tính đa dạng và
> cân đối năng lượng;*
>
> *(c) tổng hợp danh sách nguyên liệu cần mua từ thực đơn đó.*

Đóng góp của đồ án gồm bốn điểm:

1. **Về mô hình dữ liệu:** thiết kế lược đồ quan hệ chuẩn hóa cho bài toán, trong
   đó bảng trung gian giữa món ăn và nguyên liệu mang thêm thuộc tính định lượng
   và đơn vị. Đây là điều kiện tiên quyết để danh sách đi chợ có thể cộng dồn
   khối lượng một cách chính xác.
2. **Về thuật toán gợi ý:** đề xuất sử dụng **độ phủ** thay cho hệ số Jaccard
   thông dụng, kèm lập luận về lý do lựa chọn (trình bày ở mục 2.5.1), cùng cơ
   chế xếp hạng nhiều tiêu chí giúp kết quả ổn định.
3. **Về thuật toán lập thực đơn:** xây dựng thuật toán tham lam có ràng buộc theo
   thứ tự nới lỏng xác định, trong đó ràng buộc vùng miền được giữ cứng còn ràng
   buộc về bữa ăn chỉ được nới sau khi đã cạn nguồn món chưa dùng.
4. **Về kiểm chứng:** xây dựng bộ kiểm thử đơn vị tự động cho cả ba thuật toán,
   qua đó phát hiện được một khiếm khuyết thực tế về tính đa dạng của thực đơn mà
   quan sát bằng mắt thường khó nhận ra.

\newpage

# CHƯƠNG 2. NGHIÊN CỨU LÝ THUYẾT

Chương này trình bày cơ sở lý thuyết của hai nhóm nội dung: nhóm công nghệ nền
tảng dùng để xây dựng hệ thống (mục 2.1 đến 2.4) và nhóm cơ sở thuật toán dùng để
giải các bài toán nghiệp vụ (mục 2.5).

## 2.1. Kiến trúc MVC và nền tảng ASP.NET Core

### 2.1.1. Mẫu kiến trúc MVC

Model - View - Controller (MVC) là mẫu kiến trúc phân chia ứng dụng thành ba
nhóm thành phần chính là Model, View và Controller, nhằm đạt được sự **phân tách
mối quan tâm** (separation of concerns) [1]. Theo mô tả của Microsoft, yêu cầu từ
người dùng được định tuyến tới Controller; Controller có nhiệm vụ làm việc với
Model để thực hiện hành động hoặc lấy kết quả truy vấn, sau đó chọn View để hiển
thị và cung cấp cho View dữ liệu cần thiết [1].

Vai trò của từng thành phần được xác định như sau [1]:

- **Model** biểu diễn trạng thái của ứng dụng cùng các logic nghiệp vụ tác động
  lên trạng thái đó.
- **View** chịu trách nhiệm trình bày nội dung qua giao diện người dùng. Trong
  View chỉ nên chứa lượng logic tối thiểu và logic đó phải liên quan tới việc
  trình bày.
- **Controller** xử lý tương tác của người dùng, làm việc với Model và lựa chọn
  View để kết xuất.

Một đặc điểm quan trọng của mẫu này là **chiều phụ thuộc một chiều**: View và
Controller đều phụ thuộc vào Model, nhưng Model không phụ thuộc ngược lại vào
View hay Controller [1]. Nhờ đó Model có thể được xây dựng và kiểm thử độc lập
với phần trình bày, đây chính là tính chất mà đồ án khai thác để kiểm thử các
thuật toán lõi bằng kiểm thử đơn vị (trình bày ở Chương 4).

Tài liệu của Microsoft cũng khuyến nghị rằng Controller không nên gánh quá nhiều
trách nhiệm, và để tránh điều đó thì nên đẩy logic nghiệp vụ ra khỏi Controller
[1]. Khuyến nghị này là căn cứ trực tiếp cho quyết định thiết kế của đồ án: tách
riêng một tầng **Service** để chứa ba thuật toán lõi, còn Controller chỉ đóng vai
trò tiếp nhận yêu cầu, gọi Service và trả về View.

### 2.1.2. Các thành phần của ASP.NET Core MVC được sử dụng

ASP.NET Core MVC là khung làm việc mã nguồn mở, nhẹ và có tính kiểm thử cao dành
cho tầng trình bày [1]. Đồ án sử dụng trực tiếp các cơ chế sau của khung này:

- **Định tuyến (routing):** ánh xạ URL tới hành động của Controller. Đồ án sử
  dụng định tuyến theo quy ước với khuôn dạng `{controller}/{action}/{id?}` [1].
- **Model binding:** tự động chuyển dữ liệu từ yêu cầu HTTP (giá trị biểu mẫu, dữ
  liệu tuyến, tham số chuỗi truy vấn) thành đối tượng mà Controller xử lý được
  [1]. Cơ chế này được đồ án sử dụng để ánh xạ toàn bộ tham số lọc và sắp xếp của
  trang danh sách món ăn vào một lớp ViewModel duy nhất.
- **Kiểm tra hợp lệ (model validation):** thực hiện thông qua các thuộc tính chú
  giải dữ liệu, được kiểm tra ở cả phía máy khách trước khi gửi và phía máy chủ
  trước khi hành động của Controller được gọi [1].
- **Tiêm phụ thuộc (dependency injection):** Controller yêu cầu các dịch vụ cần
  thiết thông qua hàm khởi tạo [1]. Đồ án dùng cơ chế này để đưa các lớp Service
  vào Controller.
- **Bộ lọc (filters):** đóng gói các mối quan tâm cắt ngang như xử lý ngoại lệ
  hay phân quyền; thuộc tính `[Authorize]` chính là bộ lọc phân quyền có sẵn của
  khung [1].
- **Areas:** cơ chế phân hoạch ứng dụng lớn thành các nhóm chức năng nhỏ hơn [1].
  Đồ án dùng Area để tách riêng toàn bộ khu vực quản trị.
- **Razor và Tag Helpers:** Razor là ngôn ngữ đánh dấu mẫu cho phép nhúng mã C#
  vào HTML để sinh nội dung phía máy chủ; Tag Helpers cho phép mã phía máy chủ
  tham gia vào việc tạo và kết xuất phần tử HTML [1].

## 2.2. Entity Framework Core và tiếp cận Code-First

Entity Framework Core (EF Core) là bộ ánh xạ đối tượng - quan hệ (Object
Relational Mapper) cho .NET, cho phép thao tác với cơ sở dữ liệu quan hệ thông
qua các đối tượng .NET thay vì viết câu lệnh SQL trực tiếp.

Đồ án sử dụng tiếp cận **Code-First**, trong đó lược đồ cơ sở dữ liệu được sinh
ra từ các lớp thực thể trong mã nguồn. Cơ chế bảo đảm sự đồng bộ giữa mã nguồn và
cơ sở dữ liệu là **migrations**. Tài liệu của Microsoft mô tả: trong các dự án
thực tế, mô hình dữ liệu thay đổi khi các tính năng được hiện thực hóa, và lược
đồ cơ sở dữ liệu cần thay đổi tương ứng để đồng bộ với ứng dụng; tính năng
migrations của EF Core cung cấp cách cập nhật lược đồ một cách tăng dần nhằm giữ
đồng bộ với mô hình dữ liệu trong khi vẫn bảo toàn dữ liệu hiện có [2].

Cơ chế hoạt động của migrations gồm hai bước [2]:

1. Khi mô hình thay đổi, lập trình viên dùng công cụ dòng lệnh của EF Core để tạo
   một migration mô tả các cập nhật cần thiết. EF Core so sánh mô hình hiện tại
   với **ảnh chụp** (snapshot) của mô hình cũ để xác định khác biệt và sinh ra
   các tệp mã nguồn migration; các tệp này được quản lý phiên bản như mọi tệp mã
   nguồn khác.
2. Migration sau khi được sinh ra sẽ được áp dụng vào cơ sở dữ liệu. EF Core ghi
   nhận mọi migration đã áp dụng vào một **bảng lịch sử** đặc biệt, nhờ đó biết
   được migration nào đã và chưa được áp dụng.

Hai lệnh chính được sử dụng là `dotnet ef migrations add <Tên>` để tạo migration
và `dotnet ef database update` để áp dụng vào cơ sở dữ liệu [2].

Lựa chọn Code-First mang lại một lợi ích thực tiễn quan trọng cho đồ án: hội đồng
đánh giá có thể dựng lại toàn bộ cơ sở dữ liệu trên máy của mình chỉ bằng một
lệnh, không cần tệp sao lưu cơ sở dữ liệu và không phụ thuộc vào phiên bản cụ thể
của công cụ quản trị.

## 2.3. ASP.NET Core Identity

ASP.NET Core Identity là hệ thống quản lý thành viên của nền tảng, cung cấp sẵn
các chức năng đăng ký, đăng nhập, lưu trữ thông tin người dùng và quản lý vai trò
[3]. Việc sử dụng Identity thay vì tự cài đặt cơ chế xác thực mang lại ba lợi ích
mà đồ án khai thác.

**Thứ nhất, về lưu trữ mật khẩu.** Identity không lưu mật khẩu dạng nguyên bản mà
lưu giá trị băm sinh bởi hàm băm mật khẩu chuyên dụng có kèm muối (salt). Đây là
yêu cầu bắt buộc về an toàn thông tin: nếu cơ sở dữ liệu bị lộ, kẻ tấn công không
thu được mật khẩu gốc của người dùng.

**Thứ hai, về chính sách mật khẩu.** Identity cho phép cấu hình các ràng buộc như
độ dài tối thiểu, yêu cầu chữ số, chữ hoa hay ký tự đặc biệt [3]. Đồ án cấu hình
độ dài tối thiểu là 8 ký tự.

**Thứ ba, về phân quyền theo vai trò.** Identity hỗ trợ mô hình vai trò
(role-based authorization), cho phép gán người dùng vào các vai trò và giới hạn
truy cập theo vai trò. Kết hợp với bộ lọc `[Authorize]` của MVC [1], hệ thống có
thể khai báo phân quyền ngay tại lớp Controller. Đồ án định nghĩa hai vai trò là
`Admin` và `User`; toàn bộ khu vực quản trị được bảo vệ bằng
`[Authorize(Roles = "Admin")]`.

Cần phân biệt rõ hai khái niệm thường bị nhầm lẫn: **xác thực** (authentication)
trả lời câu hỏi *người dùng này là ai*, còn **phân quyền** (authorization) trả
lời câu hỏi *người dùng này được phép làm gì*. Một hệ thống chỉ kiểm tra xác thực
mà bỏ qua phân quyền vẫn có lỗ hổng: người dùng đã đăng nhập hợp lệ vẫn có thể
truy cập tài nguyên không thuộc quyền của mình. Trong đồ án, nguyên tắc này được
áp dụng ở cả mức chức năng (chặn theo vai trò) và mức bản ghi: các thao tác trên
thực đơn và danh sách đi chợ đều kiểm tra bản ghi có thuộc về đúng người dùng
đang đăng nhập hay không.

## 2.4. Hệ quản trị cơ sở dữ liệu SQL Server và triển khai đa nền tảng

Đồ án sử dụng Microsoft SQL Server làm hệ quản trị cơ sở dữ liệu quan hệ, với các
lý do: khả năng tương thích tốt nhất với EF Core trong hệ sinh thái .NET, hỗ trợ
đầy đủ ràng buộc toàn vẹn tham chiếu cần thiết cho lược đồ nhiều bảng của đề tài,
và tính phổ biến trong môi trường đào tạo.

Một vấn đề thực tiễn phát sinh trong quá trình thực hiện là môi trường phát triển
và môi trường đánh giá không đồng nhất về hệ điều hành. Giải pháp được áp dụng
như sau:

- **Trên macOS:** SQL Server được chạy trong **container** thông qua OrbStack
  hoặc Docker Desktop, sử dụng ảnh chính thức của Microsoft. Cấu hình container
  được mô tả trong tệp `docker-compose.yml` kèm theo mã nguồn.
- **Trên Windows:** SQL Server được cài đặt trực tiếp theo bản Express hoặc
  Developer.

Điểm cần nhấn mạnh là **sự khác biệt này không ảnh hưởng tới mã nguồn**. Ứng dụng
chỉ giao tiếp với cơ sở dữ liệu qua chuỗi kết nối; thay đổi giữa hai môi trường
chỉ là thay đổi giá trị chuỗi kết nối, không có bất kỳ đoạn mã nào phân nhánh
theo hệ điều hành.

Về an toàn thông tin, chuỗi kết nối chứa thông tin nhạy cảm nên **không được lưu
trong mã nguồn**. Đồ án sử dụng cơ chế User Secrets của .NET trong môi trường
phát triển; tệp `appsettings.json` được đưa vào hệ thống quản lý phiên bản chỉ
chứa giá trị rỗng làm chỗ giữ chỗ.

## 2.5. Cơ sở thuật toán

### 2.5.1. Độ đo tương đồng tập hợp cho bài toán gợi ý

Bài toán gợi ý theo nguyên liệu được phát biểu như sau. Gọi $A$ là tập nguyên
liệu mà người dùng đang có, $I_R$ là tập nguyên liệu cần thiết của món $R$. Cần
định nghĩa một hàm số đo mức độ phù hợp giữa $A$ và $I_R$ để làm cơ sở xếp hạng.

**Hệ số Jaccard.** Độ đo tương đồng tập hợp được sử dụng phổ biến nhất là hệ số
Jaccard, định nghĩa bằng tỉ số giữa lực lượng phần giao và lực lượng phần hợp của
hai tập hợp [15]:

$$J(A, I_R) = \frac{|A \cap I_R|}{|A \cup I_R|}$$

**Lý do không sử dụng hệ số Jaccard trong đề tài.** Hệ số Jaccard là độ đo *đối
xứng*, nghĩa là nó phạt cả phần thừa của $A$ lẫn phần thiếu của $I_R$. Đặc tính
này không phù hợp với ngữ nghĩa của bài toán. Xét ví dụ cụ thể: người dùng khai
báo đang có 40 nguyên liệu trong bếp, món $R$ chỉ cần 3 nguyên liệu và cả 3 đều
nằm trong số đó. Về mặt thực tế, đây là món **nấu được ngay**, cần được xếp hạng
cao nhất. Nhưng theo công thức Jaccard:

$$J(A, I_R) = \frac{3}{40} = 0{,}075$$

giá trị này rất thấp và món ăn sẽ bị xếp hạng gần cuối. Nguyên nhân là Jaccard
coi 37 nguyên liệu dư của người dùng như một sự "không tương đồng", trong khi
trên thực tế việc có dư nguyên liệu hoàn toàn không phải là điểm trừ.

**Độ phủ.** Đề tài vì vậy sử dụng độ đo **không đối xứng** là độ phủ (coverage),
chỉ xét tỉ lệ nguyên liệu của món được đáp ứng:

$$\text{coverage}(A, I_R) = \frac{|A \cap I_R|}{|I_R|}$$

Với ví dụ trên, $\text{coverage} = 3/3 = 1{,}0$, phản ánh đúng thực tế là món nấu
được ngay. Độ phủ luôn nhận giá trị trong đoạn $[0, 1]$; giá trị bằng 1 tương
đương với điều kiện $I_R \subseteq A$, tức là tập nguyên liệu còn thiếu
$M = I_R \setminus A$ là tập rỗng.

**Xếp hạng nhiều tiêu chí.** Chỉ dùng một mình độ phủ vẫn chưa đủ, vì hai món có
cùng độ phủ nhưng khác nhau về số nguyên liệu còn thiếu sẽ không phân biệt được.
Đề tài sử dụng thứ tự từ điển trên bộ bốn tiêu chí, xét lần lượt:

1. Món nấu được ngay ($M = \emptyset$) xếp trước.
2. Độ phủ giảm dần.
3. Số nguyên liệu còn thiếu $|M|$ tăng dần.
4. Tên món theo thứ tự bảng chữ cái.

Tiêu chí thứ tư có vai trò kỹ thuật quan trọng: nó bảo đảm **thứ tự tất định**
(deterministic). Nếu thiếu tiêu chí phá vỡ thế cân bằng này, hai lần chạy cùng
một truy vấn có thể cho ra thứ tự khác nhau, gây khó khăn cho việc kiểm thử tự
động và tạo trải nghiệm không nhất quán cho người dùng.

**Độ phức tạp.** Gọi $n$ là số món trong kho dữ liệu và $k$ là số nguyên liệu
trung bình của một món. Nếu biểu diễn $A$ bằng bảng băm, việc kiểm tra một nguyên
liệu có thuộc $A$ hay không tốn thời gian trung bình $O(1)$. Khi đó chi phí tính
toán độ phủ cho toàn bộ kho dữ liệu là $O(nk)$, cộng thêm $O(n \log n)$ cho bước
sắp xếp, tổng cộng là $O(nk + n \log n)$.

### 2.5.2. Thuật toán tham lam có ràng buộc cho bài toán lập thực đơn

**Phát biểu bài toán.** Cần gán món ăn vào $D \times M$ ô, với $D = 7$ ngày và
$M = 3$ bữa, tức 21 suất ăn, thỏa mãn các ràng buộc:

- **Ràng buộc cứng:** món được gán phải thuộc vùng miền người dùng chọn (nếu có).
- **Ràng buộc mềm 1:** món phải phù hợp với bữa được gán (sáng, trưa hoặc tối).
- **Ràng buộc mềm 2:** hạn chế lặp món trong cùng một tuần.
- **Tiêu chí tối ưu:** tổng năng lượng trong ngày tiệm cận mức mục tiêu.

Bài toán tối ưu toàn cục với đầy đủ các ràng buộc trên thuộc lớp bài toán khó.
Tuy nhiên, với quy mô thực tế của đề tài (21 suất, vài chục món), lời giải tối ưu
tuyệt đối không phải là yêu cầu bắt buộc: người dùng chỉ cần một thực đơn hợp lý
và luôn có thể chỉnh sửa thủ công.

**Lựa chọn chiến lược tham lam.** Thuật toán tham lam là chiến lược tại mỗi bước
chọn phương án có vẻ tốt nhất ở thời điểm hiện tại, không quay lui để xét lại các
lựa chọn đã thực hiện [14]. Chiến lược này không bảo đảm nghiệm tối ưu toàn cục
trong trường hợp tổng quát, nhưng cho lời giải chấp nhận được với chi phí tính
toán thấp, phù hợp với yêu cầu phản hồi tức thời của ứng dụng web.

**Thứ tự nới lỏng ràng buộc.** Điểm thiết kế quan trọng nhất của thuật toán là
quy định **thứ tự nới lỏng** khi tập ứng viên trở nên rỗng. Thứ tự này được xác
định theo mức độ ảnh hưởng tới trải nghiệm người dùng, từ nhẹ tới nặng:

1. Ban đầu, tập ứng viên gồm các món hợp bữa và **chưa được dùng** trong tuần.
2. Nếu tập này rỗng, nới ràng buộc **không lặp**, cho phép dùng lại món đã dùng
   nhưng vẫn giữ điều kiện hợp bữa.
3. Nếu vẫn rỗng, nới tiếp ràng buộc **hợp bữa**.
4. Ràng buộc **vùng miền không bao giờ được nới**, vì đó là lựa chọn tường minh
   của người dùng: một thực đơn được yêu cầu là món miền Nam mà lại chứa món miền
   Bắc sẽ bị xem là sai chức năng, chứ không phải là một sự linh hoạt.

**Vấn đề suy thoái tính đa dạng.** Trong quá trình kiểm thử, một khiếm khuyết đã
được phát hiện ở bước 2. Khi buộc phải lặp món, nếu tiêu chí lựa chọn vẫn là "gần
mức năng lượng mục tiêu nhất", thì do trạng thái năng lượng tích lũy trong ngày
lặp lại theo chu kỳ, thuật toán có xu hướng chọn **đúng một món** cho mọi suất
còn trống. Kết quả đo được trên dữ liệu thử nghiệm là một món xuất hiện 12 lần
trong khi nhiều món khác không được sử dụng lần nào.

**Giải pháp.** Bổ sung một tiêu chí ưu tiên đứng trước tiêu chí năng lượng, chỉ
áp dụng trong nhánh phải lặp: **ưu tiên món có số lần đã sử dụng ít nhất**. Về
bản chất, đây là chuyển từ chiến lược tham lam thuần túy sang chiến lược cân bằng
tải theo tần suất sử dụng. Sau khi áp dụng, thực đơn sinh ra đạt 17 món khác nhau
trên 21 suất. Chi tiết kiểm chứng được trình bày ở Chương 4.

**Độ phức tạp.** Với $S = D \times M = 21$ suất và $n$ món ứng viên, mỗi suất cần
duyệt và sắp xếp tập ứng viên, cho độ phức tạp $O(S \cdot n \log n)$. Với quy mô
thực tế, chi phí này là không đáng kể.

### 2.5.3. Bài toán gộp nhóm cho danh sách đi chợ

Bài toán sinh danh sách đi chợ là bài toán **gộp nhóm và tổng hợp** (grouping and
aggregation) quen thuộc trong xử lý dữ liệu. Cho thực đơn $P$ gồm các suất ăn,
mỗi suất tham chiếu tới một món, mỗi món có danh sách bộ ba (nguyên liệu, khối
lượng, đơn vị). Cần tính:

$$Q(i, u) = \sum_{\substack{s \in P \\ (i, q, u) \in R_s}} q$$

trong đó $R_s$ là tập nguyên liệu của món ở suất $s$.

Có hai điểm cần lưu ý về mặt thiết kế.

**Thứ nhất, khóa gộp nhóm phải là cặp (nguyên liệu, đơn vị) chứ không chỉ là
nguyên liệu.** Cùng một nguyên liệu có thể được định lượng bằng các đơn vị khác
nhau ở những công thức khác nhau, ví dụ hành lá tính theo gam ở món này nhưng
tính theo bó ở món khác. Cộng dồn hai giá trị khác đơn vị sẽ cho kết quả vô
nghĩa. Việc đưa đơn vị vào khóa gộp nhóm bảo đảm chỉ những giá trị cùng đơn vị
mới được cộng với nhau.

**Thứ hai, phép duyệt phải theo suất ăn chứ không theo món.** Nếu một món xuất
hiện ba lần trong tuần thì nguyên liệu của nó phải được tính ba lần. Nói cách
khác, phép duyệt thực hiện trên tập các suất ăn, không phải trên tập các món khác
nhau.

Độ phức tạp của thuật toán là $O(S \cdot k)$ với $S$ là số suất ăn và $k$ là số
nguyên liệu trung bình mỗi món, khi sử dụng bảng băm cho thao tác gộp nhóm.

## 2.6. Kết luận chương

Chương 2 đã trình bày hai nhóm cơ sở lý thuyết của đồ án.

Về công nghệ, mẫu kiến trúc MVC cùng nguyên tắc phân tách mối quan tâm là căn cứ
cho quyết định tách tầng Service khỏi Controller, nhờ đó các thuật toán lõi có
thể được kiểm thử độc lập với giao diện. EF Core theo tiếp cận Code-First cùng cơ
chế migrations bảo đảm lược đồ cơ sở dữ liệu luôn đồng bộ với mã nguồn và có thể
tái tạo trên máy khác chỉ bằng một lệnh. ASP.NET Core Identity cung cấp nền tảng
xác thực và phân quyền theo vai trò đã được kiểm chứng, tránh được các sai sót
thường gặp khi tự cài đặt.

Về thuật toán, chương đã lập luận cho ba lựa chọn thiết kế then chốt: dùng độ phủ
thay cho hệ số Jaccard do tính chất không đối xứng phù hợp với ngữ nghĩa bài
toán; dùng chiến lược tham lam có thứ tự nới lỏng ràng buộc xác định cho bài toán
lập thực đơn, kèm cơ chế cân bằng tần suất để bảo toàn tính đa dạng; và dùng khóa
gộp nhóm ghép cặp (nguyên liệu, đơn vị) để bảo đảm tính đúng đắn của phép cộng
dồn khối lượng.

Các cơ sở lý thuyết này sẽ được vận dụng trực tiếp vào phần thiết kế và cài đặt ở
Chương 3.

\newpage

# TÀI LIỆU THAM KHẢO (phần 1)

[1] Microsoft Learn. "Overview of ASP.NET Core MVC".
https://learn.microsoft.com/en-us/aspnet/core/mvc/overview. Truy cập ngày
25/07/2026.

[2] Microsoft Learn. "Migrations Overview - EF Core".
https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/. Truy cập
ngày 25/07/2026.

[3] Microsoft Learn. "Introduction to Identity on ASP.NET Core".
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity.
Truy cập ngày 25/07/2026.

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

[14] T. H. Cormen, C. E. Leiserson, R. L. Rivest, C. Stein. *Introduction to
Algorithms*, 4th ed. MIT Press, 2022. (Chương về thuật toán tham lam - Greedy
Algorithms.)

[15] C. D. Manning, P. Raghavan, H. Schütze. *Introduction to Information
Retrieval*. Cambridge University Press, 2008. (Chương về độ đo tương đồng tập
hợp.)

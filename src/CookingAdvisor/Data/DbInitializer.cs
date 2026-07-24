using CookingAdvisor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Data;

// Seeds roles, an admin account, and demo Vietnamese recipe data on first run.
// Called once at startup (see Program.cs); every step is guarded so re-running
// against an already-seeded database is a no-op.
public static class DbInitializer
{
    public const string AdminEmail = "admin@cookingadvisor.local";
    public const string AdminPassword = "Admin@2026!Cook";

    public static async Task SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<AppDbContext>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager);

        if (await db.Recipes.AnyAsync())
            return;

        var categories = await SeedCategoriesAsync(db);
        var ingredients = await SeedIngredientsAsync(db);
        await SeedRecipesAsync(db, categories, ingredients);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        if (await userManager.FindByEmailAsync(AdminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = AdminEmail,
            Email = AdminEmail,
            FullName = "Quản trị viên",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, AdminPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        await userManager.AddToRoleAsync(admin, "Admin");
    }

    private static async Task<Dictionary<string, Category>> SeedCategoriesAsync(AppDbContext db)
    {
        string[] names =
        [
            "Món chính", "Canh/Súp", "Xào", "Kho", "Chiên/Rán",
            "Nướng", "Hấp", "Gỏi/Cuốn", "Tráng miệng", "Đồ uống"
        ];

        var categories = names.Select(name => new Category { Name = name }).ToArray();
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync();

        return categories.ToDictionary(c => c.Name);
    }

    private static async Task<Dictionary<string, Ingredient>> SeedIngredientsAsync(AppDbContext db)
    {
        (string Name, string Unit, string Group)[] data =
        [
            ("Thịt heo ba chỉ", "kg", "Thịt"),
            ("Thịt heo xay", "kg", "Thịt"),
            ("Thịt bò", "kg", "Thịt"),
            ("Thịt gà", "kg", "Thịt"),
            ("Tôm", "kg", "Hải sản"),
            ("Cá basa", "kg", "Hải sản"),
            ("Cua đồng", "kg", "Hải sản"),
            ("Hành lá", "bó", "Rau củ"),
            ("Hành tím", "củ", "Rau củ"),
            ("Gừng", "củ", "Rau củ"),
            ("Cà chua", "quả", "Rau củ"),
            ("Cà rốt", "củ", "Rau củ"),
            ("Cải thìa", "bó", "Rau củ"),
            ("Rau muống", "bó", "Rau củ"),
            ("Xà lách", "bó", "Rau củ"),
            ("Giá đỗ", "kg", "Rau củ"),
            ("Đu đủ xanh", "quả", "Rau củ"),
            ("Tỏi", "củ", "Gia vị"),
            ("Sả", "cây", "Gia vị"),
            ("Ớt", "quả", "Gia vị"),
            ("Chanh", "quả", "Gia vị"),
            ("Me", "g", "Gia vị"),
            ("Nước mắm", "ml", "Gia vị"),
            ("Muối", "g", "Gia vị"),
            ("Đường", "g", "Gia vị"),
            ("Tiêu", "g", "Gia vị"),
            ("Dầu ăn", "ml", "Gia vị"),
            ("Gạo", "kg", "Ngũ cốc/Tinh bột"),
            ("Bún", "kg", "Ngũ cốc/Tinh bột"),
            ("Bánh phở", "kg", "Ngũ cốc/Tinh bột"),
            ("Miến", "kg", "Ngũ cốc/Tinh bột"),
            ("Bột mì", "kg", "Ngũ cốc/Tinh bột"),
            ("Bánh tráng", "cái", "Ngũ cốc/Tinh bột"),
            ("Đậu hũ", "miếng", "Đậu/Đỗ"),
            ("Đậu xanh", "kg", "Đậu/Đỗ"),
            ("Đậu phộng", "kg", "Đậu/Đỗ"),
            ("Trứng gà", "quả", "Trứng/Sữa"),
            ("Nước cốt dừa", "ml", "Trứng/Sữa"),
            ("Nấm hương", "kg", "Nấm"),
            ("Chuối", "quả", "Trái cây")
        ];

        var ingredients = data
            .Select(d => new Ingredient { Name = d.Name, Unit = d.Unit, Group = d.Group })
            .ToArray();
        db.Ingredients.AddRange(ingredients);
        await db.SaveChangesAsync();

        return ingredients.ToDictionary(i => i.Name);
    }

    private static async Task SeedRecipesAsync(
        AppDbContext db,
        IReadOnlyDictionary<string, Category> categories,
        IReadOnlyDictionary<string, Ingredient> ingredients)
    {
        const MealTypeFlags AllMeals = MealTypeFlags.Breakfast | MealTypeFlags.Lunch | MealTypeFlags.Dinner;
        const MealTypeFlags MainMeals = MealTypeFlags.Lunch | MealTypeFlags.Dinner;

        Recipe BuildRecipe(
            string name, string categoryName, Region region, Difficulty difficulty,
            int servings, int prepMinutes, int cookMinutes, int caloriesPerServing,
            string description, string instructions,
            MealTypeFlags mealTypes,
            params (string Ingredient, decimal Quantity, string Unit)[] items)
        {
            var recipe = new Recipe
            {
                Name = name,
                Description = description,
                Instructions = instructions,
                Servings = servings,
                PrepMinutes = prepMinutes,
                CookMinutes = cookMinutes,
                Difficulty = difficulty,
                Region = region,
                CaloriesPerServing = caloriesPerServing,
                SuitableMealTypes = mealTypes,
                Category = categories[categoryName]
            };

            foreach (var (ingredientName, quantity, unit) in items)
            {
                recipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    Ingredient = ingredients[ingredientName],
                    Quantity = quantity,
                    Unit = unit
                });
            }

            return recipe;
        }

        Recipe[] recipes =
        [
            BuildRecipe("Phở bò", "Món chính", Region.North, Difficulty.Hard, 4, 30, 180, 450,
                "Món nước đặc trưng Hà Nội với bánh phở mềm và nước dùng xương bò ninh nhiều giờ.",
                "Ninh xương bò cùng gừng, hành tím nướng để lấy nước dùng trong. Trụng bánh phở, " +
                "xếp thịt bò thái mỏng lên trên, chan nước dùng nóng và rắc hành lá.",
                AllMeals,
                ("Bánh phở", 0.4m, "kg"), ("Thịt bò", 0.3m, "kg"), ("Hành lá", 1m, "bó"),
                ("Hành tím", 2m, "củ"), ("Gừng", 1m, "củ"), ("Nước mắm", 30m, "ml"), ("Muối", 5m, "g")),

            BuildRecipe("Bún chả", "Món chính", Region.North, Difficulty.Medium, 4, 30, 25, 520,
                "Thịt heo nướng thơm lừng ăn kèm bún và nước chấm chua ngọt, đặc sản Hà Nội.",
                "Ướp thịt ba chỉ và thịt viên với tỏi, nướng vàng thơm. Pha nước chấm chua ngọt " +
                "với nước mắm, đường, chanh. Ăn kèm bún và xà lách.",
                MainMeals,
                ("Bún", 0.4m, "kg"), ("Thịt heo ba chỉ", 0.3m, "kg"), ("Thịt heo xay", 0.2m, "kg"),
                ("Tỏi", 3m, "củ"), ("Nước mắm", 50m, "ml"), ("Đường", 30m, "g"),
                ("Chanh", 1m, "quả"), ("Xà lách", 1m, "bó")),

            BuildRecipe("Bún bò Huế", "Món chính", Region.Central, Difficulty.Hard, 4, 40, 120, 480,
                "Món bún cay nồng đặc trưng xứ Huế với nước dùng sả ớt đậm đà.",
                "Ninh xương bò với sả đập dập để lấy nước dùng. Nêm nước mắm, muối và ớt cho vừa " +
                "cay. Trụng bún, xếp thịt bò, rắc hành tím phi.",
                AllMeals,
                ("Bún", 0.4m, "kg"), ("Thịt bò", 0.3m, "kg"), ("Sả", 4m, "cây"), ("Ớt", 3m, "quả"),
                ("Hành tím", 2m, "củ"), ("Nước mắm", 40m, "ml"), ("Muối", 5m, "g")),

            BuildRecipe("Cơm tấm thịt nướng", "Nướng", Region.South, Difficulty.Medium, 4, 25, 25, 580,
                "Cơm tấm ăn kèm thịt ba chỉ nướng thơm, món quen thuộc của người Sài Gòn.",
                "Ướp thịt với tỏi, đường, nước mắm rồi nướng trên than hoa cho vàng đều. Ăn cùng " +
                "cơm tấm nấu từ gạo tấm.",
                AllMeals,
                ("Gạo", 0.6m, "kg"), ("Thịt heo ba chỉ", 0.6m, "kg"), ("Tỏi", 4m, "củ"),
                ("Đường", 40m, "g"), ("Nước mắm", 40m, "ml")),

            BuildRecipe("Gỏi cuốn", "Gỏi/Cuốn", Region.South, Difficulty.Easy, 4, 30, 10, 220,
                "Cuốn tươi mát với tôm, thịt, bún và rau sống, chấm cùng nước chấm đậm đà.",
                "Luộc chín tôm và thịt ba chỉ, thái mỏng. Cuốn cùng bún, xà lách, giá đỗ trong " +
                "bánh tráng đã nhúng mềm.",
                MainMeals,
                ("Bánh tráng", 12m, "cái"), ("Tôm", 0.3m, "kg"), ("Thịt heo ba chỉ", 0.2m, "kg"),
                ("Bún", 0.2m, "kg"), ("Xà lách", 1m, "bó"), ("Giá đỗ", 0.1m, "kg")),

            BuildRecipe("Chả giò", "Chiên/Rán", Region.South, Difficulty.Medium, 4, 30, 20, 380,
                "Nem cuốn nhân thịt, miến, cà rốt chiên giòn rụm, món khai vị quen thuộc.",
                "Trộn thịt xay với miến ngâm mềm, cà rốt bào sợi, hành tím và trứng. Cuốn trong " +
                "bánh tráng, chiên ngập dầu tới khi vàng giòn.",
                MainMeals,
                ("Thịt heo xay", 0.3m, "kg"), ("Miến", 0.05m, "kg"), ("Cà rốt", 1m, "củ"),
                ("Hành tím", 1m, "củ"), ("Trứng gà", 1m, "quả"), ("Bánh tráng", 15m, "cái"),
                ("Dầu ăn", 200m, "ml")),

            BuildRecipe("Canh chua cá", "Canh/Súp", Region.South, Difficulty.Easy, 4, 15, 25, 180,
                "Canh chua thanh mát vị me, đặc trưng ẩm thực miền Tây Nam Bộ.",
                "Nấu sôi nước, cho me dằm lấy nước chua. Thả cá và cà chua vào nấu chín, nêm nếm " +
                "vừa ăn, thêm giá đỗ và hành lá trước khi tắt bếp.",
                MainMeals,
                ("Cá basa", 0.5m, "kg"), ("Me", 30m, "g"), ("Cà chua", 2m, "quả"),
                ("Giá đỗ", 0.1m, "kg"), ("Hành lá", 1m, "bó")),

            BuildRecipe("Canh cải thìa nấu tôm", "Canh/Súp", Region.South, Difficulty.Easy, 4, 15, 15, 140,
                "Canh rau củ thanh đạm, nấu nhanh cho bữa cơm gia đình hằng ngày.",
                "Phi thơm tôm băm, đổ nước vào đun sôi. Cho cải thìa vào nấu vừa chín tới, nêm " +
                "muối vừa ăn.",
                MainMeals,
                ("Cải thìa", 2m, "bó"), ("Tôm", 0.2m, "kg"), ("Hành lá", 1m, "bó"), ("Muối", 5m, "g")),

            BuildRecipe("Thịt kho trứng", "Kho", Region.South, Difficulty.Easy, 4, 15, 60, 420,
                "Thịt kho tàu nước dừa với trứng, món ăn quen thuộc ngày Tết miền Nam.",
                "Ướp thịt với nước mắm, đường rồi kho cùng nước dừa tươi và trứng luộc tới khi " +
                "thịt mềm, nước sốt sánh lại.",
                MainMeals,
                ("Thịt heo ba chỉ", 0.5m, "kg"), ("Trứng gà", 6m, "quả"), ("Nước cốt dừa", 200m, "ml"),
                ("Nước mắm", 40m, "ml"), ("Đường", 30m, "g"), ("Hành tím", 2m, "củ")),

            BuildRecipe("Cá kho tộ", "Kho", Region.South, Difficulty.Medium, 4, 15, 40, 350,
                "Cá kho đậm đà trong nồi đất, đặc sản dân dã Nam Bộ.",
                "Ướp cá với nước mắm, đường, tiêu và ớt. Kho nhỏ lửa trong nồi đất tới khi nước " +
                "sốt sánh và cá thấm đều gia vị.",
                MainMeals,
                ("Cá basa", 0.6m, "kg"), ("Nước mắm", 50m, "ml"), ("Đường", 30m, "g"),
                ("Tiêu", 5m, "g"), ("Ớt", 2m, "quả"), ("Hành tím", 2m, "củ")),

            BuildRecipe("Gà kho gừng", "Kho", Region.Central, Difficulty.Easy, 4, 15, 35, 320,
                "Gà kho gừng cay ấm, thích hợp cho những ngày trời se lạnh.",
                "Ướp gà với gừng thái sợi, tỏi và nước mắm. Kho lửa nhỏ tới khi gà chín mềm, nước " +
                "sốt sệt lại.",
                MainMeals,
                ("Thịt gà", 0.6m, "kg"), ("Gừng", 1m, "củ"), ("Nước mắm", 40m, "ml"),
                ("Đường", 20m, "g"), ("Tỏi", 3m, "củ"), ("Ớt", 2m, "quả")),

            BuildRecipe("Rau muống xào tỏi", "Xào", Region.North, Difficulty.Easy, 4, 10, 10, 90,
                "Món xào nhanh, giòn ngọt tự nhiên, gần như bắt buộc trong mâm cơm Việt.",
                "Phi thơm tỏi với dầu ăn, cho rau muống vào xào lửa lớn, nêm nước mắm rồi đảo đều " +
                "tới khi rau chín tới.",
                MainMeals,
                ("Rau muống", 2m, "bó"), ("Tỏi", 3m, "củ"), ("Dầu ăn", 30m, "ml"),
                ("Nước mắm", 15m, "ml")),

            BuildRecipe("Đậu hũ xào cà chua", "Xào", Region.North, Difficulty.Easy, 4, 10, 15, 160,
                "Món chay thanh đạm, đậu hũ mềm hòa quyện vị chua ngọt của cà chua.",
                "Chiên sơ đậu hũ cho vàng mặt ngoài. Xào cà chua chín mềm rồi cho đậu hũ vào đảo " +
                "nhẹ tay, nêm muối vừa ăn.",
                MainMeals,
                ("Đậu hũ", 4m, "miếng"), ("Cà chua", 3m, "quả"), ("Hành lá", 1m, "bó"),
                ("Dầu ăn", 30m, "ml"), ("Muối", 5m, "g")),

            BuildRecipe("Bò xào cải thìa", "Xào", Region.South, Difficulty.Medium, 4, 15, 15, 280,
                "Thịt bò xào lửa lớn giữ độ mềm, ăn cùng cải thìa giòn ngọt.",
                "Ướp thịt bò với tỏi và tiêu, xào nhanh lửa lớn cho tái rồi vớt ra. Xào cải thìa " +
                "chín tới, trộn lại với thịt bò, nêm nước mắm.",
                MainMeals,
                ("Thịt bò", 0.3m, "kg"), ("Cải thìa", 2m, "bó"), ("Tỏi", 3m, "củ"),
                ("Dầu ăn", 30m, "ml"), ("Nước mắm", 20m, "ml"), ("Tiêu", 3m, "g")),

            BuildRecipe("Tôm rang thịt", "Kho", Region.North, Difficulty.Medium, 4, 15, 20, 300,
                "Tôm và thịt ba chỉ rang săn, đậm vị mặn ngọt, ăn cùng cơm trắng rất hợp.",
                "Phi thơm hành tím, cho thịt vào xào săn rồi thêm tôm. Nêm nước mắm, đường và rang " +
                "tới khi cạn nước, tôm thịt thấm đều.",
                MainMeals,
                ("Tôm", 0.3m, "kg"), ("Thịt heo ba chỉ", 0.2m, "kg"), ("Hành tím", 2m, "củ"),
                ("Nước mắm", 30m, "ml"), ("Đường", 20m, "g")),

            BuildRecipe("Gà chiên nước mắm", "Chiên/Rán", Region.South, Difficulty.Medium, 4, 20, 20, 400,
                "Gà chiên giòn áo lớp nước mắm tỏi đường sánh vàng hấp dẫn.",
                "Áo gà qua lớp bột mì mỏng rồi chiên vàng giòn. Đun nước mắm với tỏi và đường tới " +
                "sánh lại, đảo đều gà trong hỗn hợp nước mắm.",
                MainMeals,
                ("Thịt gà", 0.6m, "kg"), ("Tỏi", 4m, "củ"), ("Nước mắm", 40m, "ml"),
                ("Đường", 30m, "g"), ("Bột mì", 0.1m, "kg"), ("Dầu ăn", 300m, "ml")),

            BuildRecipe("Chả cá", "Chiên/Rán", Region.North, Difficulty.Hard, 4, 40, 20, 340,
                "Chả cá Lã Vọng trứ danh Hà Nội, cá chiên vàng ăn kèm bún và đậu phộng rang.",
                "Ướp cá với gừng rồi chiên vàng trên chảo nóng cùng nhiều hành lá. Ăn kèm bún và " +
                "đậu phộng rang giã dập.",
                MainMeals,
                ("Cá basa", 0.5m, "kg"), ("Hành lá", 2m, "bó"), ("Bún", 0.3m, "kg"),
                ("Đậu phộng", 0.05m, "kg"), ("Gừng", 1m, "củ")),

            BuildRecipe("Bánh xèo", "Chiên/Rán", Region.South, Difficulty.Medium, 4, 30, 20, 350,
                "Bánh xèo miền Tây giòn rụm, nhân tôm thịt và giá đỗ béo ngậy nước cốt dừa.",
                "Pha bột với nước cốt dừa, đổ tráng mỏng trên chảo nóng. Cho tôm, thịt và giá đỗ " +
                "vào, gập đôi bánh khi vỏ giòn vàng.",
                MainMeals,
                ("Bột mì", 0.3m, "kg"), ("Nước cốt dừa", 200m, "ml"), ("Tôm", 0.2m, "kg"),
                ("Thịt heo ba chỉ", 0.2m, "kg"), ("Giá đỗ", 0.2m, "kg"), ("Hành lá", 1m, "bó")),

            BuildRecipe("Bún riêu cua", "Món chính", Region.North, Difficulty.Medium, 4, 30, 40, 400,
                "Bún riêu chua nhẹ với gạch cua đóng bánh, cà chua và đậu hũ chiên.",
                "Lọc lấy nước cua, đun tới khi riêu cua nổi thành bánh. Thêm cà chua, đậu hũ chiên " +
                "vào nấu cùng, trụng bún và rắc hành lá.",
                AllMeals,
                ("Bún", 0.4m, "kg"), ("Cua đồng", 0.5m, "kg"), ("Cà chua", 3m, "quả"),
                ("Đậu hũ", 3m, "miếng"), ("Trứng gà", 2m, "quả"), ("Hành lá", 1m, "bó")),

            BuildRecipe("Miến xào gà", "Xào", Region.North, Difficulty.Easy, 4, 15, 15, 310,
                "Miến xào dai mềm với thịt gà xé, cà rốt và nấm hương thơm nhẹ.",
                "Ngâm mềm miến. Xào thịt gà xé với cà rốt và nấm hương, cho miến vào đảo đều tới " +
                "khi thấm gia vị.",
                MainMeals,
                ("Miến", 0.2m, "kg"), ("Thịt gà", 0.3m, "kg"), ("Cà rốt", 1m, "củ"),
                ("Nấm hương", 0.05m, "kg"), ("Hành tím", 2m, "củ")),

            BuildRecipe("Súp cua", "Canh/Súp", Region.South, Difficulty.Medium, 4, 20, 25, 200,
                "Súp cua sánh mịn với trứng và nấm hương, món khai vị ấm bụng.",
                "Đun nước dùng cua, cho nấm hương thái nhỏ vào nấu chín. Hòa bột mì với nước rồi " +
                "rưới từ từ để tạo độ sánh, đánh trứng tan vào súp.",
                MainMeals,
                ("Cua đồng", 0.3m, "kg"), ("Trứng gà", 2m, "quả"), ("Nấm hương", 0.05m, "kg"),
                ("Hành lá", 1m, "bó"), ("Bột mì", 0.03m, "kg")),

            BuildRecipe("Nộm đu đủ", "Gỏi/Cuốn", Region.Central, Difficulty.Easy, 4, 25, 0, 180,
                "Gỏi đu đủ xanh giòn giòn, chua cay mặn ngọt hài hòa với tôm và đậu phộng.",
                "Bào sợi đu đủ xanh, trộn cùng tôm luộc. Pha nước trộn gỏi với chanh, tỏi, ớt, " +
                "đường và nước mắm, rắc đậu phộng rang trước khi ăn.",
                MainMeals,
                ("Đu đủ xanh", 1m, "quả"), ("Tôm", 0.2m, "kg"), ("Đậu phộng", 0.05m, "kg"),
                ("Chanh", 2m, "quả"), ("Tỏi", 2m, "củ"), ("Ớt", 2m, "quả"),
                ("Đường", 20m, "g"), ("Nước mắm", 30m, "ml")),

            BuildRecipe("Chè đậu xanh", "Tráng miệng", Region.South, Difficulty.Easy, 4, 20, 40, 250,
                "Chè đậu xanh ngọt bùi, thêm nước cốt dừa béo ngậy giải nhiệt ngày hè.",
                "Ninh đậu xanh tới khi mềm nhừ, thêm đường vừa ngọt. Múc ra chén, rưới nước cốt " +
                "dừa lên trên khi ăn.",
                MealTypeFlags.None,
                ("Đậu xanh", 0.3m, "kg"), ("Đường", 100m, "g"), ("Nước cốt dừa", 200m, "ml")),

            BuildRecipe("Chuối chiên", "Tráng miệng", Region.South, Difficulty.Easy, 4, 15, 15, 230,
                "Chuối chiên giòn vàng bên ngoài, mềm ngọt bên trong, món ăn vặt tuổi thơ.",
                "Nhúng chuối qua lớp bột mì pha loãng với đường. Chiên ngập dầu tới khi vàng giòn " +
                "đều các mặt.",
                MealTypeFlags.None,
                ("Chuối", 6m, "quả"), ("Bột mì", 0.15m, "kg"), ("Đường", 30m, "g"),
                ("Dầu ăn", 300m, "ml")),

            BuildRecipe("Chanh muối đường", "Đồ uống", Region.South, Difficulty.Easy, 4, 10, 0, 60,
                "Nước chanh mát lạnh vị chua ngọt hài hòa, giải khát ngày nóng.",
                "Vắt chanh lấy nước cốt, hòa cùng đường và một chút muối, thêm nước lọc và đá " +
                "trước khi dùng.",
                MealTypeFlags.None,
                ("Chanh", 4m, "quả"), ("Muối", 10m, "g"), ("Đường", 60m, "g")),

            BuildRecipe("Cá hấp gừng hành", "Hấp", Region.North, Difficulty.Easy, 4, 15, 20, 210,
                "Cá hấp gừng hành giữ trọn vị ngọt tự nhiên, thanh đạm và dễ tiêu.",
                "Xếp gừng và hành tím thái lát lên cá, hấp cách thủy tới khi cá chín tới. Phi thơm " +
                "hành lá với dầu ăn, rưới cùng nước mắm lên cá trước khi dùng.",
                MainMeals,
                ("Cá basa", 0.6m, "kg"), ("Gừng", 1m, "củ"), ("Hành lá", 1m, "bó"),
                ("Hành tím", 2m, "củ"), ("Nước mắm", 30m, "ml"))
        ];

        db.Recipes.AddRange(recipes);
        await db.SaveChangesAsync();
    }
}

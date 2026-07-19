using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RecipesController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var recipes = await db.Recipes
            .OrderBy(r => r.Name)
            .Select(r => new RecipeAdminListItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                CategoryName = r.Category.Name,
                Region = r.Region,
                Difficulty = r.Difficulty,
                IngredientCount = r.RecipeIngredients.Count
            })
            .ToListAsync();

        return View(recipes);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new RecipeFormViewModel
        {
            Ingredients = [new RecipeIngredientInputViewModel()]
        };
        await PopulateOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecipeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var recipe = new Recipe { Name = model.Name.Trim() };
        ApplyForm(recipe, model);

        db.Recipes.Add(recipe);
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã thêm món ăn mới.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe is null)
            return NotFound();

        var model = new RecipeFormViewModel
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            Servings = recipe.Servings,
            PrepMinutes = recipe.PrepMinutes,
            CookMinutes = recipe.CookMinutes,
            CaloriesPerServing = recipe.CaloriesPerServing,
            Difficulty = recipe.Difficulty,
            Region = recipe.Region,
            ImageUrl = recipe.ImageUrl,
            CategoryId = recipe.CategoryId,
            Ingredients = recipe.RecipeIngredients
                .Select(ri => new RecipeIngredientInputViewModel
                {
                    IngredientId = ri.IngredientId,
                    Quantity = ri.Quantity,
                    Unit = ri.Unit
                })
                .ToList()
        };

        await PopulateOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RecipeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateOptionsAsync(model);
            return View(model);
        }

        var recipe = await db.Recipes
            .Include(r => r.RecipeIngredients)
            .FirstOrDefaultAsync(r => r.Id == model.Id);

        if (recipe is null)
            return NotFound();

        recipe.Name = model.Name.Trim();
        ApplyForm(recipe, model);

        await db.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật món ăn.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var recipe = await db.Recipes
            .Where(r => r.Id == id)
            .Select(r => new RecipeAdminListItemViewModel
            {
                Id = r.Id,
                Name = r.Name,
                CategoryName = r.Category.Name,
                Region = r.Region,
                Difficulty = r.Difficulty,
                IngredientCount = r.RecipeIngredients.Count
            })
            .FirstOrDefaultAsync();

        if (recipe is null)
            return NotFound();

        return View(recipe);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var recipe = await db.Recipes.FindAsync(id);
        if (recipe is null)
            return NotFound();

        // RecipeIngredient rows cascade-delete with the recipe (see AppDbContext).
        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã xóa món ăn.";
        return RedirectToAction(nameof(Index));
    }

    // Copies scalar fields and rebuilds the RecipeIngredient set from the form.
    private static void ApplyForm(Recipe recipe, RecipeFormViewModel model)
    {
        recipe.Description = model.Description?.Trim();
        recipe.Instructions = model.Instructions?.Trim();
        recipe.Servings = model.Servings;
        recipe.PrepMinutes = model.PrepMinutes;
        recipe.CookMinutes = model.CookMinutes;
        recipe.CaloriesPerServing = model.CaloriesPerServing;
        recipe.Difficulty = model.Difficulty;
        recipe.Region = model.Region;
        recipe.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();
        recipe.CategoryId = model.CategoryId;

        recipe.RecipeIngredients.Clear();
        var seen = new HashSet<int>();
        foreach (var item in model.Ingredients)
        {
            if (item.IngredientId <= 0 || !seen.Add(item.IngredientId))
                continue;

            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                Unit = string.IsNullOrWhiteSpace(item.Unit) ? string.Empty : item.Unit.Trim()
            });
        }
    }

    private async Task PopulateOptionsAsync(RecipeFormViewModel model)
    {
        model.CategoryOptions = await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryOptionViewModel { Id = c.Id, Name = c.Name })
            .ToListAsync();

        model.IngredientOptions = await db.Ingredients
            .OrderBy(i => i.Group).ThenBy(i => i.Name)
            .Select(i => new IngredientOptionViewModel { Id = i.Id, Name = i.Name, Unit = i.Unit })
            .ToListAsync();
    }
}

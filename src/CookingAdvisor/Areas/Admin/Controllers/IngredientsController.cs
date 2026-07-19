using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class IngredientsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var ingredients = await db.Ingredients
            .OrderBy(i => i.Group).ThenBy(i => i.Name)
            .Select(i => new IngredientListItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Unit = i.Unit,
                Group = i.Group,
                UsageCount = i.RecipeIngredients.Count
            })
            .ToListAsync();

        return View(ingredients);
    }

    [HttpGet]
    public IActionResult Create() => View(new IngredientFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IngredientFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        db.Ingredients.Add(new Ingredient
        {
            Name = model.Name.Trim(),
            Unit = model.Unit.Trim(),
            Group = model.Group.Trim()
        });
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã thêm nguyên liệu mới.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var ingredient = await db.Ingredients.FindAsync(id);
        if (ingredient is null)
            return NotFound();

        return View(new IngredientFormViewModel
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.Unit,
            Group = ingredient.Group
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(IngredientFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var ingredient = await db.Ingredients.FindAsync(model.Id);
        if (ingredient is null)
            return NotFound();

        ingredient.Name = model.Name.Trim();
        ingredient.Unit = model.Unit.Trim();
        ingredient.Group = model.Group.Trim();
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật nguyên liệu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var ingredient = await db.Ingredients
            .Where(i => i.Id == id)
            .Select(i => new IngredientListItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Unit = i.Unit,
                Group = i.Group,
                UsageCount = i.RecipeIngredients.Count
            })
            .FirstOrDefaultAsync();

        if (ingredient is null)
            return NotFound();

        return View(ingredient);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var ingredient = await db.Ingredients
            .Include(i => i.RecipeIngredients)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (ingredient is null)
            return NotFound();

        if (ingredient.RecipeIngredients.Count > 0)
        {
            TempData["Error"] = $"Không thể xóa: nguyên liệu đang được dùng trong {ingredient.RecipeIngredients.Count} món.";
            return RedirectToAction(nameof(Index));
        }

        db.Ingredients.Remove(ingredient);
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã xóa nguyên liệu.";
        return RedirectToAction(nameof(Index));
    }
}

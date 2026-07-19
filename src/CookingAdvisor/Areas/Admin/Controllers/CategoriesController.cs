using CookingAdvisor.Data;
using CookingAdvisor.Models;
using CookingAdvisor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                RecipeCount = c.Recipes.Count
            })
            .ToListAsync();

        return View(categories);
    }

    [HttpGet]
    public IActionResult Create() => View(new CategoryFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        db.Categories.Add(new Category { Name = model.Name.Trim() });
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã thêm danh mục mới.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category is null)
            return NotFound();

        return View(new CategoryFormViewModel { Id = category.Id, Name = category.Name });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var category = await db.Categories.FindAsync(model.Id);
        if (category is null)
            return NotFound();

        category.Name = model.Name.Trim();
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã cập nhật danh mục.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await db.Categories
            .Where(c => c.Id == id)
            .Select(c => new CategoryListItemViewModel
            {
                Id = c.Id,
                Name = c.Name,
                RecipeCount = c.Recipes.Count
            })
            .FirstOrDefaultAsync();

        if (category is null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await db.Categories
            .Include(c => c.Recipes)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return NotFound();

        if (category.Recipes.Count > 0)
        {
            TempData["Error"] = $"Không thể xóa: danh mục đang có {category.Recipes.Count} món ăn.";
            return RedirectToAction(nameof(Index));
        }

        db.Categories.Remove(category);
        await db.SaveChangesAsync();

        TempData["Success"] = "Đã xóa danh mục.";
        return RedirectToAction(nameof(Index));
    }
}

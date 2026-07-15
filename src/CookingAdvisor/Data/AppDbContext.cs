using CookingAdvisor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CookingAdvisor.Data;

// Application database context. Extends IdentityDbContext<ApplicationUser> so ASP.NET
// Core Identity tables are managed here alongside the domain entities.
public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<MenuPlan> MenuPlans => Set<MenuPlan>();
    public DbSet<MenuPlanItem> MenuPlanItems => Set<MenuPlanItem>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            entity.Property(ri => ri.Quantity).HasPrecision(10, 2);

            entity.HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ri => ri.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Favorite>(entity =>
        {
            entity.HasKey(f => new { f.UserId, f.RecipeId });

            entity.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Recipe)
                .WithMany()
                .HasForeignKey(f => f.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Recipe>()
            .HasOne(r => r.Category)
            .WithMany(c => c.Recipes)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MenuPlan>()
            .HasOne(mp => mp.User)
            .WithMany()
            .HasForeignKey(mp => mp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MenuPlanItem>(entity =>
        {
            entity.HasOne(mpi => mpi.MenuPlan)
                .WithMany(mp => mp.Items)
                .HasForeignKey(mpi => mpi.MenuPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mpi => mpi.Recipe)
                .WithMany()
                .HasForeignKey(mpi => mpi.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

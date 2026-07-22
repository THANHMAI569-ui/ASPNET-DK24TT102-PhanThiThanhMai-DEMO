using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CookingAdvisor.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeSuitableMealTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SuitableMealTypes",
                table: "Recipes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuitableMealTypes",
                table: "Recipes");
        }
    }
}

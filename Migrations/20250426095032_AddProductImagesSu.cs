using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddProductImagesSu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProductImages",
                newName: "ProductImage"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProductImage",
                newName: "ProductImages"
            );
        }

    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValueTechNz.Migrations
{
    /// <inheritdoc />
    public partial class FixproductId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductKey",
                table: "Products",
                newName: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Products",
                newName: "ProductKey");
        }
    }
}

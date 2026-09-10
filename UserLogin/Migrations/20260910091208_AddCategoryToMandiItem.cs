using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserLogin.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToMandiItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "MandiItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "MandiItems");
        }
    }
}

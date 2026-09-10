using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserLogin.Migrations
{
    /// <inheritdoc />
    public partial class AddYesterdayRateToMandiItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "YesterdayRatePerQuintal",
                table: "MandiItems",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YesterdayRatePerQuintal",
                table: "MandiItems");
        }
    }
}

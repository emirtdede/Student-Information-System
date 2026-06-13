using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Student_Information_System.Migrations
{
    /// <inheritdoc />
    public partial class AddDiningBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiningBalance",
                table: "Users",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiningBalance",
                table: "Users");
        }
    }
}

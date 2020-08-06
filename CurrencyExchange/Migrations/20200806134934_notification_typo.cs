using Microsoft.EntityFrameworkCore.Migrations;

namespace CurrencyExchange.Migrations
{
    public partial class notification_typo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboverOrUnder",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "AboveOrUnder",
                table: "Notifications",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboveOrUnder",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "AboverOrUnder",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

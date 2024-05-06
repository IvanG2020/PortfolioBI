using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioBI.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateFinancialDateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ticker",
                table: "FinancialData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ticker",
                table: "FinancialData");
        }
    }
}

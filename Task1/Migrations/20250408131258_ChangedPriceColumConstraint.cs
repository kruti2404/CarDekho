using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Task1.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPriceColumConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Vehicles_Price",
                table: "Vehicles");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Vehicles_Price",
                table: "Vehicles",
                sql: "[Price] >= 200000 AND [Price] <= 200000000");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Vehicles_Price",
                table: "Vehicles");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Vehicles_Price",
                table: "Vehicles",
                sql: "[Price] >= 200000 AND [Price] <= 2000000");
        }
    }
}

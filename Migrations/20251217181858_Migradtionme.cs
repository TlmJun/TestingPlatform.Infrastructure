using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migradtionme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 17, 18, 16, 13, 279, DateTimeKind.Utc).AddTicks(6170));
        }
    }
}

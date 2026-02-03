using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migrationme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "Login", "MiddleName", "PasswordHash", "Role" },
                values: new object[] { 100, new DateTime(2025, 12, 17, 18, 16, 13, 279, DateTimeKind.Utc).AddTicks(6170), "manager@local", "Иван", "Иванов", "manager", "Иванович", "$2a$11$pThhbbceEToQ9dK9L/7yo.hZ/hi6Kg4mlXa5Z0X8T3OF61O0wHGUW", "Manager" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);
        }
    }
}

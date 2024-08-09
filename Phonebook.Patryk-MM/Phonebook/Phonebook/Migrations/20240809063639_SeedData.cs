using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Phonebook.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PhoneEntries",
                columns: new[] { "Id", "Email", "Name", "PhoneNumber" },
                values: new object[] { 1, "test@test.com", "test", "111222333" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PhoneEntries",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}

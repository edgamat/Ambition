using System.Diagnostics.CodeAnalysis;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambition.Infrastructure.Migrations
{
    [ExcludeFromCodeCoverage]
    /// <inheritdoc />
    public partial class LookupData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "Email", "Name", "Phone" },
                values: new object[] { new Guid("4f8b1c36-047b-4c5c-99e4-bf5446856d14"), "123 Main", "test@example.com", "John Doe", "123-456-7890" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[] { new Guid("45a340d8-7bd1-4f56-95b3-8ca192ef6094"), "A widget", "Widget", 10.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("4f8b1c36-047b-4c5c-99e4-bf5446856d14"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("45a340d8-7bd1-4f56-95b3-8ca192ef6094"));
        }
    }
}

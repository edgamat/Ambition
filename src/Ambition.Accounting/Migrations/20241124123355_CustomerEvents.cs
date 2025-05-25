using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambition.Accounting.Migrations
{
    /// <inheritdoc />
    public partial class CustomerEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("00948d76-1fc6-4bed-9355-6eba541017f2"));

            migrationBuilder.CreateTable(
                name: "CustomerEvents",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerEvents", x => x.EventId);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[] { new Guid("c2324a1e-d73d-462f-b6e9-ca4c807c502c"), "test@example.com", "John Doe" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerEvents");

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("c2324a1e-d73d-462f-b6e9-ca4c807c502c"));

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[] { new Guid("00948d76-1fc6-4bed-9355-6eba541017f2"), "test@example.com", "John Doe" });
        }
    }
}

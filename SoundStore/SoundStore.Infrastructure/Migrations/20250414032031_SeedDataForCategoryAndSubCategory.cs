using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoundStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataForCategoryAndSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOA MARSHALL", null },
                    { 2, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "TAI NGHE MARSHALL", null },
                    { 3, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "PHỤ KIỆN LIFESTYLE", null }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOA DI ĐỘNG", null },
                    { 2, 1, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "LOA NGHE TRONG NHÀ", null },
                    { 3, 1, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "LIMITED EDITION", null },
                    { 4, 2, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "TRUE WIRELESS", null },
                    { 5, 2, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "ON-EAR", null },
                    { 6, 2, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "OVER-EAR", null },
                    { 7, 2, new DateTime(2025, 4, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "IN-EAR", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Common.Rest.Address.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uprn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SingleLineAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Locality = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Town = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Postcode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    AddressJson = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AddressJson", "BuildingName", "BuildingNumber", "Country", "CreatedAt", "CreatedBy", "IsDeleted", "Latitude", "Locality", "Longitude", "Postcode", "SingleLineAddress", "Street", "Town", "UpdatedAt", "UpdatedBy", "Uprn" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-6c7d-8e9f0a1b2c3d"), "{\"Uprn\":\"100021234567\",\"SingleLineAddress\":\"1 High St, Edinburgh, EH1 1AA\",\"BuildingName\":\"Main Building\",\"BuildingNumber\":\"1\",\"Street\":\"High St\",\"Locality\":\"\",\"Town\":\"Edinburgh\",\"Postcode\":\"EH1 1AA\"}", "Main Building", "1", "United Kingdom", new DateTimeOffset(new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, "City Centre", null, "EH1 1AA", "1 High St, Edinburgh, EH1 1AA", "High St", "Edinburgh", null, null, "100021234567" },
                    { new Guid("f62e8b2a-8c88-4c8d-8d9e-1f2a3c4d5e6f"), "{\"Uprn\":\"100023336491\",\"SingleLineAddress\":\"10 Downing St, London, SW1A 2AA\",\"BuildingName\":\"Prime Minister\\u0027s Residence\",\"BuildingNumber\":\"10\",\"Street\":\"Downing St\",\"Locality\":\"\",\"Town\":\"London\",\"Postcode\":\"SW1A 2AA\"}", "Prime Minister's Residence", "10", "United Kingdom", new DateTimeOffset(new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, false, null, "Westminster", null, "SW1A 2AA", "10 Downing St, London, SW1A 2AA", "Downing St", "London", null, null, "100023336491" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Postcode",
                table: "Addresses",
                column: "Postcode");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_Uprn",
                table: "Addresses",
                column: "Uprn",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");
        }
    }
}

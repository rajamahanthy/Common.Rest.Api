using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Address.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAddressModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AddressJson",
                table: "Addresses",
                newName: "AdditionalInfoJson");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-6c7d-8e9f0a1b2c3d"),
                column: "AdditionalInfoJson",
                value: "{\"AddressLine1\":\"1 High St\",\"AddressLine2\":\"Edinburgh\",\"AddressLine3\":\"EH1 1AA\",\"AddressLine4\":\"\",\"AddressLine5\":\"\"}");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("f62e8b2a-8c88-4c8d-8d9e-1f2a3c4d5e6f"),
                column: "AdditionalInfoJson",
                value: "{\"AddressLine1\":\"10 Downing St\",\"AddressLine2\":\"London\",\"AddressLine3\":\"SW1A 2AA\",\"AddressLine4\":\"\",\"AddressLine5\":\"\"}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdditionalInfoJson",
                table: "Addresses",
                newName: "AddressJson");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a5b-6c7d-8e9f0a1b2c3d"),
                column: "AddressJson",
                value: "{\"Uprn\":\"100021234567\",\"SingleLineAddress\":\"1 High St, Edinburgh, EH1 1AA\",\"BuildingName\":\"Main Building\",\"BuildingNumber\":\"1\",\"Street\":\"High St\",\"Locality\":\"\",\"Town\":\"Edinburgh\",\"Postcode\":\"EH1 1AA\"}");

            migrationBuilder.UpdateData(
                table: "Addresses",
                keyColumn: "Id",
                keyValue: new Guid("f62e8b2a-8c88-4c8d-8d9e-1f2a3c4d5e6f"),
                column: "AddressJson",
                value: "{\"Uprn\":\"100023336491\",\"SingleLineAddress\":\"10 Downing St, London, SW1A 2AA\",\"BuildingName\":\"Prime Minister\\u0027s Residence\",\"BuildingNumber\":\"10\",\"Street\":\"Downing St\",\"Locality\":\"\",\"Town\":\"London\",\"Postcode\":\"SW1A 2AA\"}");
        }
    }
}

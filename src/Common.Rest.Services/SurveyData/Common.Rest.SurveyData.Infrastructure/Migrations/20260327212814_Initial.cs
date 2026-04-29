using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Common.Rest.SurveyData.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Surveys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PropertyAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LocalAuthority = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SurveyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SurveyDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    Surveyor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AssessedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FloorArea = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    FloorAreaUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertySubType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SurveyJson = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Area = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    AreaUnit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RatePerUnit = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyDetails_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Surveys",
                columns: new[] { "Id", "AssessedValue", "CreatedAt", "CreatedBy", "FloorArea", "FloorAreaUnit", "IsDeleted", "LocalAuthority", "Notes", "PostCode", "PropertyAddress", "PropertySubType", "PropertyType", "ReferenceNumber", "Status", "SurveyDate", "SurveyJson", "SurveyType", "Surveyor", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a5b-6c7d-8e9f0a1b2c3d"), 1000000m, new DateTimeOffset(new DateTime(2026, 3, 27, 21, 28, 12, 172, DateTimeKind.Unspecified).AddTicks(1814), new TimeSpan(0, 0, 0, 0, 0)), null, 1000m, "sqft", false, "Edinburgh", "This is a test survey", "EH1 1AA", "1 High St, Edinburgh, EH1 1AA", "Detached", "House", "100021234567", "Draft", new DateTimeOffset(new DateTime(2026, 3, 27, 21, 28, 12, 172, DateTimeKind.Unspecified).AddTicks(1871), new TimeSpan(0, 0, 0, 0, 0)), "{\"Uprn\":\"100021234567\",\"SingleLineAddress\":\"1 High St, Edinburgh, EH1 1AA\",\"BuildingName\":\"Main Building\",\"BuildingNumber\":\"1\",\"Street\":\"High St\",\"Locality\":\"\",\"Town\":\"Edinburgh\",\"Postcode\":\"EH1 1AA\"}", "Valuation", "John Doe", null, null },
                    { new Guid("f62e8b2a-8c88-4c8d-8d9e-1f2a3c4d5e6f"), 1000000m, new DateTimeOffset(new DateTime(2026, 3, 27, 21, 28, 12, 168, DateTimeKind.Unspecified).AddTicks(709), new TimeSpan(0, 0, 0, 0, 0)), null, 1000m, "sqft", false, "Westminster", "This is a test survey", "SW1A 2AA", "10 Downing St, London, SW1A 2AA", "Detached", "House", "100023336491", "Draft", new DateTimeOffset(new DateTime(2025, 3, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "{\"Uprn\":\"100023336491\",\"SingleLineAddress\":\"10 Downing St, London, SW1A 2AA\",\"BuildingName\":\"Prime Minister\\u0027s Residence\",\"BuildingNumber\":\"10\",\"Street\":\"Downing St\",\"Locality\":\"\",\"Town\":\"London\",\"Postcode\":\"SW1A 2AA\"}", "Valuation", "John Doe", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetails_SurveyId",
                table: "SurveyDetails",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_Surveys_ReferenceNumber",
                table: "Surveys",
                column: "ReferenceNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyDetails");

            migrationBuilder.DropTable(
                name: "Surveys");
        }
    }
}

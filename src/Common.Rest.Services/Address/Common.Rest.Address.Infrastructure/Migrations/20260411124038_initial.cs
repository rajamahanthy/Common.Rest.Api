using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Rest.Address.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UprnIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.uprn')", stored: true),
                    PostcodeIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.address.postcode')", stored: true),
                    PostTownIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.address.streetDescriptor.postTown')", stored: true),
                    OrganisationIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.address.organisation')", stored: true),
                    ThoroughfareIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.address.streetDescriptor.streetDescription')", stored: true),
                    LocalityIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.address.streetDescriptor.locality')", stored: true),
                    DependentLocalityIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.address.streetDescriptor.dependentLocality')", stored: true),
                    PartitionKey = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    DocumentType = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    JsonData = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false, defaultValue: "{}"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DependentLocalityIndex",
                table: "Documents",
                column: "DependentLocalityIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentType",
                table: "Documents",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IsDeleted",
                table: "Documents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_LocalityIndex",
                table: "Documents",
                column: "LocalityIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OrganisationIndex",
                table: "Documents",
                column: "OrganisationIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartitionKey",
                table: "Documents",
                column: "PartitionKey");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartitionKey_DocumentType",
                table: "Documents",
                columns: new[] { "PartitionKey", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PostcodeIndex",
                table: "Documents",
                column: "PostcodeIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PostTownIndex",
                table: "Documents",
                column: "PostTownIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ThoroughfareIndex",
                table: "Documents",
                column: "ThoroughfareIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UprnIndex",
                table: "Documents",
                column: "UprnIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}

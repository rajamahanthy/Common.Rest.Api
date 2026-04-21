using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Rest.Hereditament.Infrastructure.Migrations
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
                    NameIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.Hereditament.postcode')", stored: true),
                    StatusIndex = table.Column<string>(type: "nvarchar(450)", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.Hereditament.streetDescriptor.postTown')", stored: true),
                    EffectiveFromIndex = table.Column<DateOnly>(type: "date", nullable: true, computedColumnSql: "JSON_VALUE([JsonData], '$.Hereditament.organisation')", stored: true),
                    PartitionKey = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    DocumentType = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    JsonData = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false, defaultValue: "{\"uarn\":\"674e7bda-1476-44eb-94fb-7788a5813bbb\",\"name\":\"\",\"effectiveFrom\":\"2026-04-21\",\"status\":\"Draft\"}"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentType",
                table: "Documents",
                column: "DocumentType");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_EffectiveFromIndex",
                table: "Documents",
                column: "EffectiveFromIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_IsDeleted",
                table: "Documents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_NameIndex",
                table: "Documents",
                column: "NameIndex");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartitionKey",
                table: "Documents",
                column: "PartitionKey");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartitionKey_DocumentType",
                table: "Documents",
                columns: new[] { "PartitionKey", "DocumentType" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_StatusIndex",
                table: "Documents",
                column: "StatusIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}

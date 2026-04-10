using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Rest.Address.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    JsonData = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false, defaultValue: "{\"Country\":\"United Kingdom\"}"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    PartitionKey = table.Column<string>(type: "NVARCHAR(100)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    CreatedBy = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "DATETIME2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
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
                name: "IX_Documents_IsDeleted",
                table: "Documents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartitionKey",
                table: "Documents",
                column: "PartitionKey");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PartitionKey_DocumentType",
                table: "Documents",
                columns: new[] { "PartitionKey", "DocumentType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}

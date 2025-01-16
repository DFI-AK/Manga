using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manga.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsageAndDetailTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProcessorCount = table.Column<int>(type: "int", nullable: false),
                    Architecture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalMemoryKB = table.Column<long>(type: "bigint", nullable: false),
                    FreeMemoryKB = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemUsages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false, defaultValueSql: "NEWID()"),
                    SystemId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CpuUsagePercentage = table.Column<double>(type: "float", nullable: false),
                    MemoryUsagePercentage = table.Column<double>(type: "float", nullable: false),
                    NetworkBytesSent = table.Column<long>(type: "bigint", nullable: false),
                    NetworkBytesReceived = table.Column<long>(type: "bigint", nullable: false),
                    DiskReadBytes = table.Column<long>(type: "bigint", nullable: false),
                    DiskWriteBytes = table.Column<long>(type: "bigint", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemUsages_SystemDetails_SystemId",
                        column: x => x.SystemId,
                        principalTable: "SystemDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemUsages_SystemId",
                table: "SystemUsages",
                column: "SystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemUsages");

            migrationBuilder.DropTable(
                name: "SystemDetails");
        }
    }
}

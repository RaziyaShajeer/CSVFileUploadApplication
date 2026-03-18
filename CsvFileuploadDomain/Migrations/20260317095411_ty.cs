using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CsvFileuploadDomain.Migrations
{
    /// <inheritdoc />
    public partial class ty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "FileProcesses");

            migrationBuilder.RenameColumn(
                name: "CurrentRow",
                table: "FileProcesses",
                newName: "ProcessedRows");

            migrationBuilder.AddColumn<int>(
                name: "LastProcessedRow",
                table: "FileProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastProcessedRow",
                table: "FileProcesses");

            migrationBuilder.RenameColumn(
                name: "ProcessedRows",
                table: "FileProcesses",
                newName: "CurrentRow");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "FileProcesses",
                type: "datetime2",
                nullable: true);
        }
    }
}

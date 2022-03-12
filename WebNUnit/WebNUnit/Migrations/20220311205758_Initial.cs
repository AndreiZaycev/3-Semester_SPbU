using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebNUnit.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssembliesHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssembliesHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TestViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IgnoreReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LoadedAssembliesViewModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestViewModel_AssembliesHistory_LoadedAssembliesViewModelId",
                        column: x => x.LoadedAssembliesViewModelId,
                        principalTable: "AssembliesHistory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestViewModel_LoadedAssembliesViewModelId",
                table: "TestViewModel",
                column: "LoadedAssembliesViewModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestViewModel");

            migrationBuilder.DropTable(
                name: "AssembliesHistory");
        }
    }
}

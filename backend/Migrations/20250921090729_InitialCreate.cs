using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Таблица пользователей
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                          .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            // Таблица сайтов
            migrationBuilder.CreateTable(
                name: "WebSites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                          .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    URL = table.Column<string>(nullable: false),
                    ExpectedContent = table.Column<string>(nullable: false),
                    ResponseTime = table.Column<string>(nullable: true),
                    IsAvailable = table.Column<bool>(nullable: false),
                    DNS = table.Column<string>(nullable: true),
                    SSL = table.Column<string>(nullable: true),
                    TotalErrors = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebSites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Таблица данных о сайтах
            migrationBuilder.CreateTable(
                name: "WebSiteData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    StatusCode = table.Column<int>(nullable: true),
                    ErrorMessage = table.Column<string>(nullable: true),
                    LastChecked = table.Column<DateTime>(nullable: false),
                    WebSiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSiteData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebSiteData_WebSites_WebSiteId",
                        column: x => x.WebSiteId,
                        principalTable: "WebSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Таблица тестовых сценариев
            migrationBuilder.CreateTable(
                name: "TestScenarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                          .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    HttpMethod = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    ExpectedContent = table.Column<string>(nullable: true),
                    CheckJson = table.Column<bool>(nullable: false),
                    CheckXml = table.Column<bool>(nullable: false),
                    HeadersJson = table.Column<string>(nullable: true),
                    WebSiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestScenarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestScenarios_WebSites_WebSiteId",
                        column: x => x.WebSiteId,
                        principalTable: "WebSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Таблица результатов сценариев
            migrationBuilder.CreateTable(
                name: "ScenarioResults",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    StatusCode = table.Column<int>(nullable: false),
                    ResponseTime = table.Column<string>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: false),
                    LastChecked = table.Column<DateTime>(nullable: false),
                    WebSiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioResults_WebSites_WebSiteId",
                        column: x => x.WebSiteId,
                        principalTable: "WebSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Индексы для ускорения поиска
            migrationBuilder.CreateIndex(
                name: "IX_WebSites_UserId",
                table: "WebSites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WebSiteData_WebSiteId",
                table: "WebSiteData",
                column: "WebSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_TestScenarios_WebSiteId",
                table: "TestScenarios",
                column: "WebSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioResults_WebSiteId",
                table: "ScenarioResults",
                column: "WebSiteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ScenarioResults");
            migrationBuilder.DropTable(name: "TestScenarios");
            migrationBuilder.DropTable(name: "WebSiteData");
            migrationBuilder.DropTable(name: "WebSites");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}

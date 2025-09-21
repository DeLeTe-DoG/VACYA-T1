using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDnsSslResponseTimeToWebSiteData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DNS",
                table: "WebSiteData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseTime",
                table: "WebSiteData",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SSL",
                table: "WebSiteData",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DNS",
                table: "WebSiteData");

            migrationBuilder.DropColumn(
                name: "ResponseTime",
                table: "WebSiteData");

            migrationBuilder.DropColumn(
                name: "SSL",
                table: "WebSiteData");
        }
    }
}

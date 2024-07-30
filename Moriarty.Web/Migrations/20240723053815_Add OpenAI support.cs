using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moriarty.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddOpenAIsupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpenAIFileId",
                table: "Campaigns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenAIFileId",
                table: "Campaigns");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PollSurveyBuilder.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorToken",
                table: "Polls",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorToken",
                table: "Polls");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace CinemaHub.Data.Migrations
{
    public partial class IsTrainedRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTrained",
                table: "Rating",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTrained",
                table: "Rating");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessFox.Migrations
{
    /// <inheritdoc />
    public partial class user_goals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "HeightInches",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeightInches",
                table: "AspNetUsers");
        }
    }
}

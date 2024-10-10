using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateDB10001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userPeakID",
                table: "UserPeaks");

            migrationBuilder.DropColumn(
                name: "AwardUserID",
                table: "UserAwards");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "userPeakID",
                table: "UserPeaks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AwardUserID",
                table: "UserAwards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}

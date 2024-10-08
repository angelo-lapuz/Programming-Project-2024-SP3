using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class dbbUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAwards_AspNetUsers_UserID",
                table: "UserAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UserID",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserPeaks",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserPeaks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserAwards",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserAwards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserPeaks_PeakID",
                table: "UserPeaks",
                column: "PeakID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAwards_AwardID",
                table: "UserAwards",
                column: "AwardID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAwards_AspNetUsers_UserID",
                table: "UserAwards",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UserID",
                table: "UserPeaks",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAwards_AspNetUsers_UserID",
                table: "UserAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UserID",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks");

            migrationBuilder.DropIndex(
                name: "IX_UserPeaks_PeakID",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards");

            migrationBuilder.DropIndex(
                name: "IX_UserAwards_AwardID",
                table: "UserAwards");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPeaks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserAwards");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserPeaks",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "UserAwards",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks",
                columns: new[] { "PeakID", "UserID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards",
                columns: new[] { "AwardID", "UserID" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAwards_AspNetUsers_UserID",
                table: "UserAwards",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UserID",
                table: "UserPeaks",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

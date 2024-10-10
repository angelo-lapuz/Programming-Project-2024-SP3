using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateDB1000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAwards_AspNetUsers_UsersId",
                table: "UserAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAwards_Awards_AwardsAwardID",
                table: "UserAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UsersId",
                table: "UserPeaks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPeaks_Peaks_PeaksPeakID",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks");

            migrationBuilder.DropIndex(
                name: "IX_UserPeaks_UsersId",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards");

            migrationBuilder.DropIndex(
                name: "IX_UserAwards_UsersId",
                table: "UserAwards");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "UserPeaks");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "UserAwards");

            migrationBuilder.RenameColumn(
                name: "PeaksPeakID",
                table: "UserPeaks",
                newName: "userPeakID");

            migrationBuilder.RenameColumn(
                name: "AwardsAwardID",
                table: "UserAwards",
                newName: "AwardUserID");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserPeaks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "PeakID",
                table: "UserPeaks",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "UserPeaks",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserAwards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "AwardID",
                table: "UserAwards",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "UserAwards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Difficulty",
                table: "Peaks",
                type: "TEXT",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(char),
                oldType: "char",
                oldMaxLength: 1);

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
                name: "IX_UserPeaks_UserID",
                table: "UserPeaks",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAwards_AwardID",
                table: "UserAwards",
                column: "AwardID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAwards_UserID",
                table: "UserAwards",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAwards_AspNetUsers_UserID",
                table: "UserAwards",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAwards_Awards_AwardID",
                table: "UserAwards",
                column: "AwardID",
                principalTable: "Awards",
                principalColumn: "AwardID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UserID",
                table: "UserPeaks",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPeaks_Peaks_PeakID",
                table: "UserPeaks",
                column: "PeakID",
                principalTable: "Peaks",
                principalColumn: "PeakID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAwards_AspNetUsers_UserID",
                table: "UserAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAwards_Awards_AwardID",
                table: "UserAwards");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UserID",
                table: "UserPeaks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPeaks_Peaks_PeakID",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks");

            migrationBuilder.DropIndex(
                name: "IX_UserPeaks_PeakID",
                table: "UserPeaks");

            migrationBuilder.DropIndex(
                name: "IX_UserPeaks_UserID",
                table: "UserPeaks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards");

            migrationBuilder.DropIndex(
                name: "IX_UserAwards_AwardID",
                table: "UserAwards");

            migrationBuilder.DropIndex(
                name: "IX_UserAwards_UserID",
                table: "UserAwards");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserPeaks");

            migrationBuilder.DropColumn(
                name: "PeakID",
                table: "UserPeaks");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserPeaks");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserAwards");

            migrationBuilder.DropColumn(
                name: "AwardID",
                table: "UserAwards");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserAwards");

            migrationBuilder.RenameColumn(
                name: "userPeakID",
                table: "UserPeaks",
                newName: "PeaksPeakID");

            migrationBuilder.RenameColumn(
                name: "AwardUserID",
                table: "UserAwards",
                newName: "AwardsAwardID");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "UserPeaks",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "UserAwards",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<char>(
                name: "Difficulty",
                table: "Peaks",
                type: "char",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPeaks",
                table: "UserPeaks",
                columns: new[] { "PeaksPeakID", "UsersId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAwards",
                table: "UserAwards",
                columns: new[] { "AwardsAwardID", "UsersId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPeaks_UsersId",
                table: "UserPeaks",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAwards_UsersId",
                table: "UserAwards",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAwards_AspNetUsers_UsersId",
                table: "UserAwards",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAwards_Awards_AwardsAwardID",
                table: "UserAwards",
                column: "AwardsAwardID",
                principalTable: "Awards",
                principalColumn: "AwardID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPeaks_AspNetUsers_UsersId",
                table: "UserPeaks",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPeaks_Peaks_PeaksPeakID",
                table: "UserPeaks",
                column: "PeaksPeakID",
                principalTable: "Peaks",
                principalColumn: "PeakID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

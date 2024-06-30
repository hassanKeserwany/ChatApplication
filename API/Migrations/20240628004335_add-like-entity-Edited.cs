using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class addlikeentityEdited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Users_AppUserId",
                table: "Photo");

            migrationBuilder.DropForeignKey(
                name: "FK_userLike_Users_LikedUserId",
                table: "userLike");

            migrationBuilder.DropForeignKey(
                name: "FK_userLike_Users_SourceUserId",
                table: "userLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userLike",
                table: "userLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photo",
                table: "Photo");

            migrationBuilder.RenameTable(
                name: "userLike",
                newName: "UserLikes");

            migrationBuilder.RenameTable(
                name: "Photo",
                newName: "Photos");

            migrationBuilder.RenameIndex(
                name: "IX_userLike_LikedUserId",
                table: "UserLikes",
                newName: "IX_UserLikes_LikedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Photo_AppUserId",
                table: "Photos",
                newName: "IX_Photos_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLikes",
                table: "UserLikes",
                columns: new[] { "SourceUserId", "LikedUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photos",
                table: "Photos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_AppUserId",
                table: "Photos",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_Users_LikedUserId",
                table: "UserLikes",
                column: "LikedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLikes_Users_SourceUserId",
                table: "UserLikes",
                column: "SourceUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_AppUserId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_Users_LikedUserId",
                table: "UserLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLikes_Users_SourceUserId",
                table: "UserLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLikes",
                table: "UserLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photos",
                table: "Photos");

            migrationBuilder.RenameTable(
                name: "UserLikes",
                newName: "userLike");

            migrationBuilder.RenameTable(
                name: "Photos",
                newName: "Photo");

            migrationBuilder.RenameIndex(
                name: "IX_UserLikes_LikedUserId",
                table: "userLike",
                newName: "IX_userLike_LikedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Photos_AppUserId",
                table: "Photo",
                newName: "IX_Photo_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userLike",
                table: "userLike",
                columns: new[] { "SourceUserId", "LikedUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photo",
                table: "Photo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Users_AppUserId",
                table: "Photo",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userLike_Users_LikedUserId",
                table: "userLike",
                column: "LikedUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userLike_Users_SourceUserId",
                table: "userLike",
                column: "SourceUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

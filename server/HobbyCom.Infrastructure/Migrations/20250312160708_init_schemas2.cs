using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_schemas2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sessions_userProfiles_user_id",
                schema: "public",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userProfiles",
                schema: "public",
                table: "userProfiles");

            migrationBuilder.RenameTable(
                name: "userProfiles",
                schema: "public",
                newName: "user_profiles",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_userProfiles_username",
                schema: "public",
                table: "user_profiles",
                newName: "IX_user_profiles_username");

            migrationBuilder.RenameIndex(
                name: "IX_userProfiles_email",
                schema: "public",
                table: "user_profiles",
                newName: "IX_user_profiles_email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_profiles",
                schema: "public",
                table: "user_profiles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_user_profiles_user_id",
                schema: "public",
                table: "sessions",
                column: "user_id",
                principalSchema: "public",
                principalTable: "user_profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_sessions_user_profiles_user_id",
                schema: "public",
                table: "sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_profiles",
                schema: "public",
                table: "user_profiles");

            migrationBuilder.RenameTable(
                name: "user_profiles",
                schema: "public",
                newName: "userProfiles",
                newSchema: "public");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_username",
                schema: "public",
                table: "userProfiles",
                newName: "IX_userProfiles_username");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_email",
                schema: "public",
                table: "userProfiles",
                newName: "IX_userProfiles_email");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userProfiles",
                schema: "public",
                table: "userProfiles",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_sessions_userProfiles_user_id",
                schema: "public",
                table: "sessions",
                column: "user_id",
                principalSchema: "public",
                principalTable: "userProfiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

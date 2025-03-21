using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_schemas5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "token_revoked",
                schema: "public",
                table: "refresh_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "token_revoked",
                schema: "public",
                table: "refresh_tokens");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_schemas3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "public",
                table: "refresh_tokens");

            migrationBuilder.AddColumn<string>(
                name: "encrypted_password",
                schema: "public",
                table: "user_profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

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
                name: "encrypted_password",
                schema: "public",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "token_revoked",
                schema: "public",
                table: "refresh_tokens");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "public",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true,
                defaultValueSql: "current_timestamp");
        }
    }
}

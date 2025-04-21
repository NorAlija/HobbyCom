using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HobbyCom.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class init_schemas6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expired_at",
                schema: "public",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "type",
                schema: "public",
                table: "user_profiles",
                newName: "role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "role",
                schema: "public",
                table: "user_profiles",
                newName: "type");

            migrationBuilder.AddColumn<DateTime>(
                name: "expired_at",
                schema: "public",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}

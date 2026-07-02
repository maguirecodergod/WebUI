using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LHA.Account.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenIssueAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IssuedAtUnixSeconds",
                table: "Auth_UserToken",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssuedAtUnixSeconds",
                table: "Auth_UserToken");
        }
    }
}

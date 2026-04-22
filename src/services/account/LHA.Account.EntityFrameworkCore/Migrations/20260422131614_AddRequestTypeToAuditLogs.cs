using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LHA.Account.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTypeToAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "RequestType",
                table: "Audit_LogPipeline",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "RequestType",
                table: "Audit_Log",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_RequestType",
                table: "Audit_LogPipeline",
                column: "RequestType");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_RequestType",
                table: "Audit_Log",
                column: "RequestType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Audit_LogPipeline_RequestType",
                table: "Audit_LogPipeline");

            migrationBuilder.DropIndex(
                name: "IX_Audit_Log_RequestType",
                table: "Audit_Log");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Audit_LogPipeline");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Audit_Log");
        }
    }
}

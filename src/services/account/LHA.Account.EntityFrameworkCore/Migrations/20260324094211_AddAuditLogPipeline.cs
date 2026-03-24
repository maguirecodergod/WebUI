using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LHA.Account.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogPipeline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogPipeline",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    InstanceId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ActionName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ActionType = table.Column<byte>(type: "smallint", nullable: false),
                    UserId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    TraceId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SpanId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    RequestPath = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    RequestBody = table.Column<string>(type: "text", nullable: true),
                    ResponseBody = table.Column<string>(type: "text", nullable: true),
                    ClientIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Tags = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogPipeline", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogPipeline_ServiceName_Timestamp",
                table: "AuditLogPipeline",
                columns: new[] { "ServiceName", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogPipeline_TenantId",
                table: "AuditLogPipeline",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogPipeline_Timestamp",
                table: "AuditLogPipeline",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogPipeline_TraceId",
                table: "AuditLogPipeline",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogPipeline_UserId",
                table: "AuditLogPipeline",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogPipeline");
        }
    }
}

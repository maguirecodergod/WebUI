using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LHA.Account.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAuditLogPipelineIdentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit_LogPipeline");

            migrationBuilder.CreateTable(
                name: "Audit_LogPipeline",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    InstanceId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ActionName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ActionType = table.Column<byte>(type: "smallint", nullable: false),
                    RequestType = table.Column<byte>(type: "smallint", nullable: false),
                    UserId = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Audit_LogPipeline", x => x.Id);
                });

            CreateAuditLogPipelineIndexes(migrationBuilder);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit_LogPipeline");

            migrationBuilder.CreateTable(
                name: "Audit_LogPipeline",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DurationMs = table.Column<long>(type: "bigint", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    InstanceId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ActionName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ActionType = table.Column<byte>(type: "smallint", nullable: false),
                    RequestType = table.Column<byte>(type: "smallint", nullable: false),
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
                    table.PrimaryKey("PK_Audit_LogPipeline", x => x.Id);
                });

            CreateAuditLogPipelineIndexes(migrationBuilder);
        }

        private static void CreateAuditLogPipelineIndexes(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_RequestType",
                table: "Audit_LogPipeline",
                column: "RequestType");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_ServiceName_Timestamp",
                table: "Audit_LogPipeline",
                columns: new[] { "ServiceName", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_TenantId",
                table: "Audit_LogPipeline",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_Timestamp",
                table: "Audit_LogPipeline",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_TraceId",
                table: "Audit_LogPipeline",
                column: "TraceId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_LogPipeline_UserId",
                table: "Audit_LogPipeline",
                column: "UserId");
        }
    }
}

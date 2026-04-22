using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LHA.AuditLog.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestTypeToAuditModuleLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogActions_AuditLogs_AuditLogId",
                table: "AuditLogActions");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityChanges_AuditLogs_AuditLogId",
                table: "EntityChanges");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityPropertyChanges_EntityChanges_EntityChangeId",
                table: "EntityPropertyChanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityPropertyChanges",
                table: "EntityPropertyChanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityChanges",
                table: "EntityChanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogActions",
                table: "AuditLogActions");

            migrationBuilder.RenameTable(
                name: "EntityPropertyChanges",
                newName: "Audit_PropertyChange");

            migrationBuilder.RenameTable(
                name: "EntityChanges",
                newName: "Audit_EntityChange");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "Audit_Log");

            migrationBuilder.RenameTable(
                name: "AuditLogActions",
                newName: "Audit_Action");

            migrationBuilder.RenameIndex(
                name: "IX_EntityPropertyChanges_EntityChangeId",
                table: "Audit_PropertyChange",
                newName: "IX_Audit_PropertyChange_EntityChangeId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityChanges_TenantId",
                table: "Audit_EntityChange",
                newName: "IX_Audit_EntityChange_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityChanges_EntityTypeFullName_EntityId",
                table: "Audit_EntityChange",
                newName: "IX_Audit_EntityChange_EntityTypeFullName_EntityId");

            migrationBuilder.RenameIndex(
                name: "IX_EntityChanges_AuditLogId",
                table: "Audit_EntityChange",
                newName: "IX_Audit_EntityChange_AuditLogId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "Audit_Log",
                newName: "IX_Audit_Log_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_TenantId",
                table: "Audit_Log",
                newName: "IX_Audit_Log_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_HttpStatusCode",
                table: "Audit_Log",
                newName: "IX_Audit_Log_HttpStatusCode");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_ExecutionTime",
                table: "Audit_Log",
                newName: "IX_Audit_Log_ExecutionTime");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_CorrelationId",
                table: "Audit_Log",
                newName: "IX_Audit_Log_CorrelationId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogActions_TenantId",
                table: "Audit_Action",
                newName: "IX_Audit_Action_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogActions_AuditLogId",
                table: "Audit_Action",
                newName: "IX_Audit_Action_AuditLogId");

            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                table: "Audit_Log",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "RequestType",
                table: "Audit_Log",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audit_PropertyChange",
                table: "Audit_PropertyChange",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audit_EntityChange",
                table: "Audit_EntityChange",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audit_Log",
                table: "Audit_Log",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Audit_Action",
                table: "Audit_Action",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_RequestType",
                table: "Audit_Log",
                column: "RequestType");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_Action_Audit_Log_AuditLogId",
                table: "Audit_Action",
                column: "AuditLogId",
                principalTable: "Audit_Log",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_EntityChange_Audit_Log_AuditLogId",
                table: "Audit_EntityChange",
                column: "AuditLogId",
                principalTable: "Audit_Log",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Audit_PropertyChange_Audit_EntityChange_EntityChangeId",
                table: "Audit_PropertyChange",
                column: "EntityChangeId",
                principalTable: "Audit_EntityChange",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Audit_Action_Audit_Log_AuditLogId",
                table: "Audit_Action");

            migrationBuilder.DropForeignKey(
                name: "FK_Audit_EntityChange_Audit_Log_AuditLogId",
                table: "Audit_EntityChange");

            migrationBuilder.DropForeignKey(
                name: "FK_Audit_PropertyChange_Audit_EntityChange_EntityChangeId",
                table: "Audit_PropertyChange");

            migrationBuilder.DropTable(
                name: "Audit_LogPipeline");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audit_PropertyChange",
                table: "Audit_PropertyChange");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audit_Log",
                table: "Audit_Log");

            migrationBuilder.DropIndex(
                name: "IX_Audit_Log_RequestType",
                table: "Audit_Log");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audit_EntityChange",
                table: "Audit_EntityChange");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Audit_Action",
                table: "Audit_Action");

            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "Audit_Log");

            migrationBuilder.DropColumn(
                name: "RequestType",
                table: "Audit_Log");

            migrationBuilder.RenameTable(
                name: "Audit_PropertyChange",
                newName: "EntityPropertyChanges");

            migrationBuilder.RenameTable(
                name: "Audit_Log",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "Audit_EntityChange",
                newName: "EntityChanges");

            migrationBuilder.RenameTable(
                name: "Audit_Action",
                newName: "AuditLogActions");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_PropertyChange_EntityChangeId",
                table: "EntityPropertyChanges",
                newName: "IX_EntityPropertyChanges_EntityChangeId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Log_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Log_TenantId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Log_HttpStatusCode",
                table: "AuditLogs",
                newName: "IX_AuditLogs_HttpStatusCode");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Log_ExecutionTime",
                table: "AuditLogs",
                newName: "IX_AuditLogs_ExecutionTime");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Log_CorrelationId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_CorrelationId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_EntityChange_TenantId",
                table: "EntityChanges",
                newName: "IX_EntityChanges_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_EntityChange_EntityTypeFullName_EntityId",
                table: "EntityChanges",
                newName: "IX_EntityChanges_EntityTypeFullName_EntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_EntityChange_AuditLogId",
                table: "EntityChanges",
                newName: "IX_EntityChanges_AuditLogId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Action_TenantId",
                table: "AuditLogActions",
                newName: "IX_AuditLogActions_TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Audit_Action_AuditLogId",
                table: "AuditLogActions",
                newName: "IX_AuditLogActions_AuditLogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityPropertyChanges",
                table: "EntityPropertyChanges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityChanges",
                table: "EntityChanges",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogActions",
                table: "AuditLogActions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogActions_AuditLogs_AuditLogId",
                table: "AuditLogActions",
                column: "AuditLogId",
                principalTable: "AuditLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityChanges_AuditLogs_AuditLogId",
                table: "EntityChanges",
                column: "AuditLogId",
                principalTable: "AuditLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityPropertyChanges_EntityChanges_EntityChangeId",
                table: "EntityPropertyChanges",
                column: "EntityChangeId",
                principalTable: "EntityChanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

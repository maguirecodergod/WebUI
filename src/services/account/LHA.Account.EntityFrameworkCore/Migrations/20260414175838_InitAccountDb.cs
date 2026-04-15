using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LHA.Account.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitAccountDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Audit_Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    ActionName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ImpersonatorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ImpersonatorTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExecutionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    HttpMethod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    HttpStatusCode = table.Column<int>(type: "integer", nullable: true),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Exceptions = table.Column<string>(type: "text", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_Log", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Auth_ClaimType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    Regex = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    RegexDescription = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_ClaimType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_PermissionGrant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IsGranted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_PermissionGrant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatic = table.Column<bool>(type: "boolean", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_SecurityLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationName = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Identity = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: true),
                    Action = table.Column<string>(type: "character varying(96)", maxLength: 96, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CorrelationId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    ClientIpAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    ExtraProperties = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_SecurityLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SecurityStamp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event_Inbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ConsumerGroup = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ReceivedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event_Inbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Event_Outbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    EventVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    PartitionKey = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event_Outbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission_Definition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    GroupName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    MultiTenancySide = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_Definition", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission_Grant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_Grant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission_Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission_Template",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_Template", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant_Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    DatabaseStyle = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    EntityVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audit_Action",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServiceName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    MethodName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Parameters = table.Column<string>(type: "text", nullable: true),
                    ExecutionTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ExecutionDuration = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_Action_Audit_Log_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "Audit_Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audit_EntityChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuditLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ChangeTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ChangeType = table.Column<byte>(type: "smallint", nullable: false),
                    EntityTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    EntityId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    EntityTypeFullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_EntityChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_EntityChange_Audit_Log_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "Audit_Log",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_RoleClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_RoleClaim_Auth_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Auth_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_UserClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ClaimValue = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_UserClaim_Auth_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Auth_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_UserLogin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ProviderKey = table.Column<string>(type: "character varying(196)", maxLength: 196, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_UserLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_UserLogin_Auth_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Auth_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_UserRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_UserRole_Auth_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Auth_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_UserToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_UserToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_UserToken_Auth_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Auth_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permission_GroupItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionDefinitionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_GroupItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_GroupItem_Permission_Group_PermissionGroupId",
                        column: x => x.PermissionGroupId,
                        principalTable: "Permission_Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permission_TemplateItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionGroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission_TemplateItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permission_TemplateItem_Permission_Template_PermissionTempl~",
                        column: x => x.PermissionTemplateId,
                        principalTable: "Permission_Template",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tenant_ConnectionString",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant_ConnectionString", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenant_ConnectionString_Tenant_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant_Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Audit_PropertyChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityChangeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    PropertyName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PropertyTypeFullName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OriginalValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    NewValue = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_PropertyChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Audit_PropertyChange_Audit_EntityChange_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "Audit_EntityChange",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Action_AuditLogId",
                table: "Audit_Action",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Action_TenantId",
                table: "Audit_Action",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_EntityChange_AuditLogId",
                table: "Audit_EntityChange",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_EntityChange_EntityTypeFullName_EntityId",
                table: "Audit_EntityChange",
                columns: new[] { "EntityTypeFullName", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Audit_EntityChange_TenantId",
                table: "Audit_EntityChange",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_CorrelationId",
                table: "Audit_Log",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_ExecutionTime",
                table: "Audit_Log",
                column: "ExecutionTime");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_HttpStatusCode",
                table: "Audit_Log",
                column: "HttpStatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_TenantId",
                table: "Audit_Log",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_Log_UserId",
                table: "Audit_Log",
                column: "UserId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Audit_PropertyChange_EntityChangeId",
                table: "Audit_PropertyChange",
                column: "EntityChangeId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_ClaimType_Name",
                table: "Auth_ClaimType",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auth_PermissionGrant_TenantId_Name_ProviderName_ProviderKey",
                table: "Auth_PermissionGrant",
                columns: new[] { "TenantId", "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Role_NormalizedName",
                table: "Auth_Role",
                column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_RoleClaim_RoleId",
                table: "Auth_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_SecurityLog_CreationTime",
                table: "Auth_SecurityLog",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_SecurityLog_TenantId_Action",
                table: "Auth_SecurityLog",
                columns: new[] { "TenantId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_Auth_SecurityLog_UserId",
                table: "Auth_SecurityLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_User_NormalizedEmail",
                table: "Auth_User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_User_NormalizedUserName",
                table: "Auth_User",
                column: "NormalizedUserName");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_UserClaim_UserId",
                table: "Auth_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_UserLogin_LoginProvider_ProviderKey",
                table: "Auth_UserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Auth_UserLogin_UserId",
                table: "Auth_UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_UserRole_UserId_RoleId",
                table: "Auth_UserRole",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auth_UserToken_UserId_LoginProvider_Name",
                table: "Auth_UserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboxMessages_EventId_ConsumerGroup",
                table: "Event_Inbox",
                columns: new[] { "EventId", "ConsumerGroup" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Pending",
                table: "Event_Outbox",
                column: "CreatedAtUtc",
                filter: "\"ProcessedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Definition_Name",
                table: "Permission_Definition",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Definition_ServiceName",
                table: "Permission_Definition",
                column: "ServiceName");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Grant_Name_ProviderName_ProviderKey",
                table: "Permission_Grant",
                columns: new[] { "Name", "ProviderName", "ProviderKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Grant_ProviderName_ProviderKey",
                table: "Permission_Grant",
                columns: new[] { "ProviderName", "ProviderKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Grant_TenantId",
                table: "Permission_Grant",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Group_Name",
                table: "Permission_Group",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_GroupItem_PermissionGroupId_PermissionDefinition~",
                table: "Permission_GroupItem",
                columns: new[] { "PermissionGroupId", "PermissionDefinitionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Template_Name",
                table: "Permission_Template",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permission_TemplateItem_PermissionTemplateId_PermissionGrou~",
                table: "Permission_TemplateItem",
                columns: new[] { "PermissionTemplateId", "PermissionGroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_ConnectionString_TenantId_Name",
                table: "Tenant_ConnectionString",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Tenant_NormalizedName",
                table: "Tenant_Tenant",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit_Action");

            migrationBuilder.DropTable(
                name: "Audit_LogPipeline");

            migrationBuilder.DropTable(
                name: "Audit_PropertyChange");

            migrationBuilder.DropTable(
                name: "Auth_ClaimType");

            migrationBuilder.DropTable(
                name: "Auth_PermissionGrant");

            migrationBuilder.DropTable(
                name: "Auth_RoleClaim");

            migrationBuilder.DropTable(
                name: "Auth_SecurityLog");

            migrationBuilder.DropTable(
                name: "Auth_UserClaim");

            migrationBuilder.DropTable(
                name: "Auth_UserLogin");

            migrationBuilder.DropTable(
                name: "Auth_UserRole");

            migrationBuilder.DropTable(
                name: "Auth_UserToken");

            migrationBuilder.DropTable(
                name: "Event_Inbox");

            migrationBuilder.DropTable(
                name: "Event_Outbox");

            migrationBuilder.DropTable(
                name: "Permission_Definition");

            migrationBuilder.DropTable(
                name: "Permission_Grant");

            migrationBuilder.DropTable(
                name: "Permission_GroupItem");

            migrationBuilder.DropTable(
                name: "Permission_TemplateItem");

            migrationBuilder.DropTable(
                name: "Tenant_ConnectionString");

            migrationBuilder.DropTable(
                name: "Audit_EntityChange");

            migrationBuilder.DropTable(
                name: "Auth_Role");

            migrationBuilder.DropTable(
                name: "Auth_User");

            migrationBuilder.DropTable(
                name: "Permission_Group");

            migrationBuilder.DropTable(
                name: "Permission_Template");

            migrationBuilder.DropTable(
                name: "Tenant_Tenant");

            migrationBuilder.DropTable(
                name: "Audit_Log");
        }
    }
}

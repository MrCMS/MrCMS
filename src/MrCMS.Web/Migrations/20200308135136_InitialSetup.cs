using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MrCMS.Web.Migrations
{
    public partial class InitialSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageTemplateData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    SiteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    BaseUrl = table.Column<string>(nullable: false),
                    StagingUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSetting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SettingType = table.Column<string>(nullable: true),
                    PropertyName = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    FrequencyInSeconds = table.Column<int>(nullable: false),
                    LastStarted = table.Column<DateTime>(nullable: true),
                    LastCompleted = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    CurrentEncryption = table.Column<string>(nullable: true),
                    Source = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    LastLoginDate = table.Column<DateTime>(nullable: true),
                    LoginAttempts = table.Column<int>(nullable: false),
                    ResetPasswordGuid = table.Column<Guid>(nullable: true),
                    ResetPasswordExpiry = table.Column<DateTime>(nullable: true),
                    TwoFactorCode = table.Column<string>(nullable: true),
                    TwoFactorCodeExpiry = table.Column<DateTime>(nullable: true),
                    DisableNotifications = table.Column<bool>(nullable: false),
                    LastNotificationReadDate = table.Column<DateTime>(nullable: true),
                    UICulture = table.Column<string>(nullable: true),
                    AvatarImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Batch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batch_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CropType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CropType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CropType_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Form",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    FormSubmittedMessage = table.Column<string>(maxLength: 500, nullable: true),
                    FormRedirectUrl = table.Column<string>(nullable: true),
                    FormEmailTitle = table.Column<string>(maxLength: 250, nullable: true),
                    SendFormTo = table.Column<string>(maxLength: 500, nullable: true),
                    FormMessage = table.Column<string>(nullable: true),
                    SubmitButtonCssClass = table.Column<string>(maxLength: 100, nullable: true),
                    SubmitButtonText = table.Column<string>(maxLength: 100, nullable: true),
                    FormDesign = table.Column<string>(nullable: true),
                    DeleteEntriesAfter = table.Column<int>(nullable: true),
                    SendByEmailOnly = table.Column<bool>(nullable: false),
                    ShowGDPRConsentBox = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Form", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Form_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ExceptionData = table.Column<string>(nullable: true),
                    RequestData = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Detail = table.Column<string>(nullable: true),
                    LogLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LuceneFieldBoost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Definition = table.Column<string>(nullable: true),
                    Boost = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LuceneFieldBoost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LuceneFieldBoost_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    MessageTemplateType = table.Column<string>(nullable: true),
                    FromAddress = table.Column<string>(nullable: false),
                    FromName = table.Column<string>(nullable: false),
                    ToAddress = table.Column<string>(nullable: false),
                    ToName = table.Column<string>(nullable: true),
                    Cc = table.Column<string>(nullable: true),
                    Bcc = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    IsHtml = table.Column<bool>(nullable: false),
                    Imported = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageTemplate_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QueuedMessage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    FromAddress = table.Column<string>(nullable: true),
                    FromName = table.Column<string>(nullable: true),
                    ToAddress = table.Column<string>(nullable: true),
                    ToName = table.Column<string>(nullable: true),
                    Cc = table.Column<string>(nullable: true),
                    Bcc = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Body = table.Column<string>(nullable: true),
                    SentOn = table.Column<DateTime>(nullable: true),
                    Tries = table.Column<int>(nullable: false),
                    IsHtml = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueuedMessage_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QueuedTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Tries = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    QueuedAt = table.Column<DateTime>(nullable: true),
                    StartedAt = table.Column<DateTime>(nullable: true),
                    CompletedAt = table.Column<DateTime>(nullable: true),
                    FailedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueuedTask_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RedirectedDomain",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedirectedDomain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedirectedDomain_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SettingType = table.Column<string>(nullable: true),
                    PropertyName = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Setting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Setting_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StringResource",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    SiteId = table.Column<int>(nullable: true),
                    UICulture = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StringResource_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    NotificationType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Issuer = table.Column<string>(nullable: true),
                    Claim = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: true),
                    ProviderKey = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserProfileData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    ProfileInfoType = table.Column<string>(nullable: false),
                    Bio = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfileData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfileData_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ACLRole",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    UserRoleId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ACLRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ACLRole_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ACLRole_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersToRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    UserRoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersToRoles", x => new { x.UserRoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UsersToRoles_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersToRoles_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchJob",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    Tries = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    discriminator = table.Column<string>(nullable: false),
                    IndexName = table.Column<string>(nullable: true),
                    UrlSegment = table.Column<string>(nullable: true),
                    WebpageId = table.Column<int>(nullable: true),
                    MergedIntoId = table.Column<int>(nullable: true),
                    NewParentId = table.Column<int>(nullable: true),
                    NewUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchJob_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchJob_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchRun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    BatchId = table.Column<int>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchRun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchRun_Batch_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchRun_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormPosting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    FormId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormPosting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormPosting_Form_FormId",
                        column: x => x.FormId,
                        principalTable: "Form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormPosting_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormProperty",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    LabelText = table.Column<string>(nullable: true),
                    Required = table.Column<bool>(nullable: false),
                    CssClass = table.Column<string>(nullable: true),
                    HtmlId = table.Column<string>(nullable: true),
                    FormId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    PropertyType = table.Column<string>(nullable: false),
                    PlaceHolder = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormProperty_Form_FormId",
                        column: x => x.FormId,
                        principalTable: "Form",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormProperty_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QueuedMessageAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    QueuedMessageId = table.Column<int>(nullable: false),
                    Data = table.Column<byte[]>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    FileSize = table.Column<long>(nullable: false),
                    FileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedMessageAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueuedMessageAttachment_QueuedMessage_QueuedMessageId",
                        column: x => x.QueuedMessageId,
                        principalTable: "QueuedMessage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QueuedMessageAttachment_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BatchRunResult",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    BatchRunId = table.Column<int>(nullable: false),
                    BatchJobId = table.Column<int>(nullable: false),
                    ExecutionOrder = table.Column<int>(nullable: false),
                    MillisecondsTaken = table.Column<decimal>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchRunResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BatchRunResult_BatchJob_BatchJobId",
                        column: x => x.BatchJobId,
                        principalTable: "BatchJob",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchRunResult_BatchRun_BatchRunId",
                        column: x => x.BatchRunId,
                        principalTable: "BatchRun",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BatchRunResult_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormValue",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    FormPostingId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    IsFile = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormValue_FormPosting_FormPostingId",
                        column: x => x.FormPostingId,
                        principalTable: "FormPosting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormValue_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormListOption",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    FormPropertyId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false),
                    Selected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormListOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormListOption_FormProperty_FormPropertyId",
                        column: x => x.FormPropertyId,
                        principalTable: "FormProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormListOption_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResizedImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    MediaFileId = table.Column<int>(nullable: true),
                    CropId = table.Column<int>(nullable: true),
                    Url = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResizedImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResizedImage_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Crop",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    MediaFileId = table.Column<int>(nullable: false),
                    CropTypeId = table.Column<int>(nullable: false),
                    Left = table.Column<int>(nullable: false),
                    Top = table.Column<int>(nullable: false),
                    Right = table.Column<int>(nullable: false),
                    Bottom = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crop", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crop_CropType_CropTypeId",
                        column: x => x.CropTypeId,
                        principalTable: "CropType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Crop_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentBlock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    WebpageId = table.Column<int>(nullable: false),
                    DisplayOrder = table.Column<int>(nullable: false),
                    discriminator = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentBlock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentBlock_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTags",
                columns: table => new
                {
                    DocumentId = table.Column<int>(nullable: false),
                    TagId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTags", x => new { x.DocumentId, x.TagId });
                    table.ForeignKey(
                        name: "FK_DocumentTags_Tag_TagId",
                        column: x => x.TagId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentVersion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    DocumentId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentVersion_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentVersion_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FrontEndAllowedRole",
                columns: table => new
                {
                    WebpageId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontEndAllowedRole", x => new { x.RoleId, x.WebpageId });
                    table.ForeignKey(
                        name: "FK_FrontEndAllowedRole_UserRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HiddenWidgets",
                columns: table => new
                {
                    WebpageId = table.Column<int>(nullable: false),
                    WidgetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiddenWidgets", x => new { x.WidgetId, x.WebpageId });
                });

            migrationBuilder.CreateTable(
                name: "LayoutArea",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    LayoutId = table.Column<int>(nullable: false),
                    AreaName = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LayoutArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LayoutArea_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MediaFile",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    FileExtension = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    MediaCategoryId = table.Column<int>(nullable: true),
                    FileUrl = table.Column<string>(maxLength: 450, nullable: true),
                    ContentLength = table.Column<long>(nullable: false),
                    FileName = table.Column<string>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Width = table.Column<int>(nullable: false),
                    Height = table.Column<int>(nullable: false),
                    MediaFileType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MediaFile_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PageTemplateName = table.Column<string>(nullable: false),
                    PageType = table.Column<string>(nullable: false),
                    LayoutId = table.Column<int>(nullable: true),
                    UrlGeneratorType = table.Column<string>(nullable: false),
                    SingleUse = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageTemplate_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    UrlSegment = table.Column<string>(nullable: true),
                    HideInAdminNav = table.Column<bool>(nullable: false),
                    DocumentType = table.Column<string>(nullable: false),
                    Hidden = table.Column<bool>(nullable: true),
                    MetaTitle = table.Column<string>(maxLength: 250, nullable: true),
                    MetaDescription = table.Column<string>(maxLength: 250, nullable: true),
                    IsGallery = table.Column<bool>(nullable: true),
                    SEOTargetPhrase = table.Column<string>(maxLength: 250, nullable: true),
                    MetaKeywords = table.Column<string>(maxLength: 250, nullable: true),
                    ExplicitCanonicalLink = table.Column<string>(nullable: true),
                    RevealInNavigation = table.Column<bool>(nullable: true),
                    IncludeInSitemap = table.Column<bool>(nullable: true),
                    CustomHeaderScripts = table.Column<string>(maxLength: 8000, nullable: true),
                    CustomFooterScripts = table.Column<string>(maxLength: 8000, nullable: true),
                    RequiresSSL = table.Column<bool>(nullable: true),
                    Published = table.Column<bool>(nullable: true),
                    PublishOn = table.Column<DateTime>(nullable: true),
                    BodyContent = table.Column<string>(nullable: true),
                    BlockAnonymousAccess = table.Column<bool>(nullable: true),
                    HasCustomPermissions = table.Column<bool>(nullable: true),
                    PermissionType = table.Column<int>(nullable: true),
                    InheritFrontEndRolesFromParent = table.Column<bool>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PasswordAccessToken = table.Column<Guid>(nullable: true),
                    PageTemplateId = table.Column<int>(nullable: true),
                    DoNotCache = table.Column<bool>(nullable: true),
                    RedirectUrl = table.Column<string>(nullable: true),
                    Permanent = table.Column<bool>(nullable: true),
                    FeatureImage = table.Column<string>(nullable: true),
                    Abstract = table.Column<string>(maxLength: 500, nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    PageSize = table.Column<int>(nullable: true),
                    AllowPaging = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Document_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_PageTemplate_PageTemplateId",
                        column: x => x.PageTemplateId,
                        principalTable: "PageTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Document_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UrlHistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    UrlSegment = table.Column<string>(nullable: false),
                    WebpageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrlHistory_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UrlHistory_Document_WebpageId",
                        column: x => x.WebpageId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Widget",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    LayoutAreaId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CustomLayout = table.Column<string>(nullable: true),
                    WebpageId = table.Column<int>(nullable: true),
                    DisplayOrder = table.Column<int>(nullable: false),
                    Cache = table.Column<bool>(nullable: false),
                    CacheLength = table.Column<int>(nullable: false),
                    CacheExpiryType = table.Column<int>(nullable: false),
                    IsRecursive = table.Column<bool>(nullable: false),
                    WidgetType = table.Column<string>(nullable: false),
                    ArticleListId = table.Column<int>(nullable: true),
                    ShowNameAsTitle = table.Column<bool>(nullable: true),
                    NumberOfArticles = table.Column<int>(nullable: true),
                    RelatedNewsListId = table.Column<int>(nullable: true),
                    Image = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    IncludeChildren = table.Column<bool>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Image1 = table.Column<string>(nullable: true),
                    Link1 = table.Column<string>(nullable: true),
                    Description1 = table.Column<string>(nullable: true),
                    Image2 = table.Column<string>(nullable: true),
                    Link2 = table.Column<string>(nullable: true),
                    Description2 = table.Column<string>(nullable: true),
                    Image3 = table.Column<string>(nullable: true),
                    Link3 = table.Column<string>(nullable: true),
                    Description3 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widget_LayoutArea_LayoutAreaId",
                        column: x => x.LayoutAreaId,
                        principalTable: "LayoutArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Widget_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Widget_Document_WebpageId",
                        column: x => x.WebpageId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Widget_Document_ArticleListId",
                        column: x => x.ArticleListId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Widget_Document_ArticleListId1",
                        column: x => x.ArticleListId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Widget_Document_RelatedNewsListId",
                        column: x => x.RelatedNewsListId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageWidgetSort",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    WebpageId = table.Column<int>(nullable: false),
                    LayoutAreaId = table.Column<int>(nullable: false),
                    WidgetId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageWidgetSort", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageWidgetSort_LayoutArea_LayoutAreaId",
                        column: x => x.LayoutAreaId,
                        principalTable: "LayoutArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageWidgetSort_Site_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageWidgetSort_Document_WebpageId",
                        column: x => x.WebpageId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageWidgetSort_Widget_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShownWidgets",
                columns: table => new
                {
                    WebpageId = table.Column<int>(nullable: false),
                    WidgetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShownWidgets", x => new { x.WidgetId, x.WebpageId });
                    table.ForeignKey(
                        name: "FK_ShownWidgets_Document_WebpageId",
                        column: x => x.WebpageId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShownWidgets_Widget_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ACLRole_SiteId",
                table: "ACLRole",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ACLRole_UserRoleId",
                table: "ACLRole",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Batch_SiteId",
                table: "Batch",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchJob_BatchId",
                table: "BatchJob",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchJob_SiteId",
                table: "BatchJob",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchRun_BatchId",
                table: "BatchRun",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchRun_SiteId",
                table: "BatchRun",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchRunResult_BatchJobId",
                table: "BatchRunResult",
                column: "BatchJobId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchRunResult_BatchRunId",
                table: "BatchRunResult",
                column: "BatchRunId");

            migrationBuilder.CreateIndex(
                name: "IX_BatchRunResult_SiteId",
                table: "BatchRunResult",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlock_SiteId",
                table: "ContentBlock",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentBlock_WebpageId",
                table: "ContentBlock",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_Crop_CropTypeId",
                table: "Crop",
                column: "CropTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Crop_MediaFileId",
                table: "Crop",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Crop_SiteId",
                table: "Crop",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_CropType_SiteId",
                table: "CropType",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ParentId",
                table: "Document",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_SiteId",
                table: "Document",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_PageTemplateId",
                table: "Document",
                column: "PageTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_UserId",
                table: "Document",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTags_TagId",
                table: "DocumentTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersion_DocumentId",
                table: "DocumentVersion",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersion_SiteId",
                table: "DocumentVersion",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersion_UserId",
                table: "DocumentVersion",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Form_SiteId",
                table: "Form",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FormListOption_FormPropertyId",
                table: "FormListOption",
                column: "FormPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_FormListOption_SiteId",
                table: "FormListOption",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FormPosting_FormId",
                table: "FormPosting",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormPosting_SiteId",
                table: "FormPosting",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FormProperty_FormId",
                table: "FormProperty",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_FormProperty_SiteId",
                table: "FormProperty",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FormValue_FormPostingId",
                table: "FormValue",
                column: "FormPostingId");

            migrationBuilder.CreateIndex(
                name: "IX_FormValue_SiteId",
                table: "FormValue",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_FrontEndAllowedRole_WebpageId",
                table: "FrontEndAllowedRole",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_HiddenWidgets_WebpageId",
                table: "HiddenWidgets",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_LayoutArea_LayoutId",
                table: "LayoutArea",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_LayoutArea_SiteId",
                table: "LayoutArea",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_SiteId",
                table: "Log",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_LuceneFieldBoost_SiteId",
                table: "LuceneFieldBoost",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_MediaCategoryId",
                table: "MediaFile",
                column: "MediaCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFile_SiteId",
                table: "MediaFile",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageTemplate_SiteId",
                table: "MessageTemplate",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_SiteId",
                table: "Notification",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTemplate_LayoutId",
                table: "PageTemplate",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_PageTemplate_SiteId",
                table: "PageTemplate",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PageWidgetSort_LayoutAreaId",
                table: "PageWidgetSort",
                column: "LayoutAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PageWidgetSort_SiteId",
                table: "PageWidgetSort",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PageWidgetSort_WebpageId",
                table: "PageWidgetSort",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageWidgetSort_WidgetId",
                table: "PageWidgetSort",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuedMessage_SiteId",
                table: "QueuedMessage",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuedMessageAttachment_QueuedMessageId",
                table: "QueuedMessageAttachment",
                column: "QueuedMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuedMessageAttachment_SiteId",
                table: "QueuedMessageAttachment",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_QueuedTask_SiteId",
                table: "QueuedTask",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_RedirectedDomain_SiteId",
                table: "RedirectedDomain",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ResizedImage_CropId",
                table: "ResizedImage",
                column: "CropId");

            migrationBuilder.CreateIndex(
                name: "IX_ResizedImage_MediaFileId",
                table: "ResizedImage",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_ResizedImage_SiteId",
                table: "ResizedImage",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_SiteId",
                table: "Setting",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ShownWidgets_WebpageId",
                table: "ShownWidgets",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_StringResource_SiteId",
                table: "StringResource",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_SiteId",
                table: "Tag",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_SiteId",
                table: "UrlHistory",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_UrlHistory_WebpageId",
                table: "UrlHistory",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfileData_UserId",
                table: "UserProfileData",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersToRoles_UserId",
                table: "UsersToRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_LayoutAreaId",
                table: "Widget",
                column: "LayoutAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_SiteId",
                table: "Widget",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_WebpageId",
                table: "Widget",
                column: "WebpageId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_ArticleListId",
                table: "Widget",
                column: "ArticleListId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_ArticleListId1",
                table: "Widget",
                column: "ArticleListId");

            migrationBuilder.CreateIndex(
                name: "IX_Widget_RelatedNewsListId",
                table: "Widget",
                column: "RelatedNewsListId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResizedImage_MediaFile_MediaFileId",
                table: "ResizedImage",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResizedImage_Crop_CropId",
                table: "ResizedImage",
                column: "CropId",
                principalTable: "Crop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Crop_MediaFile_MediaFileId",
                table: "Crop",
                column: "MediaFileId",
                principalTable: "MediaFile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlock_Document_WebpageId",
                table: "ContentBlock",
                column: "WebpageId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTags_Document_DocumentId",
                table: "DocumentTags",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentVersion_Document_DocumentId",
                table: "DocumentVersion",
                column: "DocumentId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FrontEndAllowedRole_Document_WebpageId",
                table: "FrontEndAllowedRole",
                column: "WebpageId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HiddenWidgets_Document_WebpageId",
                table: "HiddenWidgets",
                column: "WebpageId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HiddenWidgets_Widget_WidgetId",
                table: "HiddenWidgets",
                column: "WidgetId",
                principalTable: "Widget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LayoutArea_Document_LayoutId",
                table: "LayoutArea",
                column: "LayoutId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MediaFile_Document_MediaCategoryId",
                table: "MediaFile",
                column: "MediaCategoryId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageTemplate_Document_LayoutId",
                table: "PageTemplate",
                column: "LayoutId",
                principalTable: "Document",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Document_Site_SiteId",
                table: "Document");

            migrationBuilder.DropForeignKey(
                name: "FK_PageTemplate_Site_SiteId",
                table: "PageTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_PageTemplate_Document_LayoutId",
                table: "PageTemplate");

            migrationBuilder.DropTable(
                name: "ACLRole");

            migrationBuilder.DropTable(
                name: "BatchRunResult");

            migrationBuilder.DropTable(
                name: "ContentBlock");

            migrationBuilder.DropTable(
                name: "DocumentTags");

            migrationBuilder.DropTable(
                name: "DocumentVersion");

            migrationBuilder.DropTable(
                name: "FormListOption");

            migrationBuilder.DropTable(
                name: "FormValue");

            migrationBuilder.DropTable(
                name: "FrontEndAllowedRole");

            migrationBuilder.DropTable(
                name: "HiddenWidgets");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "LuceneFieldBoost");

            migrationBuilder.DropTable(
                name: "MessageTemplate");

            migrationBuilder.DropTable(
                name: "MessageTemplateData");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PageWidgetSort");

            migrationBuilder.DropTable(
                name: "QueuedMessageAttachment");

            migrationBuilder.DropTable(
                name: "QueuedTask");

            migrationBuilder.DropTable(
                name: "RedirectedDomain");

            migrationBuilder.DropTable(
                name: "ResizedImage");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "ShownWidgets");

            migrationBuilder.DropTable(
                name: "StringResource");

            migrationBuilder.DropTable(
                name: "SystemSetting");

            migrationBuilder.DropTable(
                name: "TaskSettings");

            migrationBuilder.DropTable(
                name: "UrlHistory");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserProfileData");

            migrationBuilder.DropTable(
                name: "UsersToRoles");

            migrationBuilder.DropTable(
                name: "BatchJob");

            migrationBuilder.DropTable(
                name: "BatchRun");

            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "FormProperty");

            migrationBuilder.DropTable(
                name: "FormPosting");

            migrationBuilder.DropTable(
                name: "QueuedMessage");

            migrationBuilder.DropTable(
                name: "Crop");

            migrationBuilder.DropTable(
                name: "Widget");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Batch");

            migrationBuilder.DropTable(
                name: "Form");

            migrationBuilder.DropTable(
                name: "CropType");

            migrationBuilder.DropTable(
                name: "MediaFile");

            migrationBuilder.DropTable(
                name: "LayoutArea");

            migrationBuilder.DropTable(
                name: "Site");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "PageTemplate");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

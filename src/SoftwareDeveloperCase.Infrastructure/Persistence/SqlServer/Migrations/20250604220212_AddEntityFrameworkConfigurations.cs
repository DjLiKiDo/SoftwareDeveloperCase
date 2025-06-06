using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityFrameworkConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Team_TeamId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_Role_ParentRoleId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_User_AssignedToId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComment_User_AuthorId",
                table: "TaskComment");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "User",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Team",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Team",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "TaskComment",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Task",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Task",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Role",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Project",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permission",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permission",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JwtId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3122));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3120));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3111));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3117));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3257));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3264));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3301));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3309));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3304));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3306));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3298));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3441));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3426));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3434));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3445));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3494));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3481));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3492));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3489));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3487));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 4, 22, 2, 11, 742, DateTimeKind.Utc).AddTicks(3484));

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId_RoleId",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_IsActive",
                table: "User",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                table: "User",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_Status",
                table: "TeamMember",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId_Status",
                table: "TeamMember",
                columns: new[] { "TeamId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId_UserId",
                table: "TeamMember",
                columns: new[] { "TeamId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamRole",
                table: "TeamMember",
                column: "TeamRole");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_UserId_Status",
                table: "TeamMember",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Team_Name",
                table: "Team",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_AuthorId_CreatedAt",
                table: "TaskComment",
                columns: new[] { "AuthorId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_CreatedAt",
                table: "TaskComment",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_TaskId_CreatedAt",
                table: "TaskComment",
                columns: new[] { "TaskId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Task_AssignedToId_Status",
                table: "Task",
                columns: new[] { "AssignedToId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Task_DueDate",
                table: "Task",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Priority",
                table: "Task",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ProjectId_AssignedToId",
                table: "Task",
                columns: new[] { "ProjectId", "AssignedToId" });

            migrationBuilder.CreateIndex(
                name: "IX_Task_ProjectId_Status",
                table: "Task",
                columns: new[] { "ProjectId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Task_Status",
                table: "Task",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Task_Status_Priority",
                table: "Task",
                columns: new[] { "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Task_Title",
                table: "Task",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId_PermissionId",
                table: "RolePermission",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_Name",
                table: "Project",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Priority",
                table: "Project",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Status",
                table: "Project",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Status_Priority",
                table: "Project",
                columns: new[] { "Status", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_TeamId_Status",
                table: "Project",
                columns: new[] { "TeamId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_Name",
                table: "Permission",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_ExpiresAt",
                table: "RefreshToken",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IsRevoked",
                table: "RefreshToken",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_JwtId",
                table: "RefreshToken",
                column: "JwtId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Team_TeamId",
                table: "Project",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Role_ParentRoleId",
                table: "Role",
                column: "ParentRoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task",
                column: "ParentTaskId",
                principalTable: "Task",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Task_User_AssignedToId",
                table: "Task",
                column: "AssignedToId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComment_User_AuthorId",
                table: "TaskComment",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Team_TeamId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_Role_ParentRoleId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_User_AssignedToId",
                table: "Task");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskComment_User_AuthorId",
                table: "TaskComment");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId_RoleId",
                table: "UserRole");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_IsActive",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Name",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_Status",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_TeamId_Status",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_TeamId_UserId",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_TeamRole",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_TeamMember_UserId_Status",
                table: "TeamMember");

            migrationBuilder.DropIndex(
                name: "IX_Team_Name",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_TaskComment_AuthorId_CreatedAt",
                table: "TaskComment");

            migrationBuilder.DropIndex(
                name: "IX_TaskComment_CreatedAt",
                table: "TaskComment");

            migrationBuilder.DropIndex(
                name: "IX_TaskComment_TaskId_CreatedAt",
                table: "TaskComment");

            migrationBuilder.DropIndex(
                name: "IX_Task_AssignedToId_Status",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_DueDate",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_Priority",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_ProjectId_AssignedToId",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_ProjectId_Status",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_Status",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_Status_Priority",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_Task_Title",
                table: "Task");

            migrationBuilder.DropIndex(
                name: "IX_RolePermission_RoleId_PermissionId",
                table: "RolePermission");

            migrationBuilder.DropIndex(
                name: "IX_Role_Name",
                table: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Project_Name",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_Priority",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_Status",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_Status_Priority",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_TeamId_Status",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Permission_Name",
                table: "Permission");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Team",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Team",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "TaskComment",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Task",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Task",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Role",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Project",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Permission",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5566));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5564));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5557));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5560));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5722));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5728));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5794));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5802));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5797));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5800));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5791));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5970));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5955));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5963));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5976));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(6078));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(6018));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(6076));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(6073));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(6070));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(6067));

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Team_TeamId",
                table: "Project",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Role_ParentRoleId",
                table: "Role",
                column: "ParentRoleId",
                principalTable: "Role",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_Task_ParentTaskId",
                table: "Task",
                column: "ParentTaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_User_AssignedToId",
                table: "Task",
                column: "AssignedToId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskComment_User_AuthorId",
                table: "TaskComment",
                column: "AuthorId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

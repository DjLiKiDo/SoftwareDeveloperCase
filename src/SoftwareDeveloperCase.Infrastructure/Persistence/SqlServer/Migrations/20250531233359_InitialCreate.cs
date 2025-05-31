using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Role_ParentRoleId",
                        column: x => x.ParentRoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Project_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMember",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamRole = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeftDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMember", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMember_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamMember_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    EstimatedHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    ActualHours = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedToId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Task_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_Task_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "Task",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Task_User_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskComment_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskComment_User_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskHierarchy",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HierarchyLevel = table.Column<int>(type: "int", nullable: false),
                    HierarchyPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HierarchyOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHierarchy", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskHierarchy_Task_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Task",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Description", "LastModifiedBy", "LastModifiedOn", "Name" },
                values: new object[,]
                {
                    { new Guid("4a03f568-69e4-4548-85b6-8100cad15631"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1980), null, null, null, "Delete" },
                    { new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1970), null, null, null, "Update" },
                    { new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1970), null, null, null, "Read" },
                    { new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1970), null, null, null, "Add" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn", "Name", "ParentRoleId" },
                values: new object[] { new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2180), null, null, "Employee", null });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Email", "IsActive", "LastModifiedBy", "LastModifiedOn", "Name", "Password" },
                values: new object[,]
                {
                    { new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2370), "itemployee@sdc.com", true, null, null, "IT Employee", "sdc" },
                    { new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2360), "hremployee@sdc.com", true, null, null, "HR Employee", "sdc" },
                    { new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2360), "hrmanager@sdc.com", true, null, null, "HR Manager", "sdc" },
                    { new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2370), "itmanager@sdc.com", true, null, null, "IT Manager", "sdc" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn", "Name", "ParentRoleId" },
                values: new object[] { new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2190), null, null, "Manager", new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a") });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId" },
                values: new object[] { new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2210), null, null, new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"), new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a") });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn", "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2410), null, null, new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"), new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a") },
                    { new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420), null, null, new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"), new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43") },
                    { new Guid("5562714c-a186-439f-a0df-25321703cf7e"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420), null, null, new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"), new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c") },
                    { new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2410), null, null, new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"), new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4") }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2220), null, null, new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"), new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4") },
                    { new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2230), null, null, new Guid("4a03f568-69e4-4548-85b6-8100cad15631"), new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4") },
                    { new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2220), null, null, new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"), new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4") },
                    { new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2220), null, null, new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"), new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4") }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "LastModifiedBy", "LastModifiedOn", "RoleId", "UserId" },
                values: new object[,]
                {
                    { new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420), null, null, new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"), new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43") },
                    { new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"), "InitialSeed", new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420), null, null, new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"), new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Project_TeamId",
                table: "Project",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ParentRoleId",
                table: "Role",
                column: "ParentRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_AssignedToId",
                table: "Task",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ParentTaskId",
                table: "Task",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Task_ProjectId",
                table: "Task",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_AuthorId",
                table: "TaskComment",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskComment_TaskId",
                table: "TaskComment",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_TeamId",
                table: "TeamMember",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMember_UserId",
                table: "TeamMember",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "TaskComment");

            migrationBuilder.DropTable(
                name: "TaskHierarchy");

            migrationBuilder.DropTable(
                name: "TeamMember");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Team");
        }
    }
}

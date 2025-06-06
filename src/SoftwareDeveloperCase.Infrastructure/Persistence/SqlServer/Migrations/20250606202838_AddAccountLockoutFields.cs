using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountLockoutFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedOutAt",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutExpiresAt",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5005));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5001));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(4990));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(4997));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5223));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5231));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5286));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5299));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5289));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5295));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5282));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                columns: new[] { "CreatedOn", "FailedLoginAttempts", "LockedOutAt", "LockoutExpiresAt" },
                values: new object[] { new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5486), 0, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                columns: new[] { "CreatedOn", "FailedLoginAttempts", "LockedOutAt", "LockoutExpiresAt" },
                values: new object[] { new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5463), 0, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                columns: new[] { "CreatedOn", "FailedLoginAttempts", "LockedOutAt", "LockoutExpiresAt" },
                values: new object[] { new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5475), 0, null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                columns: new[] { "CreatedOn", "FailedLoginAttempts", "LockedOutAt", "LockoutExpiresAt" },
                values: new object[] { new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5494), 0, null, null });

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5569));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5549));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5565));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5562));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5558));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                column: "CreatedOn",
                value: new DateTime(2025, 6, 6, 20, 28, 37, 861, DateTimeKind.Utc).AddTicks(5553));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LockedOutAt",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LockoutExpiresAt",
                table: "User");

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
        }
    }
}

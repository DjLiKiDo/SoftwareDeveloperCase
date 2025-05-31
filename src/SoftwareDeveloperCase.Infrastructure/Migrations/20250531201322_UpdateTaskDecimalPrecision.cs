using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareDeveloperCase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 906, DateTimeKind.Utc).AddTicks(9900));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 906, DateTimeKind.Utc).AddTicks(9900));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 906, DateTimeKind.Utc).AddTicks(9890));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 906, DateTimeKind.Utc).AddTicks(9900));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(40));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(40));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(520));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(530));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(520));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(520));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(510));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1090));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1070));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1090));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1100));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1240));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1230));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1240));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1240));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1230));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 13, 21, 907, DateTimeKind.Utc).AddTicks(1230));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(3940));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(3930));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(3930));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(3930));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4090));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4090));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4120));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4280));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4270));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4280));

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4290));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4320));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4330));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 20, 11, 56, 717, DateTimeKind.Utc).AddTicks(4320));
        }
    }
}

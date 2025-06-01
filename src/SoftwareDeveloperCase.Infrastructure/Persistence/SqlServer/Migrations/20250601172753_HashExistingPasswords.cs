using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoftwareDeveloperCase.Infrastructure.Persistence.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class HashExistingPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First update the seed data passwords with hashed versions
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5970), "$2a$12$AS4usWgmy9p3N35.v68YyOoo9A0czxyK2KaJuswyOzk5T430HwS6S" }); // "sdc" hashed

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5955), "$2a$12$AS4usWgmy9p3N35.v68YyOoo9A0czxyK2KaJuswyOzk5T430HwS6S" }); // "sdc" hashed

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5963), "$2a$12$AS4usWgmy9p3N35.v68YyOoo9A0czxyK2KaJuswyOzk5T430HwS6S" }); // "sdc" hashed

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 6, 1, 17, 27, 53, 95, DateTimeKind.Utc).AddTicks(5976), "$2a$12$AS4usWgmy9p3N35.v68YyOoo9A0czxyK2KaJuswyOzk5T430HwS6S" }); // "sdc" hashed

            // Hash any existing non-seed users' passwords that might be plain text
            // This SQL will only update passwords that don't start with $2a$, $2b$, or $2y$ (i.e., not already hashed)
            migrationBuilder.Sql(@"
                UPDATE [User] 
                SET [Password] = '$2a$12$AS4usWgmy9p3N35.v68YyOoo9A0czxyK2KaJuswyOzk5T430HwS6S'
                WHERE [Password] NOT LIKE '$2a$%' 
                  AND [Password] NOT LIKE '$2b$%' 
                  AND [Password] NOT LIKE '$2y$%'
                  AND [Id] NOT IN (
                      '29d6cf7d-2335-4329-91ad-4a7ec437d73c',
                      '2d7aa3b0-f221-4753-b77f-ff261858a13a', 
                      '64a19c7d-a7a9-4481-a498-7df87f341da4',
                      '94651e82-5fc3-43d0-9c64-5a16ac517d43'
                  )");

            // Update other seed data timestamps
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore original plain text passwords for seed data
            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("29d6cf7d-2335-4329-91ad-4a7ec437d73c"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2370), "sdc" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2360), "sdc" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("64a19c7d-a7a9-4481-a498-7df87f341da4"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2360), "sdc" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("94651e82-5fc3-43d0-9c64-5a16ac517d43"),
                columns: new[] { "CreatedOn", "Password" },
                values: new object[] { new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2370), "sdc" });
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("4a03f568-69e4-4548-85b6-8100cad15631"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1980));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("5162e7da-6b87-424a-a08d-fdd6e3c6b4b2"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1970));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("9a6ae1d8-0688-43d4-b1ce-2a13608fa68c"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1970));

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("d2e69e18-e1a5-48c2-b5b5-eb888c13d46b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(1970));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("2d7aa3b0-f221-4753-b77f-ff261858a13a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2180));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9eca8d57-f7ca-4f8d-9c83-73b659225ae4"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2190));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("00aeb653-9bfd-46bd-98e8-631c080a7cd3"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2220));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("0754e6c4-4e12-42b1-845b-21ad1fc2f6b0"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2230));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("265e22e5-2252-432e-b6d2-e994740f6f08"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2220));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("5b953aa8-6bde-4f47-b984-d3622a8e0550"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2220));

            migrationBuilder.UpdateData(
                table: "RolePermission",
                keyColumn: "Id",
                keyValue: new Guid("9e89d8f2-c8dd-474c-b7fa-267a9570488a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2210));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("07a59e4e-ead7-4597-bb36-96a06ee3a847"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("47175080-a8c0-4d4f-bf88-85cd5cbff45b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2410));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("4dc7a2ea-5e37-4038-8810-79fd9df7ac0a"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("5562714c-a186-439f-a0df-25321703cf7e"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("8e49f3ba-11f1-4256-a195-91f9a8f3a41b"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2420));

            migrationBuilder.UpdateData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: new Guid("cf61c58c-e416-4b84-a195-dd7b77ef96dd"),
                column: "CreatedOn",
                value: new DateTime(2025, 5, 31, 23, 33, 58, 473, DateTimeKind.Utc).AddTicks(2410));
        }
    }
}

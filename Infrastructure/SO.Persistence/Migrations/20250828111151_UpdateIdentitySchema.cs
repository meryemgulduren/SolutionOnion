using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    public partial class UpdateIdentitySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Skipping adding "CreatedById" to avoid duplicate column error when it already exists.

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginDate",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AspNetRoles",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AspNetRoles",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetRoles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "Addresses",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<string>(
                name: "ManagedById",
                table: "Accounts",
                type: "varchar(255)",
                nullable: true);

            // Skipping index creation for "CreatedById" to avoid duplicate index when it already exists.

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ManagedById",
                table: "Accounts",
                column: "ManagedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AspNetUsers_ManagedById",
                table: "Accounts",
                column: "ManagedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // Skipping foreign key creation for "CreatedById" to avoid duplicate constraint when it already exists.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AspNetUsers_ManagedById",
                table: "Accounts");

            // No-op: foreign key for "CreatedById" was not created in Up.

            // No-op: index for "CreatedById" was not created in Up.

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ManagedById",
                table: "Accounts");

            // No-op: column "CreatedById" was not added in Up.

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "ManagedById",
                table: "Accounts");

            migrationBuilder.AlterColumn<string>(
                name: "Active",
                table: "Addresses",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}

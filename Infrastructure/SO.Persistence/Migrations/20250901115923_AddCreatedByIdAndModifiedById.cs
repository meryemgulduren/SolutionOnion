using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByIdAndModifiedById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "SuccessCriteria",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "SuccessCriteria",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ResourceRequirements",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "ResourceRequirements",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ProposalItems",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "ProposalItems",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "ProjectStakeholders",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "ProjectStakeholders",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Milestones",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Milestones",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "CustomerBeneficiaries",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "CustomerBeneficiaries",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "CriticalSuccessFactors",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "CriticalSuccessFactors",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "BusinessObjectives",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "BusinessObjectives",
                type: "longtext",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Addresses",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Addresses",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Accounts",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "Accounts",
                type: "longtext",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SuccessCriteria");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "SuccessCriteria");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ResourceRequirements");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "ResourceRequirements");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProposalItems");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "ProposalItems");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "ProjectStakeholders");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "ProjectStakeholders");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "CustomerBeneficiaries");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "CustomerBeneficiaries");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "CriticalSuccessFactors");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "CriticalSuccessFactors");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "BusinessObjectives");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "BusinessObjectives");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "Accounts");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "AspNetUsers",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}

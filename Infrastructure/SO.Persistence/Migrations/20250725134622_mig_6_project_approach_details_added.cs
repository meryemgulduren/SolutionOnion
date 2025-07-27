using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    public partial class mig_6_project_approach_details_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Interoperability",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OutsourcingPlans",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phasing",
                table: "Proposals",
                type: "longtext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interoperability",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "OutsourcingPlans",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Phasing",
                table: "Proposals");
        }
    }
}

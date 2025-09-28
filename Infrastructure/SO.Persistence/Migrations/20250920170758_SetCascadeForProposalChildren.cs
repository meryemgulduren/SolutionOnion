using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SetCascadeForProposalChildren : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompetitivePrice",
                table: "Proposals");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "ProposalPartners",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "ProposalCompetitors",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CompetitivePrice",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "ProposalPartners",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                table: "ProposalCompetitors",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200);
        }
    }
}

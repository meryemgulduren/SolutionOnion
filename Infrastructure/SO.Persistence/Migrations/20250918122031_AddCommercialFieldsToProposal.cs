using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCommercialFieldsToProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommercialNote",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CompetitivePrice",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerm",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TargetPrice",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProposalCompetitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CompanyName = table.Column<string>(type: "longtext", nullable: false),
                    OfferedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Note = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalCompetitors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalCompetitors_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProposalPartners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CompanyName = table.Column<string>(type: "longtext", nullable: false),
                    Role = table.Column<string>(type: "longtext", nullable: true),
                    Note = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalPartners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalPartners_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalCompetitors_ProposalId",
                table: "ProposalCompetitors",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalPartners_ProposalId",
                table: "ProposalPartners",
                column: "ProposalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposalCompetitors");

            migrationBuilder.DropTable(
                name: "ProposalPartners");

            migrationBuilder.DropColumn(
                name: "CommercialNote",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "CompetitivePrice",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "PaymentTerm",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "TargetPrice",
                table: "Proposals");
        }
    }
}

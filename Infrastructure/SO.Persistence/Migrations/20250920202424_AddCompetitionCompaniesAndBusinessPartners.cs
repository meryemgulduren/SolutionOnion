using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitionCompaniesAndBusinessPartners : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessPartners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    PartnerName = table.Column<string>(type: "longtext", nullable: false),
                    Role = table.Column<string>(type: "longtext", nullable: false),
                    ContactInfo = table.Column<string>(type: "longtext", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: true),
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
                    table.PrimaryKey("PK_BusinessPartners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPartners_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompetitionCompanies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CompanyName = table.Column<string>(type: "longtext", nullable: false),
                    CompetedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "longtext", nullable: true),
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
                    table.PrimaryKey("PK_CompetitionCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitionCompanies_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPartners_ProposalId",
                table: "BusinessPartners",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetitionCompanies_ProposalId",
                table: "CompetitionCompanies",
                column: "ProposalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessPartners");

            migrationBuilder.DropTable(
                name: "CompetitionCompanies");
        }
    }
}

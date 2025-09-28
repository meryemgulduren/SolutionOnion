using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProposalTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProposalCompetitors");

            migrationBuilder.DropTable(
                name: "ProposalPartners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProposalCompetitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CompanyName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true),
                    OfferedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
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
                    CompanyName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Note = table.Column<string>(type: "longtext", nullable: true),
                    Role = table.Column<string>(type: "longtext", nullable: true)
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
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DropLegacyProposalArtifacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessObjectives");

            migrationBuilder.DropTable(
                name: "CriticalSuccessFactors");

            migrationBuilder.DropTable(
                name: "CustomerBeneficiaries");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropTable(
                name: "ProjectStakeholders");

            migrationBuilder.DropTable(
                name: "ProposalItems");

            migrationBuilder.DropTable(
                name: "ResourceRequirements");

            migrationBuilder.DropTable(
                name: "SuccessCriteria");

            migrationBuilder.DropColumn(
                name: "Interoperability",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "OutsourcingPlans",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Phasing",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ProjectApproach",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ProjectCode",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "StatementOfNeed",
                table: "Proposals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "ProjectApproach",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectCode",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatementOfNeed",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessObjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Alignment = table.Column<string>(type: "longtext", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Objective = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessObjectives_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CriticalSuccessFactors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriticalSuccessFactors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriticalSuccessFactors_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CustomerBeneficiaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Beneficiary = table.Column<string>(type: "longtext", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NeedsAndConcern = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerBeneficiaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerBeneficiaries_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    IsCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    PlannedCompletionDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milestones_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProjectStakeholders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Responsibilities = table.Column<string>(type: "longtext", nullable: true),
                    Role = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStakeholders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStakeholders_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProposalItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "longtext", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProposalItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProposalItems_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ResourceRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Resource = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceRequirements_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SuccessCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    CreatedById = table.Column<string>(type: "longtext", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedById = table.Column<string>(type: "longtext", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessCriteria_Proposals_ProposalId",
                        column: x => x.ProposalId,
                        principalTable: "Proposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessObjectives_ProposalId",
                table: "BusinessObjectives",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CriticalSuccessFactors_ProposalId",
                table: "CriticalSuccessFactors",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerBeneficiaries_ProposalId",
                table: "CustomerBeneficiaries",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_ProposalId",
                table: "Milestones",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStakeholders_ProposalId",
                table: "ProjectStakeholders",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProposalItems_ProposalId",
                table: "ProposalItems",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceRequirements_ProposalId",
                table: "ResourceRequirements",
                column: "ProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessCriteria_ProposalId",
                table: "SuccessCriteria",
                column: "ProposalId");
        }
    }
}

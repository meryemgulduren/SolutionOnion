using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SO.Persistence.Migrations
{
    public partial class mig2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Proposals",
                newName: "StatementOfNeed");

            migrationBuilder.AddColumn<string>(
                name: "ProjectDescription",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BusinessObjectives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Objective = table.Column<string>(type: "longtext", nullable: false),
                    Alignment = table.Column<string>(type: "longtext", nullable: false),
                    ProposalId = table.Column<Guid>(type: "char(36)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "longtext", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_BusinessObjectives_ProposalId",
                table: "BusinessObjectives",
                column: "ProposalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessObjectives");

            migrationBuilder.DropColumn(
                name: "ProjectDescription",
                table: "Proposals");

            migrationBuilder.RenameColumn(
                name: "StatementOfNeed",
                table: "Proposals",
                newName: "Description");
        }
    }
}

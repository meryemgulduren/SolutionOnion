using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneralDefinitionFieldsToProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemLogs");

            migrationBuilder.AddColumn<Guid>(
                name: "AddressId",
                table: "Proposals",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryDurationDays",
                table: "Proposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralNote",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferDurationDays",
                table: "Proposals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfferOwner",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectCode",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuantityUnit",
                table: "Proposals",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "QuantityValue",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "DeliveryDurationDays",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "GeneralNote",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "OfferDurationDays",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "OfferOwner",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ProjectCode",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "QuantityUnit",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "QuantityValue",
                table: "Proposals");

            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    AdditionalData = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Exception = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    HttpMethod = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    Level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    RequestPath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    ResponseTime = table.Column<long>(type: "bigint", nullable: true),
                    Source = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    UserName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace SO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Level = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Exception = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    Source = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    Action = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    UserName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    RequestPath = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true),
                    HttpMethod = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    ResponseTime = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AdditionalData = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemLogs", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemLogs");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostalTracker.Orchestrator.Host.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostalStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<string>(type: "varchar(50)", nullable: false),
                    Timestamp = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    AddressDelivery = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    AddressSender = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    InWayDurationToken = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.CorrelationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostalStates");
        }
    }
}

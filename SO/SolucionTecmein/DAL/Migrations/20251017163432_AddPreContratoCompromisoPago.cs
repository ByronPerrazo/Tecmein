using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddPreContratoCompromisoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreContratoCompromisoPagos",
                columns: table => new
                {
                    Secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecPreContrato = table.Column<int>(type: "int", nullable: false),
                    NumeroCuota = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Tipo = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreContratoCompromisoPagos", x => x.Secuencial);
                    table.ForeignKey(
                        name: "FK_PreContratoCompromisoPagos_precontrato_SecPreContrato",
                        column: x => x.SecPreContrato,
                        principalTable: "precontrato",
                        principalColumn: "SecPreContrato",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_PreContratoCompromisoPagos_SecPreContrato",
                table: "PreContratoCompromisoPagos",
                column: "SecPreContrato");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreContratoCompromisoPagos");
        }
    }
}

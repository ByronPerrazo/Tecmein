using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMontoPagadoToCuota : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreContrato_FormaPago",
                table: "precontrato");

            migrationBuilder.DropIndex(
                name: "FK_PreContrato_FormaPago_idx",
                table: "precontrato");

            migrationBuilder.DropColumn(
                name: "FechaAnticipo",
                table: "precontrato");

            migrationBuilder.DropColumn(
                name: "FechaPrimeraCuota",
                table: "precontrato");

            migrationBuilder.DropColumn(
                name: "NumeroCuotas",
                table: "precontrato");

            migrationBuilder.DropColumn(
                name: "SecFormaPago",
                table: "precontrato");

            migrationBuilder.DropColumn(
                name: "ValorAnticipo",
                table: "precontrato");

            migrationBuilder.DropColumn(
                name: "ValorContrato",
                table: "precontrato");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoPagado",
                table: "cuota",
                type: "decimal(65,30)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontoPagado",
                table: "cuota");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAnticipo",
                table: "precontrato",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaPrimeraCuota",
                table: "precontrato",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroCuotas",
                table: "precontrato",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SecFormaPago",
                table: "precontrato",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorAnticipo",
                table: "precontrato",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorContrato",
                table: "precontrato",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "FK_PreContrato_FormaPago_idx",
                table: "precontrato",
                column: "SecFormaPago");

            migrationBuilder.AddForeignKey(
                name: "FK_PreContrato_FormaPago",
                table: "precontrato",
                column: "SecFormaPago",
                principalTable: "formapago",
                principalColumn: "SecFormaPago");
        }
    }
}

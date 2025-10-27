using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRolMenuPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PRIMARY",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "secuencial",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "esActivo",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "fechaRegistro",
                table: "rolmenu");

            migrationBuilder.AlterColumn<int>(
                name: "secRol",
                table: "rolmenu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "secMenu",
                table: "rolmenu",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Actualizar",
                table: "rolmenu",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Crear",
                table: "rolmenu",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Eliminar",
                table: "rolmenu",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Leer",
                table: "rolmenu",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_rolmenu",
                table: "rolmenu",
                columns: new[] { "secRol", "secMenu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_rolmenu",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "Actualizar",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "Crear",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "Eliminar",
                table: "rolmenu");

            migrationBuilder.DropColumn(
                name: "Leer",
                table: "rolmenu");

            migrationBuilder.AlterColumn<int>(
                name: "secMenu",
                table: "rolmenu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "secRol",
                table: "rolmenu",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "secuencial",
                table: "rolmenu",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<short>(
                name: "esActivo",
                table: "rolmenu",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fechaRegistro",
                table: "rolmenu",
                type: "datetime",
                nullable: true,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddPrimaryKey(
                name: "PRIMARY",
                table: "rolmenu",
                column: "secuencial");
        }
    }
}

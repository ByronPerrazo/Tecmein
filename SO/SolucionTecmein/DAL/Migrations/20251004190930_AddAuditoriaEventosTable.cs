using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditoriaEventosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb3");

            migrationBuilder.CreateTable(
                name: "AuditoriaEventos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FechaHora = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: true),
                    NombreUsuario = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    TipoEvento = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Detalle = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    DireccionIp = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaEventos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombreArchivo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    urlCatalogo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "constructora",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    direccion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    correo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    atencion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    administrador = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    telefonoAdministrador = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    correoAdministrador = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "diccionarioparametro",
                columns: table => new
                {
                    Secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Parametro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Descripcion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "empresa",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    urlLogo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombreLogo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    identificacion = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    correo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    direccion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    telefono = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    codigoOperador = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "equiposvisita",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false),
                    secVisita = table.Column<int>(type: "int", nullable: false),
                    tipoEquipo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    sistema = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    marca = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    capacidad = table.Column<int>(type: "int", nullable: false),
                    velocidad = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    salaMaquinas = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    salaControl = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    numeroPersonas = table.Column<int>(type: "int", nullable: true),
                    numeroParadas = table.Column<int>(type: "int", nullable: true),
                    nombresParadas = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    embarque = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    tipoDucto = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    medidasAFDucto = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    tipoMotor = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    foso = table.Column<int>(type: "int", nullable: true),
                    recorrido = table.Column<int>(type: "int", nullable: true),
                    ingresosFrontales = table.Column<int>(type: "int", nullable: true),
                    ingresosPosteriores = table.Column<int>(type: "int", nullable: true),
                    SobreRecorrido = table.Column<int>(type: "int", nullable: true),
                    dimencionEntrada = table.Column<int>(type: "int", nullable: true),
                    alturaEntrePisos = table.Column<int>(type: "int", nullable: true),
                    materialPuertas = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    energia = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    cantidad = table.Column<int>(type: "int", nullable: true),
                    estaActivo = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "etapa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "formapago",
                columns: table => new
                {
                    SecFormaPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EstaActivo = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecFormaPago);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "menu",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(type: "varchar(130)", maxLength: 130, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    secMenuPadre = table.Column<int>(type: "int", nullable: true),
                    icono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    controlador = table.Column<string>(type: "varchar(130)", maxLength: 130, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    paginaAccion = table.Column<string>(type: "varchar(130)", maxLength: 130, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    esActivo = table.Column<short>(type: "smallint", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "FK_Menu_Menu",
                        column: x => x.secMenuPadre,
                        principalTable: "menu",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "permiso",
                columns: table => new
                {
                    IdPermiso = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Descripcion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdPermiso);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "PolizaGarantia",
                columns: table => new
                {
                    Secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolizaGarantia", x => x.Secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "provincia",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    codigo = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    esActivo = table.Column<short>(type: "smallint", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "tipodocumento",
                columns: table => new
                {
                    SecTipoDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Descripcion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecTipoDocumento);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "tipoimpuesto",
                columns: table => new
                {
                    Secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EsIva = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EsImportacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Secuencial);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "cliente",
                columns: table => new
                {
                    SecCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecConstructora = table.Column<int>(type: "int", nullable: false),
                    NumeroCliente = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecCliente);
                    table.ForeignKey(
                        name: "FK_Cliente_Constructora",
                        column: x => x.SecConstructora,
                        principalTable: "constructora",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "contacto",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secConstructora = table.Column<int>(type: "int", nullable: false),
                    titulo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    apellidos = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    correo = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "FK_Contacto_Constructora",
                        column: x => x.secConstructora,
                        principalTable: "constructora",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "empresacorreo",
                columns: table => new
                {
                    SecEmpresa = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    clave = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    alias = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    host = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    puerto = table.Column<int>(type: "int", nullable: true),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecEmpresa);
                    table.ForeignKey(
                        name: "Fk_Empresa_EmpCorreo",
                        column: x => x.SecEmpresa,
                        principalTable: "empresa",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "empresastorage",
                columns: table => new
                {
                    secEmpresa = table.Column<int>(type: "int", nullable: false),
                    email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    clave = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ruta = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    apiKey = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    carpetaUsuario = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    carpetaProducto = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    carpetaLogo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secEmpresa);
                    table.ForeignKey(
                        name: "empresastorage_ibfk_1",
                        column: x => x.secEmpresa,
                        principalTable: "empresa",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "formatonumerocliente",
                columns: table => new
                {
                    SecFormatoNumeroCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecEmpresa = table.Column<int>(type: "int", nullable: false),
                    UsaFormato = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Formato = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    NumeroInicio = table.Column<int>(type: "int", nullable: false),
                    LongitudNumero = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecFormatoNumeroCliente);
                    table.ForeignKey(
                        name: "FK_FormatoNumeroCliente_Empresa",
                        column: x => x.SecEmpresa,
                        principalTable: "empresa",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "canton",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secProvincia = table.Column<int>(type: "int", nullable: false),
                    codigo = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "Fk_Provincia_Canton",
                        column: x => x.secProvincia,
                        principalTable: "provincia",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "rolmenu",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secRol = table.Column<int>(type: "int", nullable: true),
                    secMenu = table.Column<int>(type: "int", nullable: true),
                    esActivo = table.Column<short>(type: "smallint", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "FK_Menu_Rol",
                        column: x => x.secMenu,
                        principalTable: "menu",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_Rol_Menu",
                        column: x => x.secRol,
                        principalTable: "rol",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "rolpermiso",
                columns: table => new
                {
                    SecRol = table.Column<int>(type: "int", nullable: false),
                    IdPermiso = table.Column<string>(type: "varchar(100)", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rolpermiso", x => new { x.SecRol, x.IdPermiso });
                    table.ForeignKey(
                        name: "FK_RolPermiso_Permiso",
                        column: x => x.IdPermiso,
                        principalTable: "permiso",
                        principalColumn: "IdPermiso");
                    table.ForeignKey(
                        name: "FK_RolPermiso_Rol",
                        column: x => x.SecRol,
                        principalTable: "rol",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    correo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    telefono = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    secRol = table.Column<int>(type: "int", nullable: true),
                    urlFoto = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombreFoto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    clave = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    esActivo = table.Column<short>(type: "smallint", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "Fk_Rol_Usuario",
                        column: x => x.secRol,
                        principalTable: "rol",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "plantillaprecontrato",
                columns: table => new
                {
                    SecPlantillaPreContrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    NumeracionInicial = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true),
                    EstaActivo = table.Column<int>(type: "int", nullable: true),
                    SecTipoDocumento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecPlantillaPreContrato);
                    table.ForeignKey(
                        name: "FK_PlantillaPreContrato_TipoDocumento",
                        column: x => x.SecTipoDocumento,
                        principalTable: "tipodocumento",
                        principalColumn: "SecTipoDocumento");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "impuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Descripcion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ValorFijo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CodigoSri = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    SecTipoImpuesto = table.Column<int>(type: "int", nullable: false),
                    Vigente = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Impuesto_TipoImpuesto",
                        column: x => x.SecTipoImpuesto,
                        principalTable: "tipoimpuesto",
                        principalColumn: "Secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "parroquia",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secCanton = table.Column<int>(type: "int", nullable: false),
                    codigo = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "Fk_Canton_Parroquia",
                        column: x => x.secCanton,
                        principalTable: "canton",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "plantillaprecontratoparrafo",
                columns: table => new
                {
                    SecPlantillaPreContratoParrafo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecPlantillaPreContrato = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "TEXT", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecPlantillaPreContratoParrafo);
                    table.ForeignKey(
                        name: "FK_plantillaprecontratoparrafo_plantillaprecontrato",
                        column: x => x.SecPlantillaPreContrato,
                        principalTable: "plantillaprecontrato",
                        principalColumn: "SecPlantillaPreContrato",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "visita",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secUsuario = table.Column<int>(type: "int", nullable: false),
                    secProvincia = table.Column<int>(type: "int", nullable: true),
                    secCanton = table.Column<int>(type: "int", nullable: true),
                    secParroquia = table.Column<int>(type: "int", nullable: true),
                    IdEtapa = table.Column<int>(type: "int", nullable: false),
                    SecEmpresa = table.Column<int>(type: "int", nullable: true),
                    nombre = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    direccion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    fechaSiguienteVisita = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    geoUbicacion = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    detalle = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true),
                    SecConstructora = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "FK_Visita_Empresa",
                        column: x => x.SecEmpresa,
                        principalTable: "empresa",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_Visita_Etapa",
                        column: x => x.IdEtapa,
                        principalTable: "etapa",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_visita_constructora_SecConstructora",
                        column: x => x.SecConstructora,
                        principalTable: "constructora",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "Fk_Visita_Canton",
                        column: x => x.secCanton,
                        principalTable: "canton",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "Fk_Visita_Parroquia",
                        column: x => x.secParroquia,
                        principalTable: "parroquia",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "Fk_Visita_Provincia",
                        column: x => x.secProvincia,
                        principalTable: "provincia",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "Fk_Visita_Usuario",
                        column: x => x.secUsuario,
                        principalTable: "usuario",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "contactovisita",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secContacto = table.Column<int>(type: "int", nullable: false),
                    secVisita = table.Column<int>(type: "int", nullable: false),
                    estaActivo = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "Fk_Contacto_ContactoVisita",
                        column: x => x.secContacto,
                        principalTable: "contacto",
                        principalColumn: "secuencial",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Fk_Visita_ContactoVisita",
                        column: x => x.secVisita,
                        principalTable: "visita",
                        principalColumn: "secuencial",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "cotizacion",
                columns: table => new
                {
                    secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    secVisita = table.Column<int>(type: "int", nullable: false),
                    enviadoProveedor = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    enviadoCliente = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    confirmacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    valorImpuestos = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    totalConImpuestos = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    valorIVA = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    valorImportacion = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    estaActivo = table.Column<short>(type: "smallint", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true),
                    fechaModificacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    secUsuario = table.Column<int>(type: "int", nullable: true),
                    secCotizacionOriginal = table.Column<int>(type: "int", nullable: true),
                    secUsuarioModifica = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.secuencial);
                    table.ForeignKey(
                        name: "FK_Cotizacion_CotizacionOriginal",
                        column: x => x.secCotizacionOriginal,
                        principalTable: "cotizacion",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_Cotizacion_Usuario",
                        column: x => x.secUsuario,
                        principalTable: "usuario",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_Cotizacion_UsuarioModifica",
                        column: x => x.secUsuarioModifica,
                        principalTable: "usuario",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_Cotizacion_Visita",
                        column: x => x.secVisita,
                        principalTable: "visita",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "contrato",
                columns: table => new
                {
                    IdContrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdCotizacion = table.Column<int>(type: "int", nullable: false),
                    FechaFirma = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdUsuarioCarga = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    RutaArchivo = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    EsActivo = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    SecCliente = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdContrato);
                    table.ForeignKey(
                        name: "FK_Contrato_Cliente",
                        column: x => x.SecCliente,
                        principalTable: "cliente",
                        principalColumn: "SecCliente");
                    table.ForeignKey(
                        name: "FK_Contrato_Cotizacion",
                        column: x => x.IdCotizacion,
                        principalTable: "cotizacion",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_Contrato_Usuario",
                        column: x => x.IdUsuarioCarga,
                        principalTable: "usuario",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "cotizaciondetalle",
                columns: table => new
                {
                    Secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecCotizacion = table.Column<int>(type: "int", nullable: false),
                    SecEquipoVisita = table.Column<int>(type: "int", nullable: true),
                    DetalleEquipo = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ValorCompra = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MargenGanancia = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    EstaActivo = table.Column<short>(type: "smallint", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Secuencial);
                    table.ForeignKey(
                        name: "FK_Cotizaciondetalle_Cotizacion",
                        column: x => x.SecCotizacion,
                        principalTable: "cotizacion",
                        principalColumn: "secuencial",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "impuestocotizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecCotizacion = table.Column<int>(type: "int", nullable: false),
                    ImpuestoId = table.Column<int>(type: "int", nullable: false),
                    BaseImponible = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorImpuesto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exento = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImpuestoCotizacion_Cotizacion",
                        column: x => x.SecCotizacion,
                        principalTable: "cotizacion",
                        principalColumn: "secuencial",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImpuestoCotizacion_Impuesto",
                        column: x => x.ImpuestoId,
                        principalTable: "impuesto",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "precontrato",
                columns: table => new
                {
                    SecPreContrato = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecCotizacion = table.Column<int>(type: "int", nullable: false),
                    SecPlantillaPreContrato = table.Column<int>(type: "int", nullable: false),
                    SecUsuarioCrea = table.Column<int>(type: "int", nullable: false),
                    SecFormaPago = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    Dias = table.Column<int>(type: "int", nullable: false),
                    TipoDias = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ValorContrato = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AniosGarantia = table.Column<int>(type: "int", nullable: false),
                    MesesGarantia = table.Column<int>(type: "int", nullable: false),
                    PeriodoMantenimiento = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    PolizaGarantia = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ValorAnticipo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaAnticipo = table.Column<DateTime>(type: "datetime", nullable: true),
                    NumeroCuotas = table.Column<int>(type: "int", nullable: false),
                    FechaPrimeraCuota = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecPreContrato);
                    table.ForeignKey(
                        name: "FK_PreContrato_Cotizacion",
                        column: x => x.SecCotizacion,
                        principalTable: "cotizacion",
                        principalColumn: "secuencial");
                    table.ForeignKey(
                        name: "FK_PreContrato_FormaPago",
                        column: x => x.SecFormaPago,
                        principalTable: "formapago",
                        principalColumn: "SecFormaPago");
                    table.ForeignKey(
                        name: "FK_PreContrato_PlantillaPreContrato",
                        column: x => x.SecPlantillaPreContrato,
                        principalTable: "plantillaprecontrato",
                        principalColumn: "SecPlantillaPreContrato");
                    table.ForeignKey(
                        name: "FK_PreContrato_Usuario",
                        column: x => x.SecUsuarioCrea,
                        principalTable: "usuario",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "seguimiento",
                columns: table => new
                {
                    SecSeguimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecCotizacion = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Detalle = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaAccion = table.Column<DateTime>(type: "datetime", nullable: false),
                    AceptacionCliente = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.SecSeguimiento);
                    table.ForeignKey(
                        name: "FK_Seguimiento_Cotizacion",
                        column: x => x.SecCotizacion,
                        principalTable: "cotizacion",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "plandepago",
                columns: table => new
                {
                    IdPlanDePago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdContrato = table.Column<int>(type: "int", nullable: false),
                    SecFormaPago = table.Column<int>(type: "int", nullable: false),
                    ValorContrato = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorAnticipo = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaAnticipo = table.Column<DateTime>(type: "datetime", nullable: true),
                    NumeroCuotas = table.Column<int>(type: "int", nullable: false),
                    FechaPrimeraCuota = table.Column<DateTime>(type: "datetime", nullable: true),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdPlanDePago);
                    table.ForeignKey(
                        name: "FK_PlanDePago_FormaPago",
                        column: x => x.SecFormaPago,
                        principalTable: "formapago",
                        principalColumn: "SecFormaPago");
                    table.ForeignKey(
                        name: "FK_plandepago_contrato_IdContrato",
                        column: x => x.IdContrato,
                        principalTable: "contrato",
                        principalColumn: "IdContrato",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "precontratoparrafo",
                columns: table => new
                {
                    Secuencial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SecPreContrato = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "TEXT", nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_precontratoparrafo", x => x.Secuencial);
                    table.ForeignKey(
                        name: "FK_precontratoparrafo_precontrato_SecPreContrato",
                        column: x => x.SecPreContrato,
                        principalTable: "precontrato",
                        principalColumn: "SecPreContrato",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "cuota",
                columns: table => new
                {
                    IdCuota = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPlanDePago = table.Column<int>(type: "int", nullable: false),
                    NumeroCuota = table.Column<int>(type: "int", nullable: false),
                    MontoEsperado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime", nullable: false),
                    Estado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdCuota);
                    table.ForeignKey(
                        name: "FK_Cuota_PlanDePago",
                        column: x => x.IdPlanDePago,
                        principalTable: "plandepago",
                        principalColumn: "IdPlanDePago",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateTable(
                name: "pago",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdPlanDePago = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime", nullable: false),
                    ComprobanteUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    ComprobanteNombre = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb3_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb3"),
                    RegistradoPorUsuarioId = table.Column<int>(type: "int", nullable: false),
                    EstaActivo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_Pago_PlanDePago",
                        column: x => x.IdPlanDePago,
                        principalTable: "plandepago",
                        principalColumn: "IdPlanDePago");
                    table.ForeignKey(
                        name: "FK_Pago_Usuario",
                        column: x => x.RegistradoPorUsuarioId,
                        principalTable: "usuario",
                        principalColumn: "secuencial");
                })
                .Annotation("MySql:CharSet", "utf8mb3")
                .Annotation("Relational:Collation", "utf8mb3_general_ci");

            migrationBuilder.CreateIndex(
                name: "Fk_Provincia_Canton",
                table: "canton",
                column: "secProvincia");

            migrationBuilder.CreateIndex(
                name: "FK_Cliente_Constructora_idx",
                table: "cliente",
                column: "SecConstructora",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_Contacto_Constructora_idx",
                table: "contacto",
                column: "secConstructora");

            migrationBuilder.CreateIndex(
                name: "Fk_Contacto_ContactoVisita",
                table: "contactovisita",
                column: "secContacto");

            migrationBuilder.CreateIndex(
                name: "Fk_Contacto_Visita",
                table: "contactovisita",
                column: "secVisita");

            migrationBuilder.CreateIndex(
                name: "FK_Contrato_Cotizacion_idx",
                table: "contrato",
                column: "IdCotizacion");

            migrationBuilder.CreateIndex(
                name: "FK_Contrato_Usuario_idx",
                table: "contrato",
                column: "IdUsuarioCarga");

            migrationBuilder.CreateIndex(
                name: "IX_contrato_SecCliente",
                table: "contrato",
                column: "SecCliente");

            migrationBuilder.CreateIndex(
                name: "FK_Cotizacion_Visita_idx",
                table: "cotizacion",
                column: "secVisita");

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_secCotizacionOriginal",
                table: "cotizacion",
                column: "secCotizacionOriginal");

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_secUsuario",
                table: "cotizacion",
                column: "secUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_secUsuarioModifica",
                table: "cotizacion",
                column: "secUsuarioModifica");

            migrationBuilder.CreateIndex(
                name: "IX_cotizaciondetalle_SecCotizacion",
                table: "cotizaciondetalle",
                column: "SecCotizacion");

            migrationBuilder.CreateIndex(
                name: "FK_Cuota_PlanDePago_idx",
                table: "cuota",
                column: "IdPlanDePago");

            migrationBuilder.CreateIndex(
                name: "UQ_DiccionarioParametro_Parametro",
                table: "diccionarioparametro",
                column: "Parametro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "secuencial_UNIQUE",
                table: "equiposvisita",
                column: "secuencial",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_FormatoNumeroCliente_Empresa_idx",
                table: "formatonumerocliente",
                column: "SecEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_impuesto_SecTipoImpuesto",
                table: "impuesto",
                column: "SecTipoImpuesto");

            migrationBuilder.CreateIndex(
                name: "IX_impuestocotizacion_ImpuestoId",
                table: "impuestocotizacion",
                column: "ImpuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_impuestocotizacion_SecCotizacion",
                table: "impuestocotizacion",
                column: "SecCotizacion");

            migrationBuilder.CreateIndex(
                name: "FK_Menu_Menu_idx",
                table: "menu",
                column: "secMenuPadre");

            migrationBuilder.CreateIndex(
                name: "FK_Pago_PlanDePago_idx",
                table: "pago",
                column: "IdPlanDePago");

            migrationBuilder.CreateIndex(
                name: "FK_Pago_Usuario_idx",
                table: "pago",
                column: "RegistradoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "FK_Parroquia_Canton_idx",
                table: "parroquia",
                column: "secCanton");

            migrationBuilder.CreateIndex(
                name: "FK_PlanDePago_Contrato_idx",
                table: "plandepago",
                column: "IdContrato",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_PlanDePago_FormaPago_idx",
                table: "plandepago",
                column: "SecFormaPago");

            migrationBuilder.CreateIndex(
                name: "FK_PlantillaPreContrato_TipoDocumento_idx",
                table: "plantillaprecontrato",
                column: "SecTipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_plantillaprecontratoparrafo_plantilla",
                table: "plantillaprecontratoparrafo",
                column: "SecPlantillaPreContrato");

            migrationBuilder.CreateIndex(
                name: "FK_PreContrato_Cotizacion_idx",
                table: "precontrato",
                column: "SecCotizacion");

            migrationBuilder.CreateIndex(
                name: "FK_PreContrato_FormaPago_idx",
                table: "precontrato",
                column: "SecFormaPago");

            migrationBuilder.CreateIndex(
                name: "FK_PreContrato_Plantilla_idx",
                table: "precontrato",
                column: "SecPlantillaPreContrato");

            migrationBuilder.CreateIndex(
                name: "FK_PreContrato_Usuario_idx",
                table: "precontrato",
                column: "SecUsuarioCrea");

            migrationBuilder.CreateIndex(
                name: "IX_precontratoparrafo_SecPreContrato",
                table: "precontratoparrafo",
                column: "SecPreContrato");

            migrationBuilder.CreateIndex(
                name: "FK_Menu_Rol_idx",
                table: "rolmenu",
                column: "secMenu");

            migrationBuilder.CreateIndex(
                name: "FK_Rol_Menu_idx",
                table: "rolmenu",
                column: "secRol");

            migrationBuilder.CreateIndex(
                name: "IX_rolpermiso_IdPermiso",
                table: "rolpermiso",
                column: "IdPermiso");

            migrationBuilder.CreateIndex(
                name: "FK_Seguimiento_Cotizacion_idx",
                table: "seguimiento",
                column: "SecCotizacion");

            migrationBuilder.CreateIndex(
                name: "Fk_Rol_Usuario_idx",
                table: "usuario",
                column: "secRol");

            migrationBuilder.CreateIndex(
                name: "Fk_Visita_Canton",
                table: "visita",
                column: "secCanton");

            migrationBuilder.CreateIndex(
                name: "Fk_Visita_Parroquia",
                table: "visita",
                column: "secParroquia");

            migrationBuilder.CreateIndex(
                name: "Fk_Visita_Provincia",
                table: "visita",
                column: "secProvincia");

            migrationBuilder.CreateIndex(
                name: "Fk_Visita_Usuario_idx",
                table: "visita",
                column: "secUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_visita_IdEtapa",
                table: "visita",
                column: "IdEtapa");

            migrationBuilder.CreateIndex(
                name: "IX_visita_SecConstructora",
                table: "visita",
                column: "SecConstructora");

            migrationBuilder.CreateIndex(
                name: "IX_visita_SecEmpresa",
                table: "visita",
                column: "SecEmpresa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriaEventos");

            migrationBuilder.DropTable(
                name: "catalogo");

            migrationBuilder.DropTable(
                name: "contactovisita");

            migrationBuilder.DropTable(
                name: "cotizaciondetalle");

            migrationBuilder.DropTable(
                name: "cuota");

            migrationBuilder.DropTable(
                name: "diccionarioparametro");

            migrationBuilder.DropTable(
                name: "empresacorreo");

            migrationBuilder.DropTable(
                name: "empresastorage");

            migrationBuilder.DropTable(
                name: "equiposvisita");

            migrationBuilder.DropTable(
                name: "formatonumerocliente");

            migrationBuilder.DropTable(
                name: "impuestocotizacion");

            migrationBuilder.DropTable(
                name: "pago");

            migrationBuilder.DropTable(
                name: "plantillaprecontratoparrafo");

            migrationBuilder.DropTable(
                name: "PolizaGarantia");

            migrationBuilder.DropTable(
                name: "precontratoparrafo");

            migrationBuilder.DropTable(
                name: "rolmenu");

            migrationBuilder.DropTable(
                name: "rolpermiso");

            migrationBuilder.DropTable(
                name: "seguimiento");

            migrationBuilder.DropTable(
                name: "contacto");

            migrationBuilder.DropTable(
                name: "impuesto");

            migrationBuilder.DropTable(
                name: "plandepago");

            migrationBuilder.DropTable(
                name: "precontrato");

            migrationBuilder.DropTable(
                name: "menu");

            migrationBuilder.DropTable(
                name: "permiso");

            migrationBuilder.DropTable(
                name: "tipoimpuesto");

            migrationBuilder.DropTable(
                name: "contrato");

            migrationBuilder.DropTable(
                name: "formapago");

            migrationBuilder.DropTable(
                name: "plantillaprecontrato");

            migrationBuilder.DropTable(
                name: "cliente");

            migrationBuilder.DropTable(
                name: "cotizacion");

            migrationBuilder.DropTable(
                name: "tipodocumento");

            migrationBuilder.DropTable(
                name: "visita");

            migrationBuilder.DropTable(
                name: "empresa");

            migrationBuilder.DropTable(
                name: "etapa");

            migrationBuilder.DropTable(
                name: "constructora");

            migrationBuilder.DropTable(
                name: "parroquia");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "canton");

            migrationBuilder.DropTable(
                name: "rol");

            migrationBuilder.DropTable(
                name: "provincia");
        }
    }
}

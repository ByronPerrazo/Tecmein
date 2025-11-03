-- =================================================================
-- Script para la creación completa de la base de datos Tecmein
-- Generado automáticamente a partir de las entidades del proyecto
-- =================================================================

CREATE DATABASE IF NOT EXISTS Tecmein;
USE Tecmein;

-- =================================================================
-- Grupo 1: Tablas sin dependencias externas
-- =================================================================

-- Tabla: AuditoriaEvento
CREATE TABLE IF NOT EXISTS AuditoriaEvento (
    Id BIGINT PRIMARY KEY AUTO_INCREMENT,
    FechaHora DATETIME NOT NULL,
    IdUsuario INT NULL,
    NombreUsuario VARCHAR(255) NULL,
    TipoEvento VARCHAR(255) NOT NULL,
    Detalle TEXT NOT NULL,
    DireccionIp VARCHAR(45) NULL
);

-- Tabla: Catalogo
CREATE TABLE IF NOT EXISTS Catalogo (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(255) NOT NULL,
    NombreArchivo VARCHAR(255) NOT NULL,
    UrlCatalogo VARCHAR(2048) NULL,
    FechaRegistro DATETIME NULL,
    EstaActivo BIT(1) NULL
);

-- Tabla: Constructora
CREATE TABLE IF NOT EXISTS Constructora (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(255) NULL,
    Direccion VARCHAR(500) NULL,
    Telefono VARCHAR(20) NULL,
    Correo VARCHAR(255) NULL,
    Atencion VARCHAR(255) NULL,
    Administrador VARCHAR(255) NULL,
    TelefonoAdministrador VARCHAR(20) NULL,
    CorreoAdministrador VARCHAR(255) NULL,
    EstaActivo BIT(1) NULL
);

-- Tabla: DiccionarioParametro
CREATE TABLE IF NOT EXISTS DiccionarioParametro (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Parametro VARCHAR(255) NOT NULL,
    Descripcion VARCHAR(500) NOT NULL,
    EstaActivo BIT(1) NOT NULL
);

-- Tabla: Empresa
CREATE TABLE IF NOT EXISTS Empresa (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    UrlLogo VARCHAR(2048) NULL,
    NombreLogo VARCHAR(255) NULL,
    Identificacion VARCHAR(20) NULL,
    Nombre VARCHAR(255) NULL,
    Correo VARCHAR(255) NULL,
    Direccion VARCHAR(500) NULL,
    Telefono VARCHAR(20) NULL,
    CodigoOperador VARCHAR(50) NULL,
    EstaActivo BIT(1) NULL
);

-- Tabla: Etapa
CREATE TABLE IF NOT EXISTS Etapa (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Codigo VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(255) NOT NULL,
    Orden INT NOT NULL,
    EstaActivo BIT(1) NOT NULL
);

-- Tabla: FormaPago
CREATE TABLE IF NOT EXISTS FormaPago (
    SecFormaPago INT PRIMARY KEY AUTO_INCREMENT,
    Descripcion VARCHAR(255) NOT NULL,
    EstaActivo BIT(1) NOT NULL
);

-- Tabla: Menu
CREATE TABLE IF NOT EXISTS Menu (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Descripcion VARCHAR(255) NULL,
    SecMenuPadre INT NULL,
    Icono VARCHAR(100) NULL,
    Controlador VARCHAR(100) NULL,
    PaginaAccion VARCHAR(100) NULL,
    EsActivo BIT(1) NULL,
    MostrarEnMenu BIT(1) NOT NULL,
    Orden INT NOT NULL,
    FechaRegistro DATETIME NULL,
    FOREIGN KEY (SecMenuPadre) REFERENCES Menu(Secuencial)
);

-- Tabla: Permiso
CREATE TABLE IF NOT EXISTS Permiso (
    IdPermiso VARCHAR(100) PRIMARY KEY,
    Descripcion VARCHAR(255) NOT NULL
);

-- Tabla: PolizaGarantia
CREATE TABLE IF NOT EXISTS PolizaGarantia (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Descripcion VARCHAR(255) NOT NULL,
    EstaActivo BIT(1) NOT NULL
);

-- Tabla: Provincia
CREATE TABLE IF NOT EXISTS Provincia (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Codigo VARCHAR(10) NULL,
    Nombre VARCHAR(255) NULL,
    EstaActivo BIT(1) NULL
);

-- Tabla: Rol
CREATE TABLE IF NOT EXISTS Rol (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Descripcion VARCHAR(255) NULL,
    EsActivo BIT(1) NULL,
    FechaRegistro DATETIME NULL
);

-- Tabla: TipoDocumento
CREATE TABLE IF NOT EXISTS TipoDocumento (
    SecTipoDocumento INT PRIMARY KEY AUTO_INCREMENT,
    Codigo VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(255) NOT NULL,
    EstaActivo BIT(1) NULL,
    FechaRegistro DATETIME NULL
);

-- Tabla: TipoImpuesto
CREATE TABLE IF NOT EXISTS TipoImpuesto (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(255) NOT NULL,
    EsIva BIT(1) NOT NULL,
    EsImportacion BIT(1) NOT NULL,
    EstaActivo BIT(1) NOT NULL,
    FechaCreacion DATETIME NOT NULL,
    FechaModificacion DATETIME NULL
);

-- =================================================================
-- Grupo 2: Tablas con dependencias del Grupo 1
-- =================================================================

-- Tabla: Canton
CREATE TABLE IF NOT EXISTS Canton (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecProvincia INT NOT NULL,
    Codigo VARCHAR(10) NULL,
    Nombre VARCHAR(255) NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecProvincia) REFERENCES Provincia(Secuencial)
);

-- Tabla: Cliente
CREATE TABLE IF NOT EXISTS Cliente (
    SecCliente INT PRIMARY KEY AUTO_INCREMENT,
    SecConstructora INT NOT NULL,
    NumeroCliente VARCHAR(50) NOT NULL,
    FechaCreacion DATETIME NOT NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecConstructora) REFERENCES Constructora(Secuencial)
);

-- Tabla: Contacto
CREATE TABLE IF NOT EXISTS Contacto (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecConstructora INT NOT NULL,
    Titulo VARCHAR(50) NULL,
    Nombres VARCHAR(255) NULL,
    Apellidos VARCHAR(255) NULL,
    Telefono VARCHAR(20) NULL,
    Correo VARCHAR(255) NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecConstructora) REFERENCES Constructora(Secuencial)
);

-- Tabla: Empresacorreo
CREATE TABLE IF NOT EXISTS Empresacorreo (
    SecEmpresa INT PRIMARY KEY,
    Email VARCHAR(255) NULL,
    Clave VARCHAR(255) NULL,
    Alias VARCHAR(255) NULL,
    Host VARCHAR(255) NULL,
    Puerto INT NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecEmpresa) REFERENCES Empresa(Secuencial)
);

-- Tabla: Empresastorage
CREATE TABLE IF NOT EXISTS Empresastorage (
    SecEmpresa INT PRIMARY KEY,
    Email VARCHAR(255) NULL,
    Clave VARCHAR(255) NULL,
    Ruta VARCHAR(2048) NULL,
    ApiKey VARCHAR(500) NULL,
    CarpetaUsuario VARCHAR(255) NULL,
    CarpetaProducto VARCHAR(255) NULL,
    CarpetaLogo VARCHAR(255) NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecEmpresa) REFERENCES Empresa(Secuencial)
);

-- Tabla: FormatoNumeroCliente
CREATE TABLE IF NOT EXISTS FormatoNumeroCliente (
    SecFormatoNumeroCliente INT PRIMARY KEY AUTO_INCREMENT,
    SecEmpresa INT NOT NULL,
    UsaFormato BIT(1) NOT NULL,
    Formato VARCHAR(100) NULL,
    NumeroInicio INT NOT NULL,
    LongitudNumero INT NOT NULL,
    FOREIGN KEY (SecEmpresa) REFERENCES Empresa(Secuencial)
);

-- Tabla: Impuesto
CREATE TABLE IF NOT EXISTS Impuesto (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Codigo VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(255) NOT NULL,
    Porcentaje DECIMAL(18, 2) NULL,
    ValorFijo DECIMAL(18, 2) NULL,
    CodigoSri VARCHAR(50) NULL,
    SecTipoImpuesto INT NOT NULL,
    Vigente BIT(1) NOT NULL,
    FechaCreacion DATETIME NOT NULL,
    FechaModificacion DATETIME NULL,
    FOREIGN KEY (SecTipoImpuesto) REFERENCES TipoImpuesto(Secuencial)
);

-- Tabla: PlantillaPreContrato
CREATE TABLE IF NOT EXISTS PlantillaPreContrato (
    SecPlantillaPreContrato INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(255) NULL,
    NumeracionInicial VARCHAR(50) NULL,
    FechaRegistro DATETIME NULL,
    EstaActivo INT NULL,
    SecTipoDocumento INT NOT NULL,
    FOREIGN KEY (SecTipoDocumento) REFERENCES TipoDocumento(SecTipoDocumento)
);

-- Tabla: RolPermiso
CREATE TABLE IF NOT EXISTS RolPermiso (
    SecRol INT NOT NULL,
    IdPermiso VARCHAR(100) NOT NULL,
    PRIMARY KEY (SecRol, IdPermiso),
    FOREIGN KEY (SecRol) REFERENCES Rol(Secuencial),
    FOREIGN KEY (IdPermiso) REFERENCES Permiso(IdPermiso)
);

-- Tabla: Usuario
CREATE TABLE IF NOT EXISTS Usuario (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Nombre VARCHAR(255) NULL,
    Correo VARCHAR(255) NULL,
    Telefono VARCHAR(20) NULL,
    SecRol INT NULL,
    UrlFoto VARCHAR(2048) NULL,
    NombreFoto VARCHAR(255) NULL,
    Clave VARCHAR(255) NULL,
    EsActivo BIT(1) NULL,
    FechaRegistro DATETIME NULL,
    FOREIGN KEY (SecRol) REFERENCES Rol(Secuencial)
);

-- =================================================================
-- Grupo 3: Tablas con dependencias del Grupo 2
-- =================================================================

-- Tabla: Parroquia
CREATE TABLE IF NOT EXISTS Parroquia (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecCanton INT NOT NULL,
    Codigo VARCHAR(10) NULL,
    Nombre VARCHAR(255) NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecCanton) REFERENCES Canton(Secuencial)
);

-- Tabla: PlantillaPreContratoParrafo
CREATE TABLE IF NOT EXISTS PlantillaPreContratoParrafo (
    SecPlantillaPreContratoParrafo INT PRIMARY KEY AUTO_INCREMENT,
    SecPlantillaPreContrato INT NOT NULL,
    Orden INT NOT NULL,
    Contenido TEXT NULL,
    EstaActivo BIT(1) NULL,
    FOREIGN KEY (SecPlantillaPreContrato) REFERENCES PlantillaPreContrato(SecPlantillaPreContrato)
);

-- Tabla: RolMenu
CREATE TABLE IF NOT EXISTS RolMenu (
    SecRol INT NOT NULL,
    SecMenu INT NOT NULL,
    VerMenu BIT(1) NOT NULL,
    Crear BIT(1) NOT NULL,
    Leer BIT(1) NOT NULL,
    Actualizar BIT(1) NOT NULL,
    Eliminar BIT(1) NOT NULL,
    PRIMARY KEY (SecRol, SecMenu),
    FOREIGN KEY (SecRol) REFERENCES Rol(Secuencial),
    FOREIGN KEY (SecMenu) REFERENCES Menu(Secuencial)
);

-- Tabla: Visita
CREATE TABLE IF NOT EXISTS Visita (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecUsuario INT NOT NULL,
    SecProvincia INT NULL,
    SecCanton INT NULL,
    SecParroquia INT NULL,
    IdEtapa INT NOT NULL,
    SecEmpresa INT NULL,
    Nombre VARCHAR(255) NULL,
    Direccion VARCHAR(500) NULL,
    FechaRegistro DATETIME NULL,
    FechaSiguienteVisita DATETIME NULL,
    GeoUbicacion VARCHAR(100) NULL,
    Detalle TEXT NULL,
    EstaActivo BIT(1) NULL,
    SecConstructora INT NULL,
    FOREIGN KEY (SecUsuario) REFERENCES Usuario(Secuencial),
    FOREIGN KEY (SecProvincia) REFERENCES Provincia(Secuencial),
    FOREIGN KEY (SecCanton) REFERENCES Canton(Secuencial),
    FOREIGN KEY (SecParroquia) REFERENCES Parroquia(Secuencial),
    FOREIGN KEY (IdEtapa) REFERENCES Etapa(Id),
    FOREIGN KEY (SecEmpresa) REFERENCES Empresa(Secuencial),
    FOREIGN KEY (SecConstructora) REFERENCES Constructora(Secuencial)
);

-- =================================================================
-- Grupo 4: Tablas con dependencias del Grupo 3
-- =================================================================

-- Tabla: ContactoVisita
CREATE TABLE IF NOT EXISTS ContactoVisita (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecContacto INT NOT NULL,
    SecVisita INT NOT NULL,
    EstaActivo BIT(1) NOT NULL,
    FOREIGN KEY (SecContacto) REFERENCES Contacto(Secuencial),
    FOREIGN KEY (SecVisita) REFERENCES Visita(Secuencial)
);

-- Tabla: Cotizacion
CREATE TABLE IF NOT EXISTS Cotizacion (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecVisita INT NOT NULL,
    EnviadoProveedor BIT(1) NOT NULL,
    EnviadoCliente BIT(1) NOT NULL,
    Confirmacion BIT(1) NOT NULL,
    Subtotal DECIMAL(18, 2) NOT NULL,
    ValorImpuestos DECIMAL(18, 2) NOT NULL,
    TotalConImpuestos DECIMAL(18, 2) NOT NULL,
    ValorIVA DECIMAL(18, 2) NOT NULL,
    ValorImportacion DECIMAL(18, 2) NOT NULL,
    EstaActivo BIT(1) NULL,
    FechaRegistro DATETIME NULL,
    FechaModificacion DATETIME NULL,
    SecUsuario INT NULL,
    SecCotizacionOriginal INT NULL,
    SecUsuarioModifica INT NULL,
    FOREIGN KEY (SecVisita) REFERENCES Visita(Secuencial),
    FOREIGN KEY (SecUsuario) REFERENCES Usuario(Secuencial),
    FOREIGN KEY (SecCotizacionOriginal) REFERENCES Cotizacion(Secuencial),
    FOREIGN KEY (SecUsuarioModifica) REFERENCES Usuario(Secuencial)
);

-- Tabla: Equiposvisita
CREATE TABLE IF NOT EXISTS Equiposvisita (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecVisita INT NOT NULL,
    TipoEquipo VARCHAR(255) NOT NULL,
    Sistema VARCHAR(255) NOT NULL,
    Marca VARCHAR(255) NOT NULL,
    Capacidad INT NOT NULL,
    Velocidad DECIMAL(18, 2) NULL,
    SalaMaquinas VARCHAR(255) NULL,
    SalaControl VARCHAR(255) NULL,
    NumeroPersonas INT NULL,
    NumeroParadas INT NULL,
    NombresParadas VARCHAR(1000) NULL,
    Embarque VARCHAR(255) NULL,
    TipoDucto VARCHAR(255) NULL,
    MedidasAfducto VARCHAR(255) NULL,
    TipoMotor VARCHAR(255) NULL,
    Foso INT NULL,
    Recorrido INT NULL,
    IngresosFrontales INT NULL,
    IngresosPosteriores INT NULL,
    SobreRecorrido INT NULL,
    DimencionEntrada INT NULL,
    AlturaEntrePisos INT NULL,
    MaterialPuertas VARCHAR(255) NULL,
    Energia VARCHAR(255) NULL,
    Cantidad INT NULL,
    EstaActivo BIT(1) NOT NULL,
    FOREIGN KEY (SecVisita) REFERENCES Visita(Secuencial)
);

-- =================================================================
-- Grupo 5: Tablas con dependencias del Grupo 4
-- =================================================================

-- Tabla: Contrato
CREATE TABLE IF NOT EXISTS Contrato (
    IdContrato INT PRIMARY KEY AUTO_INCREMENT,
    IdCotizacion INT NOT NULL,
    FechaFirma DATETIME NOT NULL,
    IdUsuarioCarga INT NOT NULL,
    NombreArchivo VARCHAR(255) NOT NULL,
    RutaArchivo VARCHAR(2048) NOT NULL,
    FechaCreacion DATETIME NULL,
    EsActivo BIT(1) NULL,
    SecCliente INT NOT NULL,
    FOREIGN KEY (IdCotizacion) REFERENCES Cotizacion(Secuencial),
    FOREIGN KEY (IdUsuarioCarga) REFERENCES Usuario(Secuencial),
    FOREIGN KEY (SecCliente) REFERENCES Cliente(SecCliente)
);

-- Tabla: Cotizaciondetalle
CREATE TABLE IF NOT EXISTS Cotizaciondetalle (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    SecEquipoVisita INT NULL,
    DetalleEquipo TEXT NULL,
    ValorCompra DECIMAL(18, 2) NOT NULL,
    MargenGanancia DECIMAL(18, 2) NOT NULL,
    Total DECIMAL(18, 2) NOT NULL,
    Cantidad INT NOT NULL,
    EstaActivo BIT(1) NULL,
    FechaRegistro DATETIME NULL,
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial),
    FOREIGN KEY (SecEquipoVisita) REFERENCES Equiposvisita(Secuencial)
);

-- Tabla: ImpuestoCotizacion
CREATE TABLE IF NOT EXISTS ImpuestoCotizacion (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    ImpuestoId INT NOT NULL,
    BaseImponible DECIMAL(18, 2) NOT NULL,
    ValorImpuesto DECIMAL(18, 2) NOT NULL,
    Exento BIT(1) NOT NULL,
    Observaciones TEXT NULL,
    FechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial),
    FOREIGN KEY (ImpuestoId) REFERENCES Impuesto(Id)
);

-- Tabla: PreContrato
CREATE TABLE IF NOT EXISTS PreContrato (
    SecPreContrato INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    SecPlantillaPreContrato INT NOT NULL,
    SecUsuarioCrea INT NOT NULL,
    Version INT NOT NULL,
    Estado VARCHAR(50) NOT NULL,
    EstaActivo BIT(1) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    Dias INT NOT NULL,
    TipoDias VARCHAR(50) NOT NULL,
    AniosGarantia INT NOT NULL,
    MesesGarantia INT NOT NULL,
    PeriodoMantenimiento VARCHAR(100) NOT NULL,
    PolizaGarantia VARCHAR(255) NOT NULL,
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial),
    FOREIGN KEY (SecPlantillaPreContrato) REFERENCES PlantillaPreContrato(SecPlantillaPreContrato),
    FOREIGN KEY (SecUsuarioCrea) REFERENCES Usuario(Secuencial)
);

-- Tabla: Seguimiento
CREATE TABLE IF NOT EXISTS Seguimiento (
    SecSeguimiento INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    Accion VARCHAR(255) NOT NULL,
    Detalle TEXT NOT NULL,
    FechaAccion DATETIME NOT NULL,
    AceptacionCliente BIT(1) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial)
);

-- =================================================================
-- Grupo 6: Tablas con dependencias del Grupo 5
-- =================================================================

-- Tabla: PlanDePago
CREATE TABLE IF NOT EXISTS PlanDePago (
    IdPlanDePago INT PRIMARY KEY AUTO_INCREMENT,
    IdContrato INT NOT NULL,
    SecFormaPago INT NOT NULL,
    ValorContrato DECIMAL(18, 2) NOT NULL,
    ValorAnticipo DECIMAL(18, 2) NOT NULL,
    FechaAnticipo DATETIME NULL,
    NumeroCuotas INT NOT NULL,
    FechaPrimeraCuota DATETIME NULL,
    EstaActivo BIT(1) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (IdContrato) REFERENCES Contrato(IdContrato),
    FOREIGN KEY (SecFormaPago) REFERENCES FormaPago(SecFormaPago)
);

-- Tabla: PreContratoCompromisoPago
CREATE TABLE IF NOT EXISTS PreContratoCompromisoPago (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecPreContrato INT NOT NULL,
    NumeroCuota INT NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL,
    FechaVencimiento DATETIME NOT NULL,
    Tipo VARCHAR(50) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (SecPreContrato) REFERENCES PreContrato(SecPreContrato)
);

-- Tabla: PreContratoParrafo
CREATE TABLE IF NOT EXISTS PreContratoParrafo (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecPreContrato INT NOT NULL,
    Contenido TEXT NOT NULL,
    Orden INT NOT NULL,
    FOREIGN KEY (SecPreContrato) REFERENCES PreContrato(SecPreContrato)
);

-- =================================================================
-- Grupo 7: Tablas con dependencias del Grupo 6
-- =================================================================

-- Tabla: Cuota
CREATE TABLE IF NOT EXISTS Cuota (
    IdCuota INT PRIMARY KEY AUTO_INCREMENT,
    IdPlanDePago INT NOT NULL,
    NumeroCuota INT NOT NULL,
    MontoEsperado DECIMAL(18, 2) NOT NULL,
    MontoPagado DECIMAL(18, 2) NULL,
    FechaVencimiento DATETIME NOT NULL,
    Estado VARCHAR(50) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (IdPlanDePago) REFERENCES PlanDePago(IdPlanDePago)
);

-- Tabla: Pago
CREATE TABLE IF NOT EXISTS Pago (
    IdPago INT PRIMARY KEY AUTO_INCREMENT,
    IdPlanDePago INT NOT NULL,
    Monto DECIMAL(18, 2) NOT NULL,
    FechaPago DATETIME NOT NULL,
    ComprobanteUrl VARCHAR(2048) NULL,
    ComprobanteNombre VARCHAR(255) NULL,
    RegistradoPorUsuarioId INT NOT NULL,
    EstaActivo BIT(1) NOT NULL,
    FechaRegistro DATETIME NOT NULL,
    FOREIGN KEY (IdPlanDePago) REFERENCES PlanDePago(IdPlanDePago),
    FOREIGN KEY (RegistradoPorUsuarioId) REFERENCES Usuario(Secuencial)
);

-- =================================================================
-- Fin del script
-- =================================================================

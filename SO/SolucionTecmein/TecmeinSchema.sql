CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId` varchar(150) CHARACTER SET utf8mb4 NOT NULL,
    `ProductVersion` varchar(32) CHARACTER SET utf8mb4 NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    ALTER DATABASE CHARACTER SET utf8mb3;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `AuditoriaEventos` (
        `Id` bigint NOT NULL AUTO_INCREMENT,
        `FechaHora` datetime(6) NOT NULL,
        `IdUsuario` int NULL,
        `NombreUsuario` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `TipoEvento` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Detalle` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `DireccionIp` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        CONSTRAINT `PK_AuditoriaEventos` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `catalogo` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `nombre` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `nombreArchivo` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `urlCatalogo` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `fechaRegistro` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `constructora` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `nombre` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `direccion` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `telefono` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `correo` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `atencion` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `administrador` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `telefonoAdministrador` varchar(10) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `correoAdministrador` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `diccionarioparametro` (
        `Secuencial` int NOT NULL AUTO_INCREMENT,
        `Parametro` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Descripcion` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`Secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `empresa` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `urlLogo` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombreLogo` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `identificacion` varchar(15) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombre` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `correo` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `direccion` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `telefono` varchar(10) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `codigoOperador` varchar(5) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `equiposvisita` (
        `secuencial` int NOT NULL,
        `secVisita` int NOT NULL,
        `tipoEquipo` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `sistema` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `marca` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `capacidad` int NOT NULL,
        `velocidad` decimal(18,2) NULL,
        `salaMaquinas` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `salaControl` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `numeroPersonas` int NULL,
        `numeroParadas` int NULL,
        `nombresParadas` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `embarque` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `tipoDucto` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `medidasAFDucto` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `tipoMotor` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `foso` int NULL,
        `recorrido` int NULL,
        `ingresosFrontales` int NULL,
        `ingresosPosteriores` int NULL,
        `SobreRecorrido` int NULL,
        `dimencionEntrada` int NULL,
        `alturaEntrePisos` int NULL,
        `materialPuertas` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `energia` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `cantidad` int NULL,
        `estaActivo` smallint NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `etapa` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Codigo` varchar(5) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Descripcion` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Orden` int NOT NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `formapago` (
        `SecFormaPago` int NOT NULL AUTO_INCREMENT,
        `Descripcion` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `EstaActivo` smallint NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecFormaPago`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `menu` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `descripcion` varchar(130) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `secMenuPadre` int NULL,
        `icono` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `controlador` varchar(130) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `paginaAccion` varchar(130) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `esActivo` smallint NULL,
        `fechaRegistro` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `FK_Menu_Menu` FOREIGN KEY (`secMenuPadre`) REFERENCES `menu` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `permiso` (
        `IdPermiso` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Descripcion` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`IdPermiso`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `PolizaGarantia` (
        `Secuencial` int NOT NULL AUTO_INCREMENT,
        `Descripcion` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        CONSTRAINT `PK_PolizaGarantia` PRIMARY KEY (`Secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `provincia` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `codigo` varchar(5) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombre` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `rol` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `descripcion` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `esActivo` smallint NULL,
        `fechaRegistro` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `tipodocumento` (
        `SecTipoDocumento` int NOT NULL AUTO_INCREMENT,
        `Codigo` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Descripcion` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `EstaActivo` tinyint(1) NULL,
        `FechaRegistro` datetime NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecTipoDocumento`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `tipoimpuesto` (
        `Secuencial` int NOT NULL AUTO_INCREMENT,
        `Nombre` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `EsIva` tinyint(1) NOT NULL,
        `EsImportacion` tinyint(1) NOT NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        `FechaCreacion` datetime NOT NULL,
        `FechaModificacion` datetime NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`Secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `cliente` (
        `SecCliente` int NOT NULL AUTO_INCREMENT,
        `SecConstructora` int NOT NULL,
        `NumeroCliente` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
        `EstaActivo` tinyint(1) NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecCliente`),
        CONSTRAINT `FK_Cliente_Constructora` FOREIGN KEY (`SecConstructora`) REFERENCES `constructora` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `contacto` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secConstructora` int NOT NULL,
        `titulo` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombre` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `apellidos` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `telefono` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `correo` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `FK_Contacto_Constructora` FOREIGN KEY (`secConstructora`) REFERENCES `constructora` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `empresacorreo` (
        `SecEmpresa` int NOT NULL,
        `email` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `clave` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `alias` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `host` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `puerto` int NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecEmpresa`),
        CONSTRAINT `Fk_Empresa_EmpCorreo` FOREIGN KEY (`SecEmpresa`) REFERENCES `empresa` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `empresastorage` (
        `secEmpresa` int NOT NULL,
        `email` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `clave` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `ruta` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `apiKey` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `carpetaUsuario` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `carpetaProducto` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `carpetaLogo` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secEmpresa`),
        CONSTRAINT `empresastorage_ibfk_1` FOREIGN KEY (`secEmpresa`) REFERENCES `empresa` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `formatonumerocliente` (
        `SecFormatoNumeroCliente` int NOT NULL AUTO_INCREMENT,
        `SecEmpresa` int NOT NULL,
        `UsaFormato` tinyint(1) NOT NULL,
        `Formato` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `NumeroInicio` int NOT NULL,
        `LongitudNumero` int NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecFormatoNumeroCliente`),
        CONSTRAINT `FK_FormatoNumeroCliente_Empresa` FOREIGN KEY (`SecEmpresa`) REFERENCES `empresa` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `canton` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secProvincia` int NOT NULL,
        `codigo` varchar(5) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombre` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `Fk_Provincia_Canton` FOREIGN KEY (`secProvincia`) REFERENCES `provincia` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `rolmenu` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secRol` int NULL,
        `secMenu` int NULL,
        `esActivo` smallint NULL,
        `fechaRegistro` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `FK_Menu_Rol` FOREIGN KEY (`secMenu`) REFERENCES `menu` (`secuencial`),
        CONSTRAINT `FK_Rol_Menu` FOREIGN KEY (`secRol`) REFERENCES `rol` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `rolpermiso` (
        `SecRol` int NOT NULL,
        `IdPermiso` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        CONSTRAINT `PK_rolpermiso` PRIMARY KEY (`SecRol`, `IdPermiso`),
        CONSTRAINT `FK_RolPermiso_Permiso` FOREIGN KEY (`IdPermiso`) REFERENCES `permiso` (`IdPermiso`),
        CONSTRAINT `FK_RolPermiso_Rol` FOREIGN KEY (`SecRol`) REFERENCES `rol` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `usuario` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `nombre` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `correo` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `telefono` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `secRol` int NULL,
        `urlFoto` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombreFoto` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `clave` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `esActivo` smallint NULL,
        `fechaRegistro` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `Fk_Rol_Usuario` FOREIGN KEY (`secRol`) REFERENCES `rol` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `plantillaprecontrato` (
        `SecPlantillaPreContrato` int NOT NULL AUTO_INCREMENT,
        `Nombre` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `NumeracionInicial` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `FechaRegistro` datetime NULL,
        `EstaActivo` int NULL,
        `SecTipoDocumento` int NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecPlantillaPreContrato`),
        CONSTRAINT `FK_PlantillaPreContrato_TipoDocumento` FOREIGN KEY (`SecTipoDocumento`) REFERENCES `tipodocumento` (`SecTipoDocumento`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `impuesto` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `Codigo` varchar(10) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Descripcion` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Porcentaje` decimal(5,2) NULL,
        `ValorFijo` decimal(18,2) NULL,
        `CodigoSri` varchar(5) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `SecTipoImpuesto` int NOT NULL,
        `Vigente` tinyint(1) NOT NULL,
        `FechaCreacion` datetime NOT NULL,
        `FechaModificacion` datetime NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_Impuesto_TipoImpuesto` FOREIGN KEY (`SecTipoImpuesto`) REFERENCES `tipoimpuesto` (`Secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `parroquia` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secCanton` int NOT NULL,
        `codigo` varchar(5) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `nombre` varchar(150) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `Fk_Canton_Parroquia` FOREIGN KEY (`secCanton`) REFERENCES `canton` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `plantillaprecontratoparrafo` (
        `SecPlantillaPreContratoParrafo` int NOT NULL AUTO_INCREMENT,
        `SecPlantillaPreContrato` int NOT NULL,
        `Orden` int NOT NULL,
        `Contenido` TEXT CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `EstaActivo` tinyint(1) NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecPlantillaPreContratoParrafo`),
        CONSTRAINT `FK_plantillaprecontratoparrafo_plantillaprecontrato` FOREIGN KEY (`SecPlantillaPreContrato`) REFERENCES `plantillaprecontrato` (`SecPlantillaPreContrato`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `visita` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secUsuario` int NOT NULL,
        `secProvincia` int NULL,
        `secCanton` int NULL,
        `secParroquia` int NULL,
        `IdEtapa` int NOT NULL,
        `SecEmpresa` int NULL,
        `nombre` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `direccion` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `fechaRegistro` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        `fechaSiguienteVisita` datetime NULL DEFAULT CURRENT_TIMESTAMP,
        `geoUbicacion` varchar(250) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `detalle` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `estaActivo` smallint NULL,
        `SecConstructora` int NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `FK_Visita_Empresa` FOREIGN KEY (`SecEmpresa`) REFERENCES `empresa` (`secuencial`),
        CONSTRAINT `FK_Visita_Etapa` FOREIGN KEY (`IdEtapa`) REFERENCES `etapa` (`Id`),
        CONSTRAINT `FK_visita_constructora_SecConstructora` FOREIGN KEY (`SecConstructora`) REFERENCES `constructora` (`secuencial`),
        CONSTRAINT `Fk_Visita_Canton` FOREIGN KEY (`secCanton`) REFERENCES `canton` (`secuencial`),
        CONSTRAINT `Fk_Visita_Parroquia` FOREIGN KEY (`secParroquia`) REFERENCES `parroquia` (`secuencial`),
        CONSTRAINT `Fk_Visita_Provincia` FOREIGN KEY (`secProvincia`) REFERENCES `provincia` (`secuencial`),
        CONSTRAINT `Fk_Visita_Usuario` FOREIGN KEY (`secUsuario`) REFERENCES `usuario` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `contactovisita` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secContacto` int NOT NULL,
        `secVisita` int NOT NULL,
        `estaActivo` smallint NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `Fk_Contacto_ContactoVisita` FOREIGN KEY (`secContacto`) REFERENCES `contacto` (`secuencial`) ON DELETE CASCADE,
        CONSTRAINT `Fk_Visita_ContactoVisita` FOREIGN KEY (`secVisita`) REFERENCES `visita` (`secuencial`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `cotizacion` (
        `secuencial` int NOT NULL AUTO_INCREMENT,
        `secVisita` int NOT NULL,
        `enviadoProveedor` tinyint(1) NOT NULL,
        `enviadoCliente` tinyint(1) NOT NULL,
        `confirmacion` tinyint(1) NOT NULL,
        `subtotal` decimal(18,2) NOT NULL,
        `valorImpuestos` decimal(18,2) NOT NULL,
        `totalConImpuestos` decimal(18,2) NOT NULL,
        `valorIVA` decimal(18,2) NOT NULL,
        `valorImportacion` decimal(18,2) NOT NULL,
        `estaActivo` smallint NULL,
        `fechaRegistro` datetime NULL,
        `fechaModificacion` datetime NULL,
        `secUsuario` int NULL,
        `secCotizacionOriginal` int NULL,
        `secUsuarioModifica` int NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`secuencial`),
        CONSTRAINT `FK_Cotizacion_CotizacionOriginal` FOREIGN KEY (`secCotizacionOriginal`) REFERENCES `cotizacion` (`secuencial`),
        CONSTRAINT `FK_Cotizacion_Usuario` FOREIGN KEY (`secUsuario`) REFERENCES `usuario` (`secuencial`),
        CONSTRAINT `FK_Cotizacion_UsuarioModifica` FOREIGN KEY (`secUsuarioModifica`) REFERENCES `usuario` (`secuencial`),
        CONSTRAINT `FK_Cotizacion_Visita` FOREIGN KEY (`secVisita`) REFERENCES `visita` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `contrato` (
        `IdContrato` int NOT NULL AUTO_INCREMENT,
        `IdCotizacion` int NOT NULL,
        `FechaFirma` datetime NOT NULL,
        `IdUsuarioCarga` int NOT NULL,
        `NombreArchivo` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `RutaArchivo` varchar(1024) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `FechaCreacion` datetime NULL,
        `EsActivo` tinyint(1) NULL,
        `SecCliente` int NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`IdContrato`),
        CONSTRAINT `FK_Contrato_Cliente` FOREIGN KEY (`SecCliente`) REFERENCES `cliente` (`SecCliente`),
        CONSTRAINT `FK_Contrato_Cotizacion` FOREIGN KEY (`IdCotizacion`) REFERENCES `cotizacion` (`secuencial`),
        CONSTRAINT `FK_Contrato_Usuario` FOREIGN KEY (`IdUsuarioCarga`) REFERENCES `usuario` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `cotizaciondetalle` (
        `Secuencial` int NOT NULL AUTO_INCREMENT,
        `SecCotizacion` int NOT NULL,
        `SecEquipoVisita` int NULL,
        `DetalleEquipo` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `ValorCompra` decimal(65,30) NOT NULL,
        `MargenGanancia` decimal(65,30) NOT NULL,
        `Total` decimal(65,30) NOT NULL,
        `Cantidad` int NOT NULL,
        `EstaActivo` smallint NULL,
        `FechaRegistro` datetime(6) NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`Secuencial`),
        CONSTRAINT `FK_Cotizaciondetalle_Cotizacion` FOREIGN KEY (`SecCotizacion`) REFERENCES `cotizacion` (`secuencial`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `impuestocotizacion` (
        `Id` int NOT NULL AUTO_INCREMENT,
        `SecCotizacion` int NOT NULL,
        `ImpuestoId` int NOT NULL,
        `BaseImponible` decimal(18,2) NOT NULL,
        `ValorImpuesto` decimal(18,2) NOT NULL,
        `Exento` tinyint(1) NOT NULL,
        `Observaciones` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `FechaRegistro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`Id`),
        CONSTRAINT `FK_ImpuestoCotizacion_Cotizacion` FOREIGN KEY (`SecCotizacion`) REFERENCES `cotizacion` (`secuencial`) ON DELETE CASCADE,
        CONSTRAINT `FK_ImpuestoCotizacion_Impuesto` FOREIGN KEY (`ImpuestoId`) REFERENCES `impuesto` (`Id`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `precontrato` (
        `SecPreContrato` int NOT NULL AUTO_INCREMENT,
        `SecCotizacion` int NOT NULL,
        `SecPlantillaPreContrato` int NOT NULL,
        `SecUsuarioCrea` int NOT NULL,
        `SecFormaPago` int NULL,
        `Version` int NOT NULL,
        `Estado` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        `FechaRegistro` datetime NOT NULL,
        `Dias` int NOT NULL,
        `TipoDias` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `ValorContrato` decimal(18,2) NOT NULL,
        `AniosGarantia` int NOT NULL,
        `MesesGarantia` int NOT NULL,
        `PeriodoMantenimiento` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `PolizaGarantia` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `ValorAnticipo` decimal(18,2) NOT NULL,
        `FechaAnticipo` datetime NULL,
        `NumeroCuotas` int NOT NULL,
        `FechaPrimeraCuota` datetime NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecPreContrato`),
        CONSTRAINT `FK_PreContrato_Cotizacion` FOREIGN KEY (`SecCotizacion`) REFERENCES `cotizacion` (`secuencial`),
        CONSTRAINT `FK_PreContrato_FormaPago` FOREIGN KEY (`SecFormaPago`) REFERENCES `formapago` (`SecFormaPago`),
        CONSTRAINT `FK_PreContrato_PlantillaPreContrato` FOREIGN KEY (`SecPlantillaPreContrato`) REFERENCES `plantillaprecontrato` (`SecPlantillaPreContrato`),
        CONSTRAINT `FK_PreContrato_Usuario` FOREIGN KEY (`SecUsuarioCrea`) REFERENCES `usuario` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `seguimiento` (
        `SecSeguimiento` int NOT NULL AUTO_INCREMENT,
        `SecCotizacion` int NOT NULL,
        `Accion` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Detalle` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `FechaAccion` datetime NOT NULL,
        `AceptacionCliente` tinyint(1) NOT NULL,
        `FechaRegistro` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`SecSeguimiento`),
        CONSTRAINT `FK_Seguimiento_Cotizacion` FOREIGN KEY (`SecCotizacion`) REFERENCES `cotizacion` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `plandepago` (
        `IdPlanDePago` int NOT NULL AUTO_INCREMENT,
        `IdContrato` int NOT NULL,
        `SecFormaPago` int NOT NULL,
        `ValorContrato` decimal(18,2) NOT NULL,
        `ValorAnticipo` decimal(18,2) NOT NULL,
        `FechaAnticipo` datetime NULL,
        `NumeroCuotas` int NOT NULL,
        `FechaPrimeraCuota` datetime NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        `FechaRegistro` datetime NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`IdPlanDePago`),
        CONSTRAINT `FK_PlanDePago_FormaPago` FOREIGN KEY (`SecFormaPago`) REFERENCES `formapago` (`SecFormaPago`),
        CONSTRAINT `FK_plandepago_contrato_IdContrato` FOREIGN KEY (`IdContrato`) REFERENCES `contrato` (`IdContrato`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `precontratoparrafo` (
        `Secuencial` int NOT NULL AUTO_INCREMENT,
        `SecPreContrato` int NOT NULL,
        `Contenido` TEXT CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `Orden` int NOT NULL,
        CONSTRAINT `PK_precontratoparrafo` PRIMARY KEY (`Secuencial`),
        CONSTRAINT `FK_precontratoparrafo_precontrato_SecPreContrato` FOREIGN KEY (`SecPreContrato`) REFERENCES `precontrato` (`SecPreContrato`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `cuota` (
        `IdCuota` int NOT NULL AUTO_INCREMENT,
        `IdPlanDePago` int NOT NULL,
        `NumeroCuota` int NOT NULL,
        `MontoEsperado` decimal(18,2) NOT NULL,
        `FechaVencimiento` datetime NOT NULL,
        `Estado` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `FechaRegistro` datetime NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`IdCuota`),
        CONSTRAINT `FK_Cuota_PlanDePago` FOREIGN KEY (`IdPlanDePago`) REFERENCES `plandepago` (`IdPlanDePago`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE TABLE `pago` (
        `IdPago` int NOT NULL AUTO_INCREMENT,
        `IdPlanDePago` int NOT NULL,
        `Monto` decimal(18,2) NOT NULL,
        `FechaPago` datetime NOT NULL,
        `ComprobanteUrl` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `ComprobanteNombre` varchar(255) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NULL,
        `RegistradoPorUsuarioId` int NOT NULL,
        `EstaActivo` tinyint(1) NOT NULL,
        `FechaRegistro` datetime NOT NULL,
        CONSTRAINT `PRIMARY` PRIMARY KEY (`IdPago`),
        CONSTRAINT `FK_Pago_PlanDePago` FOREIGN KEY (`IdPlanDePago`) REFERENCES `plandepago` (`IdPlanDePago`),
        CONSTRAINT `FK_Pago_Usuario` FOREIGN KEY (`RegistradoPorUsuarioId`) REFERENCES `usuario` (`secuencial`)
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Provincia_Canton` ON `canton` (`secProvincia`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE UNIQUE INDEX `FK_Cliente_Constructora_idx` ON `cliente` (`SecConstructora`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Contacto_Constructora_idx` ON `contacto` (`secConstructora`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Contacto_ContactoVisita` ON `contactovisita` (`secContacto`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Contacto_Visita` ON `contactovisita` (`secVisita`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Contrato_Cotizacion_idx` ON `contrato` (`IdCotizacion`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Contrato_Usuario_idx` ON `contrato` (`IdUsuarioCarga`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_contrato_SecCliente` ON `contrato` (`SecCliente`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Cotizacion_Visita_idx` ON `cotizacion` (`secVisita`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_cotizacion_secCotizacionOriginal` ON `cotizacion` (`secCotizacionOriginal`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_cotizacion_secUsuario` ON `cotizacion` (`secUsuario`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_cotizacion_secUsuarioModifica` ON `cotizacion` (`secUsuarioModifica`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_cotizaciondetalle_SecCotizacion` ON `cotizaciondetalle` (`SecCotizacion`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Cuota_PlanDePago_idx` ON `cuota` (`IdPlanDePago`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE UNIQUE INDEX `UQ_DiccionarioParametro_Parametro` ON `diccionarioparametro` (`Parametro`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE UNIQUE INDEX `secuencial_UNIQUE` ON `equiposvisita` (`secuencial`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_FormatoNumeroCliente_Empresa_idx` ON `formatonumerocliente` (`SecEmpresa`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_impuesto_SecTipoImpuesto` ON `impuesto` (`SecTipoImpuesto`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_impuestocotizacion_ImpuestoId` ON `impuestocotizacion` (`ImpuestoId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_impuestocotizacion_SecCotizacion` ON `impuestocotizacion` (`SecCotizacion`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Menu_Menu_idx` ON `menu` (`secMenuPadre`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Pago_PlanDePago_idx` ON `pago` (`IdPlanDePago`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Pago_Usuario_idx` ON `pago` (`RegistradoPorUsuarioId`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Parroquia_Canton_idx` ON `parroquia` (`secCanton`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE UNIQUE INDEX `FK_PlanDePago_Contrato_idx` ON `plandepago` (`IdContrato`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_PlanDePago_FormaPago_idx` ON `plandepago` (`SecFormaPago`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_PlantillaPreContrato_TipoDocumento_idx` ON `plantillaprecontrato` (`SecTipoDocumento`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_plantillaprecontratoparrafo_plantilla` ON `plantillaprecontratoparrafo` (`SecPlantillaPreContrato`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_PreContrato_Cotizacion_idx` ON `precontrato` (`SecCotizacion`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_PreContrato_FormaPago_idx` ON `precontrato` (`SecFormaPago`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_PreContrato_Plantilla_idx` ON `precontrato` (`SecPlantillaPreContrato`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_PreContrato_Usuario_idx` ON `precontrato` (`SecUsuarioCrea`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_precontratoparrafo_SecPreContrato` ON `precontratoparrafo` (`SecPreContrato`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Menu_Rol_idx` ON `rolmenu` (`secMenu`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Rol_Menu_idx` ON `rolmenu` (`secRol`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_rolpermiso_IdPermiso` ON `rolpermiso` (`IdPermiso`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `FK_Seguimiento_Cotizacion_idx` ON `seguimiento` (`SecCotizacion`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Rol_Usuario_idx` ON `usuario` (`secRol`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Visita_Canton` ON `visita` (`secCanton`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Visita_Parroquia` ON `visita` (`secParroquia`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Visita_Provincia` ON `visita` (`secProvincia`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `Fk_Visita_Usuario_idx` ON `visita` (`secUsuario`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_visita_IdEtapa` ON `visita` (`IdEtapa`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_visita_SecConstructora` ON `visita` (`SecConstructora`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    CREATE INDEX `IX_visita_SecEmpresa` ON `visita` (`SecEmpresa`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251004190930_AddAuditoriaEventosTable') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251004190930_AddAuditoriaEventosTable', '8.0.6');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;

START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP FOREIGN KEY `FK_PreContrato_FormaPago`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP INDEX `FK_PreContrato_FormaPago_idx`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP COLUMN `FechaAnticipo`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP COLUMN `FechaPrimeraCuota`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP COLUMN `NumeroCuotas`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP COLUMN `SecFormaPago`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP COLUMN `ValorAnticipo`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `precontrato` DROP COLUMN `ValorContrato`;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    ALTER TABLE `cuota` ADD `MontoPagado` decimal(65,30) NULL;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251011004847_AddMontoPagadoToCuota') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251011004847_AddMontoPagadoToCuota', '8.0.6');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;


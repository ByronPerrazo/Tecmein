-- ==================================================================
-- Script para la creación de las tablas precontrato y precontratoparrafo
-- Actualizado para reflejar la estructura de la base de datos existente.
-- ==================================================================

-- Tabla principal para los Pre-Contratos
CREATE TABLE `precontrato` (
  `SecPreContrato` int NOT NULL AUTO_INCREMENT,
  `SecCotizacion` int NOT NULL,
  `SecPlantillaPreContrato` int NOT NULL,
  `SecUsuarioCrea` int NOT NULL,
  `SecFormaPago` int DEFAULT NULL,
  `Version` int NOT NULL DEFAULT '1',
  `Estado` varchar(50) NOT NULL DEFAULT 'Borrador',
  `Dias` int NOT NULL,
  `TipoDias` varchar(50) NOT NULL,
  `ValorContrato` decimal(18,2) NOT NULL,
  `AniosGarantia` int NOT NULL,
  `MesesGarantia` int NOT NULL,
  `PeriodoMantenimiento` varchar(100) NOT NULL,
  `PolizaGarantia` varchar(100) NOT NULL,
  `ValorAnticipo` decimal(18,2) NOT NULL,
  `FechaAnticipo` datetime NOT NULL,
  `NumeroCuotas` int NOT NULL,
  `FechaPrimeraCuota` datetime NOT NULL,
  `EstaActivo` smallint NOT NULL DEFAULT '1',
  `FechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`SecPreContrato`),
  KEY `SecCotizacion` (`SecCotizacion`),
  KEY `FK_PreContrato_PlantillaPreContrato` (`SecPlantillaPreContrato`),
  KEY `FK_PreContrato_Usuario` (`SecUsuarioCrea`),
  KEY `FK_PreContrato_FormaPago` (`SecFormaPago`),
  CONSTRAINT `FK_PreContrato_FormaPago` FOREIGN KEY (`SecFormaPago`) REFERENCES `formapago` (`SecFormaPago`),
  CONSTRAINT `FK_PreContrato_PlantillaPreContrato` FOREIGN KEY (`SecPlantillaPreContrato`) REFERENCES `plantillaprecontrato` (`SecPlantillaPreContrato`),
  CONSTRAINT `FK_PreContrato_Usuario` FOREIGN KEY (`SecUsuarioCrea`) REFERENCES `usuario` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb3;

-- Tabla para los párrafos específicos de cada Pre-Contrato (copia editable)
CREATE TABLE `precontratoparrafo` (
    `Secuencial` INT PRIMARY KEY AUTO_INCREMENT,
    `SecPreContrato` INT NOT NULL,
    `Contenido` TEXT,
    `Orden` INT NOT NULL,
    CONSTRAINT `fk_precontratoparrafo_precontrato` FOREIGN KEY (`SecPreContrato`) REFERENCES `precontrato`(`SecPreContrato`) ON DELETE CASCADE
);
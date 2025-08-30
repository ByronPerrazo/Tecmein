-- Creación de tablas para el Módulo de Plantillas de Pre-Contrato (Versión Final Alineada con Código Existente)

-- 1. Tabla para las plantillas que agrupan párrafos
CREATE TABLE `plantillaprecontrato` (
  `SecPlantillaPreContrato` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(150) NULL,
  `NumeracionInicial` VARCHAR(50) NULL,
  `FechaRegistro` DATETIME NULL,
  `EstaActivo` INT NULL,
  PRIMARY KEY (`SecPlantillaPreContrato`)
);

-- 2. Tabla para los párrafos que componen una plantilla
CREATE TABLE `plantillaprecontratoparrafo` (
  `SecPlantillaPreContratoParrafo` INT NOT NULL AUTO_INCREMENT,
  `SecPlantillaPreContrato` INT NULL,
  `Orden` INT NULL,
  `Contenido` TEXT NULL,
  `EstaActivo` INT NULL,
  PRIMARY KEY (`SecPlantillaPreContratoParrafo`),
  INDEX `IX_plantillaprecontratoparrafo_plantilla` (`SecPlantillaPreContrato` ASC),
  CONSTRAINT `FK_plantillaprecontratoparrafo_plantillaprecontrato`
    FOREIGN KEY (`SecPlantillaPreContrato`)
    REFERENCES `plantillaprecontrato` (`SecPlantillaPreContrato`)
    ON DELETE CASCADE
    ON UPDATE NO ACTION
);

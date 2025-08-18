-- Script de modificaciones para la tabla 'impuesto'
-- ADVERTENCIA: La eliminación de la columna 'TipoImpuesto' resultará en la pérdida de datos existentes en esa columna.
-- Asegúrese de hacer una copia de seguridad de su base de datos antes de ejecutar este script.

ALTER TABLE `impuesto`
    DROP COLUMN `TipoImpuesto`,
    ADD COLUMN `SecTipoImpuesto` INT NOT NULL AFTER `CodigoSri`,
    MODIFY COLUMN `Id` INT NOT NULL AUTO_INCREMENT,
    MODIFY COLUMN `Codigo` VARCHAR(10) NOT NULL,
    MODIFY COLUMN `Descripcion` VARCHAR(100) NOT NULL,
    MODIFY COLUMN `Porcentaje` DECIMAL(5,2) DEFAULT NULL,
    MODIFY COLUMN `ValorFijo` DECIMAL(18,2) DEFAULT NULL,
    MODIFY COLUMN `CodigoSri` VARCHAR(5) DEFAULT NULL,
    MODIFY COLUMN `Vigente` TINYINT(1) NOT NULL DEFAULT '1',
    ADD COLUMN `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ADD COLUMN `FechaModificacion` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP;

-- Añadir la clave foránea a la tabla 'impuesto'
ALTER TABLE `impuesto`
    ADD CONSTRAINT `FK_Impuesto_TipoImpuesto` FOREIGN KEY (`SecTipoImpuesto`) REFERENCES `tipoimpuesto` (`Secuencial`);

-- Script para la creación de la nueva tabla 'tipoimpuesto'
CREATE TABLE `tipoimpuesto` (
  `Secuencial` INT NOT NULL AUTO_INCREMENT,
  `Nombre` VARCHAR(100) NOT NULL,
  `EsIva` TINYINT(1) NOT NULL DEFAULT '0',
  `EsImportacion` TINYINT(1) NOT NULL DEFAULT '0',
  `EstaActivo` TINYINT(1) NOT NULL DEFAULT '1',
  `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaModificacion` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
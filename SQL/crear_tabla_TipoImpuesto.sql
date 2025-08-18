CREATE TABLE `tipoimpuesto` (
  `Secuencial` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) NOT NULL,
  `EsIva` tinyint(1) NOT NULL DEFAULT '0',
  `EsImportacion` tinyint(1) NOT NULL DEFAULT '0',
  `EstaActivo` tinyint(1) NOT NULL DEFAULT '1',
  `FechaCreacion` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `FechaModificacion` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
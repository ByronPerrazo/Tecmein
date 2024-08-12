CREATE TABLE `catalogo` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `Nombre` int NOT NULL,
  `urlCatalogo` varchar(500) DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;


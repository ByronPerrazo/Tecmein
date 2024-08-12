
CREATE TABLE `tipoproducto` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) DEFAULT NULL,
  `descripcion` varchar(150) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime default CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;

CREATE TABLE `producto` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secTipoProducto` int,
  `nombre` varchar(50) DEFAULT NULL,
  `marca` varchar(50) DEFAULT NULL,
  `sistema` varchar(50) DEFAULT NULL,
  `capacidad` varchar(50) DEFAULT NULL,
  `motor` varchar(50) DEFAULT NULL,
  `stock` int NOT NULL DEFAULT 0,
  `urlImagen` varchar(500) DEFAULT NULL,
  `nombreImagen` varchar(100) DEFAULT NULL,
  `precio` decimal(10,2),
  `descripcion` varchar(150) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime default CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`),
  KEY `Fk_TipoProducto_Producto_idx` (`secTipoProducto`),
  CONSTRAINT `Fk_TipoProducto_Producto` FOREIGN KEY (`secTipoProducto`) REFERENCES `tipoproducto` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;

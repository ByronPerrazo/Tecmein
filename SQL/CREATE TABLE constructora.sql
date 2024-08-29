CREATE TABLE `constructora` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `direccion` varchar(250) DEFAULT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `correo` varchar(150) DEFAULT NULL,
  `atencion` varchar(150) DEFAULT NULL,
  `administrador` varchar(150) DEFAULT NULL,
  `telefonoAdministrador` varchar(10) DEFAULT NULL,
  `correoAdministrador` varchar(150) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb3;

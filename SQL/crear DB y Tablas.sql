

CREATE TABLE `empresa` (
    `secuencial` INT NOT NULL AUTO_INCREMENT,
    `urlLogo` VARCHAR(500) DEFAULT NULL,
    `nombreLogo` VARCHAR(100) DEFAULT NULL,
    `numeroDocumento` VARCHAR(50) DEFAULT NULL,
    `nombre` VARCHAR(50) DEFAULT NULL,
    `correo` VARCHAR(150) DEFAULT NULL,
    `direccion` VARCHAR(250) DEFAULT NULL,
    `telefono` VARCHAR(10) DEFAULT NULL,
    `porcentajeImpuesto` DECIMAL(10 , 2 ) DEFAULT NULL,
    `simboloMoneda` VARCHAR(5) DEFAULT NULL,
    `estaActivo` SMALLINT DEFAULT NULL,
    PRIMARY KEY (`secuencial`)
)  ENGINE=INNODB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `empresacorreo` (
  `SecEmpresa` int NOT NULL,
  `email` varchar(150) DEFAULT NULL,
  `clave` varchar(255) DEFAULT NULL,
  `alias` varchar(50) DEFAULT NULL,
  `host` varchar(50) DEFAULT NULL,
  `puerto` int DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`SecEmpresa`),
  CONSTRAINT `Fk_Empresa_EmpCorreo` FOREIGN KEY (`SecEmpresa`) REFERENCES `empresa` (`secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `empresastorage` (
  `secEmpresa` int NOT NULL,
  `email` varchar(150) DEFAULT NULL,
  `clave` varchar(255) DEFAULT NULL,
  `ruta` varchar(255) DEFAULT NULL,
  `apiKey` varchar(255) DEFAULT NULL,
  `carpetaUsuario` varchar(255) DEFAULT NULL,
  `carpetaProducto` varchar(255) DEFAULT NULL,
  `carpetaLogo` varchar(255) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secEmpresa`),
  CONSTRAINT `empresastorage_ibfk_1` FOREIGN KEY (`secEmpresa`) REFERENCES `empresa` (`secuencial`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `rol` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(150) DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `menu` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(130) DEFAULT NULL,
  `secMenuPadre` int DEFAULT NULL,
  `icono` varchar(30) DEFAULT NULL,
  `controlador` varchar(130) DEFAULT NULL,
  `paginaAccion` varchar(130) DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`),
  KEY `FK_Menu_Menu_idx` (`secMenuPadre`),
  CONSTRAINT `FK_Menu_Menu` FOREIGN KEY (`secMenuPadre`) REFERENCES `menu` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `usuario` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) DEFAULT NULL,
  `correo` varchar(150) DEFAULT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `secRol` int DEFAULT NULL,
  `urlFoto` varchar(500) DEFAULT NULL,
  `nombreFoto` varchar(100) DEFAULT NULL,
  `clave` varchar(255) DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`),
  KEY `Fk_Rol_Usuario_idx` (`secRol`),
  CONSTRAINT `Fk_Rol_Usuario` FOREIGN KEY (`secRol`) REFERENCES `rol` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `rolmenu` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secRol` int DEFAULT NULL,
  `secMenu` int DEFAULT NULL,
  `esActivo` smallint DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`secuencial`),
  KEY `FK_Rol_Menu_idx` (`secRol`),
  KEY `FK_Menu_Rol_idx` (`secMenu`),
  CONSTRAINT `FK_Menu_Rol` FOREIGN KEY (`secMenu`) REFERENCES `menu` (`secuencial`),
  CONSTRAINT `FK_Rol_Menu` FOREIGN KEY (`secRol`) REFERENCES `rol` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `provincia` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(5) DEFAULT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `canton` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secProvincia` int NOT NULL,
  `codigo` varchar(5) DEFAULT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`),
  CONSTRAINT `Fk_Provincia_Canton` FOREIGN KEY (`SecProvincia`) REFERENCES `provincia` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `parroquia` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secCanton` int NOT NULL,
  `codigo` varchar(5) DEFAULT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`),
  KEY `FK_Parroquia_Canton_idx` (`secCanton`),
  CONSTRAINT `Fk_Canton_Parroquia` FOREIGN KEY (`SecCanton`) REFERENCES `canton` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;


CREATE TABLE `visita` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `secProvincia` int DEFAULT NULL,
  `secCanton` int DEFAULT NULL,
  `secParroquia` int DEFAULT NULL,
  `direccion` varchar(250) DEFAULT NULL,
  `fechaRegistro` datetime DEFAULT CURRENT_TIMESTAMP,
  `geoUbicacion` varchar(250) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`),
  CONSTRAINT `Fk_Visita_Provincia` FOREIGN KEY (`SecProvincia`) REFERENCES `Provincia` (`secuencial`),
  CONSTRAINT `Fk_Visita_Canton` FOREIGN KEY (`SecCanton`) REFERENCES `canton` (`secuencial`),
  CONSTRAINT `Fk_Visita_Parroquia` FOREIGN KEY (`SecParroquia`) REFERENCES `parroquia` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `detallevisita` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secVisita` int DEFAULT NULL,
  `sigVisita` smallint DEFAULT NULL,
  `fechaSigVisita` datetime DEFAULT CURRENT_TIMESTAMP,
  `detalle` varchar(300) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`),
  CONSTRAINT `Fk_Visita_DetalleVisita` FOREIGN KEY (`secVisita`) REFERENCES `visita` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `contacto` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) DEFAULT NULL,
  `telefono` varchar(50) DEFAULT NULL,
  `correo` varchar(50) DEFAULT NULL,
  `titulo` varchar(50) DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

CREATE TABLE `contactovista` (
  `secuencial` int NOT NULL AUTO_INCREMENT,
  `secContacto` int DEFAULT NULL,
  `secVisita` int DEFAULT NULL,
  `estaActivo` smallint DEFAULT NULL,
  PRIMARY KEY (`secuencial`),
  CONSTRAINT `Fk_Contacto_ContactoVisita` FOREIGN KEY (`secContacto`) REFERENCES `contacto` (`secuencial`),
  CONSTRAINT `Fk_Contacto_Visita` FOREIGN KEY (`secVisita`) REFERENCES `visita` (`secuencial`)
) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8mb4;

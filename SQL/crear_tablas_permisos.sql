-- (Versión 4 - MySQL - Nombres de tabla corregidos)

CREATE TABLE IF NOT EXISTS `permiso` (
    `IdPermiso` VARCHAR(100) PRIMARY KEY,
    `Descripcion` VARCHAR(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS `rolpermiso` (
    `SecRol` INT NOT NULL,
    `IdPermiso` VARCHAR(100) NOT NULL,
    PRIMARY KEY (`SecRol`, `IdPermiso`),
    CONSTRAINT `fk_rolpermiso_rol` FOREIGN KEY (`SecRol`) REFERENCES `rol`(`secuencial`),
    CONSTRAINT `fk_rolpermiso_permiso` FOREIGN KEY (`IdPermiso`) REFERENCES `permiso`(`IdPermiso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

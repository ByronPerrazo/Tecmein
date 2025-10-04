CREATE TABLE `AuditoriaEventos` (
    `Id` BIGINT NOT NULL AUTO_INCREMENT,
    `FechaHora` DATETIME NOT NULL,
    `IdUsuario` INT NULL,
    `NombreUsuario` VARCHAR(255) NULL,
    `TipoEvento` VARCHAR(100) NOT NULL,
    `Detalle` TEXT NOT NULL,
    `DireccionIp` VARCHAR(50) NULL,
    PRIMARY KEY (`Id`)
);

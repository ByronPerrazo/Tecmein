START TRANSACTION;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251017163432_AddPreContratoCompromisoPago') THEN

    CREATE TABLE `PreContratoCompromisoPagos` (
        `Secuencial` int NOT NULL AUTO_INCREMENT,
        `SecPreContrato` int NOT NULL,
        `NumeroCuota` int NOT NULL,
        `Monto` decimal(18,2) NOT NULL,
        `FechaVencimiento` datetime(6) NOT NULL,
        `Tipo` longtext CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci NOT NULL,
        `FechaRegistro` datetime(6) NOT NULL,
        CONSTRAINT `PK_PreContratoCompromisoPagos` PRIMARY KEY (`Secuencial`),
        CONSTRAINT `FK_PreContratoCompromisoPagos_precontrato_SecPreContrato` FOREIGN KEY (`SecPreContrato`) REFERENCES `precontrato` (`SecPreContrato`) ON DELETE CASCADE
    ) CHARACTER SET=utf8mb3 COLLATE=utf8mb3_general_ci;

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251017163432_AddPreContratoCompromisoPago') THEN

    CREATE INDEX `IX_PreContratoCompromisoPagos_SecPreContrato` ON `PreContratoCompromisoPagos` (`SecPreContrato`);

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

DROP PROCEDURE IF EXISTS MigrationsScript;
DELIMITER //
CREATE PROCEDURE MigrationsScript()
BEGIN
    IF NOT EXISTS(SELECT 1 FROM `__EFMigrationsHistory` WHERE `MigrationId` = '20251017163432_AddPreContratoCompromisoPago') THEN

    INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
    VALUES ('20251017163432_AddPreContratoCompromisoPago', '8.0.6');

    END IF;
END //
DELIMITER ;
CALL MigrationsScript();
DROP PROCEDURE MigrationsScript;

COMMIT;


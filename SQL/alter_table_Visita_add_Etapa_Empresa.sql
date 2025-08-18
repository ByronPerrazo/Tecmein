
-- Agrega la columna para la relacion con Etapa
ALTER TABLE `visita` ADD COLUMN `IdEtapa` INT NOT NULL DEFAULT 1 AFTER `Detalle`;

-- Agrega la columna para la relacion con Empresa (Operador)
ALTER TABLE `visita` ADD COLUMN `SecEmpresa` INT NULL AFTER `IdEtapa`;

-- Crea el indice para la nueva columna IdEtapa
ALTER TABLE `visita` ADD INDEX `IX_Visita_IdEtapa` (`IdEtapa`);

-- Crea el indice para la nueva columna SecEmpresa
ALTER TABLE `visita` ADD INDEX `IX_Visita_SecEmpresa` (`SecEmpresa`);

-- Agrega la llave foranea a la tabla Etapa
ALTER TABLE `visita` ADD CONSTRAINT `FK_Visita_Etapa` FOREIGN KEY (`IdEtapa`) REFERENCES `etapa`(`Id`) ON DELETE RESTRICT ON UPDATE CASCADE;

-- Agrega la llave foranea a la tabla Empresa
ALTER TABLE `visita` ADD CONSTRAINT `FK_Visita_Empresa` FOREIGN KEY (`SecEmpresa`) REFERENCES `empresa`(`Secuencial`) ON DELETE SET NULL ON UPDATE CASCADE;

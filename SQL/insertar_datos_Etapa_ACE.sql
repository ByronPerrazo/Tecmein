-- Insertar la nueva etapa 'ACE' si no existe
INSERT INTO `etapa` (`Codigo`, `Descripcion`, `Orden`, `EstaActivo`) VALUES
('ACE', 'Cotización Aceptada', 4, 1)
ON DUPLICATE KEY UPDATE
`Descripcion` = VALUES(`Descripcion`),
`EstaActivo` = VALUES(`EstaActivo`);

-- Actualizar el orden de las etapas posteriores
UPDATE `etapa` SET `Orden` = 5 WHERE `Codigo` = 'PRE';
UPDATE `etapa` SET `Orden` = 6 WHERE `Codigo` = 'CON';
UPDATE `etapa` SET `Orden` = 7 WHERE `Codigo` = 'POR';
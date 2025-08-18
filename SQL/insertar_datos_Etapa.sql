
INSERT INTO `etapa` (`Codigo`, `Descripcion`, `Orden`, `EstaActivo`) VALUES
('VIS', 'Visita', 1, 1),
('COT', 'Cotización', 2, 1),
('SEG', 'Seguimiento', 3, 1),
('PRE', 'Pre-Contrato', 4, 1),
('CON', 'Contrato', 5, 1),
('POR', 'Por Renovar', 6, 1)
ON DUPLICATE KEY UPDATE
`Descripcion` = VALUES(`Descripcion`),
`Orden` = VALUES(`Orden`),
`EstaActivo` = VALUES(`EstaActivo`);

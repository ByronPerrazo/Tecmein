
CREATE TABLE IF NOT EXISTS `etapa` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Codigo` varchar(5) NOT NULL,
  `Descripcion` varchar(100) NOT NULL,
  `Orden` int NOT NULL,
  `EstaActivo` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UK_Etapa_Codigo` (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

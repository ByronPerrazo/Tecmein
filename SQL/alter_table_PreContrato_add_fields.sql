-- Renombrar columnas existentes para coincidir con la entidad actualizada
ALTER TABLE precontrato CHANGE COLUMN Secuencial SecPreContrato INT AUTO_INCREMENT;
ALTER TABLE precontrato CHANGE COLUMN FechaCreacion FechaRegistro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP;

-- Añadir nuevas columnas a la tabla precontrato
ALTER TABLE precontrato ADD COLUMN SecFormaPago INT NULL AFTER SecUsuarioCrea;
ALTER TABLE precontrato ADD COLUMN Dias INT NULL;
ALTER TABLE precontrato ADD COLUMN TipoDias VARCHAR(50) NULL;
ALTER TABLE precontrato ADD COLUMN ValorContrato DECIMAL(18, 2) NULL;
ALTER TABLE precontrato ADD COLUMN AniosGarantia INT NULL;
ALTER TABLE precontrato ADD COLUMN MesesGarantia INT NULL;
ALTER TABLE precontrato ADD COLUMN PeriodoMantenimiento VARCHAR(255) NULL;
ALTER TABLE precontrato ADD COLUMN PolizaGarantia VARCHAR(255) NULL;
ALTER TABLE precontrato ADD COLUMN ValorAnticipo DECIMAL(18, 2) NULL;
ALTER TABLE precontrato ADD COLUMN FechaAnticipo DATETIME NULL;
ALTER TABLE precontrato ADD COLUMN NumeroCuotas INT NULL;
ALTER TABLE precontrato ADD COLUMN FechaPrimeraCuota DATETIME NULL;

-- Añadir la clave foránea para SecFormaPago
ALTER TABLE precontrato
ADD CONSTRAINT FK_PreContrato_FormaPago
FOREIGN KEY (SecFormaPago) REFERENCES formapago(SecFormaPago);

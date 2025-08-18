CREATE TABLE PreContrato (
    SecPreContrato INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    Dias INT NOT NULL,
    TipoDias VARCHAR(50) NOT NULL,
    ValorContrato DECIMAL(18, 2) NOT NULL,
    AniosGarantia INT NOT NULL,
    MesesGarantia INT NOT NULL,
    PeriodoMantenimiento VARCHAR(100) NOT NULL,
    PolizaGarantia VARCHAR(100) NOT NULL,
    ValorAnticipo DECIMAL(18, 2) NOT NULL,
    FechaAnticipo DATETIME NOT NULL,
    FormaPago VARCHAR(100) NOT NULL,
    NumeroCuotas INT NOT NULL,
    FechaPrimeraCuota DATETIME NOT NULL,
    EstaActivo SMALLINT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial)
);

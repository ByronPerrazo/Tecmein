CREATE TABLE Contrato (
    SecContrato INT PRIMARY KEY AUTO_INCREMENT,
    SecPreContrato INT NOT NULL,
    FechaFirma DATETIME NOT NULL,
    EstaFirmado BOOLEAN NOT NULL DEFAULT 0,
    UrlDocumento VARCHAR(500),
    NombreDocumento VARCHAR(255),
    EstaActivo SMALLINT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SecPreContrato) REFERENCES PreContrato(SecPreContrato)
);

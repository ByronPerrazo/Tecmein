CREATE TABLE Cotizacion (
    Secuencial INT PRIMARY KEY IDENTITY(1,1),
    SecVisita INT NOT NULL,
    EnviadoProveedor BIT NOT NULL DEFAULT 0,
    EnviadoCliente BIT NOT NULL DEFAULT 0,
    Confirmacion BIT NOT NULL DEFAULT 0,
    EstaActivo SMALLINT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    FechaModificacion DATETIME,
    FOREIGN KEY (SecVisita) REFERENCES Visita(Secuencial)
);
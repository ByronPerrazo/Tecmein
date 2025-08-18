CREATE TABLE CotizacionDetalle (
    Secuencial INT PRIMARY KEY IDENTITY(1,1),
    SecCotizacion INT NOT NULL,
    DetalleEquipo NVARCHAR(MAX),
    ValorCompra DECIMAL(18, 2) NOT NULL,
    MargenGanancia DECIMAL(5, 2) NOT NULL,
    Total DECIMAL(18, 2) NOT NULL,
    EstaActivo SMALLINT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial)
);
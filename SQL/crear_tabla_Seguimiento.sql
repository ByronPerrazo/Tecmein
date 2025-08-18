CREATE TABLE Seguimiento (
    SecSeguimiento INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    Accion VARCHAR(50) NOT NULL,
    Detalle VARCHAR(500) NOT NULL,
    FechaAccion DATETIME NOT NULL,
    AceptacionCliente BOOLEAN NOT NULL DEFAULT 0,
    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial)
);

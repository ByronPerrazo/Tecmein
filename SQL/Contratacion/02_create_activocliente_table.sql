CREATE TABLE ActivoCliente (
    IdActivoCliente INT AUTO_INCREMENT PRIMARY KEY,
    SecCliente INT NOT NULL,
    SecEquipo INT NULL, -- Puede ser nulo si el equipo no está en el catálogo
    Descripcion VARCHAR(500) NOT NULL,
    FechaInstalacion DATETIME NOT NULL,
    SecContratoOrigen INT NULL, -- Puede ser nulo si no proviene de un contrato de venta específico
    
    FOREIGN KEY (SecCliente) REFERENCES Cliente(SecCliente),
    FOREIGN KEY (SecContratoOrigen) REFERENCES Contrato(IdContrato)
    -- FOREIGN KEY (SecEquipo) REFERENCES Equipo(IdEquipo) -- Descomentar si se crea la tabla Equipo
);
USE tecmeindb; -- Asegúrate de usar la base de datos correcta

-- Tabla para registrar los tipos de impuestos
CREATE TABLE impuesto (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Codigo VARCHAR(10) NOT NULL UNIQUE,
    Descripcion VARCHAR(100) NOT NULL,
    Porcentaje DECIMAL(5,2),
    ValorFijo DECIMAL(18,2),
    CodigoSri VARCHAR(5),
    TipoImpuesto VARCHAR(20) NOT NULL,
    Vigente BOOLEAN NOT NULL DEFAULT TRUE,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME ON UPDATE CURRENT_TIMESTAMP
);

-- Tabla para asociar impuestos a cotizaciones
CREATE TABLE impuesto_cotizacion (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SecCotizacion INT NOT NULL,
    ImpuestoId INT NOT NULL,
    BaseImponible DECIMAL(18,2) NOT NULL,
    ValorImpuesto DECIMAL(18,2) NOT NULL,
    Exento BOOLEAN NOT NULL DEFAULT FALSE,
    Observaciones TEXT,
    FechaRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_ImpuestoCotizacion_Cotizacion FOREIGN KEY (SecCotizacion) REFERENCES cotizacion(Secuencial),
    CONSTRAINT FK_ImpuestoCotizacion_Impuesto FOREIGN KEY (ImpuestoId) REFERENCES impuesto(Id)
);

-- Alterar la tabla cotizacion para añadir campos de impuestos
ALTER TABLE cotizacion
ADD COLUMN Subtotal DECIMAL(18,2) NOT NULL DEFAULT 0,
ADD COLUMN ValorImpuestos DECIMAL(18,2) NOT NULL DEFAULT 0,
ADD COLUMN TotalConImpuestos DECIMAL(18,2) NOT NULL DEFAULT 0;

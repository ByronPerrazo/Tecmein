-- ==================================================================
-- Script para la creación de las tablas PreContrato y PreContratoParrafo
-- ==================================================================

-- Tabla principal para los Pre-Contratos
CREATE TABLE PreContrato (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecCotizacion INT NOT NULL,
    SecPlantillaPreContrato INT NOT NULL,
    SecUsuarioCrea INT NOT NULL,
    Version INT NOT NULL DEFAULT 1,
    Estado VARCHAR(50) NOT NULL, -- Borrador, Enviado, Aprobado, Rechazado
    EstaActivo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT FK_PreContrato_Cotizacion FOREIGN KEY (SecCotizacion) REFERENCES Cotizacion(Secuencial),
    CONSTRAINT FK_PreContrato_Plantilla FOREIGN KEY (SecPlantillaPreContrato) REFERENCES PlantillaPreContrato(SecPlantillaPreContrato),
    CONSTRAINT FK_PreContrato_Usuario FOREIGN KEY (SecUsuarioCrea) REFERENCES Usuario(Secuencial)
);

-- Tabla para los párrafos específicos de cada Pre-Contrato (copia editable)
CREATE TABLE PreContratoParrafo (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    SecPreContrato INT NOT NULL,
    Contenido TEXT,
    Orden INT NOT NULL,

    CONSTRAINT FK_PreContratoParrafo_PreContrato FOREIGN KEY (SecPreContrato) REFERENCES PreContrato(Secuencial) ON DELETE CASCADE
);


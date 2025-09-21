-- =============================================
-- Autor:		Gemini
-- Fecha de Creación: 19-09-2025
-- Descripción:	Crea la tabla Contrato para almacenar
--              la información del contrato final firmado.
-- =============================================

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Contrato' and xtype='U')
BEGIN
    CREATE TABLE Contrato (
        IdContrato INT PRIMARY KEY IDENTITY(1,1),
        IdCotizacion INT NOT NULL,
        FechaFirma DATETIME NOT NULL,
        IdUsuarioCarga INT NOT NULL,
        NombreArchivo NVARCHAR(255) NOT NULL,
        RutaArchivo NVARCHAR(1024) NOT NULL,
        FechaCreacion DATETIME DEFAULT GETDATE(),
        EsActivo BIT DEFAULT 1,

        CONSTRAINT FK_Contrato_Cotizacion FOREIGN KEY (IdCotizacion) REFERENCES Cotizacion(IdCotizacion),
        CONSTRAINT FK_Contrato_Usuario FOREIGN KEY (IdUsuarioCarga) REFERENCES Usuario(IdUsuario)
    );
    PRINT 'Tabla Contrato creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Contrato ya existe.';
END
GO
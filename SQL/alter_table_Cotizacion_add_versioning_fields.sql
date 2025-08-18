ALTER TABLE Cotizacion
ADD SecCotizacionOriginal INT NULL,
ADD SecUsuarioModifica INT NULL;

ALTER TABLE Cotizacion
ADD CONSTRAINT FK_Cotizacion_UsuarioModifica
FOREIGN KEY (SecUsuarioModifica) REFERENCES Usuario(Secuencial);

ALTER TABLE Cotizacion
ADD SecUsuario INT NULL;

ALTER TABLE Cotizacion
ADD CONSTRAINT FK_Cotizacion_Usuario
FOREIGN KEY (SecUsuario) REFERENCES Usuario(Secuencial);

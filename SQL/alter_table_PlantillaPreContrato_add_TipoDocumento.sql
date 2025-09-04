ALTER TABLE plantillaprecontrato
ADD COLUMN SecTipoDocumento INT NOT NULL;

ALTER TABLE plantillaprecontrato
ADD CONSTRAINT FK_PlantillaPreContrato_TipoDocumento
FOREIGN KEY (SecTipoDocumento) REFERENCES tipodocumento(SecTipoDocumento);

-- Nota: Si ya existen plantillas, necesitarás asignarles un tipo de documento.
-- Por ejemplo, para asignar el tipo 'PRE-CONTRATO' (cuyo SecTipoDocumento asumimos es 1) a todas las plantillas existentes:
-- UPDATE plantillaprecontrato SET SecTipoDocumento = 1;

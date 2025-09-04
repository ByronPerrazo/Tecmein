-- Corrección para la tabla precontrato
--
-- Paso 1: Eliminar la columna 'FormaPago' que tiene un tipo de dato incorrecto.
-- ADVERTENCIA: Esto eliminará cualquier dato que exista en esa columna.
ALTER TABLE precontrato DROP COLUMN FormaPago;

-- Paso 2: Añadir las columnas que faltan para que coincida con la entidad de C#.
ALTER TABLE precontrato ADD COLUMN SecPlantillaPreContrato INT NOT NULL AFTER SecCotizacion;
ALTER TABLE precontrato ADD COLUMN SecUsuarioCrea INT NOT NULL AFTER SecPlantillaPreContrato;
ALTER TABLE precontrato ADD COLUMN SecFormaPago INT NULL AFTER SecUsuarioCrea;
ALTER TABLE precontrato ADD COLUMN Version INT NOT NULL DEFAULT 1 AFTER SecFormaPago;
ALTER TABLE precontrato ADD COLUMN Estado VARCHAR(50) NOT NULL DEFAULT 'Borrador' AFTER Version;

-- Paso 3: Añadir las claves foráneas para las nuevas columnas.
ALTER TABLE precontrato 
ADD CONSTRAINT FK_PreContrato_PlantillaPreContrato 
FOREIGN KEY (SecPlantillaPreContrato) REFERENCES plantillaprecontrato(SecPlantillaPreContrato);

ALTER TABLE precontrato 
ADD CONSTRAINT FK_PreContrato_Usuario 
FOREIGN KEY (SecUsuarioCrea) REFERENCES usuario(secuencial);

ALTER TABLE precontrato 
ADD CONSTRAINT FK_PreContrato_FormaPago 
FOREIGN KEY (SecFormaPago) REFERENCES formapago(SecFormaPago);

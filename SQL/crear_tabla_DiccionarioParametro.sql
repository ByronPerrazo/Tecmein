-- Creación de la tabla para el Diccionario de Parámetros
CREATE TABLE DiccionarioParametro (
    Secuencial INT PRIMARY KEY AUTO_INCREMENT,
    Parametro VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255) NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);

-- Se añade una restricción UNIQUE para asegurar que no haya parámetros duplicados
ALTER TABLE DiccionarioParametro
ADD CONSTRAINT UQ_DiccionarioParametro_Parametro UNIQUE (Parametro);


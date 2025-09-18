CREATE TABLE PolizaGarantia (
    Secuencial INT PRIMARY KEY IDENTITY(1,1),
    Descripcion NVARCHAR(255) NOT NULL,
    EstaActivo BIT NOT NULL DEFAULT 1
);
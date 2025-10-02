CREATE TABLE Cuota (
    IdCuota INT PRIMARY KEY IDENTITY(1,1),
    IdPlanDePago INT NOT NULL,
    NumeroCuota INT NOT NULL,
    MontoEsperado DECIMAL(18,2) NOT NULL,
    FechaVencimiento DATETIME NOT NULL,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Pendiente',
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Cuota_PlanDePago FOREIGN KEY (IdPlanDePago) REFERENCES PlanDePago(IdPlanDePago)
);

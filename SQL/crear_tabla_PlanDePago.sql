CREATE TABLE PlanDePago (
    IdPlanDePago INT PRIMARY KEY IDENTITY(1,1),
    IdContrato INT NOT NULL,
    SecFormaPago INT NOT NULL,
    ValorContrato DECIMAL(18, 2) NOT NULL,
    ValorAnticipo DECIMAL(18, 2) NOT NULL,
    FechaAnticipo DATETIME NULL,
    NumeroCuotas INT NOT NULL,
    FechaPrimeraCuota DATETIME NULL,
    EstaActivo BIT NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PlanDePago_Contrato FOREIGN KEY (IdContrato) REFERENCES Contrato(IdContrato),
    CONSTRAINT FK_PlanDePago_FormaPago FOREIGN KEY (SecFormaPago) REFERENCES FormaPago(SecFormaPago)
);

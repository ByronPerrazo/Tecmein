CREATE TABLE FormatoNumeroCliente (
    SecFormatoNumeroCliente INT PRIMARY KEY AUTO_INCREMENT,
    SecEmpresa INT NOT NULL,
    UsaFormato BOOLEAN NOT NULL DEFAULT 0,
    Formato VARCHAR(100),
    NumeroInicio INT NOT NULL,
    FOREIGN KEY (SecEmpresa) REFERENCES Empresa(Secuencial)
);

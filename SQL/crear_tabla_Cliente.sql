CREATE TABLE Cliente (
    SecCliente INT PRIMARY KEY AUTO_INCREMENT,
    SecConstructora INT NOT NULL,
    NumeroCliente VARCHAR(50) NOT NULL,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    EstaActivo BOOLEAN DEFAULT 1,
    UNIQUE(SecConstructora),
    UNIQUE(NumeroCliente),
    FOREIGN KEY (SecConstructora) REFERENCES Constructora(SecConstructora)
);

CREATE TABLE PlantillaPreContratoParrafo (
    SecPlantillaPreContratoParrafo INT PRIMARY KEY AUTO_INCREMENT,
    SecPlantillaPreContrato INT NOT NULL,
    Orden INT NOT NULL,
    Contenido TEXT NOT NULL,
    EstaActivo SMALLINT NOT NULL DEFAULT 1,
    FOREIGN KEY (SecPlantillaPreContrato) REFERENCES PlantillaPreContrato(SecPlantillaPreContrato)
);

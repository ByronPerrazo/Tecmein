CREATE TABLE tipodocumento (
    SecTipoDocumento INT AUTO_INCREMENT PRIMARY KEY,
    Codigo VARCHAR(50) NOT NULL UNIQUE,
    Descripcion VARCHAR(255) NOT NULL,
    EstaActivo BOOLEAN NOT NULL DEFAULT TRUE,
    FechaRegistro TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Insertar tipos de documento iniciales
INSERT INTO tipodocumento (Codigo, Descripcion, EstaActivo) VALUES
('PRE-CONTRATO', 'Documento de Pre-Contrato', TRUE),
('CONTRATO', 'Documento de Contrato Final', TRUE),
('OFERTA', 'Documento de Oferta Comercial', TRUE);

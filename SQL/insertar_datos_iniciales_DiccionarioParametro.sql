-- ==================================================================
-- Script para la carga inicial de parámetros en DiccionarioParametro
-- ==================================================================

-- Se recomienda ejecutar este script una sola vez para poblar la tabla.

INSERT INTO DiccionarioParametro (Parametro, Descripcion, EstaActivo) VALUES

-- Datos Generales
('{{fecha_actual}}', 'La fecha del día en que se genera el documento.', 1),
('{{hora_actual}}', 'La hora en que se genera el documento.', 1),

-- Datos de tu Empresa (Emisor)
('{{empresa_nombre}}', 'Nombre de tu empresa.', 1),
('{{empresa_identificacion}}', 'RUC o identificación de tu empresa.', 1),
('{{empresa_direccion}}', 'Dirección de tu empresa.', 1),
('{{empresa_telefono}}', 'Teléfono de tu empresa.', 1),
('{{empresa_correo}}', 'Correo electrónico de tu empresa.', 1),

-- Datos del Cliente (Constructora)
('{{cliente_nombre}}', 'Razón social o nombre de la constructora.', 1),
('{{cliente_direccion}}', 'Dirección principal de la constructora.', 1),
('{{cliente_telefono}}', 'Teléfono principal de la constructora.', 1),
('{{cliente_correo}}', 'Correo principal de la constructora.', 1),
('{{cliente_representante_legal}}', 'Nombre del administrador o representante de la constructora.', 1),
('{{cliente_representante_telefono}}', 'Teléfono del administrador.', 1),
('{{cliente_representante_correo}}', 'Correo del administrador.', 1),

-- Datos del Contacto Principal
('{{contacto_nombre_completo}}', 'Nombres y apellidos del contacto principal asociado a la visita/cotización.', 1),
('{{contacto_titulo}}', 'Título profesional del contacto (ej: Ing., Arq.).', 1),
('{{contacto_correo}}', 'Correo del contacto.', 1),
('{{contacto_telefono}}', 'Teléfono del contacto.', 1),

-- Datos del Proyecto
('{{proyecto_nombre}}', 'Nombre del proyecto u obra.', 1),
('{{proyecto_direccion}}', 'Dirección específica del proyecto.', 1),
('{{proyecto_detalle}}', 'Descripción o detalles de la visita.', 1),
('{{proyecto_provincia}}', 'Provincia donde se ubica el proyecto.', 1),
('{{proyecto_canton}}', 'Cantón donde se ubica el proyecto.', 1),

-- Datos de la Cotización
('{{cotizacion_numero}}', 'El número o secuencial de la cotización base.', 1),
('{{cotizacion_subtotal}}', 'El valor subtotal de la cotización.', 1),
('{{cotizacion_iva}}', 'El valor del IVA calculado.', 1),
('{{cotizacion_total}}', 'El valor final (Total con Impuestos).', 1),
('{{cotizacion_fecha_emision}}', 'La fecha de registro de la cotización.', 1),

-- Parámetros Especiales (reemplazo con HTML)
('{{tabla_detalle_cotizacion}}', 'Parámetro especial que se reemplazará por una tabla HTML con el detalle de la cotización.', 1);


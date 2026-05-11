-- Optimizacion del Motor de Impuestos - Tecmein
-- Fecha: 2026-05-07

-- 1. Mejorar la tabla de tipos de impuestos para soportar cascada (prioridad)
ALTER TABLE tipoimpuesto 
ADD COLUMN Prioridad INT DEFAULT 0 COMMENT 'Orden de calculo, menor prioridad se calcula primero';

-- 2. Mejorar el detalle de cotizacion para tener subtotales mas claros
ALTER TABLE cotizaciondetalle
ADD COLUMN Subtotal decimal(18,2) DEFAULT 0,
ADD COLUMN Impuestos decimal(18,2) DEFAULT 0;

-- 3. Añadir relacion de impuestos a nivel de item para maxima precision (opcional pero recomendado para el futuro)
-- Por ahora mantendremos el resumen en impuestocotizacion pero calculado item por item.

-- 4. Inicializar prioridades para Impuestos conocidos
UPDATE tipoimpuesto SET Prioridad = 1 WHERE EsIva = 1;
UPDATE tipoimpuesto SET Prioridad = 0 WHERE EsImportacion = 1;

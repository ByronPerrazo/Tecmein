-- Script para eliminar campos de la tabla PreContrato que han sido movidos a la nueva entidad PlanDePago.

-- Paso 1: Eliminar la clave foránea que conecta PreContrato con FormaPago.
-- El nombre del constraint 'FK_PreContrato_FormaPago' se basa en el script 'alter_table_PreContrato_add_fields.sql'.
-- Si al ejecutarlo da un error indicando que el constraint no existe, se deberá buscar el nombre correcto en la definición de la tabla.
ALTER TABLE PreContrato DROP FOREIGN KEY FK_PreContrato_FormaPago;

-- Paso 2: Eliminar las columnas que ya no son parte de la entidad PreContrato.
ALTER TABLE PreContrato
    DROP COLUMN SecFormaPago,
    DROP COLUMN ValorContrato,
    DROP COLUMN ValorAnticipo,
    DROP COLUMN FechaAnticipo,
    DROP COLUMN NumeroCuotas,
    DROP COLUMN FechaPrimeraCuota;

-- Paso 3: Eliminar la columna 'FormaPago' de tipo VARCHAR del script de creación original.
-- Este comando se incluye por si la columna aún existe en la base de datos.
-- Si la columna no existe, este comando fallará. Si ese es el caso, simplemente comente o elimine esta línea.
-- ALTER TABLE PreContrato DROP COLUMN FormaPago;


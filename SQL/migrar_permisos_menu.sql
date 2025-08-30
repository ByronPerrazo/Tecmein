-- FASE 1, PASO 1.3 (Versión MySQL)
-- Migración de los permisos de menú desde Permisosrol

START TRANSACTION;

-- Inserta en la nueva tabla RolPermiso basándose en la tabla Permisosrol existente.
INSERT INTO RolPermiso (SecRol, IdPermiso)
SELECT
    pr.SecRol,
    CONCAT('Menu.Ver.', REPLACE(m.Nombre, ' ', '')) AS IdPermiso
FROM
    Permisosrol pr
JOIN
    Menu m ON pr.SecMenu = m.SecMenu
WHERE
    -- Asegurarse de que el permiso construido exista en la tabla Permiso
    EXISTS (SELECT 1 FROM Permiso p WHERE p.IdPermiso = CONCAT('Menu.Ver.', REPLACE(m.Nombre, ' ', '')))
    -- Y asegurarse de no insertar duplicados si el script se corre más de una vez
    AND NOT EXISTS (
        SELECT 1 FROM RolPermiso rp 
        WHERE rp.SecRol = pr.SecRol 
        AND rp.IdPermiso = CONCAT('Menu.Ver.', REPLACE(m.Nombre, ' ', ''))
    );

COMMIT;

SELECT 'Migración de permisos de menú a RolPermiso completada para MySQL.' AS `status`;
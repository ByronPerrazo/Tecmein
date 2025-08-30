-- Asigna todos los permisos definidos en la tabla Permiso al rol con SecRol = 1 (Administrador)

INSERT INTO `rolpermiso` (SecRol, IdPermiso)
SELECT
    1, -- SecRol del administrador
    p.IdPermiso
FROM
    `permiso` p
WHERE
    NOT EXISTS (
        SELECT 1 FROM `rolpermiso` rp
        WHERE rp.SecRol = 1 AND rp.IdPermiso = p.IdPermiso
    );

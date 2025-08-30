-- (Versión 4 - MySQL - Nombres de tabla corregidos)

INSERT INTO `rolpermiso` (SecRol, IdPermiso)
SELECT
    rm.secRol,
    CONCAT('Menu.Ver.', rm.secMenu) AS IdPermiso
FROM
    rolmenu rm
WHERE
    (rm.esActivo IS NULL OR rm.esActivo = 1)
    AND rm.secRol IS NOT NULL
    AND rm.secMenu IS NOT NULL
    AND NOT EXISTS (
        SELECT 1 FROM `rolpermiso` rp 
        WHERE rp.SecRol = rm.secRol 
        AND rp.IdPermiso = CONCAT('Menu.Ver.', rm.secMenu)
    );
-- =====================================================================
-- Script para refactorizar el sistema de permisos a un modelo genérico
-- =====================================================================

-- Paso 1: Limpiar las asignaciones de permisos de rol existentes,
-- ya que los permisos subyacentes van a cambiar.
DELETE FROM rolpermiso;

-- Paso 2: Limpiar la tabla de permisos para eliminar los registros específicos del módulo.
DELETE FROM permiso;

-- Paso 3: Resetear el autoincremental de la tabla de permisos (opcional, buena práctica).
-- Nota: En MySQL, se usa ALTER TABLE. En SQL Server sería DBCC CHECKIDENT.
ALTER TABLE permiso AUTO_INCREMENT = 1;

-- Paso 4: Insertar los nuevos permisos genéricos y reutilizables.
INSERT INTO permiso (IdPermiso, Descripcion) VALUES 
('CREATE', 'Permite crear nuevas entidades (registros)'),
('READ', 'Permite leer/ver la información y listas de entidades'),
('UPDATE', 'Permite actualizar entidades existentes'),
('DELETE', 'Permite eliminar entidades existentes'),
('VIEWMENU', 'Permite ver el elemento en el menú principal');

-- =====================================================================
-- Fin del script.
-- La base de datos está ahora lista para la Fase 1 (Rediseño de la UI).
-- =====================================================================

-- ==================================================================================================
-- Script para migrar al nuevo sistema de permisos granulares y añadir el CRUD de Permisos al menú
-- Versión: 1.0
-- Fecha: 2025-08-24
-- ==================================================================================================

-- Desactivar la verificación de claves foráneas para permitir el vaciado de las tablas
SET FOREIGN_KEY_CHECKS=0;

-- 1. Limpiar la configuración de permisos anterior
TRUNCATE TABLE `rolpermiso`;
TRUNCATE TABLE `permiso`;

-- Reactivar la verificación de claves foráneas
SET FOREIGN_KEY_CHECKS=1;

-- 2. Insertar los nuevos permisos granulares en la tabla `permiso`
INSERT INTO `permiso` (`IdPermiso`, `Descripcion`) VALUES
-- Permisos para el módulo de Usuarios
('USUARIO_CREATE', 'Permite crear nuevos usuarios'),
('USUARIO_READ', 'Permite ver la lista de usuarios'),
('USUARIO_UPDATE', 'Permite editar usuarios existentes'),
('USUARIO_DELETE', 'Permite eliminar usuarios'),
('USUARIO_VIEWMENU', 'Permite ver el menú de Usuarios'),

-- Permisos para el módulo de Roles
('ROL_CREATE', 'Permite crear nuevos roles'),
('ROL_READ', 'Permite ver la lista de roles'),
('ROL_UPDATE', 'Permite editar roles existentes'),
('ROL_DELETE', 'Permite eliminar roles'),
('ROL_VIEWMENU', 'Permite ver el menú de Roles'),
('ROL_PERMISSIONS', 'Permite administrar los permisos de un rol'),

-- Permisos para el módulo de Permisos (el nuevo CRUD)
('PERMISO_CREATE', 'Permite crear nuevos tipos de permisos'),
('PERMISO_READ', 'Permite ver la lista de permisos'),
('PERMISO_UPDATE', 'Permite editar permisos existentes'),
('PERMISO_DELETE', 'Permite eliminar permisos'),
('PERMISO_VIEWMENU', 'Permite ver el menú de Permisos'),

-- Permisos para el módulo de Menú
('MENU_CREATE', 'Permite crear nuevos menús'),
('MENU_READ', 'Permite ver la lista de menús'),
('MENU_UPDATE', 'Permite editar menús existentes'),
('MENU_DELETE', 'Permite eliminar menús'),
('MENU_VIEWMENU', 'Permite ver el menú de Menús'),

-- Permisos para el módulo de Empresa
('EMPRESA_READ', 'Permite ver los datos de la empresa'),
('EMPRESA_UPDATE', 'Permite editar los datos de la empresa'),
('EMPRESA_VIEWMENU', 'Permite ver el menú de Empresa'),

-- Permisos para el módulo de Constructora
('CONSTRUCTORA_CREATE', 'Permite crear nuevas constructoras'),
('CONSTRUCTORA_READ', 'Permite ver la lista de constructoras'),
('CONSTRUCTORA_UPDATE', 'Permite editar constructoras existentes'),
('CONSTRUCTORA_DELETE', 'Permite eliminar constructoras'),
('CONSTRUCTORA_VIEWMENU', 'Permite ver el menú de Constructoras'),

-- Permisos para el módulo de Contacto
('CONTACTO_CREATE', 'Permite crear nuevos contactos'),
('CONTACTO_READ', 'Permite ver la lista de contactos'),
('CONTACTO_UPDATE', 'Permite editar contactos existentes'),
('CONTACTO_DELETE', 'Permite eliminar contactos'),
('CONTACTO_VIEWMENU', 'Permite ver el menú de Contactos');

-- 3. Insertar el nuevo ítem de menú para la administración de permisos
-- Asumimos que el menú 'Administración' tiene secuencial = 2
INSERT INTO `menu` (`secuencial`, `descripcion`, `secMenuPadre`, `icono`, `controlador`, `paginaAccion`, `esActivo`, `fechaRegistro`) VALUES
(19, 'Permisos', 2, NULL, 'Permiso', 'Index', 1, NOW());

-- 4. Asignar todos los permisos al rol de Administrador (asumiendo SecRol = 1)
INSERT INTO `rolpermiso` (`SecRol`, `IdPermiso`)
SELECT 1, p.IdPermiso FROM `permiso` p;

-- Fin del script

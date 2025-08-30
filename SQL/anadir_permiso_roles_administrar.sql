-- ==================================================================================================
-- Script para añadir el permiso 'Roles.Administrar' y asignarlo al rol de Administrador
-- Versión: 1.0
-- Fecha: 2025-08-25
-- Descripción:
-- Este script asegura que el permiso 'Roles.Administrar', requerido por la política de autorización
-- del RolController, exista en la base de datos y esté asignado al rol de Administrador.
-- ==================================================================================================

-- Desactivar la verificación de claves foráneas para permitir la manipulación de datos
SET FOREIGN_KEY_CHECKS=0;

-- 1. Insertar el permiso 'Roles.Administrar' en la tabla 'permiso' si no existe
INSERT IGNORE INTO `permiso` (`IdPermiso`, `Descripcion`) VALUES
('Roles.Administrar', 'Permite acceso completo al módulo de administración de roles');

-- 2. Asignar el permiso 'Roles.Administrar' al rol de Administrador (SecRol = 1) si no está ya asignado
INSERT IGNORE INTO `rolpermiso` (`SecRol`, `IdPermiso`) VALUES
(1, 'Roles.Administrar');

-- Reactivar la verificación de claves foráneas
SET FOREIGN_KEY_CHECKS=1;

-- Fin del script

-- Agrega permisos de acción para el módulo de Contactos

INSERT INTO Permiso (IdPermiso, Descripcion) VALUES
('Contacto.Ver', 'Permite ver la lista de contactos'),
('Contacto.Crear', 'Permite crear nuevos contactos'),
('Contacto.Editar', 'Permite editar contactos existentes'),
('Contacto.Eliminar', 'Permite eliminar contactos');

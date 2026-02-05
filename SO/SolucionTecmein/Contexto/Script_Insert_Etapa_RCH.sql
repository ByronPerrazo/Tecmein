INSERT INTO Etapa (Codigo, Descripcion, Orden, EstaActivo) 
SELECT 'RCH', 'Rechazada', 99, 1
WHERE NOT EXISTS (SELECT 1 FROM Etapa WHERE Codigo = 'RCH');

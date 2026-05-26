# Memorias de Proyecto - Tecmein

## Objetivo General
Homologar la identidad visual del sistema Tecmein bajo un entorno más profesional, premium y minimalista, tomando como referencia los avances de diseño realizados en el proyecto NexAsis.

## Avances y Ajustes Realizados:
1. **Identidad Visual y Colores**:
   - Se aplicaron variables CSS en el `:root` de la vista para centralizar los colores.
   - Se cambió el color de la cabecera de las tablas a **Azul Petróleo / Teal Oscuro** (`#005f73`).
2. **Diseño de Listados (DataTables)**:
   - **Disposición**: Alineación en una sola fila del botón de creación, la barra de búsqueda y los iconos de exportación.
   - **Buscador**: Eliminación de la etiqueta "Buscar:", redondeado del input e integración de lupa vía SVG/CSS.
   - **Botones de Exportación**: Eliminación de textos (iconos puros de Excel, PDF, Print), fondos grises y sombras. Agregado de escala (`scale(1.2)`) en hover.
   - **Selector de Registros**: Compactado eliminando la palabra "registros".
   - **Acciones**: Unificación de botones en un dropdown tipo cápsula (`rounded-pill`) con icono de engranaje amarillo.
3. **Diseño de Modales y Formularios**:
   - Reducción de paddings y márgenes para formularios compactos.
   - Eliminación de tarjetas (`.card`) internas redundantes dentro de ventanas modales.
   - Cabeceras premium (`.modal-header-premium`) con alineación Flexbox.

## Estado de Avance General (100% Homologado)
Se ha completado la réplica del estándar visual premium en los siguientes módulos del sistema:
1. **Visitas** (Módulo Piloto)
2. **Cliente**
3. **Constructora**
4. **Contacto**
5. **Usuario**
6. **Rol**
7. **Activo del Cliente**
8. **Catálogo (Equipos)**
9. **Diccionario de Parámetros**
10. **Empresa**
11. **Formas de Pago**
12. **Formato de Número de Cliente**
13. **Impuestos**
14. **Menú del Sistema**
15. **Permisos**
16. **Plantillas de Pre-Contrato**
17. **Párrafos de Plantillas**
18. **Pólizas de Garantía**
19. **Gestión de Permisos por Rol**
20. **Roles y Menús (Rol Menú)**
21. **Tipos de Documento**
22. **Tipos de Impuesto**
23. **Cotización**
24. **Contrato**
25. **Financiero / Plan de Pago**
26. **Pre-Contrato**

## Hitos Técnicos Centralizados:
- **Control de Caché Activo**: Integración de `asp-append-version="true"` en todas las hojas de estilo y scripts de vistas para garantizar recargas limpias en producción.
- **Evitación de Deadlocks**: Migración de helpers síncronos `@Html.Partial` a `@await Html.PartialAsync` en vistas clave.
- **Traducción DataTable Local**: Migración de dependencias CDN a variables locales (`lenguajeEspanol`), garantizando funcionamiento autónomo offline.

## Próximos Pasos
- Despliegue en ambiente de pruebas y validación manual del usuario de la navegación responsiva y estética unificada.

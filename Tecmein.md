# Memorias de Proyecto - Tecmein

## Objetivo General
Homologar la identidad visual del sistema Tecmein bajo un entorno más profesional, premium y minimalista, tomando como referencia los avances de diseño realizados en el proyecto NexAsis.

## Estado Actual - Módulo Piloto: Visitas
Se ha utilizado el módulo de **Visitas** como plan piloto para estandarizar el proceso de diseño antes de replicarlo en el resto del sistema.

### Avances y Ajustes Realizados:
1. **Identidad Visual y Colores**:
   - Se aplicaron variables CSS en el `:root` de la vista para centralizar los colores (simulando el proceso de NexAsis).
   - Se cambió el color de la cabecera de las tablas a **Azul Petróleo / Teal Oscuro** (`#005f73`).

2. **Diseño de Listados (DataTables)**:
   - **Disposición**: Se alinearon en una sola fila el botón "Nueva Visita", la barra de búsqueda y los iconos de exportación.
   - **Buscador**: Se eliminó la etiqueta "Buscar:", se redondeó el input y se le integró un icono de lupa vía SVG.
   - **Botones de Exportación**: Se eliminaron los textos. Son solo iconos (Excel, PDF, Print). Se eliminaron por completo los fondos grises, bordes y sombras heredados de `modern-table.css`. Se agregó un efecto de escala (`scale(1.2)`) en hover.
   - **Selector de Registros**: Se eliminó la palabra "registros" del final para compactar el espacio.
   - **Acciones**: Se unificaron los botones en un dropdown tipo cápsula (`rounded-pill`) de color azul con un icono de engranaje amarillo.

3. **Diseño de Modales y Formularios**:
   - Se redujo el padding y los márgenes para tener un formulario más compacto.
   - Se reubicó el campo **Etapa** (antes en la cabecera del modal, ahora en la primera fila del formulario).
   - Se reubicó **Próxima Visita** (ahora junto a la Dirección).
   - Se hizo visible la "X" de cerrar el modal con un color oscuro contrastante.

## Pendientes y Próximos Pasos (Estandarización)
Para poder replicar esto en el resto del sistema de manera centralizada, se requiere:
1. **Centralización de Estilos de DataTables**: Mover las reglas de anulación de bordes y fondos de los botones de exportación, así como el diseño del buscador, a un archivo CSS compartido (o actualizar `modern-table.css` si es seguro).
2. **Centralización de Colores**: Implementar el proceso de NexAsis para centralizar los colores que se usan en el sistema (posiblemente mediante una vista parcial o lectura de base de datos).
3. **Réplica**: Aplicar esta misma estructura de `dom` en DataTables y estilos de formulario en los demás módulos.

# Principios y Convenciones del Proyecto Tecmein

Este archivo contiene los principios arquitectónicos, convenciones de código y decisiones de diseño de alto nivel para el proyecto Tecmein.

## Arquitectura y Diseño

- **Arquitectura General:** El proyecto sigue una arquitectura en capas con un fuerte uso de Inyección de Dependencias (IoC/DI) e interfaces, buscando la aplicación de los principios SOLID (SRP, OCP, DIP).
- **Generación de Documentos:** El sistema utiliza un motor reutilizable basado en el Patrón Strategy. Un `GeneradorDocumentoService` orquesta la creación de documentos, delegando la lógica a "Estrategias" específicas (ej. `EstrategiaPreContrato`).
- **Base de Datos:** El motor de base de datos es MySQL.

## Convenciones de Backend

- **Nomenclatura de Claves de BD:**
  - Clave primaria numérica autoincremental: `secuencial` (o `id_nombre_tabla` para tablas clave).
  - Clave foránea: `sec_nombre_tabla_origen` (o `id_nombre_tabla_origen`).
  - En general, los nombres de campos en la base de datos utilizan `snake_case`.
- **Manejo de Decimales:** Se utiliza `CultureInfo.InvariantCulture` en el backend para todas las conversiones entre `string` y `decimal`, asegurando que el punto (`.`) sea siempre el separador decimal.
- **Contenido HTML:** El campo `Contenido` en las entidades de párrafos (ej. `PlantillaPreContratoParrafo`) está diseñado para almacenar HTML de un editor de texto enriquecido.

## Convenciones de Frontend y UI/UX

- **Estándar CRUD:** El módulo de **Usuario** (`UsuarioController`, vistas y JS) sirve como la implementación de referencia para todos los nuevos CRUDs.
- **Patrón de Interfaz:** El patrón estándar para las interfaces CRUD incluye:
  - **Listados:** Usando la librería DataTables.
  - **Creación/Edición:** A través de ventanas modales.
  - **Confirmaciones:** Usando la librería SweetAlert para diálogos de acciones destructivas.
- **Editor de Texto Enriquecido (TinyMCE):** La integración sigue un patrón de alta calidad:
  - La clave de API se carga desde un endpoint seguro del backend, no se expone en el cliente.
  - El script del editor se carga dinámicamente.
  - El contenido se manipula a través de las funciones `get/setContent` del editor.

## Información General

- **Ruta de Logs:** `C:\Proyectos\Tecmein\SO\SolucionTecmein\TecmeinAplicacionWeb\logs`
- **Contexto del Asistente:** Si el directorio de trabajo es `C:\Proyectos\Tecmein`, el enfoque debe ser exclusivo a este proyecto.
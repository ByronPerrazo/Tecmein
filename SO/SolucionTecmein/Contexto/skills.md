# Catálogo de "Skills" (Patrones y Habilidades de Arquitectura)

Este documento cataloga los "skills" o patrones de diseño y arquitectura recurrentes y de alto valor identificados en el sistema Tecmein. El propósito es tener un repositorio de conocimiento sobre "cómo hacemos las cosas" para acelerar el desarrollo, mantener la consistencia y facilitar la incorporación de nuevos miembros al equipo.

---

## 1. Orquestación de Flujo de Negocio (Llamada BLL-a-BLL)

**Skill:** Un servicio de la capa de lógica de negocio (BLL) invoca a otro servicio de la misma capa para avanzar el estado de un proceso de negocio más grande.

**Descripción:**
En lugar de que el controlador sea responsable de coordinar múltiples servicios, la responsabilidad se delega a un servicio "orquestador" que encapsula un paso completo del flujo. Esto centraliza la lógica de negocio y hace que los controladores sean más delgados.

**Ejemplo Clave:**
- `CotizacionServices.Crear()`: Después de crear exitosamente una `Cotizacion`, este servicio llama a `_visitaServices.CambiarEtapa(..., "COT")`.
- `SeguimientoServices.Crear()`: Después de crear un `Seguimiento`, llama a `_visitaServices.CambiarEtapa(..., "SEG")`. Si el seguimiento es una aceptación, vuelve a llamar a `_visitaServices.CambiarEtapa(..., "PRE")`.

**Cuándo usarlo:**
Cuando una acción en un dominio (ej. crear una cotización) debe desencadenar un cambio de estado en otro dominio relacionado (ej. la visita).

---

## 2. Versionamiento de Entidades (Inactivación y Creación)

**Skill:** En lugar de actualizar un registro existente (UPDATE), se marca como inactivo y se crea uno nuevo que representa la nueva versión.

**Descripción:**
Este patrón es fundamental para la auditoría y el seguimiento de cambios. Se asegura de que nunca se pierda el historial de una entidad. La versión "actual" es siempre la que tiene el flag `EstaActivo = true` y, a menudo, el número de versión más alto.

**Ejemplos Clave:**
- `CotizacionServices.Editar()`: Marca la cotización existente con `EstaActivo = 0` y crea una nueva cotización con un número de versión incrementado, vinculada a la original.
- `PreContratoServices.ActualizarContenidoPreContrato()`: Sigue el mismo patrón para el contenido del pre-contrato, garantizando que cada versión del documento quede guardada.

**Cuándo usarlo:**
Para entidades críticas donde el historial de cambios es importante para el negocio o para auditoría (ofertas, contratos, documentos legales).

---

## 3. Generación de Documentos con Plantillas (Motor de Plantillas)

**Skill:** Un servicio dedicado (`GeneratorService`) combina plantillas de base de datos con datos dinámicos del sistema para producir documentos complejos (HTML/PDF).

**Descripción:**
Este patrón desacopla la generación de documentos de la lógica de negocio principal. Las plantillas se almacenan en la base de datos (`PlantillaPreContrato`, `PlantillaPreContratoParrafo`) y contienen placeholders (ej. `{{cliente_nombre}}`). Un servicio generador se encarga de cargar la plantilla, recolectar todos los datos necesarios de diferentes fuentes (cotización, cliente, etc.) y realizar el reemplazo de los placeholders.

**Ejemplo Clave:**
- `PreContratoGeneratorService.GenerarVistaPreviaHtml()`: Es el corazón de la generación de pre-contratos. Orquesta la recolección de datos y el reemplazo de parámetros definidos en `DiccionarioParametro`.

**Cuándo usarlo:**
Cuando se necesita generar documentos estandarizados pero con contenido dinámico, como contratos, reportes o correos electrónicos complejos.

---

## 4. Control de Acceso Granular (Filtro de Acción + Servicio)

**Skill:** Proteger los endpoints del backend utilizando un filtro de acción personalizado que delega la lógica de autorización a un servicio central.

**Descripción:**
Se utiliza un atributo `[ValidatePermission("ACCION")]` en los métodos de los controladores. Este filtro intercepta la solicitud y llama a un `AutorizacionService`. El servicio verifica, basándose en el rol del usuario y el menú asociado al controlador, si la acción (`CREAR`, `LEER`, `ACTUALIZAR`, `ELIMINAR`) está permitida. La configuración de permisos se gestiona en la base de datos (`RolMenu`).

**Ejemplo Clave:**
- `[ValidatePermission("CREAR")]` en `VisitaController.CrearVisita`.
- `AutorizacionService.TienePermiso()`: La lógica central que consulta la base de datos (con caché) para validar el permiso.

**Cuándo usarlo:**
Para proteger cualquier endpoint que realice una operación sensible. Es el mecanismo de seguridad estándar para las acciones CRUD en todo el sistema.

---

## 5. Manejo de Respuestas Genéricas en Frontend (dataSrc)

**Skill:** Configurar correctamente los componentes de UI en JavaScript para que puedan interpretar la estructura de respuesta estándar del backend (`GenericResponse<T>`).

**Descripción:**
El backend envuelve la mayoría de sus respuestas de lista en un objeto `GenericResponse<T>`, que al ser serializado produce una estructura JSON con metadatos (`$id`, `$values`). El frontend debe ser consciente de esto. Para componentes como DataTables, se utiliza la opción `ajax.dataSrc` para apuntar a la propiedad correcta (`response.objeto.$values`) donde se encuentra el array de datos.

**Ejemplo Clave:**
- La configuración de `DataTable` en `Endpoints_Listas_DataTables.md` que define `dataSrc` como una función para extraer los datos del wrapper.

**Cuándo usarlo:**
Siempre que se consuma un endpoint de lista desde el frontend, ya sea para una tabla, un dropdown o cualquier otro propósito.

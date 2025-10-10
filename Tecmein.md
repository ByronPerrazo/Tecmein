# Principios y Convenciones del Proyecto Tecmein

Este archivo contiene los principios arquitectónicos, convenciones de código y decisiones de diseño de alto nivel para el proyecto Tecmein.

## Arquitectura y Diseño

- **Arquitectura General:** El proyecto sigue una arquitectura en capas con un fuerte uso de Inyección de Dependencias (IoC/DI) e interfaces, buscando la aplicación de los principios SOLID (SRP, OCP, DIP).
- **Generación de Documentos:** El sistema utiliza un motor reutilizable basado en el Patrón Strategy. Un `GeneradorDocumentoService` orquesta la creación de documentos, delegando la lógica a "Estrategias" específicas (ej. `EstrategiaPreContrato`).
- **Base de Datos:** El motor de base de datos es MySQL.

## Convenciones de Backend

- **Nomenclatura de Claves de BD:**
  - Clave primaria numérica autoincremental: `Secuencial`.
  - Clave primaria de tipo string: `Codigo`.
  - Clave foránea: `Sec` + `[NombreTablaOrigen]`.
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


## Reglas y Patrones Específicos (Memorizados por Gemini)

1.  **Seguimiento de Pagos:** Implementar un sistema para `PlanDePago` y `Pago`, separado de `PreContrato`.
2.  **Ciclo de Vida Contrato/Cliente:** Un plan de 3 fases (actualmente en pausa) para crear las entidades `Contrato` y `Cliente` y migrar datos históricos.
3.  **Verificación Manual de Contratos:** Implementar a futuro una pantalla para que un administrador compare el precontrato del sistema con el PDF firmado.
4.  **Convenciones de Nomenclatura:** Usar el formato `[NombreEntidad]VM` para ViewModels y `camelCase` para variables y métodos.
5.  **Flujo de Planes de Pago:** La información en `PreContrato` es una propuesta; el `PlanDePago` formal y opcional se vincula solo al `Contrato`.
6.  **Gestión de Clientes y Contratos:** Definición de las fuentes de clientes, CRUD de clientes y los dos flujos para la creación de contratos.
7.  **Patrón para DataTables:** Al usar DataTables, la API devuelve un `GenericResponse`, y se debe usar la opción `dataSrc` en el frontend para apuntar a `response.objeto.$values`.
8.  **Patrón de Consumo de API (Listas):** Al consumir listas desde el frontend, siempre acceder al array de datos a través de `response.objeto.$values`.
9.  **Contexto del Proyecto:** Al iniciar una sesión en el directorio de Tecmein, debo revisar todos los archivos `.md` para cargar el contexto completo.
10. **Patrón de Botones en Tablas:** El orden de los botones de acción debe ser: Editar (izquierda), otros, Eliminar (derecha).
11. **Patrón de Versionamiento de Cotizaciones:** Se define un proceso específico para editar cotizaciones antes y después de ser enviadas al cliente.
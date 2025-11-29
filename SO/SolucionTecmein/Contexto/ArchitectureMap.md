# Mapa de Arquitectura y Flujos de Negocio - Tecmein

Este documento sirve como un mapa de conocimiento técnico para el sistema Tecmein. Su propósito es documentar los flujos de negocio principales, las interacciones entre componentes, las dependencias y la lógica clave para facilitar el análisis de impacto y el desarrollo.

---

## Proceso Troncal: Desde Visita hasta Cliente

Este es el flujo de negocio principal que convierte un prospecto en un cliente.

### Paso 1: Creación y Gestión de `Visita`

La `Visita` es el punto de partida del ciclo de ventas. Representa el contacto inicial o el levantamiento de información en la ubicación de un cliente potencial (`Constructora`).

#### Componentes Clave:

*   **Entidad Principal:** `Entity/Visita.cs`
*   **Punto de Entrada Web:** `TecmeinAplicacionWeb/Controllers/VisitaController.cs`
*   **ViewModel:** `Models/ViewModels/VisitaVM.cs`
*   **Servicio (Lógica):** `BLL/Implementacion/VisitaServices.cs`
*   **Interfaz de Servicio:** `BLL/Interfaces/IVisitaServices.cs`
*   **Repositorio:** `DAL/Implementacion/GenericRepository.cs` (Invocado a través de `IGenericRepository<Visita>`)

#### Flujo de Creación de una Visita:

1.  **Request HTTP:** El proceso se inicia con una petición `POST` a la acción `VisitaController.CrearVisita`. Los datos del formulario viajan como un string JSON que se deserializa en un `VisitaVM`.
2.  **Mapeo (Controller):** El controlador utiliza `AutoMapper` para convertir el `VisitaVM` (un objeto plano para la vista) en la entidad `Visita` (el objeto que representa la tabla en la base de datos). Se asigna el `SecUsuario` del usuario logueado.
3.  **Llamada al Servicio (Controller -> BLL):** Se invoca el método `_visitaServices.CreaVisita(visitaEntity)`.
4.  **Lógica de Negocio (BLL):**
    *   Dentro de `VisitaServices`, el método `CreaVisita` realiza una acción crítica antes de guardar:
    *   Llama a `_etapaServices.ObtenerPorCodigo("VIS")` para obtener la entidad `Etapa` que representa el estado inicial "Visita".
    *   Asigna el `Id` de esta etapa a la propiedad `visitaEntity.IdEtapa`. **Esto asegura que toda nueva visita comience en la etapa correcta del ciclo de vida definido.**
5.  **Persistencia (BLL -> DAL):** El servicio invoca `_repositorio.Crear(visitaEntity)`, donde `_repositorio` es una instancia de `IGenericRepository<Visita>`.
6.  **Operación de Base de Datos (DAL):** El `GenericRepository` utiliza Entity Framework Core para traducir la entidad `Visita` en una sentencia `INSERT` y ejecutarla en la base de datos.
7.  **Respuesta:** La entidad creada (ya con su `Secuencial` asignado por la BD) se devuelve hacia el servicio, luego al controlador, se mapea de nuevo a un `VisitaVM` y se envía como respuesta al cliente web.

#### Dependencias Notables:

*   `VisitaServices` tiene una dependencia de `IEtapaServices`. Esto es fundamental, ya que **acopla el ciclo de vida de la `Visita` al sistema de `Etapas`**, permitiendo controlar el flujo del proceso de negocio.
*   El `VisitaController` depende de una multitud de servicios (`IProvinciaServices`, `IConstructoraServices`, etc.) para cargar datos necesarios para los formularios (comboboxes, listas).

---

### Paso 2: Creación y Gestión de `Cotizacion`

Una vez que se ha recopilado la información en la `Visita`, el siguiente paso es generar una `Cotizacion` formal. Este componente es el corazón de la oferta comercial.

#### Componentes Clave:

*   **Entidades Principales:** `Entity/Cotizacion.cs`, `Entity/Cotizaciondetalle.cs`
*   **Punto de Entrada Web:** `TecmeinAplicacionWeb/Controllers/CotizacionController.cs`
*   **ViewModel:** `Models/ViewModels/CotizacionVM.cs`
*   **Servicio (Lógica):** `BLL/Implementacion/CotizacionServices.cs`
*   **Interfaz de Servicio:** `BLL/Interfaces/ICotizacionServices.cs`

#### Flujo de Creación de una Cotización:

1.  **Disparador (UI):** El usuario selecciona una `Visita` que no tenga una cotización activa. La UI probablemente llama a `CotizacionController.VerificarVisita(idVisita)` para confirmar esto antes de habilitar la opción de crear.
2.  **Request HTTP:** La acción `CotizacionController.Crear` recibe los datos, incluyendo el `SecVisita` y una lista de `CotizaciondetalleVM` que representan los productos o servicios ofertados.
3.  **Mapeo (Controller):** `AutoMapper` convierte el `CotizacionVM` y sus detalles en las entidades `Cotizacion` y `Cotizaciondetalle`.
4.  **Llamada al Servicio (Controller -> BLL):** Se invoca el método `_cotizacionServices.Crear(cotizacionEntity, idUsuario)`.
5.  **Lógica de Negocio (BLL - `CotizacionServices`):
    *   **Validación Crucial:** El servicio primero verifica que no exista otra cotización con `EstaActivo = 1` para la misma `SecVisita`. Si existe, lanza una excepción.
    *   **Cálculos de Totales:** Itera sobre la colección `Cotizaciondetalles`, calcula el `Total` de cada línea (Cantidad * ValorCompra * (1 + Margen)) y suma todo para obtener el `Subtotal` de la cotización.
    *   **Cálculo de Impuestos:** Llama a `_impuestoServices` para obtener los impuestos vigentes (IVA, Importación), los calcula sobre el subtotal y los añade a la colección `ImpuestoCotizaciones`.
    *   **Persistencia (BLL -> DAL):** Invoca `_repositorio.Crear(cotizacionEntity)`. Entity Framework se encarga de guardar la `Cotizacion` (maestro) y sus `Cotizaciondetalles` (detalles) en una única transacción.
    *   **¡Cambio de Etapa! (BLL -> BLL):** Después de que la cotización se ha guardado correctamente, el servicio ejecuta la línea `await _visitaServices.CambiarEtapa(cotizacionCreada.SecVisita, "COT")`. **Este es el punto exacto donde el proceso de negocio avanza formalmente a la siguiente fase.**
6.  **Respuesta:** La cotización recién creada se devuelve en la cadena de llamadas hasta el navegador.

#### Lógica de Edición y Versionamiento:

El método `Editar` en `CotizacionServices` implementa una estrategia de versionamiento. En lugar de actualizar directamente un registro de cotización, el proceso es:
1.  Se busca la cotización original y se marca como inactiva (`EstaActivo = 0`).
2.  Se crea una **nueva** entidad `Cotizacion` con `EstaActivo = 1`.
3.  Esta nueva cotización se vincula a la primera versión a través del campo `SecCotizacionOriginal`.

> **Implicación:** Esto es excelente para la auditoría y el seguimiento, ya que nunca se pierde el historial de cambios de una oferta. Sin embargo, significa que las consultas deben filtrar siempre por `EstaActivo = 1` para obtener la versión más reciente.

#### Dependencias Notables:

*   `CotizacionServices` depende de `IVisitaServices`. Este acoplamiento es **intencional y necesario** para orquestar la transición de etapas del flujo de negocio principal.
*   `CotizacionServices` también depende de `IImpuestoServices` para la lógica de cálculo de impuestos, demostrando una buena separación de responsabilidades.

---

### Paso 3: `Seguimiento` y Aceptación de la Cotización

Una vez que la cotización está en manos del cliente, comienza el proceso de seguimiento. Este módulo no solo registra las interacciones, sino que también contiene la lógica para formalizar la aceptación del cliente, actuando como puente hacia la fase de Pre-Contrato.

#### Componentes Clave:

*   **Entidad Principal:** `Entity/Seguimiento.cs`
*   **Punto de Entrada Web:** `TecmeinAplicacionWeb/Controllers/SeguimientoController.cs`
*   **Servicio (Lógica):** `BLL/Implementacion/SeguimientoServices.cs`
*   **Interfaz de Servicio:** `BLL/Interfaces/ISeguimientoServices.cs`

#### Flujo de Creación de un Seguimiento:

1.  **Request HTTP:** El proceso se inicia desde la interfaz de una cotización, enviando una petición `POST` a `SeguimientoController.Crear` con los detalles de la interacción (acción, detalle, fecha).
2.  **Llamada al Servicio (Controller -> BLL):** Se invoca `_seguimientoServices.Crear(seguimientoEntity)`.
3.  **Lógica de Negocio (BLL - `SeguimientoServices`):
    *   **Cambio de Etapa a "Seguimiento":** Inmediatamente después de guardar el nuevo registro de `Seguimiento`, el servicio ejecuta `await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "SEG")`. Esto asegura que, tan pronto como haya una interacción registrada, toda la `Visita` se mueva a la etapa de "Seguimiento".
    *   **El Gatillo de Aceptación:** El servicio comprueba el valor booleano `seguimientoEntity.AceptacionCliente`. Si es `true`, se desencadena la lógica de aprobación:
        1.  **Cambio de Etapa a "Pre-Contrato":** Se ejecuta `await _visitaServices.CambiarEtapa(cotizacion.SecVisita, "PRE")`. Este es el avance oficial del proceso a la siguiente fase.
        2.  **Confirmación de la Cotización:** Se actualiza la entidad `Cotizacion` estableciendo `cotizacion.Confirmacion = true` y se guarda en la base de datos. Este campo es la bandera que marca la oferta como aceptada y lista para el siguiente paso.

#### Lógica de Creación de Pre-Contrato (Decisión de Diseño Clave):

Dentro de `SeguimientoServices`, en el bloque que maneja la aceptación del cliente, existe un fragmento de código **comentado** que originalmente creaba un `PreContrato` de forma automática.

```csharp
// Se comenta la creación automática para moverla a un proceso manual desde la pantalla de Pre-Contratos.
/*
var nuevoPreContrato = new PreContrato { ... };
await _preContratoServices.Crear(nuevoPreContrato);
*/
```

> **Análisis e Implicación:** Esta es una decisión de arquitectura deliberada. La aceptación de una cotización **no crea un pre-contrato automáticamente**. En su lugar, la cotización queda marcada con `Confirmacion = true` y se convierte en una "cotización aprobada pendiente de pre-contrato". Un usuario debe ir a una sección diferente de la aplicación para iniciar manualmente la creación del `PreContrato` a partir de esta cotización aprobada. El método `ObtenerCotizacionesAprobadasSinPreContrato` en el mismo servicio confirma esta lógica, ya que su propósito es precisamente alimentar esa pantalla.

#### Dependencias Notables:

*   `SeguimientoServices` actúa como un **orquestador**, ya que tiene dependencias de `IVisitaServices` y `IGenericRepository<Cotizacion>`. Esto le da el poder no solo de gestionar sus propios datos, sino también de modificar el estado de otros dominios (`Visita` y `Cotizacion`) para mover el proceso de negocio general hacia adelante.

---

### Paso 4: Generación y Gestión del `PreContrato`

Este paso representa la formalización de la oferta aceptada. Es un módulo complejo que transforma los datos de una cotización aprobada y nueva información financiera/legal en un documento preliminar. La arquitectura aquí es notablemente sofisticada, centrada en un sistema de generación de documentos basado en plantillas.

#### Componentes Clave:

*   **Entidades Principales:** `PreContrato`, `PreContratoParrafo`.
*   **Entidades de Plantillas:** `PlantillaPreContrato`, `PlantillaPreContratoParrafo`.
*   **Entidad de Configuración:** `DiccionarioParametro` (define los placeholders como `{{cliente_nombre}}`).
*   **Punto de Entrada Web:** `PreContratoController`.
*   **Servicio Principal:** `PreContratoServices`.
*   **Servicio Generador:** `PreContratoGeneratorService` (el motor de las plantillas).

#### Flujo de Creación de un Pre-Contrato:

1.  **Disparador Manual (UI):** El proceso es iniciado por un usuario. La interfaz muestra una lista de cotizaciones que han sido aprobadas (`Confirmacion = true`) pero que aún no tienen un pre-contrato activo. Esta lista se alimenta del método `SeguimientoServices.ObtenerCotizacionesAprobadasSinPreContrato()`.
2.  **Selección y Recopilación de Datos (UI):** El usuario selecciona una cotizacion aprobada, una `PlantillaPreContrato` de una lista, y rellena un formulario con datos adicionales (Forma de Pago, Garantías, Plazos de entrega, etc.).
3.  **Generación de Vista Previa (BLL):**
    *   La UI llama a `PreContratoController.GenerarVistaPrevia`.
    *   Este invoca a `PreContratoGeneratorService.GenerarVistaPreviaHtml`.
    *   El `GeneratorService` carga la `PlantillaPreContrato` y sus párrafos. Luego, recopila todos los datos necesarios de la cotización, visita, cliente, y el formulario actual.
    *   Busca todos los placeholders (ej. `{{valor_contrato}}`) en el texto de la plantilla y los reemplaza con los datos reales, devolviendo un string HTML completo.
4.  **Creación y Persistencia (Controller -> BLL -> DAL):**
    *   Al guardar (`PreContratoController.CrearDesdeModal`), se invoca a `PreContratoServices.CrearDesdeModal`.
    *   Se crea una nueva entidad `PreContrato` con `Version = 1` y `Estado = 'Borrador'`. Se le asocia la cotización y la plantilla seleccionadas.
    *   **Crucial:** El contenido HTML completo generado en el paso anterior se guarda como un registro en la tabla `PreContratoParrafo`, vinculado al `PreContrato` recién creado.

#### Versionamiento de Contenido:

El sistema implementa un robusto versionamiento del contenido del documento, no solo de los metadatos.
*   **Flujo de Edición:** Cuando un usuario edita el contenido HTML de un pre-contrato existente (`PreContratoServices.ActualizarContenidoPreContrato`), el sistema no modifica el registro actual.
*   **Inactivación:** La versión actual del `PreContrato` se marca como `EstaActivo = false`.
*   **Nueva Versión:** Se crea un **nuevo** registro de `PreContrato` con un número de `Version` incrementado y `EstaActivo = true`. Se copian los metadatos de la versión anterior.
*   **Nuevo Contenido:** El nuevo contenido HTML se guarda en un nuevo registro de `PreContratoParrafo`, asociado a la nueva versión del `PreContrato`.
> **Implicación:** Esta estrategia garantiza una auditoría perfecta, permitiendo saber exactamente qué decía cada versión del documento en cualquier punto del tiempo.

#### Aprobación (El Gatillo Final):

*   El flujo de este módulo culmina con la acción `PreContratoController.Aprobar(id)`.
*   Esta acción llama a `_preContratoService.Aprobar(id)`, que simplemente actualiza el campo `Estado` del `PreContrato` a **"Aprobado"**.
*   Este cambio de estado es el **evento que habilita el último paso del proceso principal: la creación del `Contrato` final.**

---

### Paso 5: Creación del `Contrato` (Formalización)

Este paso representa la formalización final del acuerdo, probablemente mediante la carga de un documento escaneado y firmado.

#### Componentes Clave:

*   **Entidad Principal:** `Contrato`
*   **Punto de Entrada Web:** `ContratoController`
*   **Servicio (Lógica):** `ContratoService`
*   **Servicio de Almacenamiento:** `IStorageServices` (para guardar el archivo)

#### Flujo de Creación de un Contrato:

1.  **Disparador Manual (UI):** Un usuario inicia el proceso. La interfaz muestra una lista de Pre-Contratos con `Estado = "Aprobado"`, alimentada por `ContratoService.ListarPreContratosParaContrato()`.
2.  **Carga de Documento:** El usuario selecciona el pre-contrato correspondiente, rellena la fecha de firma y, lo más importante, sube un archivo (ej. el PDF del contrato firmado).
3.  **Lógica de Creación (`ContratoService.Crear`):
    *   El servicio primero crea el registro `Contrato` en la base de datos con los campos de archivo vacíos. El objetivo es obtener un `IdContrato` autoincremental.
    *   Utiliza el `IdContrato` para construir una ruta de almacenamiento única (ej. `contratos/123/`).
    *   Invoca a `_storageService.SubirStorage()` para subir el archivo a un almacenamiento en la nube (como Firebase Storage).
    *   Finalmente, **actualiza** el registro del `Contrato` con el `NombreArchivo` y la `RutaArchivo` (la URL devuelta por el servicio de storage).

---

### Paso 6: Creación del `Cliente` (El Eslabón Perdido)

Este es el paso final del flujo de negocio, donde la `Constructora` se convierte oficialmente en un `Cliente` de Tecmein.

#### Componentes Clave:

*   **Entidades:** `Cliente`, `Constructora`, `FormatoNumeroCliente`
*   **Servicio:** `ClienteServices`

#### Análisis del Flujo:

*   **Lógica Existente:** El `ClienteServices` contiene un método `Crear` que recibe un `SecConstructora`, verifica que no exista previamente como cliente, y crea un nuevo registro en la tabla `Cliente`, asignando un `NumeroCliente`.
*   **Desconexión del Flujo Principal:** Tras una revisión exhaustiva, se ha determinado que el `ContratoService` **no tiene dependencia de `IClienteServices` y no invoca su método `Crear`**. 

> **Conclusión Final del Flujo:** La creación de un `Cliente` **no es un proceso automático** que se dispara al crear un `Contrato`. Es una acción que debe ser realizada manualmente por un usuario en otra parte del sistema, o bien, es una integración que está planificada pero aún no implementada. El flujo de negocio principal queda en un estado de "Contrato Firmado", pero la conversión a cliente es un paso manual posterior.

---

**Análisis de Arquitectura Completado.**

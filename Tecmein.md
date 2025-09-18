## Tecmein Project Memories
- En el proyecto SolucionTecmein, la gestión de la entidad `Menu` es crítica para el layout. Un `Menu` con `SecMenuPadre = null` es un menú padre/raíz. El frontend (`Menu_Index.js`) envía `null` intencionadamente para estos casos, y el backend (`MenuServices.cs` y `MenuController.cs`) está diseñado para manejarlo así. El controlador devuelve los mensajes de error en una propiedad JSON llamada `mensajes`.
- El proyecto SolucionTecmein sigue una arquitectura en capas con fuerte uso de inyección de dependencias (DIP) y interfaces (ISP). Se busca mejorar el SRP refactorizando métodos y creando servicios transversales como ValidacionServices. ValidacionServices centraliza validaciones comunes (correo, teléfono Ecuador, nombres) para reutilización, inyectándose como IValidationServices en otros servicios de negocio. Esto promueve un código modular, limpio y extensible (OCP).
- Para el proyecto SolucionTecmein, se está implementando el módulo de 'Cotización'. Se ha completado el backend (Entidades, Servicios, Controlador) y el frontend básico (Vista, JS con CRUD) para este módulo. El flujo de trabajo acordado es que una Cotización se crea a partir de una Visita existente, se le asignan valores de compra y márgenes de ganancia a los equipos de la visita, y se guarda. El siguiente paso pendiente es analizar el documento de impuestos para implementar la Fase 3 (cálculo de impuestos).
- Plan de Acción para el proyecto Tecmein:
- **Fase 1: Módulo de Clientes.** Crear entidades `Cliente` y `FormatoNumeroCliente`, sus scripts SQL, servicios, vistas y pruebas. El objetivo es formalizar la conversión de `Constructora` a `Cliente`.
- **Fase 2: Módulo de Seguimiento.** Crear entidad `Seguimiento` (asociada a `Cotizacion`), su script SQL, servicio, UI y pruebas. La aceptación del seguimiento convierte la constructora en cliente.
- **Fase 3: Módulo de Pre-Contrato.** Crear entidades `PreContrato`, `FormaPago` y `PlantillaPreContrato`. Incluye lógica para generar un documento Word editable desde plantillas.
- **Fase 4: Módulo de Contrato.** Crear entidad `Contrato` para el documento final firmado, no editable.
- **Fase 5: Mejoras.** Completar UI de `Cotizacion` (cálculo de impuestos) y crear un CRUD para plantillas de correo.
- Plan de Acción para el módulo de Cotización (Cálculo de Impuestos):

Fase 1: Actualizaciones de Base de Datos y Entidades
*   Crear nuevas entidades: `Impuesto` (para los tipos de impuesto, con campos como `id`, `codigo`, `descripcion`, `porcentaje`, `valor_fijo`, `codigo_sri`, `tipo_impuesto`, `vigente`) e `ImpuestoCotizacion` (para vincular impuestos a una cotización específica, con campos como `id`, `cotizacion_id`, `impuesto_id`, `base_imponible`, `valor_impuesto`, `exento`).
*   Actualizar `Cotizacion` entidad: Añadir campos para `Subtotal` (suma de `Cotizacion detalle.Total`), `ValorImpuestos` (total de impuestos de la cotización), y `TotalConImpuestos` (total final).

Fase 2: Lógica de Backend (Servicios)
*   Crear `ImpuestoServices`: Para gestionar el CRUD de la entidad `Impuesto`.
*   Modificar `CotizacionServices`:
    *   En los métodos `Crear` y `Editar`:
        *   Calcular `Subtotal` para la `Cotizacion`.
        *   Implementar la lógica de cálculo de impuestos:
            *   Obtener impuestos aplicables de la tabla `Impuesto`.
            *   Aplicar impuestos al `Subtotal` para obtener `ValorImpuestos` y `TotalConImpuestos`.
            *   Almacenar el desglose de impuestos en `ImpuestoCotizacion`.
        *   Actualizar la entidad `Cotizacion` con `Subtotal`, `ValorImpuestos` y `TotalConImpuestos`.

Fase 3: Frontend (UI)
*   Actualizar `CotizacionVM`: Añadir propiedades correspondientes para `Subtotal`, `ValorImpuestos`, `TotalConImpuestos`.
*   Modificar `Cotizacion_Index.js` (o archivo UI relevante):
    *   Mostrar `Subtotal`, `ValorImpuestos`, `TotalConImpuestos`.
    *   Posiblemente añadir elementos UI para seleccionar impuestos aplicables si el sistema lo permite.
- La ruta a los archivos de log para el proyecto SolucionTecmein es C:\Proyectos\Tecmein\SO\SolucionTecmein\TecmeinAplicacionWeb\logs.
- En el proyecto SolucionTecmein, el manejo de decimales sigue un estándar global: el frontend estandariza a punto ('.') como separador decimal antes de enviar datos. En el backend, AutoMapper y JsonConvert deben usar CultureInfo.InvariantCulture para todas las conversiones entre string y decimal. Se debe evitar la conversión directa de decimal a string sin especificar InvariantCulture si el JS espera punto como decimal. El versionado de cotizaciones es híbrido: se crea una nueva versión (inactivando la anterior) solo si hay cambios significativos en el método Editar. Se usan SecUsuario, SecUsuarioModifica y SecCotizacionOriginal para trazabilidad. La validación 'Visita Activa' impide crear cotizaciones duplicadas para visitas activas. La configuración de DataTable requiere que el número de <th> en el HTML coincida con las columnas JS, y los renderizadores deben manejar nulos. Los diálogos de confirmación usan swal para acciones destructivas.
- El problema de duplicación de Cotizacion detalle y SecEquipoVisita nulo se debe a que el frontend no envía SecEquipoVisita al crear Cotizaciones. Esto causa que el campo sea nulo en la creación inicial, se propague en el versionado y genere duplicados en la sincronización. La solución más efectiva es que el frontend envíe SecEquipoVisita, o una refactorización compleja del backend.
- Plan de Acción para Tecmein (Pre-Contratos): Fase 1 (Seguridad Unificada): Crear tablas Permiso y RolPermiso, migrar Permisosrol y refactorizar el backend/UI para usar un sistema de permisos único basado en acciones y visibilidad de menús. Fase 2 (Admin Plantillas): Crear tablas PlantillaPreContrato, PlantillaPreContratoParrafo, DiccionarioParametro y el backend/UI para que un admin diseñe plantillas de forma modular. Fase 3 (Generación Pre-Contrato): Crear tablas PreContrato, PreContratoParrafo (copia editable), PreContratoHistorial y el backend/UI para generar, versionar (con historial de cambios de datos y texto), y exportar pre-contratos. Todos los scripts de BD deben generarse en la carpeta SQL/.
- El proyecto Tecmein usa una base de datos MySQL.
- El proyecto Tecmein se detuvo hoy. Nos encontramos en la Fase 1: Arquitectura de Seguridad Unificada, Paso 2.2: Crear la Interfaz de Administración de Permisos. Estábamos a punto de aplicar la corrección en TecmeindbContext.cs para solucionar el error 'MySqlException: Table \'\'tecmeindb.permisos\'\' doesn\'t exist' al acceder a /Rol/GestionarPermisos. La solución pendiente es añadir 'modelBuilder.Entity<Permiso>(entity => { entity.ToTable("permiso"); });' en TecmeindbContext.cs.
- El proceso actual se detuvo con los siguientes avances:
- El menú principal y los submenús se cargan correctamente, y el problema de duplicación ha sido resuelto.
- La interfaz de usuario de mantenimiento de permisos (GestionarPermisos.cshtml) ha sido actualizada para alinearse con el diseño general del sistema (estilo de tarjeta, JS externalizado, estilos de botones consistentes).
- El DataTable de la lista de permisos (Permiso/Index.cshtml) ahora carga correctamente, y su diseño también ha sido actualizado.
- Se identificó y solucionó el problema de acceso del usuario Administrador al mantenimiento de Roles. Se generó el script SQL `anadir_permiso_roles_administrar.sql` para añadir el permiso `Roles.Administrar` y asignarlo al rol de Administrador. El usuario debe ejecutar este script para completar la solución.
- Se completó una refactorización mayor del sistema de permisos en Tecmein. El nuevo sistema usa permisos genéricos (CREATE, READ, etc.) y específicos (Roles.Administrar). La visibilidad de los menús se controla con la tabla `rolmenu`. La lógica de login en `AccesoController` fue actualizada para generar `Claims` que reflejan esta nueva arquitectura (ej. `CONTACTO_READ`). El `MenuViewComponent` fue modificado para que si un submenú está permitido, sus padres se muestren automáticamente. Se rediseñó la UI de `GestionarPermisos` y se corrigieron las políticas de autorización en `Program.cs`. También se solucionó un bug en la página de Perfil.
- Los problemas pendientes relacionados con la MySqlException en TecmeindbContext.cs y la necesidad de ejecutar el script anadir_permiso_roles_administrar.sql ya fueron solucionados.
- Se completó la corrección del módulo de Mantenimiento de Menús en el proyecto Tecmein. Se solucionaron errores de JavaScript (SweetAlert), de base de datos (foreign key constraint y 'connection in use') y de compilación en el proyecto de pruebas. La lógica de eliminación ahora borra correctamente las dependencias en 'RolMenu' antes de eliminar el 'Menu'. El próximo paso es continuar con el plan de acción para Pre-Contratos, que se encuentra en la Fase 2: Administración de Plantillas.
- Para el proyecto SolucionTecmein, el módulo de plantillas de pre-contrato ha sido completamente alineado y compilado sin errores. La estructura final de las entidades PlantillaPreContrato y PlantillaPreContratoParrafo se ha ajustado al código BLL y de pruebas existente, incluyendo propiedades como SecPlantillaPreContrato, NumeracionInicial (string), FechaRegistro y EstaActivo. Se ha implementado la UI básica (controlador, vista Index, modal CRUD) y se han resuelto problemas de carga de DataTables, visualización de modales, guardado de registros y confirmación de eliminación con SweetAlert2. El proceso de pre-contratos se basa en esta estructura simplificada.
- Para el proyecto SolucionTecmein, el campo `Contenido` en la entidad `PlantillaPreContratoParrafo` está diseñado para almacenar HTML generado por un Editor de Texto Enriquecido (Rich Text Editor), lo cual es fundamental para el formato de los párrafos en las plantillas de pre-contrato.
- Para el proyecto SolucionTecmein, el estándar para CRUDs es usar modales y DataTables con respuestas `{ data: [...] }`. Se utiliza TinyMCE para texto enriquecido. Las claves de API de servicios de terceros (como TinyMCE) no deben estar en el cliente; se deben almacenar en `appsettings.json` y ser expuestas a través de un endpoint seguro en el backend.
- En el proyecto Tecmein, la integración de TinyMCE sigue un patrón de alta calidad: la API key se obtiene de un endpoint seguro del backend, el script del editor se carga dinámicamente, y la inicialización de la página espera a que la carga se complete mediante el callback 'onload'. El contenido se gestiona con las funciones get/setContent del editor.
- Para el proyecto Tecmein, las convenciones de nomenclatura de claves son: 1. Clave primaria numérica autoincremental: `Secuencial`. 2. Clave primaria de tipo string: `Codigo`. 3. Clave foránea: `Sec` + `[NombreTablaOrigen]`.
- El plan para la generación de documentos en Tecmein es un motor reutilizable. Usará 'Tipos de Documento' para clasificar plantillas, un servicio orquestador (IGeneradorDocumentoService) que delega a 'Estrategias' específicas (ej. EstrategiaPreContrato) para la lógica de cada documento, y un motor de reemplazo de parámetros genérico.
- En el proyecto SolucionTecmein, el módulo de 'Usuario' (`TecmeinAplicacionWeb\Controllers\UsuarioController.cs`, `TecmeinAplicacionWeb\Views\Usuario\Index.cshtml`, `TecmeinAplicacionWeb\Views\Usuario\_UsuarioModal.cshtml`, `TecmeinAplicacionWeb\wwwroot\js\Usuario_Index.js`) sirve como formato base y referencia de diseño para las interfaces de usuario CRUD. Este formato incluye el uso de DataTables para listados, modales para operaciones de creación/edición, llamadas AJAX para poblar desplegables y SweetAlert para diálogos de confirmación.
- Si el directorio de trabajo inicial es 'C:\Proyectos\Tecmein', debo enfocarme exclusivamente en las memorias y el contexto del proyecto Tecmein.
### Estado Actual del Proyecto (Guardado el 9 de septiembre de 2025)

**Módulo de Pre-Contrato:**

*   **Listado Principal:**
    *   La interfaz (`Index.cshtml`, `PreContrato_Index.js`) ha sido refactorizada siguiendo el estilo del módulo `Usuario`.
    *   Ahora muestra solo la **última versión activa** de cada pre-contrato.
    *   El botón "Cancelar" en la vista del editor funciona correctamente.
*   **Creación de Pre-Contrato (Flujo Opción B):**
    *   El modal "Nuevo" (`_PreContratoModal.cshtml`) ha sido actualizado para incluir todos los campos de la entidad `PreContrato` y un dropdown para "Seleccionar Plantilla".
    *   El JavaScript (`PreContrato_Index.js`) se encarga de poblar todos los dropdowns (cotizaciones aprobadas, formas de pago, plantillas) y de enviar todos los datos del formulario al backend.
    *   Se ha creado el `ViewModel` `PreContratoModalVM` y se ha configurado AutoMapper para su mapeo.
    *   El endpoint `CrearDesdeModal` en `PreContratoController` y su lógica en `PreContratoServices` están implementados para guardar los detalles del `PreContrato` y manejar el versionamiento.
*   **Modal del Editor de Contenido:**
    *   Se ha creado el modal `_EditorPreContratoModal.cshtml` para el editor TinyMCE.
    *   El archivo `PreContrato_EditorModal.js` se encarga de inicializar TinyMCE, obtener el contenido pre-populado (plantilla + placeholders) desde el endpoint `/PreContrato/PrevisualizarContenido/{id}` y manejar el guardado.
    *   `PreContrato_Index.js` llama a `abrirEditorModal` después de la creación exitosa del pre-contrato.
    *   La lógica para `ObtenerContenidoPrevisualizado` en `IPreContratoServices.cs` y `PreContratoServices.cs` está implementada (recupera datos, plantilla, parámetros y realiza el reemplazo).
    *   El endpoint `PrevisualizarContenido` en `PreContratoController.cs` está disponible.
*   **Funcionalidad de Historial:**
    *   El modal `_HistorialCambiosModal.cshtml` ha sido creado e incluido.
    *   `PreContrato_Index.js` abre el modal y carga los datos del historial desde `/PreContrato/Historial/{id}`.
    *   Se ha implementado la funcionalidad para ver el contenido de versiones históricas en un modal anidado (`/PreContrato/ContenidoParrafo/{id}`).
    *   Las interfaces y servicios (`IPreContratoServices.cs`, `PreContratoServices.cs`) y el controlador (`PreContratoController.cs`) para el historial están implementados.

**Tareas Pendientes (para continuar mañana):**

*   **Implementar la lógica del botón "Guardar Contenido"** dentro del editor (`/PreContrato/ActualizarContenido`). Esto implica actualizar el `PreContratoParrafo` con el contenido editado. (Esta era la tarea que estaba a punto de realizar).
*   **Implementar la funcionalidad del botón "Editar"** en el listado principal.
*   **Decidir sobre el botón "Ver Documento"** en el listado principal (si se mantiene o se elimina por redundancia con el historial).
*   **Limpiar archivos obsoletos:** Eliminar `Editor.cshtml` y `PreContrato_Editor.js`.
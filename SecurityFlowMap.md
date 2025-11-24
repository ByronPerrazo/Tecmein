## Control de Acceso por Acciones (Basado en Roles y Menús)

### Estrategia General

El control de acceso a las acciones del sistema se implementa principalmente en el **backend** utilizando un atributo de filtro personalizado `[ValidatePermission]`. El **frontend** se encarga de iniciar las solicitudes y de interpretar las respuestas del backend, mostrando mensajes de error adecuados en caso de denegación de permisos.

### Componentes Clave

1.  **Atributo `[ValidatePermission("ACCION_CRUD")]` (Backend - ASP.NET Core):**
    *   **Ubicación:** Aplicado a métodos de acción específicos en los controladores (ej., `VisitaController.CrearVisita`, `VisitaController.EditarVisita`, `VisitaController.Eliminar`).
    *   **Funcionalidad:** Este filtro intercepta las solicitudes HTTP. Antes de ejecutar la acción, utiliza el `AutorizacionService` para verificar si el usuario autenticado, a través de su `Rol` y el `Menú` asociado al `Controller` actual, posee el permiso para la `ACCION_CRUD` especificada.
    *   **Comportamiento:**
        *   Si el usuario tiene el permiso: La acción se ejecuta normalmente.
        *   Si el usuario NO tiene el permiso: La ejecución de la acción se detiene y se devuelve una respuesta HTTP con estado `403 Forbidden` (Acceso Denegado).

2.  **`AutorizacionService` (BLL - Servicio Central de Permisos):**
    *   Este servicio contiene la lógica central para determinar si un usuario tiene permiso.
    *   **Método Clave:** `Task<bool> TienePermiso(int secRol, string accion, string controllerName)`
    *   **Funcionamiento:**
        1.  Intenta recuperar el resultado del permiso desde una caché en memoria para mejorar el rendimiento.
        2.  Busca la entidad `Menu` asociada al `controllerName` proporcionado. **Es crucial que todo controlador que use `[ValidatePermission]` tenga una entrada correspondiente en la tabla `Menu` con su campo `Controlador` correctamente configurado.**
        3.  Busca la entrada en la tabla `RolMenu` que relaciona el `secRol` del usuario con el `secuencial` del `Menú` encontrado.
        4.  Evalúa la `accion` (que es el string pasado al atributo `[ValidatePermission]`) contra las columnas booleanas específicas en la entidad `RolMenu`.
        5.  **Acciones Reconocidas (case-insensitive):**
            *   `"VER_MENU"`: Verifica `rolMenu.VerMenu`
            *   `"CREAR"`: Verifica `rolMenu.Crear`
            *   `"LEER"`: Verifica `rolMenu.Leer`
            *   `"ACTUALIZAR"`: Verifica `rolMenu.Actualizar`
            *   `"ELIMINAR"`: Verifica `rolMenu.Eliminar`
        6.  Cualquier otra `accion` resultará en una denegación de permiso por defecto.
        7.  Almacena el resultado en caché por 5 minutos.

3.  **Manejo de `fetch` y `.catch()` (Frontend - JavaScript):**
    *   **Ubicación:** En las llamadas `fetch` que interactúan con las acciones protegidas del backend (ej., en `Visita_Index.js` para `CrearVisita`, `EditarVisita`, `Eliminar`).
    *   **Funcionalidad:** El frontend realiza la llamada `fetch` al backend. La clave está en cómo se maneja la promesa resultante:
        *   `response.ok ? response.json() : Promise.reject(response)`: Esta construcción asegura que si la respuesta HTTP no es exitosa (ej., un `403 Forbidden`), la promesa se rechaza con el objeto `response` completo.
        *   `.catch(error => { ... })`: Un bloque `catch` se encarga de interceptar las promesas rechazadas. Dentro de este bloque, se verifica el `error.status`.
    *   **Comportamiento:**
        *   Si `error.status === 403`: Se detecta un error de permiso. El frontend muestra un mensaje `Swal.fire("Acceso Denegado", "No tiene permisos para...", "error");`.
        *   Para otros errores: Se muestra un mensaje de error genérico o específico según el tipo de error.

### Flujo de Permisos (Ejemplo: Crear Visita)

1.  El usuario hace clic en el botón "Nuevo" en el frontend.
2.  El frontend (en `Visita_Index.js`) llama a `fetch("CrearVisita", { method: "POST", ... })`.
3.  La solicitud llega al backend y es interceptada por `[ValidatePermission("CREAR")]` en `VisitaController.CrearVisita`.
4.  El filtro `ValidatePermission` extrae el `RolId` del usuario y el `controllerName` ("Visita").
5.  Invoca a `AutorizacionService.TienePermiso(RolId, "CREAR", "Visita")`.
6.  `AutorizacionService` busca el `Menú` donde `Controlador` es "Visita".
7.  Luego busca la entrada en `RolMenu` para el `RolId` y `SecMenu` encontrado.
8.  Verifica si `rolMenu.Crear` es `true`.
    *   **Caso 1: Permiso concedido.** La acción `CrearVisita` se ejecuta, la visita se guarda en la DB, y el backend devuelve una respuesta exitosa (ej., 200 OK con el objeto de la visita creada). El frontend procesa el éxito.
    *   **Caso 2: Permiso denegado.** `AutorizacionService` devuelve `false`. El filtro `ValidatePermission` detiene la ejecución y devuelve un `403 Forbidden`.
9.  El frontend recibe la respuesta `403 Forbidden`. En el bloque `.catch()` de la llamada `fetch`, se detecta `error.status === 403`.
10. El frontend muestra un `Swal.fire("Acceso Denegado", "No tiene permisos para crear visitas.", "error");` al usuario.

### Ventajas de esta Estrategia

*   **Seguridad Centralizada y Granular:** La lógica de seguridad reside en el backend, ofreciendo un control detallado por cada acción CRUD y por cada elemento de menú.
*   **Consistencia:** Todos los puntos de entrada al backend protegidos con el atributo `[ValidatePermission]` utilizan el mismo mecanismo.
*   **Mantenibilidad:** Los permisos se gestionan en las tablas `Menu` y `RolMenu` de la base de datos, facilitando actualizaciones y configuraciones dinámicas.
*   **Rendimiento:** El uso de caché en memoria para los permisos mejora la velocidad de las verificaciones.
*   **Separación de Responsabilidades:** El frontend se enfoca en la UI/UX, y el backend en la lógica de negocio y seguridad.
*   **Validación de Menús:** La dependencia de la tabla `Menu` asegura que cada controlador protegido tenga un registro asociado, lo que puede ayudar a mantener la estructura de navegación y permisos sincronizada.
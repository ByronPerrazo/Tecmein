## Control de Acceso por Acciones (Basado en Permisos)

### Estrategia General

El control de acceso a las acciones del sistema se implementa principalmente en el **backend** utilizando un atributo de filtro personalizado `[ValidatePermission]`. El **frontend** se encarga de iniciar las solicitudes y de interpretar las respuestas del backend, mostrando mensajes de error adecuados en caso de denegación de permisos.

### Componentes Clave

1.  **Atributo `[ValidatePermission("NOMBRE_DEL_PERMISO")]` (Backend - ASP.NET Core):**
    *   **Ubicación:** Aplicado a métodos de acción específicos en los controladores (ej., `VisitaController.CrearVisita`, `VisitaController.EditarVisita`, `VisitaController.Eliminar`).
    *   **Funcionalidad:** Este filtro intercepta las solicitudes HTTP. Antes de ejecutar la acción, verifica si el usuario autenticado (a través de sus `Claims` y los servicios de permisos asociados a su rol) posee el permiso especificado (`NOMBRE_DEL_PERMISO`).
    *   **Comportamiento:**
        *   Si el usuario tiene el permiso: La acción se ejecuta normalmente.
        *   Si el usuario NO tiene el permiso: La ejecución de la acción se detiene y se devuelve una respuesta HTTP con estado `403 Forbidden` (Acceso Denegado).

2.  **Manejo de `fetch` y `.catch()` (Frontend - JavaScript):**
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
4.  El filtro `ValidatePermission` verifica si el rol del usuario tiene el permiso "CREAR".
    *   **Caso 1: Permiso concedido.** La acción `CrearVisita` se ejecuta, la visita se guarda en la DB, y el backend devuelve una respuesta exitosa (ej., 200 OK con el objeto de la visita creada). El frontend procesa el éxito.
    *   **Caso 2: Permiso denegado.** El filtro `ValidatePermission` detiene la ejecución y devuelve un `403 Forbidden`.
5.  El frontend recibe la respuesta `403 Forbidden`. En el bloque `.catch()` de la llamada `fetch`, se detecta `error.status === 403`.
6.  El frontend muestra un `Swal.fire("Acceso Denegado", "No tiene permisos para crear visitas.", "error");` al usuario.

### Ventajas de esta Estrategia

*   **Seguridad Centralizada:** La lógica de seguridad reside en el backend, que es más seguro y confiable.
*   **Consistencia:** Todos los puntos de entrada al backend están protegidos de la misma manera.
*   **Mantenibilidad:** Los permisos se gestionan en un solo lugar (el backend y la base de datos), facilitando actualizaciones.
*   **Separación de Responsabilidades:** El frontend se enfoca en la UI/UX, y el backend en la lógica de negocio y seguridad.
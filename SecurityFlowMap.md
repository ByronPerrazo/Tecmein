# Mapa de Flujo de Seguridad y Menú Dinámico - Tecmein

Este documento detalla el proceso técnico de autenticación, autorización, carga de permisos y construcción del menú de navegación del sistema Tecmein.

---

## Visión General

El sistema utiliza un modelo de seguridad híbrido y sofisticado:

1.  **Autenticación por Cookies:** La identidad del usuario se gestiona a través de una cookie de sesión estándar de .NET.
2.  **Autorización Basada en Claims:** En el momento del login, el sistema carga una lista exhaustiva de permisos en la cookie del usuario en forma de `Claims`. Las políticas de autorización generales se basan en estos claims.
3.  **Validación de Permisos en Tiempo Real:** Para acciones específicas y críticas, un `ActionFilter` personalizado (`ValidatePermissionAttribute`) realiza una consulta a la base de datos para validar el permiso en tiempo real, asegurando que los cambios en los permisos se apliquen instantáneamente.

---

## Paso 1: Autenticación (Login)

El proceso de inicio de sesión es el punto de partida que establece la identidad y los permisos del usuario para toda la sesión.

#### Componentes Clave:

*   **Punto de Entrada:** `AccesoController.Login()` (Vista y `[HttpPost]`)
*   **Servicio de Validación:** `UsuarioServices.ObtenerPorCredenciales()`

#### Flujo de Autenticación:

1.  **Envío de Credenciales:** El usuario envía su correo y clave desde la vista `Acceso/Login` al método `[HttpPost] Login` del `AccesoController`.
2.  **Validación en BD:** El controlador llama a `_usuarioServices.ObtenerPorCredenciales(correo, clave)`. Este servicio encripta la clave proporcionada con `SHA256` y busca en la tabla `Usuario` un registro que coincida tanto con el correo como con la clave encriptada.
3.  **Fallo:** Si no se encuentra ningún usuario, se devuelve la vista de Login con un mensaje de error.
4.  **Éxito:** Si el usuario es válido, el flujo continúa hacia el Paso 2 dentro del mismo método `Login`.

---

## Paso 2: Creación de Claims y Sesión

Una vez que el usuario es autenticado, el `AccesoController` se convierte en un orquestador para construir la identidad de la sesión del usuario.

#### Componentes Clave:

*   **Controlador Orquestador:** `AccesoController`
*   **Entidades Consultadas:** `RolPermiso`, `RolMenu`, `Menu`
*   **Resultado:** Una cookie de autenticación enriquecida con `Claims`.

#### Flujo de Carga de Permisos:

1.  **Claims Básicos:** Se crea una lista de `Claims` inicial con información básica: `Name` (nombre del usuario), `NameIdentifier` (ID del usuario), `Role` (ID del rol) y `UrlFoto`.
2.  **Carga de Permisos Directos:** El sistema consulta la tabla `RolPermiso` para obtener todos los permisos explícitos asignados al rol del usuario (ej. "CREATE", "DELETE", "Roles.Administrar"). Cada uno de estos permisos se añade a la lista como un `new Claim("Permission", "[NombreDelPermiso]")`.
3.  **Carga de Permisos de Menú:** Se consulta la tabla `RolMenu` para obtener todos los menús a los que el rol tiene acceso.
4.  **Creación de Permisos Compuestos:** El sistema itera sobre los menús obtenidos y los permisos directos. Si un rol tiene el permiso "CREATE" y acceso al menú "Visita" (cuyo controlador es `VisitaController`), el sistema **genera dinámicamente** un permiso compuesto y lo añade a la lista de claims: `new Claim("Permission", "VISITA_CREATE")`.
5.  **Creación de la Cookie:** Una vez que la lista de `Claims` está completa (conteniendo permisos básicos, directos y compuestos), se crea la identidad (`ClaimsIdentity`) y el `ClaimsPrincipal`. Finalmente, se invoca a `HttpContext.SignInAsync` para generar la cookie de autenticación encriptada y enviarla al navegador del usuario.

> **Implicación:** El usuario ahora lleva consigo todos sus permisos en cada petición que realiza. Esto permite una validación de permisos muy rápida para la mayoría de los casos, ya que no se necesita consultar la base de datos.

---

## Paso 3: Construcción del Menú Dinámico

El menú de navegación no es estático; se construye dinámicamente basándose en los permisos específicos de visualización del rol.

#### Componentes Clave:

*   **Servicio de Lógica:** `MenuServices.ObtieneMenu()`
*   **Entidades Consultadas:** `RolPermiso`, `Menu`

#### Flujo de Construcción:

1.  **Disparador:** Una vista (probablemente un `ViewComponent` en `_Layout.cshtml`) llama a `_menuServices.ObtieneMenu(idUsuario)`.
2.  **Obtención de Permisos de Vista:** El `MenuServices` **no** consulta `RolMenu`. En su lugar, consulta `RolPermiso` buscando todos los permisos para el rol del usuario que terminan en el sufijo `_VIEWMENU` (ej. `USUARIO_VIEWMENU`, `VISITA_VIEWMENU`).
3.  **Mapeo a Controladores:** El servicio extrae la raíz de estos permisos (ej. "USUARIO", "VISITA") y busca en la tabla `Menu` todos los menús cuyo campo `Controlador` coincida con estas raíces.
4.  **Construcción de Jerarquía:** Una vez que tiene la lista de menús a los que el usuario tiene permiso de ver, el servicio organiza la lista plana en una jerarquía de padres e hijos.
5.  **Renderizado:** La lista jerárquica final se devuelve a la vista para ser renderizada como el menú de navegación lateral.

> **Implicación:** Para que un usuario vea un elemento en el menú, no es suficiente con asignarle el menú en la pantalla de `RolMenu`. Es **indispensable** que su rol también tenga el permiso explícito `[NombreDelControlador]_VIEWMuco`.

---

## Paso 4: Validación de Permisos en Tiempo Real

Para acciones críticas, el sistema utiliza un mecanismo de validación que consulta la base de datos en el momento de la ejecución, garantizando que los cambios de permisos se apliquen de inmediato.

#### Componentes Clave:

*   **Atributo de Acción:** `ValidatePermissionAttribute`
*   **Servicio de Autorización:** `AutorizacionService` (inyectado en el atributo)

#### Flujo de Validación:

1.  **Decoración de la Acción:** Un método en un controlador está decorado con el atributo, por ejemplo: `[ValidatePermission("Roles.Administrar")]`.
2.  **Intercepción de la Petición:** Antes de que se ejecute el código de la acción, el `ActionFilter` `ValidatePermissionAttribute` intercepta la petición a través de su método `OnActionExecuting`.
3.  **Consulta a la BD:** El atributo invoca al `AutorizacionService` (que obtiene a través de inyección de dependencias del `HttpContext`).
4.  **Verificación:** El `AutorizacionService` ejecuta `TienePermiso(idRol, permisoRequerido)`, que realiza una consulta directa a la tabla `RolPermiso` para ver si el rol del usuario actual tiene el permiso específico requerido por el atributo (ej. "Roles.Administrar").
5.  **Resultado:**
    *   Si la consulta devuelve `true`, la ejecución de la acción del controlador continúa normalmente.
    *   Si devuelve `false`, el atributo corta el flujo y devuelve un `ForbidResult` (HTTP 403 Prohibido), impidiendo que la acción se ejecute.

> **Implicación:** Este mecanismo de doble capa (claims para autorización general y validación en tiempo real para acciones críticas) proporciona tanto eficiencia como seguridad y flexibilidad.

# Memoria del Proyecto: Tecmein
**Fecha última actualización:** 7 Mayo 2026
**Estado actual:** Fase de Optimización Arquitectónica

## 1. Hitos Alcanzados (Sesión Actual)

### 🎯 Auditoría Automática (SaveChangesInterceptor)
- **Implementación:** Se creó `AuditSaveChangesInterceptor` en el `DbContext`.
- **Automatización:** Asignación automática de `FechaRegistro`, `SecUsuario`, `FechaModificacion` y `SecUsuarioModifica` para todas las entidades que implementan `IAuditEntity`.
- **Estandarización:** Se definió la interfaz `IAuditEntity` e implementó en `Cotizacion` y `Visita`.

### 🎯 Validación Declarativa (FluentValidation)
- **Motor:** Integración de FluentValidation en la capa BLL.
- **Validadores:** Implementados `CotizacionValidator` y `PreContratoValidator` con reglas de integridad de negocio.
- **Registro:** Configurado escaneo automático de validadores en `IOC/Dependencia.cs`.

### 📄 Generación de Contratos (DOCX)
- **Motor de Reemplazo:** Se implementó una lógica agresiva en `PreContratoGeneratorService` que fuerza el merge de 'Runs' de Word cuando contienen fragmentos de marcadores (`{{...}}`). Esto soluciona la fragmentación del XML de Word que impedía el reemplazo.
- **Data Flow:** El controlador `PlantillaPreContratoController` ahora envía el contexto completo (`SecCotizacion`, `SecTipoDocumento`) permitiendo que los proveedores técnicos (Equipos, Ductos) recuperen la información real de la DB.
- **Marcadores:** Sincronizados y validados los campos técnicos como `{{diasdeentrega}}`, `{{fechafirmacontratoenletras}}` y `{{detalleinstalacionducto}}`.

### 🛡️ Seguridad y Aislamiento
- **Global Query Filters:** Implementado aislamiento de datos a nivel de `DbContext`.
- **IUserSession:** Se creó una infraestructura para inyectar la identidad del usuario y su rol directamente en la capa de datos.
- **Entidades Blindadas:** `Visita`, `Cotizacion`, `PreContrato` y `Contrato` ahora filtran automáticamente por el usuario creador, a menos que el rol sea 1 (Administrador).

---

## 2. Pendientes y Roadmap (Siguiente Sesión)

### 🎯 Punto 4: Optimización de Errores y Seguridad (Middleware)
- **Objetivo:** Refinar el Global Exception Handler actual para asegurar un formato JSON estándar en todas las capas.
- **Lógica:** Implementar un Middleware dedicado (o refinar el Filter) que oculte detalles técnicos en producción y exponga solo códigos de error seguros.

### 🎯 Punto 5: Cobertura de IAuditEntity
- **Objetivo:** Extender la implementación de `IAuditEntity` a todas las tablas maestras y de transacciones (Seguimientos, Usuarios, etc.).

---

## 3. Notas Técnicas Clave
- **Stack:** .NET 8 (con miras a .NET 9), MySQL, AutoMapper, EF Core, FluentValidation.
- **Arquitectura:** Clean Architecture con inyección de dependencias en `IOC/Dependencia.cs`.
- **Auditoría:** Los campos se inyectan mediante interceptor, desacoplando la lógica de los servicios BLL.
- **Filtro Global:** `modelBuilder.Entity<T>().HasQueryFilter(e => _userSession == null || _userSession.SecRol == 1 || e.SecUsuario == _userSession.SecUsuario)`.

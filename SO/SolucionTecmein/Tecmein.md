# Memoria del Proyecto: Tecmein
**Fecha última actualización:** 6 Mayo 2026
**Estado actual:** Fase de Optimización Arquitectónica

## 1. Hitos Alcanzados (Sesión Actual)

### 📄 Generación de Contratos (DOCX)
- **Motor de Reemplazo:** Se implementó una lógica agresiva en `PreContratoGeneratorService` que fuerza el merge de 'Runs' de Word cuando contienen fragmentos de marcadores (`{{...}}`). Esto soluciona la fragmentación del XML de Word que impedía el reemplazo.
- **Data Flow:** El controlador `PlantillaPreContratoController` ahora envía el contexto completo (`SecCotizacion`, `SecTipoDocumento`) permitiendo que los proveedores técnicos (Equipos, Ductos) recuperen la información real de la DB.
- **Marcadores:** Sincronizados y validados los campos técnicos como `{{diasdeentrega}}`, `{{fechafirmacontratoenletras}}` y `{{detalleinstalacionducto}}`.

### 🛡️ Seguridad y Aislamiento (Punto 1)
- **Global Query Filters:** Implementado aislamiento de datos a nivel de `DbContext`.
- **IUserSession:** Se creó una infraestructura para inyectar la identidad del usuario y su rol directamente en la capa de datos.
- **Entidades Blindadas:** `Visita`, `Cotizacion`, `PreContrato` y `Contrato` ahora filtran automáticamente por el usuario creador, a menos que el rol sea 1 (Administrador).
- **Refactorización BLL:** Se eliminaron los filtros manuales redundantes en `PreContratoServices.cs`, simplificando la lógica de negocio.

### ⚙️ Estabilidad
- **DesignTime Context:** Se corrigió la factoría de diseño para permitir migraciones y herramientas de EF Core sin errores de constructor.
- **AutoMapper:** Mapeos de DTO a VM para Cotizaciones y detalles completados.

---

## 2. Pendientes y Roadmap (Siguiente Sesión)

### 🎯 Punto 3: Auditoría Automática (Prioridad Alta)
- **Objetivo:** Implementar un `SaveChangesInterceptor` en EF Core.
- **Lógica:** Detectar entidades que hereden de una base común para asignar automáticamente `FechaRegistro`, `SecUsuarioCrea`, `FechaModificacion` y `SecUsuarioModifica` usando el `IUserSession`.

### 🎯 Punto 2: Validación Declarativa (FluentValidation)
- **Objetivo:** Extraer la lógica de validación de los servicios BLL.
- **Lógica:** Implementar validadores para `CotizacionDTO` y `PreContratoDTO` que se ejecuten automáticamente antes de persistir datos.

### 🎯 Punto 4: Optimización de Seguridad y Errores
- **Objetivo:** Global Exception Handler.
- **Lógica:** Implementar un Middleware que capture todas las excepciones y devuelva un formato JSON estándar (Standard API Result) para evitar fugas de información técnica al frontend.

---

## 3. Notas Técnicas Clave
- **Stack:** .NET 8 (con miras a .NET 9), MySQL, AutoMapper, EF Core.
- **Arquitectura:** Clean Architecture con inyección de dependencias en `IOC/Dependencia.cs`.
- **Filtro Global:** `modelBuilder.Entity<T>().HasQueryFilter(e => _userSession == null || _userSession.SecRol == 1 || e.SecUsuario == _userSession.SecUsuario)`.

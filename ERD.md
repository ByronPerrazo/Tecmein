# Diagrama Entidad-Relación (ERD) Textual - Proyecto Tecmein

Este documento describe la estructura de la base de datos del proyecto Tecmein en un formato textual, detallando las entidades, sus campos, claves y relaciones, basado en el código fuente de las entidades.

---

### Entidad: Canton
- **PK**: `Secuencial` (int)
- `SecProvincia` (int) - FK a Provincia
- `Codigo` (string)
- `Nombre` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Parroquia` (ICollection<Parroquia>)
    - `Provincia SecProvinciaNavigation` (Provincia)
    - `Visita` (ICollection<Visita>)

---

### Entidad: Catalogo
- **PK**: `Secuencial` (int)
- `Nombre` (string)
- `NombreArchivo` (string)
- `UrlCatalogo` (string)
- `FechaRegistro` (DateTime)
- `EstaActivo` (short)

---

### Entidad: Cliente
- **PK**: `SecCliente` (int)
- `SecConstructora` (int) - FK a Constructora
- `NumeroCliente` (string)
- `FechaCreacion` (DateTime)
- `EstaActivo` (bool)
- **Nav. Prop.**: 
    - `Constructora SecConstructoraNavigation` (Constructora)
    - `Contratos` (ICollection<Contrato>)

---

### Entidad: Constructora
- **PK**: `Secuencial` (int)
- `Nombre` (string)
- `Direccion` (string)
- `Telefono` (string)
- `Correo` (string)
- `Atencion` (string)
- `Administrador` (string)
- `TelefonoAdministrador` (string)
- `CorreoAdministrador` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Contactos` (ICollection<Contacto>)
    - `Cliente` (Cliente)
    - `Visitas` (ICollection<Visita>)

---

### Entidad: Contacto
- **PK**: `Secuencial` (int)
- `SecConstructora` (int) - FK a Constructora
- `Titulo` (string)
- `Nombres` (string)
- `Apellidos` (string)
- `Telefono` (string)
- `Correo` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Contactovisita` (ICollection<Contactovisita>)
    - `Constructora SecConstructoraNavigation` (Constructora)

---

### Entidad: Contactovisita
- **PK**: `Secuencial` (int)
- `SecContacto` (int) - FK a Contacto
- `SecVisita` (int) - FK a Visita
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Contacto SecContactoNavigation` (Contacto)
    - `Visita SecVisitaNavigation` (Visita)

---

### Entidad: Contrato
- **PK**: `IdContrato` (int)
- `IdCotizacion` (int) - FK a Cotizacion
- `SecCliente` (int) - FK a Cliente
- `FechaFirma` (DateTime)
- `IdUsuarioCarga` (int) - FK a Usuario
- `NombreArchivo` (string)
- `RutaArchivo` (string)
- `FechaCreacion` (DateTime)
- `EsActivo` (bool)
- **Nav. Prop.**: 
    - `Cotizacion IdCotizacionNavigation` (Cotizacion)
    - `Usuario IdUsuarioCargaNavigation` (Usuario)
    - `Cliente SecClienteNavigation` (Cliente)
    - `PlanDePagoNavigation` (PlanDePago)

---

### Entidad: Cotizacion
- **PK**: `Secuencial` (int)
- `SecVisita` (int) - FK a Visita
- `EnviadoProveedor` (bool)
- `EnviadoCliente` (bool)
- `Confirmacion` (bool)
- `Subtotal` (decimal)
- `ValorImpuestos` (decimal)
- `TotalConImpuestos` (decimal)
- `ValorIVA` (decimal)
- `ValorImportacion` (decimal)
- `EstaActivo` (short)
- `FechaRegistro` (DateTime)
- `FechaModificacion` (DateTime)
- `SecUsuario` (int) - FK a Usuario
- `SecCotizacionOriginal` (int) - FK a Cotizacion (auto-referencia)
- `SecUsuarioModifica` (int) - FK a Usuario
- **Nav. Prop.**: 
    - `Usuario SecUsuarioNavigation` (Usuario)
    - `Cotizacion SecCotizacionOriginalNavigation` (Cotizacion)
    - `Usuario SecUsuarioModificaNavigation` (Usuario)
    - `Visita SecVisitaNavigation` (Visita)
    - `Cotizaciondetalles` (ICollection<Cotizaciondetalle>)
    - `Seguimientos` (ICollection<Seguimiento>)
    - `ImpuestoCotizaciones` (ICollection<ImpuestoCotizacion>)

---

### Entidad: Cotizaciondetalle
- **PK**: `Secuencial` (int)
- `SecCotizacion` (int) - FK a Cotizacion
- `SecEquipoVisita` (int) - FK a Equiposvisita
- `DetalleEquipo` (string)
- `ValorCompra` (decimal)
- `MargenGanancia` (decimal)
- `Total` (decimal)
- `Cantidad` (int)
- `EstaActivo` (short)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `Cotizacion SecCotizacionNavigation` (Cotizacion)

---

### Entidad: Cuota
- **PK**: `IdCuota` (int)
- `IdPlanDePago` (int) - FK a PlanDePago
- `NumeroCuota` (int)
- `MontoEsperado` (decimal)
- `FechaVencimiento` (DateTime)
- `Estado` (string)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**:
    - `PlanDePago IdPlanDePagoNavigation` (PlanDePago)

---

### Entidad: DiccionarioParametro
- **PK**: `Secuencial` (int)
- `Parametro` (string)
- `Descripcion` (string)
- `EstaActivo` (bool)

---

### Entidad: Empresa
- **PK**: `Secuencial` (int)
- `UrlLogo` (string)
- `NombreLogo` (string)
- `Identificacion` (string)
- `Nombre` (string)
- `Correo` (string)
- `Direccion` (string)
- `Telefono` (string)
- `CodigoOperador` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Empresacorreo` (Empresacorreo)
    - `Empresastorage` (Empresastorage)
    - `FormatosNumeroCliente` (ICollection<FormatoNumeroCliente>)

---

### Entidad: Empresacorreo
- **PK**: `SecEmpresa` (int) - FK a Empresa
- `Email` (string)
- `Clave` (string)
- `Alias` (string)
- `Host` (string)
- `Puerto` (int)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Empresa SecEmpresaNavigation` (Empresa)

---

### Entidad: Empresastorage
- **PK**: `SecEmpresa` (int) - FK a Empresa
- `Email` (string)
- `Clave` (string)
- `Ruta` (string)
- `ApiKey` (string)
- `CarpetaUsuario` (string)
- `CarpetaProducto` (string)
- `CarpetaLogo` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Empresa SecEmpresaNavigation` (Empresa)

---

### Entidad: Equiposvisita
- **PK**: `Secuencial` (int)
- `SecVisita` (int) - FK a Visita
- `TipoEquipo` (string)
- `Sistema` (string)
- `Marca` (string)
- `Capacidad` (int)
- `Velocidad` (decimal)
- `SalaMaquinas` (string)
- `SalaControl` (string)
- `NumeroPersonas` (int)
- `NumeroParadas` (int)
- `NombresParadas` (string)
- `Embarque` (string)
- `TipoDucto` (string)
- `MedidasAfducto` (string)
- `TipoMotor` (string)
- `Foso` (int)
- `Recorrido` (int)
- `IngresosFrontales` (int)
- `IngresosPosteriores` (int)
- `SobreRecorrido` (int)
- `DimencionEntrada` (int)
- `AlturaEntrePisos` (int)
- `MaterialPuertas` (string)
- `Energia` (string)
- `Cantidad` (int)
- `EstaActivo` (short)

---

### Entidad: Etapa
- **PK**: `Id` (int)
- `Codigo` (string)
- `Descripcion` (string)
- `Orden` (int)
- `EstaActivo` (bool)

---

### Entidad: FormaPago
- **PK**: `SecFormaPago` (int)
- `Descripcion` (string)
- `EstaActivo` (short)

---

### Entidad: FormatoNumeroCliente
- **PK**: `SecFormatoNumeroCliente` (int)
- `SecEmpresa` (int) - FK a Empresa
- `UsaFormato` (bool)
- `Formato` (string)
- `NumeroInicio` (int)
- `LongitudNumero` (int)
- **Nav. Prop.**: 
    - `Empresa SecEmpresaNavigation` (Empresa)

---

### Entidad: Impuesto
- **PK**: `Id` (int)
- `Codigo` (string)
- `Descripcion` (string)
- `Porcentaje` (decimal)
- `ValorFijo` (decimal)
- `CodigoSri` (string)
- `SecTipoImpuesto` (int) - FK a TipoImpuesto
- `Vigente` (bool)
- `FechaCreacion` (DateTime)
- `FechaModificacion` (DateTime)
- **Nav. Prop.**: 
    - `TipoImpuesto SecTipoImpuestoNavigation` (TipoImpuesto)
    - `ImpuestoCotizacion` (ICollection<ImpuestoCotizacion>)

---

### Entidad: ImpuestoCotizacion
- **PK**: `Id` (int)
- `SecCotizacion` (int) - FK a Cotizacion
- `ImpuestoId` (int) - FK a Impuesto
- `BaseImponible` (decimal)
- `ValorImpuesto` (decimal)
- `Exento` (bool)
- `Observaciones` (string)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `Cotizacion SecCotizacionNavigation` (Cotizacion)
    - `Impuesto ImpuestoNavigation` (Impuesto)

---

### Entidad: Menu
- **PK**: `Secuencial` (int)
- `Descripcion` (string)
- `SecMenuPadre` (int) - FK a Menu (auto-referencia)
- `Icono` (string)
- `Controlador` (string)
- `PaginaAccion` (string)
- `EsActivo` (short)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `InverseSecMenuPadreNavigation` (ICollection<Menu>)
    - `Rolmenus` (ICollection<RolMenu>)
    - `Menu SecMenuPadreNavigation` (Menu)

---

### Entidad: Pago
- **PK**: `IdPago` (int)
- `IdPlanDePago` (int) - FK a PlanDePago
- `Monto` (decimal)
- `FechaPago` (DateTime)
- `ComprobanteUrl` (string)
- `ComprobanteNombre` (string)
- `RegistradoPorUsuarioId` (int) - FK a Usuario
- `EstaActivo` (bool)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**:
    - `PlanDePago IdPlanDePagoNavigation` (PlanDePago)
    - `Usuario RegistradoPorUsuario` (Usuario)

---

### Entidad: Parroquia
- **PK**: `Secuencial` (int)
- `SecCanton` (int) - FK a Canton
- `Codigo` (string)
- `Nombre` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Canton SecCantonNavigation` (Canton)
    - `Visita` (ICollection<Visita>)

---

### Entidad: Permiso
- **PK**: `IdPermiso` (string)
- `Descripcion` (string)
- **Nav. Prop.**: 
    - `RolPermisos` (ICollection<RolPermiso>)

---

### Entidad: Permisosrol
- **PK**: `Secuencial` (int)
- `SecRol` (int) - FK a Rol
- `SecUsuarioModifica` (int) - FK a Usuario
- `FechaRegistro` (DateTime)
- `Consultar` (short)
- `Modificar` (short)
- `Eliminar` (short)
- `Activo` (short)

---

### Entidad: PlanDePago
- **PK**: `IdPlanDePago` (int)
- `IdContrato` (int) - FK a Contrato
- `SecFormaPago` (int) - FK a FormaPago
- `ValorContrato` (decimal)
- `ValorAnticipo` (decimal)
- `FechaAnticipo` (DateTime)
- `NumeroCuotas` (int)
- `FechaPrimeraCuota` (DateTime)
- `EstaActivo` (bool)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**:
    - `Contrato IdContratoNavigation` (Contrato)
    - `FormaPago SecFormaPagoNavigation` (FormaPago)
    - `Pagos` (ICollection<Pago>)
    - `Cuotas` (ICollection<Cuota>)

---

### Entidad: PlantillaCorreo
- **PK**: `SecPlantillaCorreo` (int)
- `Nombre` (string)
- `Asunto` (string)
- `ContenidoHTML` (string)
- `EstaActivo` (short)
- `FechaRegistro` (DateTime)

---

### Entidad: PlantillaPreContrato
- **PK**: `SecPlantillaPreContrato` (int)
- `Nombre` (string)
- `NumeracionInicial` (string)
- `FechaRegistro` (DateTime)
- `EstaActivo` (int)
- `SecTipoDocumento` (int) - FK a TipoDocumento
- **Nav. Prop.**: 
    - `TipoDocumento SecTipoDocumentoNavigation` (TipoDocumento)
    - `PlantillaPreContratoParrafos` (ICollection<PlantillaPreContratoParrafo>)

---

### Entidad: PlantillaPreContratoParrafo
- **PK**: `SecPlantillaPreContratoParrafo` (int)
- `SecPlantillaPreContrato` (int) - FK a PlantillaPreContrato
- `Orden` (int)
- `Contenido` (string)
- `EstaActivo` (bool)
- **Nav. Prop.**: 
    - `PlantillaPreContrato SecPlantillaPreContratoNavigation` (PlantillaPreContrato)

---

### Entidad: PolizaGarantia
- **PK**: `Secuencial` (int)
- `Descripcion` (string)
- `EstaActivo` (bool)

---

### Entidad: PreContrato
- **PK**: `SecPreContrato` (int)
- `SecCotizacion` (int) - FK a Cotizacion
- `SecPlantillaPreContrato` (int) - FK a PlantillaPreContrato
- `SecUsuarioCrea` (int) - FK a Usuario
- `SecFormaPago` (int) - FK a FormaPago
- `Version` (int)
- `Estado` (string)
- `EstaActivo` (bool)
- `FechaRegistro` (DateTime)
- `Dias` (int)
- `TipoDias` (string)
- `ValorContrato` (decimal)
- `AniosGarantia` (int)
- `MesesGarantia` (int)
- `PeriodoMantenimiento` (string)
- `PolizaGarantia` (string)
- `ValorAnticipo` (decimal)
- `FechaAnticipo` (DateTime)
- `NumeroCuotas` (int)
- `FechaPrimeraCuota` (DateTime)
- **Nav. Prop.**: 
    - `Cotizacion SecCotizacionNavigation` (Cotizacion)
    - `PlantillaPreContrato SecPlantillaPreContratoNavigation` (PlantillaPreContrato)
    - `Usuario SecUsuarioCreaNavigation` (Usuario)
    - `FormaPago SecFormaPagoNavigation` (FormaPago)
    - `PreContratoParrafos` (ICollection<PreContratoParrafo>)

---

### Entidad: PreContratoParrafo
- **PK**: `Secuencial` (int)
- `SecPreContrato` (int) - FK a PreContrato
- `Contenido` (string)
- `Orden` (int)
- **Nav. Prop.**: 
    - `PreContrato SecPreContratoNavigation` (PreContrato)

---

### Entidad: Provincia
- **PK**: `Secuencial` (int)
- `Codigo` (string)
- `Nombre` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Cantons` (ICollection<Canton>)
    - `Visita` (ICollection<Visita>)

---

### Entidad: Rol
- **PK**: `Secuencial` (int)
- `Descripcion` (string)
- `EsActivo` (short)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `Rolmenus` (ICollection<RolMenu>)
    - `Usuarios` (ICollection<Usuario>)
    - `RolPermisos` (ICollection<RolPermiso>)

---

### Entidad: Rolmenu
- **PK**: `Secuencial` (int)
- `SecRol` (int) - FK a Rol
- `SecMenu` (int) - FK a Menu
- `EsActivo` (short)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `Menu SecMenuNavigation` (Menu)
    - `Rol SecRolNavigation` (Rol)

---

### Entidad: RolPermiso
- **PK**: `SecRol` (int), `IdPermiso` (string) - Clave compuesta
- `SecRol` (int) - FK a Rol
- `IdPermiso` (string) - FK a Permiso
- **Nav. Prop.**: 
    - `Rol Rol` (Rol)
    - `Permiso Permiso` (Permiso)

---

### Entidad: Seguimiento
- **PK**: `SecSeguimiento` (int)
- `SecCotizacion` (int) - FK a Cotizacion
- `Accion` (string)
- `Detalle` (string)
- `FechaAccion` (DateTime)
- `AceptacionCliente` (bool)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `Cotizacion SecCotizacionNavigation` (Cotizacion)

---

### Entidad: TipoDocumento
- **PK**: `SecTipoDocumento` (int)
- `Codigo` (string)
- `Descripcion` (string)
- `EstaActivo` (bool)
- `FechaRegistro` (DateTime)

---

### Entidad: TipoImpuesto
- **PK**: `Secuencial` (int)
- `Nombre` (string)
- `EsIva` (bool)
- `EsImportacion` (bool)
- `EstaActivo` (bool)
- `FechaCreacion` (DateTime)
- `FechaModificacion` (DateTime)

---

### Entidad: Usuario
- **PK**: `Secuencial` (int)
- `Nombre` (string)
- `Correo` (string)
- `Telefono` (string)
- `SecRol` (int) - FK a Rol
- `UrlFoto` (string)
- `NombreFoto` (string)
- `Clave` (string)
- `EsActivo` (short)
- `FechaRegistro` (DateTime)
- **Nav. Prop.**: 
    - `Rol SecRolNavigation` (Rol)
    - `Visita` (ICollection<Visita>)

---

### Entidad: Visita
- **PK**: `Secuencial` (int)
- `SecUsuario` (int) - FK a Usuario
- `SecProvincia` (int) - FK a Provincia
- `SecCanton` (int) - FK a Canton
- `SecParroquia` (int) - FK a Parroquia
- `IdEtapa` (int) - FK a Etapa
- `SecEmpresa` (int) - FK a Empresa
- `SecConstructora` (int) - FK a Constructora
- `Nombre` (string)
- `Direccion` (string)
- `FechaRegistro` (DateTime)
- `FechaSiguienteVisita` (DateTime)
- `GeoUbicacion` (string)
- `Detalle` (string)
- `EstaActivo` (short)
- **Nav. Prop.**: 
    - `Canton SecCantonNavigation` (Canton)
    - `Parroquia SecParroquiaNavigation` (Parroquia)
    - `Provincia SecProvinciaNavigation` (Provincia)
    - `Usuario SecUsuarioNavigation` (Usuario)
    - `Contactovisita` (ICollection<Contactovisita>)
    - `Etapa IdEtapaNavigation` (Etapa)
    - `Empresa SecEmpresaNavigation` (Empresa)
    - `Constructora SecConstructoraNavigation` (Constructora)

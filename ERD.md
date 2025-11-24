# Diagrama Entidad-Relación (ERD) Textual - Proyecto Tecmein

Este documento describe la estructura de la base de datos del proyecto Tecmein en un formato textual, detallando las entidades, sus campos, claves y relaciones, basado en el esquema actual de la base de datos MySQL.

---

### Entidad: canton
- **PK**: `secuencial` (int)
- `sec_provincia` (int) - FK a provincia
- `codigo` (varchar(20))
- `nombre` (varchar(100))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `parroquia` (ICollection<parroquia>)
    - `provincia_sec_provincia_navigation` (provincia)
    - `visita` (ICollection<visita>)

---

### Entidad: catalogo
- **PK**: `secuencial` (int)
- `nombre` (varchar(255))
- `nombre_archivo` (varchar(255))
- `url_catalogo` (varchar(255))
- `fecha_registro` (datetime)
- `esta_activo` (smallint)

---

### Entidad: cliente
- **PK**: `sec_cliente` (int)
- `sec_constructora` (int) - FK a constructora
- `numero_cliente` (varchar(50))
- `fecha_creacion` (datetime)
- `esta_activo` (tinyint(1))
- **Nav. Prop.**: 
    - `constructora_sec_constructora_navigation` (constructora)
    - `contratos` (ICollection<contrato>)

---

### Entidad: constructora
- **PK**: `secuencial` (int)
- `nombre` (varchar(255))
- `direccion` (varchar(255))
- `telefono` (varchar(50))
- `correo` (varchar(255))
- `atencion` (varchar(255))
- `administrador` (varchar(255))
- `telefono_administrador` (varchar(50))
- `correo_administrador` (varchar(255))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `contactos` (ICollection<contacto>)
    - `cliente` (cliente)
    - `visitas` (ICollection<visita>)

---

### Entidad: contacto
- **PK**: `secuencial` (int)
- `sec_constructora` (int) - FK a constructora
- `titulo` (varchar(50))
- `nombres` (varchar(100))
- `apellidos` (varchar(100))
- `telefono` (varchar(50))
- `correo` (varchar(255))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `contactovisita` (ICollection<contactovisita>)
    - `constructora_sec_constructora_navigation` (constructora)

---

### Entidad: contactovisita
- **PK**: `secuencial` (int)
- `sec_contacto` (int) - FK a contacto
- `sec_visita` (int) - FK a visita
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `contacto_sec_contacto_navigation` (contacto)
    - `visita_sec_visita_navigation` (visita)

---

### Entidad: contrato
- **PK**: `id_contrato` (int)
- `id_cotizacion` (int) - FK a cotizacion
- `fecha_firma` (datetime)
- `id_usuario_carga` (int) - FK a usuario
- `nombre_archivo` (varchar(255))
- `ruta_archivo` (varchar(1024))
- `fecha_creacion` (datetime)
- `es_activo` (bit(1))
- `sec_cliente` (int) - FK a cliente
- **Nav. Prop.**: 
    - `cotizacion_id_cotizacion_navigation` (cotizacion)
    - `usuario_id_usuario_carga_navigation` (usuario)
    - `cliente_sec_cliente_navigation` (cliente)
    - `plan_de_pago_navigation` (plan_de_pago)

---

### Entidad: cotizacion
- **PK**: `secuencial` (int)
- `sec_visita` (int) - FK a visita
- `enviado_proveedor` (bit(1))
- `enviado_cliente` (bit(1))
- `confirmacion` (bit(1))
- `esta_activo` (smallint)
- `fecha_registro` (datetime)
- `fecha_modificacion` (datetime)
- `subtotal` (decimal(18,2))
- `valor_impuestos` (decimal(18,2))
- `total_con_impuestos` (decimal(18,2))
- `valor_iva` (decimal(18,2))
- `valor_importacion` (decimal(18,2))
- `sec_usuario` (int) - FK a usuario
- `sec_cotizacion_original` (int) - FK a cotizacion (auto-referencia)
- `sec_usuario_modifica` (int) - FK a usuario
- **Nav. Prop.**: 
    - `usuario_sec_usuario_navigation` (usuario)
    - `cotizacion_sec_cotizacion_original_navigation` (cotizacion)
    - `usuario_sec_usuario_modifica_navigation` (usuario)
    - `visita_sec_visita_navigation` (visita)
    - `cotizaciondetalles` (ICollection<cotizaciondetalle>)
    - `seguimientos` (ICollection<seguimiento>)
    - `impuesto_cotizaciones` (ICollection<impuesto_cotizacion>)

---

### Entidad: cotizaciondetalle
- **PK**: `secuencial` (int)
- `sec_cotizacion` (int) - FK a cotizacion
- `sec_equipo_visita` (int) - FK a equiposvisita
- `detalle_equipo` (varchar(255))
- `valor_compra` (decimal(18,2))
- `margen_ganancia` (decimal(18,2))
- `total` (decimal(18,2))
- `cantidad` (int)
- `esta_activo` (smallint)
- `fecha_registro` (datetime)
- **Nav. Prop.**: 
    - `cotizacion_sec_cotizacion_navigation` (cotizacion)

---

### Entidad: cuota
- **PK**: `id_cuota` (int)
- `id_plan_de_pago` (int) - FK a plan_de_pago
- `numero_cuota` (int)
- `monto_esperado` (decimal(18,2))
- `fecha_vencimiento` (datetime)
- `estado` (varchar(50))
- `fecha_registro` (datetime)
- `monto_pagado` (decimal(18,2))
- **Nav. Prop.**:
    - `plan_de_pago_id_plan_de_pago_navigation` (plan_de_pago)

---

### Entidad: diccionario_parametro
- **PK**: `secuencial` (int)
- `parametro` (varchar(255))
- `descripcion` (varchar(255))
- `esta_activo` (tinyint(1))

---

### Entidad: empresa
- **PK**: `secuencial` (int)
- `url_logo` (varchar(255))
- `nombre_logo` (varchar(255))
- `identificacion` (varchar(50))
- `nombre` (varchar(255))
- `correo` (varchar(255))
- `direccion` (varchar(255))
- `telefono` (varchar(50))
- `codigo_operador` (varchar(50))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `empresacorreo` (empresacorreo)
    - `empresastorage` (empresastorage)
    - `formatos_numero_cliente` (ICollection<formato_numero_cliente>)

---

### Entidad: empresacorreo
- **PK**: `sec_empresa` (int) - FK a empresa
- `email` (varchar(255))
- `clave` (varchar(255))
- `alias` (varchar(255))
- `host` (varchar(255))
- `puerto` (int)
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `empresa_sec_empresa_navigation` (empresa)

---

### Entidad: empresastorage
- **PK**: `sec_empresa` (int) - FK a empresa
- `email` (varchar(255))
- `clave` (varchar(255))
- `ruta` (varchar(255))
- `api_key` (varchar(255))
- `carpeta_usuario` (varchar(255))
- `carpeta_producto` (varchar(255))
- `carpeta_logo` (varchar(255))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `empresa_sec_empresa_navigation` (empresa)

---

### Entidad: equiposvisita
- **PK**: `secuencial` (int)
- `sec_visita` (int) - FK a visita
- `tipo_equipo` (varchar(100))
- `sistema` (varchar(100))
- `marca` (varchar(100))
- `capacidad` (int)
- `velocidad` (decimal(18,2))
- `sala_maquinas` (varchar(100))
- `sala_control` (varchar(100))
- `numero_personas` (int)
- `numero_paradas` (int)
- `nombres_paradas` (varchar(255))
- `embarque` (varchar(100))
- `tipo_ducto` (varchar(100))
- `medidas_afducto` (varchar(100))
- `tipo_motor` (varchar(100))
- `foso` (int)
- `recorrido` (int)
- `ingresos_frontales` (int)
- `ingresos_posteriores` (int)
- `sobre_recorrido` (int)
- `dimencion_entrada` (int)
- `altura_entre_pisos` (int)
- `material_puertas` (varchar(100))
- `energia` (varchar(100))
- `cantidad` (int)
- `esta_activo` (smallint)

---

### Entidad: etapa
- **PK**: `id` (int)
- `codigo` (varchar(20))
- `descripcion` (varchar(255))
- `orden` (int)
- `esta_activo` (tinyint(1))

---

### Entidad: forma_pago
- **PK**: `sec_forma_pago` (int)
- `descripcion` (varchar(255))
- `esta_activo` (smallint)

---

### Entidad: formato_numero_cliente
- **PK**: `sec_formato_numero_cliente` (int)
- `sec_empresa` (int) - FK a empresa
- `usa_formato` (tinyint(1))
- `formato` (varchar(50))
- `numero_inicio` (int)
- `longitud_numero` (int)
- **Nav. Prop.**: 
    - `empresa_sec_empresa_navigation` (empresa)

---

### Entidad: impuesto
- **PK**: `id` (int)
- `codigo` (varchar(20))
- `descripcion` (varchar(255))
- `porcentaje` (decimal(18,2))
- `valor_fijo` (decimal(18,2))
- `codigo_sri` (varchar(50))
- `sec_tipo_impuesto` (int) - FK a tipo_impuesto
- `vigente` (tinyint(1))
- `fecha_creacion` (datetime)
- `fecha_modificacion` (datetime)
- **Nav. Prop.**: 
    - `tipo_impuesto_sec_tipo_impuesto_navigation` (tipo_impuesto)
    - `impuesto_cotizacion` (ICollection<impuesto_cotizacion>)

---

### Entidad: impuesto_cotizacion
- **PK**: `id` (int)
- `sec_cotizacion` (int) - FK a cotizacion
- `impuesto_id` (int) - FK a impuesto
- `base_imponible` (decimal(18,2))
- `valor_impuesto` (decimal(18,2))
- `exento` (tinyint(1))
- `observaciones` (varchar(255))
- `fecha_registro` (datetime)
- **Nav. Prop.**: 
    - `cotizacion_sec_cotizacion_navigation` (cotizacion)
    - `impuesto_impuesto_navigation` (impuesto)

---

### Entidad: menu
- **PK**: `secuencial` (int)
- `descripcion` (varchar(130))
- `sec_menu_padre` (int) - FK a menu (auto-referencia)
- `icono` (varchar(30))
- `controlador` (varchar(130))
- `pagina_accion` (varchar(130))
- `es_activo` (smallint)
- `fecha_registro` (datetime)
- `mostrar_en_menu` (bit(1))
- `orden` (int)
- **Nav. Prop.**: 
    - `inverse_sec_menu_padre_navigation` (ICollection<menu>)
    - `rolmenus` (ICollection<rol_menu>)
    - `menu_sec_menu_padre_navigation` (menu)

---

### Entidad: pago
- **PK**: `id_pago` (int)
- `id_plan_de_pago` (int) - FK a plan_de_pago
- `monto` (decimal(18,2))
- `fecha_pago` (datetime)
- `comprobante_url` (varchar(255))
- `comprobante_nombre` (varchar(255))
- `registrado_por_usuario_id` (int) - FK a usuario
- `esta_activo` (tinyint(1))
- `fecha_registro` (datetime)
- **Nav. Prop.**:
    - `plan_de_pago_id_plan_de_pago_navigation` (plan_de_pago)
    - `usuario_registrado_por_usuario` (usuario)

---

### Entidad: parroquia
- **PK**: `secuencial` (int)
- `sec_canton` (int) - FK a canton
- `codigo` (varchar(20))
- `nombre` (varchar(100))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `canton_sec_canton_navigation` (canton)
    - `visita` (ICollection<visita>)

---

### Entidad: permiso
- **PK**: `id_permiso` (varchar(100))
- `descripcion` (varchar(255))
- **Nav. Prop.**: 
    - `rol_permisos` (ICollection<rol_permiso>)

---

### Entidad: permisosrol
- Esta tabla parece estar obsoleta o ser un remanente. La implementación actual de permisos utiliza `rol_permiso` y `rol_menu`.
- **PK**: `secuencial` (int)
- `sec_rol` (int) - FK a rol
- `sec_usuario_modifica` (int) - FK a usuario
- `fecha_registro` (datetime)
- `consultar` (smallint)
- `modificar` (smallint)
- `eliminar` (smallint)
- `activo` (smallint)

---

### Entidad: plan_de_pago
- **PK**: `id_plan_de_pago` (int)
- `id_contrato` (int) - FK a contrato
- `sec_forma_pago` (int) - FK a forma_pago
- `valor_contrato` (decimal(18,2))
- `valor_anticipo` (decimal(18,2))
- `fecha_anticipo` (datetime)
- `numero_cuotas` (int)
- `fecha_primera_cuota` (datetime)
- `esta_activo` (tinyint(1))
- `fecha_registro` (datetime)
- `fecha_modificacion` (datetime(6))
- **Nav. Prop.**:
    - `contrato_id_contrato_navigation` (contrato)
    - `forma_pago_sec_forma_pago_navigation` (forma_pago)
    - `pagos` (ICollection<pago>)
    - `cuotas` (ICollection<cuota>)

---

### Entidad: plantilla_correo
- **PK**: `sec_plantilla_correo` (int)
- `nombre` (varchar(255))
- `asunto` (varchar(255))
- `contenido_html` (text)
- `esta_activo` (smallint)
- `fecha_registro` (datetime)

---

### Entidad: plantilla_pre_contrato
- **PK**: `sec_plantilla_pre_contrato` (int)
- `nombre` (varchar(255))
- `numeracion_inicial` (varchar(50))
- `fecha_registro` (datetime)
- `esta_activo` (int)
- `sec_tipo_documento` (int) - FK a tipo_documento
- **Nav. Prop.**: 
    - `tipo_documento_sec_tipo_documento_navigation` (tipo_documento)
    - `plantilla_pre_contrato_parrafos` (ICollection<plantilla_pre_contrato_parrafo>)

---

### Entidad: plantilla_pre_contrato_parrafo
- **PK**: `sec_plantilla_pre_contrato_parrafo` (int)
- `sec_plantilla_pre_contrato` (int) - FK a plantilla_pre_contrato
- `orden` (int)
- `contenido` (text)
- `esta_activo` (tinyint(1))
- **Nav. Prop.**: 
    - `plantilla_pre_contrato_sec_plantilla_pre_contrato_navigation` (plantilla_pre_contrato)

---

### Entidad: poliza_garantia
- **PK**: `secuencial` (int)
- `descripcion` (varchar(255))
- `esta_activo` (tinyint(1))

---

### Entidad: pre_contrato
- **PK**: `sec_pre_contrato` (int)
- `sec_cotizacion` (int) - FK a cotizacion
- `sec_plantilla_pre_contrato` (int) - FK a plantilla_pre_contrato
- `sec_usuario_crea` (int) - FK a usuario
- `sec_forma_pago` (int) - FK a forma_pago
- `version` (int)
- `estado` (varchar(50))
- `esta_activo` (tinyint(1))
- `fecha_registro` (datetime)
- `dias` (int)
- `tipo_dias` (varchar(50))
- `valor_contrato` (decimal(18,2))
- `anios_garantia` (int)
- `meses_garantia` (int)
- `periodo_mantenimiento` (varchar(50))
- `poliza_garantia` (varchar(255))
- `valor_anticipo` (decimal(18,2))
- `fecha_anticipo` (datetime)
- `numero_cuotas` (int)
- `fecha_primera_cuota` (datetime)
- **Nav. Prop.**: 
    - `cotizacion_sec_cotizacion_navigation` (cotizacion)
    - `plantilla_pre_contrato_sec_plantilla_pre_contrato_navigation` (plantilla_pre_contrato)
    - `usuario_sec_usuario_crea_navigation` (usuario)
    - `forma_pago_sec_forma_pago_navigation` (forma_pago)
    - `pre_contrato_parrafos` (ICollection<pre_contrato_parrafo>)

---

### Entidad: pre_contrato_parrafo
- **PK**: `secuencial` (int)
- `sec_pre_contrato` (int) - FK a pre_contrato
- `contenido` (text)
- `orden` (int)
- **Nav. Prop.**: 
    - `pre_contrato_sec_pre_contrato_navigation` (pre_contrato)

---

### Entidad: provincia
- **PK**: `secuencial` (int)
- `codigo` (varchar(20))
- `nombre` (varchar(100))
- `esta_activo` (smallint)
- **Nav. Prop.**: 
    - `cantons` (ICollection<canton>)
    - `visita` (ICollection<visita>)

---

### Entidad: rol
- **PK**: `secuencial` (int)
- `descripcion` (varchar(150))
- `es_activo` (smallint)
- `fecha_registro` (datetime)
- **Nav. Prop.**: 
    - `rolmenus` (ICollection<rol_menu>)
    - `usuarios` (ICollection<usuario>)
    - `rol_permisos` (ICollection<rol_permiso>)

---

### Entidad: rol_menu
- **PK**: `sec_rol` (int), `sec_menu` (int) - Clave compuesta
- `sec_rol` (int) - FK a rol
- `sec_menu` (int) - FK a menu
- `actualizar` (tinyint(1))
- `crear` (tinyint(1))
- `eliminar` (tinyint(1))
- `leer` (tinyint(1))
- `ver_menu` (tinyint(1))
- **Nav. Prop.**: 
    - `menu_sec_menu_navigation` (menu)
    - `rol_sec_rol_navigation` (rol)

---

### Entidad: rol_permiso
- **PK**: `sec_rol` (int), `id_permiso` (varchar(100)) - Clave compuesta
- `sec_rol` (int) - FK a rol
- `id_permiso` (varchar(100)) - FK a permiso
- **Nav. Prop.**: 
    - `rol` (rol)
    - `permiso` (permiso)

---

### Entidad: seguimiento
- **PK**: `sec_seguimiento` (int)
- `sec_cotizacion` (int) - FK a cotizacion
- `accion` (varchar(255))
- `detalle` (text)
- `fecha_accion` (datetime)
- `aceptacion_cliente` (tinyint(1))
- `fecha_registro` (datetime)
- **Nav. Prop.**: 
    - `cotizacion_sec_cotizacion_navigation` (cotizacion)

---

### Entidad: tipo_documento
- **PK**: `sec_tipo_documento` (int)
- `codigo` (varchar(50))
- `descripcion` (varchar(255))
- `esta_activo` (tinyint(1))
- `fecha_registro` (datetime)

---

### Entidad: tipo_impuesto
- **PK**: `secuencial` (int)
- `nombre` (varchar(255))
- `es_iva` (tinyint(1))
- `es_importacion` (tinyint(1))
- `esta_activo` (tinyint(1))
- `fecha_creacion` (datetime)
- `fecha_modificacion` (datetime)

---

### Entidad: usuario
- **PK**: `secuencial` (int)
- `nombre` (varchar(150))
- `correo` (varchar(150))
- `telefono` (varchar(50))
- `sec_rol` (int) - FK a rol
- `url_foto` (varchar(500))
- `nombre_foto` (varchar(100))
- `clave` (varchar(255))
- `es_activo` (smallint)
- `fecha_registro` (datetime)
- **Nav. Prop.**: 
    - `rol_sec_rol_navigation` (rol)
    - `visita` (ICollection<visita>)

---

### Entidad: visita
- **PK**: `secuencial` (int)
- `sec_usuario` (int) - FK a usuario
- `sec_provincia` (int) - FK a provincia
- `sec_canton` (int) - FK a canton
- `sec_parroquia` (int) - FK a parroquia
- `nombre` (varchar(50))
- `direccion` (varchar(250))
- `fecha_registro` (datetime)
- `fecha_siguiente_visita` (datetime)
- `geo_ubicacion` (varchar(250))
- `detalle` (varchar(500))
- `id_etapa` (int) - FK a etapa
- `sec_empresa` (int) - FK a empresa
- `esta_activo` (smallint)
- `sec_constructora` (int) - FK a constructora
- **Nav. Prop.**: 
    - `canton_sec_canton_navigation` (canton)
    - `parroquia_sec_parroquia_navigation` (parroquia)
    - `provincia_sec_provincia_navigation` (provincia)
    - `usuario_sec_usuario_navigation` (usuario)
    - `contactovisita` (ICollection<contactovisita>)
    - `etapa_id_etapa_navigation` (etapa)
    - `empresa_sec_empresa_navigation` (empresa)
    - `constructora_sec_constructora_navigation` (constructora)

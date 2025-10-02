using Entity;
using Microsoft.EntityFrameworkCore;

namespace DAL.DBContext;


public partial class TecmeindbContext : DbContext
{
    public TecmeindbContext(DbContextOptions<TecmeindbContext> options)
       : base(options)
    {
    }


    public virtual DbSet<Constructora> Constructoras { get; set; }

    public virtual DbSet<Catalogo> Catalogos { get; set; }

    public virtual DbSet<Canton> Cantones { get; set; }

    public virtual DbSet<Contacto> Contactos { get; set; }

    public virtual DbSet<Contactovisita> Contactovista { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Empresacorreo> Empresacorreos { get; set; }

    public virtual DbSet<Empresastorage> Empresastorages { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Parroquia> Parroquia { get; set; }

    public virtual DbSet<Provincia> Provincia { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<RolMenu> RolMenus { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Visita> Visita { get; set; }

    public virtual DbSet<Equiposvisita> Equiposvisita { get; set; }
    public virtual DbSet<Permisosrol> Permisosrols { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<FormatoNumeroCliente> FormatoNumeroClientes { get; set; }

    public virtual DbSet<Seguimiento> Seguimientos { get; set; }

    public virtual DbSet<Cotizacion> Cotizacion { get; set; }

    public virtual DbSet<PreContrato> PreContratos { get; set; }
    public virtual DbSet<FormaPago> FormasPago { get; set; }
    public virtual DbSet<PlantillaPreContrato> PlantillaPreContratos { get; set; }
    public virtual DbSet<PlantillaPreContratoParrafo> PlantillaPreContratoParrafos { get; set; }

    public virtual DbSet<Contrato> Contratos { get; set; }

    public virtual DbSet<Etapa> Etapas { get; set; }

    public virtual DbSet<Impuesto> Impuestos { get; set; }

    public virtual DbSet<ImpuestoCotizacion> ImpuestoCotizacion { get; set; }

    public virtual DbSet<TipoImpuesto> TipoImpuestos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }
    public virtual DbSet<RolPermiso> RolPermisos { get; set; }

    public virtual DbSet<TipoDocumento> TipoDocumentos { get; set; }

    public virtual DbSet<PolizaGarantia> PolizaGarantia { get; set; }

    public virtual DbSet<PlanDePago> PlanesDePago { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Cuota> Cuotas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Etapa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("etapa");

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(5)
                .HasColumnName("Codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasColumnName("Descripcion");
            entity.Property(e => e.Orden).HasColumnName("Orden");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
        });

        modelBuilder.Entity<Constructora>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("constructora");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Administrador)
                .HasMaxLength(150)
                .HasColumnName("administrador");
            entity.Property(e => e.Atencion)
                .HasMaxLength(150)
                .HasColumnName("atencion");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.CorreoAdministrador)
                .HasMaxLength(150)
                .HasColumnName("correoAdministrador");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.TelefonoAdministrador)
                .HasMaxLength(10)
                .HasColumnName("telefonoAdministrador");
        });

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("contacto");

            entity.HasIndex(e => e.SecConstructora, "FK_Contacto_Constructora_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("correo");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombres)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Apellidos)
               .HasMaxLength(150)
               .HasColumnName("apellidos");
            entity.Property(e => e.SecConstructora).HasColumnName("secConstructora");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .HasColumnName("titulo");

            entity.HasOne(d => d.SecConstructoraNavigation).WithMany(p => p.Contactos)
                .HasForeignKey(d => d.SecConstructora)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contacto_Constructora");
        });

        modelBuilder.Entity<Contactovisita>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("contactovisita");

            entity.HasIndex(e => e.SecContacto, "Fk_Contacto_ContactoVisita");

            entity.HasIndex(e => e.SecVisita, "Fk_Contacto_Visita");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.SecContacto).HasColumnName("secContacto");
            entity.Property(e => e.SecVisita).HasColumnName("secVisita");

            entity.HasOne(d => d.SecContactoNavigation).WithMany(p => p.Contactovisita)
                .HasForeignKey(d => d.SecContacto)
                .HasConstraintName("Fk_Contacto_ContactoVisita");

            entity.HasOne(d => d.SecVisitaNavigation).WithMany(p => p.Contactovisita)
                .HasForeignKey(d => d.SecVisita)
                .HasConstraintName("Fk_Visita_ContactoVisita");
        });

        modelBuilder.Entity<Catalogo>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("catalogo");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreArchivo)
                .HasMaxLength(50)
                .HasColumnName("nombreArchivo");
            entity.Property(e => e.UrlCatalogo)
                .HasMaxLength(500)
                .HasColumnName("urlCatalogo");
        });

        modelBuilder.Entity<Canton>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("canton");

            entity.HasIndex(e => e.SecProvincia, "Fk_Provincia_Canton");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Codigo)
                .HasMaxLength(5)
                .HasColumnName("codigo");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.SecProvincia).HasColumnName("secProvincia");

            entity.HasOne(d => d.SecProvinciaNavigation).WithMany(p => p.Cantons)
                .HasForeignKey(d => d.SecProvincia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Provincia_Canton");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("empresa");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.CodigoOperador)
                .HasMaxLength(5)
                .HasColumnName("codigoOperador");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Identificacion)
                .HasMaxLength(15)
                .HasColumnName("identificacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreLogo)
                .HasMaxLength(100)
                .HasColumnName("nombreLogo");
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlLogo)
                .HasMaxLength(500)
                .HasColumnName("urlLogo");
        });

        modelBuilder.Entity<Empresacorreo>(entity =>
        {
            entity.HasKey(e => e.SecEmpresa).HasName("PRIMARY");

            entity.ToTable("empresacorreo");

            entity.Property(e => e.SecEmpresa).ValueGeneratedNever();
            entity.Property(e => e.Alias)
                .HasMaxLength(50)
                .HasColumnName("alias");
            entity.Property(e => e.Clave)
                .HasMaxLength(255)
                .HasColumnName("clave");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Host)
                .HasMaxLength(50)
                .HasColumnName("host");
            entity.Property(e => e.Puerto).HasColumnName("puerto");

            entity.HasOne(d => d.SecEmpresaNavigation).WithOne(p => p.Empresacorreo)
                .HasForeignKey<Empresacorreo>(d => d.SecEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Empresa_EmpCorreo");
        });

        modelBuilder.Entity<Empresastorage>(entity =>
        {
            entity.HasKey(e => e.SecEmpresa).HasName("PRIMARY");

            entity.ToTable("empresastorage");

            entity.Property(e => e.SecEmpresa)
                .ValueGeneratedNever()
                .HasColumnName("secEmpresa");
            entity.Property(e => e.ApiKey)
                .HasMaxLength(255)
                .HasColumnName("apiKey");
            entity.Property(e => e.CarpetaLogo)
                .HasMaxLength(255)
                .HasColumnName("carpetaLogo");
            entity.Property(e => e.CarpetaProducto)
                .HasMaxLength(255)
                .HasColumnName("carpetaProducto");
            entity.Property(e => e.CarpetaUsuario)
                .HasMaxLength(255)
                .HasColumnName("carpetaUsuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(255)
                .HasColumnName("clave");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Ruta)
                .HasMaxLength(255)
                .HasColumnName("ruta");

            entity.HasOne(d => d.SecEmpresaNavigation).WithOne(p => p.Empresastorage)
                .HasForeignKey<Empresastorage>(d => d.SecEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empresastorage_ibfk_1");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("menu");

            entity.HasIndex(e => e.SecMenuPadre, "FK_Menu_Menu_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Controlador)
                .HasMaxLength(130)
                .HasColumnName("controlador");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(130)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Icono)
                .HasMaxLength(30)
                .HasColumnName("icono");
            entity.Property(e => e.PaginaAccion)
                .HasMaxLength(130)
                .HasColumnName("paginaAccion");
            entity.Property(e => e.SecMenuPadre).HasColumnName("secMenuPadre");

            entity.HasOne(d => d.SecMenuPadreNavigation).WithMany(p => p.InverseSecMenuPadreNavigation)
                .HasForeignKey(d => d.SecMenuPadre)
                .HasConstraintName("FK_Menu_Menu");
        });

        modelBuilder.Entity<Parroquia>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("parroquia");

            entity.HasIndex(e => e.SecCanton, "FK_Parroquia_Canton_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Codigo)
                .HasMaxLength(5)
                .HasColumnName("codigo");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.SecCanton).HasColumnName("secCanton");

            entity.HasOne(d => d.SecCantonNavigation).WithMany(p => p.Parroquia)
                .HasForeignKey(d => d.SecCanton)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Canton_Parroquia");
        });

        modelBuilder.Entity<Provincia>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("provincia");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Codigo)
                .HasMaxLength(5)
                .HasColumnName("codigo");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("rol");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
        });

        modelBuilder.Entity<RolMenu>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");
            entity.ToTable("rolmenu");

            entity.HasIndex(e => e.SecMenu, "FK_Menu_Rol_idx");

            entity.HasIndex(e => e.SecRol, "FK_Rol_Menu_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.SecMenu).HasColumnName("secMenu");
            entity.Property(e => e.SecRol).HasColumnName("secRol");

            entity.HasOne(d => d.SecMenuNavigation).WithMany(p => p.Rolmenus)
                .HasForeignKey(d => d.SecMenu)
                .HasConstraintName("FK_Menu_Rol");

            entity.HasOne(d => d.SecRolNavigation).WithMany(p => p.Rolmenus)
                .HasForeignKey(d => d.SecRol)
                .HasConstraintName("FK_Rol_Menu");
        });

        

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");


            entity.ToTable("usuario");

            entity.HasIndex(e => e.SecRol, "Fk_Rol_Usuario_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Clave)
                .HasMaxLength(255)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo).HasColumnName("esActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreFoto)
                .HasMaxLength(100)
                .HasColumnName("nombreFoto");
            entity.Property(e => e.SecRol).HasColumnName("secRol");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.UrlFoto)
                .HasMaxLength(500)
                .HasColumnName("urlFoto");

            entity.HasOne(d => d.SecRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.SecRol)
                .HasConstraintName("Fk_Rol_Usuario");
        });

        modelBuilder.Entity<Visita>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("visita");

            entity.HasIndex(e => e.SecCanton, "Fk_Visita_Canton");

            entity.HasIndex(e => e.SecParroquia, "Fk_Visita_Parroquia");

            entity.HasIndex(e => e.SecProvincia, "Fk_Visita_Provincia");

            entity.HasIndex(e => e.SecUsuario, "Fk_Visita_Usuario_idx");


            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Detalle)
                .HasMaxLength(500)
                .HasColumnName("detalle");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.FechaSiguienteVisita)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaSiguienteVisita");
            entity.Property(e => e.GeoUbicacion)
                .HasMaxLength(250)
                .HasColumnName("geoUbicacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.SecCanton).HasColumnName("secCanton");
            entity.Property(e => e.SecParroquia).HasColumnName("secParroquia");
            entity.Property(e => e.SecProvincia).HasColumnName("secProvincia");
            entity.Property(e => e.SecUsuario).HasColumnName("secUsuario");

            entity.Property(e => e.IdEtapa).HasColumnName("IdEtapa");
            entity.Property(e => e.SecEmpresa).HasColumnName("SecEmpresa");

            entity.HasOne(d => d.IdEtapaNavigation).WithMany()
                .HasForeignKey(d => d.IdEtapa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visita_Etapa");

            entity.HasOne(d => d.SecEmpresaNavigation).WithMany()
                .HasForeignKey(d => d.SecEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visita_Empresa");

            entity.HasOne(d => d.SecCantonNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.SecCanton)
                .HasConstraintName("Fk_Visita_Canton");

            entity.HasOne(d => d.SecParroquiaNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.SecParroquia)
                .HasConstraintName("Fk_Visita_Parroquia");

            entity.HasOne(d => d.SecProvinciaNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.SecProvincia)
                .HasConstraintName("Fk_Visita_Provincia");

            entity.HasOne(d => d.SecUsuarioNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.SecUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Visita_Usuario");


        });

        modelBuilder.Entity<Equiposvisita>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("equiposvisita");

            entity.HasIndex(e => e.Secuencial, "secuencial_UNIQUE").IsUnique();

            entity.Property(e => e.Secuencial)
                .ValueGeneratedNever()
                .HasColumnName("secuencial");
            entity.Property(e => e.AlturaEntrePisos).HasColumnName("alturaEntrePisos");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Capacidad).HasColumnName("capacidad");
            entity.Property(e => e.DimencionEntrada).HasColumnName("dimencionEntrada");
            entity.Property(e => e.Embarque)
                .HasMaxLength(50)
                .HasColumnName("embarque");
            entity.Property(e => e.Energia)
                .HasMaxLength(50)
                .HasColumnName("energia");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Foso).HasColumnName("foso");
            entity.Property(e => e.IngresosFrontales).HasColumnName("ingresosFrontales");
            entity.Property(e => e.IngresosPosteriores).HasColumnName("ingresosPosteriores");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .HasColumnName("marca");
            entity.Property(e => e.MaterialPuertas)
                .HasMaxLength(50)
                .HasColumnName("materialPuertas");
            entity.Property(e => e.MedidasAfducto)
                .HasMaxLength(50)
                .HasColumnName("medidasAFDucto");
            entity.Property(e => e.NombresParadas)
                .HasMaxLength(50)
                .HasColumnName("nombresParadas");
            entity.Property(e => e.NumeroParadas).HasColumnName("numeroParadas");
            entity.Property(e => e.NumeroPersonas).HasColumnName("numeroPersonas");
            entity.Property(e => e.Recorrido).HasColumnName("recorrido");
            entity.Property(e => e.SalaControl)
                .HasMaxLength(45)
                .HasColumnName("salaControl");
            entity.Property(e => e.SalaMaquinas)
                .HasMaxLength(45)
                .HasColumnName("salaMaquinas");
            entity.Property(e => e.SecVisita).HasColumnName("secVisita");
            entity.Property(e => e.Sistema)
                .HasMaxLength(50)
                .HasColumnName("sistema");
            entity.Property(e => e.TipoDucto)
                .HasMaxLength(50)
                .HasColumnName("tipoDucto");
            entity.Property(e => e.TipoEquipo)
                .HasMaxLength(150)
                .HasColumnName("tipoEquipo");
            entity.Property(e => e.TipoMotor)
                .HasMaxLength(50)
                .HasColumnName("tipoMotor");
            entity.Property(e => e.Velocidad)
                .HasPrecision(18, 2)
                .HasColumnName("velocidad");
        });

        modelBuilder.Entity<Permisosrol>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("permisosrol");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Consultar).HasColumnName("consultar");
            entity.Property(e => e.Eliminar).HasColumnName("eliminar");
            entity.Property(e => e.FechaRegistro)
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Modificar).HasColumnName("modificar");
            entity.Property(e => e.SecRol).HasColumnName("secRol");
            entity.Property(e => e.SecUsuarioModifica).HasColumnName("secUsuarioModifica");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.SecCliente).HasName("PRIMARY");

            entity.ToTable("cliente");

            entity.HasIndex(e => e.SecConstructora, "FK_Cliente_Constructora_idx").IsUnique();

            entity.Property(e => e.SecCliente).HasColumnName("SecCliente");
            
            entity.Property(e => e.SecConstructora).HasColumnName("SecConstructora");

            entity.Property(e => e.NumeroCliente)
                .HasMaxLength(50)
                .HasColumnName("NumeroCliente");

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("FechaCreacion");

            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");

            entity.HasOne(d => d.SecConstructoraNavigation).WithOne(p => p.Cliente)
                .HasForeignKey<Cliente>(d => d.SecConstructora)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cliente_Constructora");
        });

        modelBuilder.Entity<FormatoNumeroCliente>(entity =>
        {
            entity.HasKey(e => e.SecFormatoNumeroCliente).HasName("PRIMARY");

            entity.ToTable("formatonumerocliente");

            entity.HasIndex(e => e.SecEmpresa, "FK_FormatoNumeroCliente_Empresa_idx");

            entity.Property(e => e.SecFormatoNumeroCliente).HasColumnName("SecFormatoNumeroCliente");

            entity.Property(e => e.SecEmpresa).HasColumnName("SecEmpresa");

            entity.Property(e => e.UsaFormato).HasColumnName("UsaFormato");

            entity.Property(e => e.Formato)
                .HasMaxLength(100)
                .HasColumnName("Formato");

            entity.Property(e => e.NumeroInicio).HasColumnName("NumeroInicio");

            entity.HasOne(d => d.SecEmpresaNavigation).WithMany(p => p.FormatosNumeroCliente)
                .HasForeignKey(d => d.SecEmpresa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FormatoNumeroCliente_Empresa");
        });

        modelBuilder.Entity<Seguimiento>(entity =>
        {
            entity.HasKey(e => e.SecSeguimiento).HasName("PRIMARY");

            entity.ToTable("seguimiento");

            entity.HasIndex(e => e.SecCotizacion, "FK_Seguimiento_Cotizacion_idx");

            entity.Property(e => e.SecSeguimiento).HasColumnName("SecSeguimiento");
            
            entity.Property(e => e.SecCotizacion).HasColumnName("SecCotizacion");

            entity.Property(e => e.Accion)
                .HasMaxLength(50)
                .HasColumnName("Accion");

            entity.Property(e => e.Detalle)
                .HasMaxLength(500)
                .HasColumnName("Detalle");

            entity.Property(e => e.FechaAccion)
                .HasColumnType("datetime")
                .HasColumnName("FechaAccion");

            entity.Property(e => e.AceptacionCliente).HasColumnName("AceptacionCliente");

            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("FechaRegistro");

            entity.HasOne(d => d.SecCotizacionNavigation).WithMany(p => p.Seguimientos)
                .HasForeignKey(d => d.SecCotizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seguimiento_Cotizacion");
        });

        modelBuilder.Entity<PreContrato>(entity =>
        {
            entity.HasKey(e => e.SecPreContrato).HasName("PRIMARY");
            entity.ToTable("precontrato");

            entity.HasIndex(e => e.SecCotizacion, "FK_PreContrato_Cotizacion_idx");
            entity.HasIndex(e => e.SecPlantillaPreContrato, "FK_PreContrato_Plantilla_idx");
            entity.HasIndex(e => e.SecUsuarioCrea, "FK_PreContrato_Usuario_idx");
            entity.HasIndex(e => e.SecFormaPago, "FK_PreContrato_FormaPago_idx");

            entity.Property(e => e.SecPreContrato).HasColumnName("SecPreContrato");
            entity.Property(e => e.SecCotizacion).HasColumnName("SecCotizacion");
            entity.Property(e => e.SecPlantillaPreContrato).HasColumnName("SecPlantillaPreContrato");
            entity.Property(e => e.SecUsuarioCrea).HasColumnName("SecUsuarioCrea");
            entity.Property(e => e.SecFormaPago).HasColumnName("SecFormaPago");
            entity.Property(e => e.Version).HasColumnName("Version");
            entity.Property(e => e.Estado).HasMaxLength(50).HasColumnName("Estado");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasColumnName("FechaRegistro");

            entity.Property(e => e.Dias).HasColumnName("Dias");
            entity.Property(e => e.TipoDias).HasMaxLength(50).HasColumnName("TipoDias");
            entity.Property(e => e.ValorContrato).HasPrecision(18, 2).HasColumnName("ValorContrato");
            entity.Property(e => e.AniosGarantia).HasColumnName("AniosGarantia");
            entity.Property(e => e.MesesGarantia).HasColumnName("MesesGarantia");
            entity.Property(e => e.PeriodoMantenimiento).HasMaxLength(255).HasColumnName("PeriodoMantenimiento");
            entity.Property(e => e.PolizaGarantia).HasMaxLength(255).HasColumnName("PolizaGarantia");
            entity.Property(e => e.ValorAnticipo).HasPrecision(18, 2).HasColumnName("ValorAnticipo");
            entity.Property(e => e.FechaAnticipo).HasColumnType("datetime").HasColumnName("FechaAnticipo");
            entity.Property(e => e.NumeroCuotas).HasColumnName("NumeroCuotas");
            entity.Property(e => e.FechaPrimeraCuota).HasColumnType("datetime").HasColumnName("FechaPrimeraCuota");

            entity.HasOne(d => d.SecCotizacionNavigation)
                .WithMany()
                .HasForeignKey(d => d.SecCotizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreContrato_Cotizacion");

            entity.HasOne(d => d.SecPlantillaPreContratoNavigation)
                .WithMany()
                .HasForeignKey(d => d.SecPlantillaPreContrato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreContrato_PlantillaPreContrato");

            entity.HasOne(d => d.SecUsuarioCreaNavigation)
                .WithMany()
                .HasForeignKey(d => d.SecUsuarioCrea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreContrato_Usuario");

            entity.HasOne(d => d.SecFormaPagoNavigation)
                .WithMany()
                .HasForeignKey(d => d.SecFormaPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PreContrato_FormaPago");
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("cotizacion");

            entity.HasIndex(e => e.SecVisita, "FK_Cotizacion_Visita_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.SecVisita).HasColumnName("secVisita");
            entity.Property(e => e.EnviadoProveedor).HasColumnName("enviadoProveedor");
            entity.Property(e => e.EnviadoCliente).HasColumnName("enviadoCliente");
            entity.Property(e => e.Confirmacion).HasColumnName("confirmacion");
            entity.Property(e => e.Subtotal).HasPrecision(18, 2).HasColumnName("subtotal");
            entity.Property(e => e.ValorImpuestos).HasPrecision(18, 2).HasColumnName("valorImpuestos");
            entity.Property(e => e.TotalConImpuestos).HasPrecision(18, 2).HasColumnName("totalConImpuestos");
            entity.Property(e => e.ValorIVA).HasPrecision(18, 2).HasColumnName("valorIVA");
            entity.Property(e => e.ValorImportacion).HasPrecision(18, 2).HasColumnName("valorImportacion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasColumnName("fechaRegistro");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime").HasColumnName("fechaModificacion");

            entity.Property(e => e.SecUsuario).HasColumnName("secUsuario");
            entity.Property(e => e.SecCotizacionOriginal).HasColumnName("secCotizacionOriginal");
            entity.Property(e => e.SecUsuarioModifica).HasColumnName("secUsuarioModifica");

            // Relationships
            entity.HasOne(d => d.SecVisitaNavigation).WithMany() 
                .HasForeignKey(d => d.SecVisita)
                .OnDelete(DeleteBehavior.ClientSetNull) 
                .HasConstraintName("FK_Cotizacion_Visita"); 

            entity.HasOne(d => d.SecUsuarioNavigation).WithMany()
                .HasForeignKey(d => d.SecUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cotizacion_Usuario");

            entity.HasOne(d => d.SecCotizacionOriginalNavigation).WithMany()
                .HasForeignKey(d => d.SecCotizacionOriginal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cotizacion_CotizacionOriginal");

            entity.HasOne(d => d.SecUsuarioModificaNavigation).WithMany()
                .HasForeignKey(d => d.SecUsuarioModifica)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cotizacion_UsuarioModifica"); 

            entity.HasMany(d => d.ImpuestoCotizaciones)
                .WithOne(p => p.SecCotizacionNavigation)
                .HasForeignKey(d => d.SecCotizacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ImpuestoCotizacion_Cotizacion"); 
        });

        modelBuilder.Entity<Cotizaciondetalle>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");
            entity.ToTable("cotizaciondetalle");

            entity.Property(e => e.Secuencial).HasColumnName("Secuencial");
            entity.Property(e => e.SecCotizacion).HasColumnName("SecCotizacion");
            entity.Property(e => e.DetalleEquipo).HasColumnName("DetalleEquipo");
            entity.Property(e => e.ValorCompra).HasColumnName("ValorCompra");
            entity.Property(e => e.MargenGanancia).HasColumnName("MargenGanancia");
            entity.Property(e => e.Total).HasColumnName("Total");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.FechaRegistro).HasColumnName("FechaRegistro");

            entity.HasOne(d => d.SecCotizacionNavigation)
                .WithMany(p => p.Cotizaciondetalles)
                .HasForeignKey(d => d.SecCotizacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Cotizaciondetalle_Cotizacion");
        });

        modelBuilder.Entity<Impuesto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("impuesto");

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Codigo)
                .HasMaxLength(10)
                .HasColumnName("Codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasColumnName("Descripcion");
            entity.Property(e => e.Porcentaje)
                .HasPrecision(5, 2)
                .HasColumnName("Porcentaje");
            entity.Property(e => e.ValorFijo)
                .HasPrecision(18, 2)
                .HasColumnName("ValorFijo");
            entity.Property(e => e.CodigoSri)
                .HasMaxLength(5)
                .HasColumnName("CodigoSri");
            
            entity.Property(e => e.Vigente).HasColumnName("Vigente");
            entity.Property(e => e.SecTipoImpuesto).HasColumnName("SecTipoImpuesto");

            entity.HasOne(d => d.SecTipoImpuestoNavigation).WithMany()
                .HasForeignKey(d => d.SecTipoImpuesto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Impuesto_TipoImpuesto");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("FechaCreacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("FechaModificacion");
        });

        modelBuilder.Entity<ImpuestoCotizacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("impuestocotizacion");

            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.SecCotizacion).HasColumnName("SecCotizacion");
            entity.Property(e => e.ImpuestoId).HasColumnName("ImpuestoId");
            entity.Property(e => e.BaseImponible)
                .HasPrecision(18, 2)
                .HasColumnName("BaseImponible");
            entity.Property(e => e.ValorImpuesto)
                .HasPrecision(18, 2)
                .HasColumnName("ValorImpuesto");
            entity.Property(e => e.Exento).HasColumnName("Exento");
            entity.Property(e => e.Observaciones).HasColumnName("Observaciones");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("FechaRegistro");

            entity.HasOne(d => d.SecCotizacionNavigation)
                .WithMany(p => p.ImpuestoCotizaciones)
                .HasForeignKey(d => d.SecCotizacion)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ImpuestoCotizacion_Cotizacion");

            entity.HasOne(d => d.ImpuestoNavigation).WithMany(p => p.ImpuestoCotizacion)
                .HasForeignKey(d => d.ImpuestoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImpuestoCotizacion_Impuesto");
        });

        modelBuilder.Entity<TipoImpuesto>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");
            entity.ToTable("tipoimpuesto");

            entity.Property(e => e.Secuencial).HasColumnName("Secuencial");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("Nombre");
            entity.Property(e => e.EsIva).HasColumnName("EsIva");
            entity.Property(e => e.EsImportacion).HasColumnName("EsImportacion");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("datetime")
                .HasColumnName("FechaCreacion");
            entity.Property(e => e.FechaModificacion)
                .HasColumnType("datetime")
                .HasColumnName("FechaModificacion");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PRIMARY");
            entity.ToTable("permiso");

            entity.Property(e => e.IdPermiso)
                .HasMaxLength(100)
                .HasColumnName("IdPermiso");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .HasColumnName("Descripcion");
        });

        modelBuilder.Entity<RolPermiso>(entity =>
        {
            entity.HasKey(e => new { e.SecRol, e.IdPermiso });

            entity.ToTable("rolpermiso");

            entity.HasOne(d => d.Rol)
                .WithMany(p => p.RolPermisos)
                .HasForeignKey(d => d.SecRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Rol");

            entity.HasOne(d => d.Permiso)
                .WithMany(p => p.RolPermisos)
                .HasForeignKey(d => d.IdPermiso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RolPermiso_Permiso");
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.SecTipoDocumento).HasName("PRIMARY");
            entity.ToTable("tipodocumento");

            entity.Property(e => e.SecTipoDocumento).HasColumnName("SecTipoDocumento");
            entity.Property(e => e.Codigo).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
        });

        modelBuilder.Entity<FormaPago>(entity =>
        {
            entity.HasKey(e => e.SecFormaPago).HasName("PRIMARY");
            entity.ToTable("formapago");

            entity.Property(e => e.SecFormaPago).HasColumnName("SecFormaPago");
            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
        });

        modelBuilder.Entity<PlantillaPreContrato>(entity =>
        {
            entity.HasKey(e => e.SecPlantillaPreContrato).HasName("PRIMARY");
            entity.ToTable("plantillaprecontrato");

            entity.HasIndex(e => e.SecTipoDocumento, "FK_PlantillaPreContrato_TipoDocumento_idx");

            entity.Property(e => e.SecPlantillaPreContrato).HasColumnName("SecPlantillaPreContrato");
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.NumeracionInicial).HasMaxLength(50);
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.SecTipoDocumento).HasColumnName("SecTipoDocumento");

            entity.HasOne(d => d.SecTipoDocumentoNavigation)
                .WithMany()
                .HasForeignKey(d => d.SecTipoDocumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlantillaPreContrato_TipoDocumento");
        });

        modelBuilder.Entity<PlantillaPreContratoParrafo>(entity =>
        {
            entity.HasKey(e => e.SecPlantillaPreContratoParrafo).HasName("PRIMARY");
            entity.ToTable("plantillaprecontratoparrafo");

            entity.HasIndex(e => e.SecPlantillaPreContrato, "IX_plantillaprecontratoparrafo_plantilla");

            entity.Property(e => e.SecPlantillaPreContratoParrafo).HasColumnName("SecPlantillaPreContratoParrafo");
            entity.Property(e => e.SecPlantillaPreContrato).HasColumnName("SecPlantillaPreContrato");
            entity.Property(e => e.Orden).HasColumnName("Orden");
            entity.Property(e => e.Contenido).HasColumnType("TEXT");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");

            entity.HasOne(d => d.SecPlantillaPreContratoNavigation)
                .WithMany(p => p.PlantillaPreContratoParrafos)
                .HasForeignKey(d => d.SecPlantillaPreContrato)
                .HasConstraintName("FK_plantillaprecontratoparrafo_plantillaprecontrato");
        });

        modelBuilder.Entity<DiccionarioParametro>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");
            entity.ToTable("diccionarioparametro");

            entity.HasIndex(e => e.Parametro, "UQ_DiccionarioParametro_Parametro").IsUnique();

            entity.Property(e => e.Secuencial).HasColumnName("Secuencial");
            entity.Property(e => e.Parametro).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.EstaActivo);
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.HasKey(e => e.IdContrato).HasName("PRIMARY");
            entity.ToTable("contrato");

            entity.HasIndex(e => e.IdCotizacion, "FK_Contrato_Cotizacion_idx");
            entity.HasIndex(e => e.IdUsuarioCarga, "FK_Contrato_Usuario_idx");

            entity.Property(e => e.IdContrato).HasColumnName("IdContrato");
            entity.Property(e => e.IdCotizacion).HasColumnName("IdCotizacion");
            entity.Property(e => e.FechaFirma).HasColumnType("datetime").HasColumnName("FechaFirma");
            entity.Property(e => e.IdUsuarioCarga).HasColumnName("IdUsuarioCarga");
            entity.Property(e => e.NombreArchivo).HasMaxLength(255).HasColumnName("NombreArchivo");
            entity.Property(e => e.RutaArchivo).HasMaxLength(1024).HasColumnName("RutaArchivo");
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime").HasColumnName("FechaCreacion");
            entity.Property(e => e.EsActivo).HasColumnName("EsActivo");

            entity.HasOne(d => d.SecClienteNavigation)
                .WithMany(p => p.Contratos)
                .HasForeignKey(d => d.SecCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contrato_Cliente");

            entity.HasOne(d => d.IdCotizacionNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdCotizacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contrato_Cotizacion");

            entity.HasOne(d => d.IdUsuarioCargaNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdUsuarioCarga)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contrato_Usuario");

            // Configuración para la relación uno a uno con PlanDePago
            entity.HasOne(c => c.PlanDePagoNavigation)
                .WithOne(pp => pp.IdContratoNavigation)
                .HasForeignKey<PlanDePago>(pp => pp.IdContrato);
        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<PlanDePago>(entity =>
        {
            entity.HasKey(e => e.IdPlanDePago).HasName("PRIMARY");
            entity.ToTable("plandepago");

            entity.HasIndex(e => e.IdContrato, "FK_PlanDePago_Contrato_idx").IsUnique();
            entity.HasIndex(e => e.SecFormaPago, "FK_PlanDePago_FormaPago_idx");

            entity.Property(e => e.IdPlanDePago).HasColumnName("IdPlanDePago");
            entity.Property(e => e.IdContrato).HasColumnName("IdContrato");
            entity.Property(e => e.SecFormaPago).HasColumnName("SecFormaPago");
            entity.Property(e => e.ValorContrato).HasPrecision(18, 2).HasColumnName("ValorContrato");
            entity.Property(e => e.ValorAnticipo).HasPrecision(18, 2).HasColumnName("ValorAnticipo");
            entity.Property(e => e.FechaAnticipo).HasColumnType("datetime").HasColumnName("FechaAnticipo");
            entity.Property(e => e.NumeroCuotas).HasColumnName("NumeroCuotas");
            entity.Property(e => e.FechaPrimeraCuota).HasColumnType("datetime").HasColumnName("FechaPrimeraCuota");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasColumnName("FechaRegistro");

            entity.HasOne(d => d.SecFormaPagoNavigation).WithMany()
                .HasForeignKey(d => d.SecFormaPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PlanDePago_FormaPago");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.IdPago).HasName("PRIMARY");
            entity.ToTable("pago");

            entity.HasIndex(e => e.IdPlanDePago, "FK_Pago_PlanDePago_idx");
            entity.HasIndex(e => e.RegistradoPorUsuarioId, "FK_Pago_Usuario_idx");

            entity.Property(e => e.IdPago).HasColumnName("IdPago");
            entity.Property(e => e.IdPlanDePago).HasColumnName("IdPlanDePago");
            entity.Property(e => e.Monto).HasPrecision(18, 2).HasColumnName("Monto");
            entity.Property(e => e.FechaPago).HasColumnType("datetime").HasColumnName("FechaPago");
            entity.Property(e => e.ComprobanteUrl).HasMaxLength(255).HasColumnName("ComprobanteUrl");
            entity.Property(e => e.ComprobanteNombre).HasMaxLength(255).HasColumnName("ComprobanteNombre");
            entity.Property(e => e.RegistradoPorUsuarioId).HasColumnName("RegistradoPorUsuarioId");
            entity.Property(e => e.EstaActivo).HasColumnName("EstaActivo");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasColumnName("FechaRegistro");

            entity.HasOne(d => d.IdPlanDePagoNavigation).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.IdPlanDePago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_PlanDePago");

            entity.HasOne(d => d.RegistradoPorUsuario).WithMany()
                .HasForeignKey(d => d.RegistradoPorUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pago_Usuario");
        });

        modelBuilder.Entity<Cuota>(entity =>
        {
            entity.HasKey(e => e.IdCuota).HasName("PRIMARY");
            entity.ToTable("cuota");

            entity.HasIndex(e => e.IdPlanDePago, "FK_Cuota_PlanDePago_idx");

            entity.Property(e => e.IdCuota).HasColumnName("IdCuota");
            entity.Property(e => e.IdPlanDePago).HasColumnName("IdPlanDePago");
            entity.Property(e => e.NumeroCuota).HasColumnName("NumeroCuota");
            entity.Property(e => e.MontoEsperado).HasPrecision(18, 2).HasColumnName("MontoEsperado");
            entity.Property(e => e.FechaVencimiento).HasColumnType("datetime").HasColumnName("FechaVencimiento");
            entity.Property(e => e.Estado).HasMaxLength(50).HasColumnName("Estado");
            entity.Property(e => e.FechaRegistro).HasColumnType("datetime").HasColumnName("FechaRegistro");

            entity.HasOne(d => d.IdPlanDePagoNavigation).WithMany(p => p.Cuotas)
                .HasForeignKey(d => d.IdPlanDePago)
                .OnDelete(DeleteBehavior.Cascade) // Si se borra el plan, se borran las cuotas
                .HasConstraintName("FK_Cuota_PlanDePago");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

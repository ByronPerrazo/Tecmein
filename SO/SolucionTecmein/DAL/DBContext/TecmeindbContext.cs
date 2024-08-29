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

    public virtual DbSet<ContactoVisita> Contactovista { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Empresacorreo> Empresacorreos { get; set; }

    public virtual DbSet<Empresastorage> Empresastorages { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Parroquia> Parroquia { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Provincia> Provincia { get; set; }

    public virtual DbSet<Rol> Roles { get; set; }

    public virtual DbSet<RolMenu> RolMenus { get; set; }

    public virtual DbSet<TipoProducto> TipoProductos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Visita> Visita { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

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

        modelBuilder.Entity<Contacto>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("contacto");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .HasColumnName("correo");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .HasColumnName("titulo");
        });

        modelBuilder.Entity<ContactoVisita>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("contactovista");

            entity.HasIndex(e => e.SecContacto, "Fk_Contacto_ContactoVisita");

            entity.HasIndex(e => e.SecVisita, "Fk_Contacto_Visita");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.SecContacto).HasColumnName("secContacto");
            entity.Property(e => e.SecVisita).HasColumnName("secVisita");

            entity.HasOne(d => d.SecContactoNavigation).WithMany(p => p.Contactovista)
                .HasForeignKey(d => d.SecContacto)
                .HasConstraintName("Fk_Contacto_ContactoVisita");

            entity.HasOne(d => d.SecVisitaNavigation).WithMany(p => p.Contactovista)
                .HasForeignKey(d => d.SecVisita)
                .HasConstraintName("Fk_Contacto_Visita");
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

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("producto");

            entity.HasIndex(e => e.SecTipoProducto, "Fk_TipoProducto_Producto_idx");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Capacidad)
                .HasMaxLength(50)
                .HasColumnName("capacidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Marca)
                .HasMaxLength(50)
                .HasColumnName("marca");
            entity.Property(e => e.Motor)
                .HasMaxLength(50)
                .HasColumnName("motor");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreImagen)
                .HasMaxLength(100)
                .HasColumnName("nombreImagen");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");
            entity.Property(e => e.SecTipoProducto).HasColumnName("secTipoProducto");
            entity.Property(e => e.Sistema)
                .HasMaxLength(50)
                .HasColumnName("sistema");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.UrlImagen)
                .HasMaxLength(500)
                .HasColumnName("urlImagen");

            entity.HasOne(d => d.SecTipoProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.SecTipoProducto)
                .HasConstraintName("Fk_TipoProducto_Producto");
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

        modelBuilder.Entity<TipoProducto>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("tipoproducto");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("fechaRegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .HasColumnName("nombre");
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


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

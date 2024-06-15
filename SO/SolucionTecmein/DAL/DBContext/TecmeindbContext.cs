using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Entity;

namespace DAL.DBContext;


public partial class TecmeindbContext : DbContext
{
    public TecmeindbContext(DbContextOptions<TecmeindbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Empresacorreo> Empresacorreos { get; set; }

    public virtual DbSet<Empresastorage> Empresastorages { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<RolMenu> Rolmenus { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.Secuencial).HasName("PRIMARY");

            entity.ToTable("empresa");

            entity.Property(e => e.Secuencial).HasColumnName("secuencial");
            entity.Property(e => e.Correo)
                .HasMaxLength(150)
                .HasColumnName("correo");
            entity.Property(e => e.Direccion)
                .HasMaxLength(250)
                .HasColumnName("direccion");
            entity.Property(e => e.EstaActivo).HasColumnName("estaActivo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreLogo)
                .HasMaxLength(100)
                .HasColumnName("nombreLogo");
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(50)
                .HasColumnName("numeroDocumento");
            entity.Property(e => e.PorcentajeImpuesto)
                .HasPrecision(10, 2)
                .HasColumnName("porcentajeImpuesto");
            entity.Property(e => e.SimboloMoneda)
                .HasMaxLength(5)
                .HasColumnName("simboloMoneda");
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

            entity.Property(e => e.SecEmpresa).ValueGeneratedOnAdd();
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
                .ValueGeneratedOnAdd()
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

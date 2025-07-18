using AutoMapper;
using Entity;
using TecmeinWebApp.Models.ViewModel;

namespace TecmeinWebApp.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<Rol, RolVM>().ReverseMap();
            CreateMap<Empresa, EmpresaVM>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.EstaActivo));

            CreateMap<EmpresaVM, Empresa>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1));

            #region Usuario
            CreateMap<Usuario, UsuarioVM>()
                .ForMember(destino =>
                           destino.EsActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.EsActivo))
                .ForMember(destino =>
                           destino.NombreRol,
                                    opt =>
                                    opt.MapFrom(origen =>
                                                origen.SecRolNavigation.Descripcion));

            CreateMap<UsuarioVM, Usuario>()
                .ForMember(destino =>
                           destino.EsActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EsActivo == 1))
                .ForMember(destino =>
                           destino.SecRolNavigation,
                                   opt =>
                                   opt.Ignore());
            #endregion usuario

            #region Menu

            CreateMap<Menu, MenuVM>()
                           .ForMember(destino => destino.SubMenu,
                                      opt => opt.MapFrom(origen => origen.InverseSecMenuPadreNavigation))
                           ;

            CreateMap<MenuVM, Menu>()
                .ForMember(destino =>
                    destino.InverseSecMenuPadreNavigation,
                    opt => opt.Ignore()
                );
            #endregion

            CreateMap<Provincia, ProvinciaVM>().ReverseMap();
            CreateMap<Canton, CantonVM>().ReverseMap();



            CreateMap<Parroquia, ParroquiaVM>().ReverseMap();

            #region Visita

            CreateMap<VisitaVM, Visita>()
               .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1))
                .ForMember(destino =>
                           destino.SecProvinciaNavigation,
                                   opt =>
                                   opt.Ignore())
                .ForMember(destino =>
                           destino.SecCantonNavigation,
                                   opt =>
                                   opt.Ignore())
                .ForMember(destino =>
                           destino.SecParroquiaNavigation,
                                   opt =>
                                   opt.Ignore());



            CreateMap<Visita, VisitaVM>()
               .ForMember(destino =>
                          destino.EstaActivo,
                                  opt =>
                                  opt.MapFrom(origen =>
                                              origen.EstaActivo))
               .ForMember(destino => destino.NombreProvincia,
                           opt => opt.MapFrom(origen =>
                                              origen.SecProvinciaNavigation.Nombre))
               .ForMember(destino => destino.NombreCanton,
                           opt => opt.MapFrom(origen =>
                                              origen.SecCantonNavigation.Nombre))
               .ForMember(destino => destino.NombreParroquia,
                           opt => opt.MapFrom(origen =>
                                              origen.SecParroquiaNavigation.Nombre))
               ;

            #endregion

           

            CreateMap<Catalogo, CatalogoVM>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.EstaActivo));

            CreateMap<CatalogoVM, Catalogo>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1));


            CreateMap<Constructora, ConstructoraVM>()
            .ForMember(destino =>
                       destino.EstaActivo,
                               opt =>
                               opt.MapFrom(origen =>
                                           origen.EstaActivo));

            CreateMap<ConstructoraVM, Constructora>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1));

            #region Contacto

            CreateMap<Contacto, ContactoVM>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.EstaActivo))
                .ForMember(destino =>
                           destino.NombreConstructora,
                                    opt =>
                                    opt.MapFrom(origen =>
                                                origen.SecConstructoraNavigation.Nombre));

            CreateMap<ContactoVM, Contacto>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1))
                .ForMember(destino =>
                           destino.SecConstructoraNavigation,
                                   opt =>
                                   opt.Ignore());
            #endregion 

            CreateMap<Contactovisita, ContactoVisitaVM>()
               .ForMember(destino =>
                          destino.EstaActivo,
                                  opt =>
                                  opt.MapFrom(origen =>
                                              origen.EstaActivo))
               .ForMember(destino =>
                          destino.SecConstructora,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.SecContactoNavigation.SecConstructora)); ;

            CreateMap<ContactoVisitaVM, Contactovisita>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1))
                .ForMember(destino =>
                           destino.SecContactoNavigation,
                                   opt =>
                                   opt.Ignore());

            #region ProductosVisita

            CreateMap<Equiposvisita, EquiposVisitaVM>()
              .ForMember(destino =>
                         destino.EstaActivo,
                                 opt =>
                                 opt.MapFrom(origen =>
                                             origen.EstaActivo))
              .ForMember(destino => 
                         destino.DetalleEspecifico,
                             opt =>
                             opt.MapFrom(origen => $"Sistema:{origen.Sistema} -" +
                                                   $" Tipo Eq:{origen.TipoEquipo} -" +
                                                   $" Marca:{origen.Marca} -" +
                                                   $" Capacidad:{origen.Capacidad} -" +
                                                   $" Velocidad:{origen.Velocidad} -" +
                                                   $" Sala Maq:{origen.SalaMaquinas} -" +
                                                   $" Motor:{origen.TipoMotor} -" +
                                                   $" Embarque:{origen.Embarque} -" +
                                                   $" Ducto:{origen.TipoDucto} -" +
                                                   $" MedidasAF:{origen.MedidasAfducto} -" +
                                                   $" Foso:{origen.Foso} -" +
                                                   $" Recorrido:{origen.Recorrido} -" +
                                                   $" Sbr. Recorrido:{origen.SobreRecorrido} -" +
                                                   $" Ing. Frontales:{origen.IngresosFrontales} -" +
                                                   $" Ing. Posteriores:{origen.IngresosPosteriores} -" +
                                                   $" Dime Entrada:{origen.DimencionEntrada} -" +
                                                   $" Alt Entre Pisos:{origen.AlturaEntrePisos} -" +
                                                   $" Energia:{origen.Energia} -" +
                                                   $" Puertas:{origen.MaterialPuertas} -" +
                                                   $" Num. Paradas{origen.NumeroParadas}-" +
                                                   $" Nomb. Paradas{origen.NombresParadas}-" +
                                                   $" Num Personas:{origen.NumeroPersonas} " ) );


            CreateMap<EquiposVisitaVM, Equiposvisita>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1));

            #endregion

            CreateMap<Permisosrol, PermisosrolVM>().ReverseMap();

            CreateMap<RolMenu, RolMenuVM>()
                .ForMember(destino =>
                           destino.DescripcionRol,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.SecRolNavigation.Descripcion))
                .ForMember(destino =>
                           destino.DescripcionMenu,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.SecMenuNavigation.Descripcion));

            CreateMap<RolMenuVM, RolMenu>()
                .ForMember(destino =>
                           destino.SecRolNavigation,
                                   opt =>
                                   opt.Ignore())
                .ForMember(destino =>
                           destino.SecMenuNavigation,
                                   opt =>
                                   opt.Ignore());
        }
    }
}

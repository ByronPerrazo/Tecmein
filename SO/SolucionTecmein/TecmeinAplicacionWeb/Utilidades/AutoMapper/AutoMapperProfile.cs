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
                .ForMember(destino =>
                           destino.SubMenu,
                                opt =>
                                opt.MapFrom(origen =>
                                            origen.InverseSecMenuPadreNavigation));
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

            #region TipoProducto
            CreateMap<TipoProducto, TipoProductoVM>().ForMember(destino =>
                          destino.EstaActivo,
                                  opt =>
                                  opt.MapFrom(origen =>
                                              origen.EstaActivo));

            CreateMap<TipoProductoVM, TipoProducto>().ForMember(destino =>
                          destino.EstaActivo,
                                  opt =>
                                  opt.MapFrom(origen =>
                                                     origen.EstaActivo == 1));
            #endregion

            #region Producto
            CreateMap<Producto, ProductoVM>()
                 .ForMember(destino =>
                            destino.Precio, opt =>
                                           opt.MapFrom(origen =>
                                                      Math.Round(Convert.ToDecimal(origen.Precio), 2)))
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.EstaActivo))
                .ForMember(destino =>
                           destino.NombreTipoProducto,
                                    opt =>
                                    opt.MapFrom(origen =>
                                                origen.SecTipoProductoNavigation.Nombre));

            CreateMap<ProductoVM, Producto>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1))
                .ForMember(destino =>
                           destino.Precio, opt =>
                                           opt.MapFrom(origen =>
                                                      Math.Round(Convert.ToDecimal(origen.Precio), 2)))

                .ForMember(destino =>
                           destino.SecTipoProductoNavigation,
                                   opt =>
                                   opt.Ignore());
            #endregion Producto

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
            #endregion usuar

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
                                             origen.EstaActivo));
             

            CreateMap<EquiposVisitaVM, Equiposvisita>()
                .ForMember(destino =>
                           destino.EstaActivo,
                                   opt =>
                                   opt.MapFrom(origen =>
                                                      origen.EstaActivo == 1));
            #endregion


        }
    }
}

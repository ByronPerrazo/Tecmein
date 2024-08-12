using TecmeinWebApp.Models.ViewModel;
using Entity;
using System.Globalization;
using AutoMapper;

namespace TecmeinWebApp.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<Rol, RolVM>().ReverseMap();
            CreateMap<Empresa, EmpresaVM>().ReverseMap();

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
        }
    }
}

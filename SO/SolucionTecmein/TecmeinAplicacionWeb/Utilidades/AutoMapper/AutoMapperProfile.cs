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
            
            CreateMap<Rol,RolVM>().ReverseMap();
            CreateMap<Empresa,EmpresaVM>().ReverseMap();    

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

        }
    }
}

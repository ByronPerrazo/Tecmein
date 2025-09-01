using AutoMapper;
using Entity;
using TecmeinWebApp.Models.ViewModel;

namespace TecmeinWebApp.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<Rol, RolVM>()
                .ForMember(destino => destino.FechaRegistroString,
                           opt => opt.MapFrom(origen => origen.FechaRegistro.HasValue ? origen.FechaRegistro.Value.ToString("dd/MM/yyyy") : null));
            CreateMap<RolVM, Rol>()
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => !string.IsNullOrEmpty(origen.FechaRegistroString) ? DateTime.ParseExact(origen.FechaRegistroString, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?)null));
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

            #region Permiso
            CreateMap<Permiso, PermisoVM>().ReverseMap();
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
                         destino.Cantidad, // Add Cantidad mapping
                                 opt =>
                                 opt.MapFrom(origen =>
                                             origen.Cantidad))
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

            CreateMap<FormatoNumeroCliente, FormatoNumeroClienteVm>().ReverseMap();

            CreateMap<Seguimiento, SeguimientoVM>().ReverseMap();

            CreateMap<FormaPago, FormaPagoVM>().ReverseMap();

            CreateMap<PlantillaPreContrato, PlantillaPreContratoVM>().ReverseMap();

                        CreateMap<PlantillaPreContratoParrafo, PlantillaPreContratoParrafoVM>();

            CreateMap<PlantillaPreContratoParrafoVM, PlantillaPreContratoParrafo>();

            CreateMap<Permisosrol, PermisosrolVM>().ReverseMap();

            CreateMap<Permiso, TecmeinAplicacionWeb.Models.ViewModels.PermisoVM>().ReverseMap();

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

            #region Cotizacion
            CreateMap<Cotizacion, CotizacionVM>()
                .ForMember(destino =>
                    destino.NombreObra,
                    opt => opt.MapFrom(origen => origen.SecVisitaNavigation.Nombre))
                .ForMember(destino =>
                    destino.EstaActivo,
                    opt => opt.MapFrom(origen => origen.EstaActivo == 1 ? 1 : 0))
                .ForMember(destino =>
                    destino.FechaRegistro,
                    opt => opt.MapFrom(origen => origen.FechaRegistro.HasValue ? origen.FechaRegistro.Value.ToString("dd/MM/yyyy") : ""))
                .ForMember(destino =>
                    destino.FechaModificacion,
                    opt => opt.MapFrom(origen => origen.FechaModificacion.HasValue ? origen.FechaModificacion.Value.ToString("dd/MM/yyyy") : ""))
                // New mappings for tax fields
                .ForMember(destino => destino.Subtotal, opt => opt.MapFrom(origen => origen.Subtotal))
                .ForMember(destino => destino.ValorImpuestos, opt => opt.MapFrom(origen => origen.ValorImpuestos))
                .ForMember(destino => destino.TotalConImpuestos, opt => opt.MapFrom(origen => origen.TotalConImpuestos))
                .ForMember(destino => destino.ValorIVA, opt => opt.MapFrom(origen => origen.ValorIVA))
                .ForMember(destino => destino.ValorImportacion, opt => opt.MapFrom(origen => origen.ValorImportacion))
                .ForMember(destino =>
                    destino.SecUsuario,
                    opt => opt.MapFrom(origen => origen.SecUsuario))
                                .ForMember(destino =>
                    destino.NombreUsuario,
                    opt => opt.MapFrom(origen => origen.SecUsuarioNavigation != null ? origen.SecUsuarioNavigation.Nombre : "N/A"))
                .ForMember(destino =>
                    destino.SecCotizacionOriginal,
                    opt => opt.MapFrom(origen => origen.SecCotizacionOriginal))
                .ForMember(destino =>
                    destino.SecUsuarioModifica,
                    opt => opt.MapFrom(origen => origen.SecUsuarioModifica))
                .ForMember(destino =>
                    destino.NombreUsuarioModifica,
                    opt => opt.MapFrom(origen => origen.SecUsuarioModificaNavigation != null ? origen.SecUsuarioModificaNavigation.Nombre : "N/A"));;

            CreateMap<CotizacionVM, Cotizacion>()
                .ForMember(destino =>
                    destino.EstaActivo,
                    opt => opt.MapFrom(origen => (short)origen.EstaActivo))
                .ForMember(destino =>
                    destino.SecVisitaNavigation,
                    opt => opt.Ignore())
                // New mappings for tax fields
                .ForMember(destino => destino.Subtotal, opt => opt.MapFrom(origen => origen.Subtotal))
                .ForMember(destino => destino.ValorImpuestos, opt => opt.MapFrom(origen => origen.ValorImpuestos))
                .ForMember(destino => destino.TotalConImpuestos, opt => opt.MapFrom(origen => origen.TotalConImpuestos))
                .ForMember(destino => destino.ValorIVA, opt => opt.MapFrom(origen => origen.ValorIVA))
                .ForMember(destino => destino.ValorImportacion, opt => opt.MapFrom(origen => origen.ValorImportacion))
                .ForMember(destino =>
                    destino.SecUsuario,
                    opt => opt.MapFrom(origen => origen.SecUsuario))
                .ForMember(destino =>
                    destino.SecCotizacionOriginal,
                    opt => opt.MapFrom(origen => origen.SecCotizacionOriginal))
                .ForMember(destino =>
                    destino.SecUsuarioModifica,
                    opt => opt.MapFrom(origen => origen.SecUsuarioModifica));

            CreateMap<Cotizaciondetalle, CotizaciondetalleVM>()
                .ForMember(destino =>
                    destino.ValorCompra,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.ValorCompra, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(destino =>
                    destino.MargenGanancia,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.MargenGanancia, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(destino =>
                    destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToString(origen.Total, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(destino =>
                    destino.Cantidad,
                    opt => opt.MapFrom(origen => origen.Cantidad));

            CreateMap<CotizaciondetalleVM, Cotizaciondetalle>()
                .ForMember(destino =>
                    destino.ValorCompra,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.ValorCompra, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(destino =>
                    destino.MargenGanancia,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.MargenGanancia, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(destino =>
                    destino.Total,
                    opt => opt.MapFrom(origen => Convert.ToDecimal(origen.Total, System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(destino =>
                    destino.Cantidad,
                    opt => opt.MapFrom(origen => origen.Cantidad));
            #endregion

            CreateMap<Etapa, EtapaVM>().ReverseMap();

            CreateMap<TipoImpuesto, TipoImpuestoVM>().ReverseMap();

            CreateMap<Impuesto, ImpuestoVM>()
                .ForMember(destino =>
                           destino.NombreTipoImpuesto,
                                   opt =>
                                   opt.MapFrom(origen =>
                                               origen.SecTipoImpuestoNavigation.Nombre))
                        .ForMember(destino =>
                                   destino.EsIva,
                                           opt =>
                                           opt.MapFrom(origen =>
                                                       origen.SecTipoImpuestoNavigation.EsIva))
                        .ForMember(destino =>
                                   destino.EsImportacion,
                                           opt =>
                                           opt.MapFrom(origen =>
                                                       origen.SecTipoImpuestoNavigation.EsImportacion));

            CreateMap<ImpuestoVM, Impuesto>()
                .ForMember(destino =>
                           destino.SecTipoImpuestoNavigation,
                                   opt =>
                                   opt.Ignore());

            CreateMap<ImpuestoCotizacion, ImpuestoCotizacionVM>()
                .ForMember(destino =>
                           destino.NombreImpuesto,
                                   opt =>
                                   opt.MapFrom(origen => origen.ImpuestoNavigation.Descripcion))
                .ForMember(destino =>
                           destino.TipoImpuestoDescripcion,
                                   opt =>
                                   opt.MapFrom(origen => origen.ImpuestoNavigation.SecTipoImpuestoNavigation.Nombre))
                .ForMember(destino =>
                           destino.FechaRegistro,
                                   opt =>
                                   opt.MapFrom(origen => origen.FechaRegistro.ToString("dd/MM/yyyy")));

            CreateMap<ImpuestoCotizacionVM, ImpuestoCotizacion>()
                .ForMember(destino =>
                           destino.ImpuestoNavigation,
                                   opt =>
                                   opt.Ignore());
        }
    }
}

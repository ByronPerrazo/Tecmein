using AutoMapper;
using BLL.DTOs;
using Entity;
using TecmeinAplicacionWeb.Models.ViewModels;

namespace TecmeinAplicacionWeb.Utilidades.AutoMapper // <-- Restaurado
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

            CreateMap<TipoDocumento, TipoDocumentoVM>()
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => origen.FechaRegistro.HasValue ? origen.FechaRegistro.Value.ToString("dd/MM/yyyy") : null));

            CreateMap<TipoDocumentoVM, TipoDocumento>()
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => !string.IsNullOrEmpty(origen.FechaRegistro) ? DateTime.ParseExact(origen.FechaRegistro, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?)null));

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
                .ForMember(destino => destino.DescripcionEtapa,
                            opt => opt.MapFrom(origen =>
                                               origen.IdEtapaNavigation.Descripcion))
                .ForMember(destino => destino.NombreEmpresa,
                            opt => opt.MapFrom(origen =>
                                               origen.SecEmpresaNavigation.Nombre))
                .ForMember(destino => destino.NombreConstructora,
                            opt => opt.MapFrom(origen =>
                                               origen.SecConstructoraNavigation.Nombre))
                .ForMember(destino => destino.CodigoEtapa,
                            opt => opt.MapFrom(origen =>
                                               origen.IdEtapaNavigation.Codigo))
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


            CreateMap<Constructora, ConstructoraVM>().ReverseMap();

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

            CreateMap<FormatoNumeroCliente, FormatoNumeroClienteVM>().ReverseMap();

            CreateMap<Cliente, ClienteVM>()
                .ForMember(destino =>
                    destino.SecConstructora, // Añadido para que el VM tenga el ID
                    opt => opt.MapFrom(origen => origen.SecConstructora))
                .ForMember(destino =>
                    destino.NombreConstructora,
                    opt => opt.MapFrom(origen => origen.SecConstructoraNavigation.Nombre))
                .ForMember(destino =>
                    destino.FechaCreacion,
                    opt => opt.MapFrom(origen => origen.FechaCreacion.ToString("dd/MM/yyyy")));

            CreateMap<ClienteVM, Cliente>()
                .ForMember(destino =>
                    destino.SecCliente,
                    opt => opt.MapFrom(origen => origen.SecCliente))
                .ForMember(destino =>
                    destino.SecConstructora,
                    opt => opt.MapFrom(origen => origen.SecConstructora))
                .ForMember(destino =>
                    destino.EstaActivo,
                    opt => opt.MapFrom(origen => origen.EstaActivo))
                .ForMember(destino =>
                    destino.SecConstructoraNavigation, // Ignorar la propiedad de navegación
                    opt => opt.Ignore());





            CreateMap<Seguimiento, SeguimientoVM>().ReverseMap();

            CreateMap<FormaPago, FormaPagoVM>().ReverseMap();

            CreateMap<PlantillaPreContrato, PlantillaPreContratoVM>()
                .ForMember(destino => destino.DescripcionTipoDocumento,
                           opt => opt.MapFrom(origen => origen.SecTipoDocumentoNavigation.Descripcion))
                .ReverseMap();

            CreateMap<PlantillaPreContratoParrafo, PlantillaPreContratoParrafoVM>();

            CreateMap<PlantillaPreContratoParrafoVM, PlantillaPreContratoParrafo>();

            CreateMap<RolPermiso, RolPermisoVM>()
                .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.Rol.Descripcion))
                .ForMember(dest => dest.DescripcionPermiso, opt => opt.MapFrom(src => src.Permiso.Descripcion));

            CreateMap<RolPermisoVM, RolPermiso>();

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

            CreateMap<PolizaGarantia, PolizaGarantiaVM>().ReverseMap();

            //CreateMap<PreContratoModalVM, PreContrato>()
            //    .ForMember(dest => dest.FechaAnticipo, opt => opt.MapFrom(src =>
            //        !string.IsNullOrEmpty(src.FechaAnticipo.ToString()) ? (DateTime?)DateTime.Parse(src.FechaAnticipo.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null))
            //    .ForMember(dest => dest.FechaPrimeraCuota, opt => opt.MapFrom(src =>
            //        !string.IsNullOrEmpty(src.FechaPrimeraCuota.ToString()) ? (DateTime?)DateTime.Parse(src.FechaPrimeraCuota.ToString(), System.Globalization.CultureInfo.InvariantCulture) : null));

            CreateMap<PreContrato, PreContratoVM>()
                .ForMember(destino => destino.NombreUsuarioCrea,
                           opt => opt.MapFrom(origen => origen.SecUsuarioCreaNavigation.Nombre))
                .ForMember(destino => destino.NombreObra,
                           opt => opt.MapFrom(origen => origen.SecCotizacionNavigation.SecVisitaNavigation.Nombre ?? "Sin Nombre de Obra"))
                .ForMember(destino => destino.NumeroCotizacion,
                           opt => opt.MapFrom(origen => origen.SecCotizacionNavigation.Secuencial.ToString()))
                .ForMember(destino => destino.NombreCliente,
                           opt => opt.MapFrom(origen =>
                                origen.SecCotizacionNavigation == null ? "N/A" :
                                origen.SecCotizacionNavigation.SecVisitaNavigation == null ? "N/A" :
                                origen.SecCotizacionNavigation.SecVisitaNavigation.Contactovisita == null || !origen.SecCotizacionNavigation.SecVisitaNavigation.Contactovisita.Any() ? "N/A" :
                                origen.SecCotizacionNavigation.SecVisitaNavigation.Contactovisita.FirstOrDefault().SecContactoNavigation == null ? "N/A" :
                                origen.SecCotizacionNavigation.SecVisitaNavigation.Contactovisita.FirstOrDefault().SecContactoNavigation.Nombres + " " +
                                origen.SecCotizacionNavigation.SecVisitaNavigation.Contactovisita.FirstOrDefault().SecContactoNavigation.Apellidos
                           ))
                .ForMember(destino => destino.NumeroCotizacion,
                           opt => opt.MapFrom(origen => origen.SecCotizacion.ToString()));
            CreateMap<PreContratoVM, PreContrato>();

            #region Contrato
            CreateMap<Contrato, ContratoVM>()
                .ForMember(dest => dest.NombreObra,
                           opt => opt.MapFrom(src => src.IdCotizacionNavigation.SecVisitaNavigation.Nombre ?? "Sin Obra Asociada"))
                .ForMember(dest => dest.NombreUsuarioCarga,
                           opt => opt.MapFrom(src => src.IdUsuarioCargaNavigation.Nombre))
                .ForMember(dest => dest.FechaFirma,
                           opt => opt.MapFrom(src => src.FechaFirma.ToString("dd/MM/yyyy")));

            CreateMap<ContratoVM, Contrato>()
                .ForMember(dest => dest.FechaFirma, 
                           opt => opt.MapFrom(src => DateTime.ParseExact(src.FechaFirma, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.IdCotizacionNavigation, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioCargaNavigation, opt => opt.Ignore());
            #endregion

            CreateMap<PlanDePago, PlanDePagoVM>()
                .ForMember(destino => destino.DescripcionFormaPago,
                           opt => opt.MapFrom(origen => origen.SecFormaPagoNavigation.Descripcion))
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => origen.FechaRegistro.ToString("dd/MM/yyyy")))
                .ForMember(destino => destino.FechaAnticipo,
                           opt => opt.MapFrom(origen => origen.FechaAnticipo.HasValue ? origen.FechaAnticipo.Value.ToString("dd/MM/yyyy") : null))
                .ForMember(destino => destino.FechaPrimeraCuota,
                           opt => opt.MapFrom(origen => origen.FechaPrimeraCuota.HasValue ? origen.FechaPrimeraCuota.Value.ToString("dd/MM/yyyy") : null));
            CreateMap<PlanDePagoVM, PlanDePago>()
                .ForMember(destino => destino.SecFormaPagoNavigation, opt => opt.Ignore());

            CreateMap<Cuota, CuotaVM>()
                .ForMember(destino => destino.FechaVencimiento,
                           opt => opt.MapFrom(origen => origen.FechaVencimiento.ToString("dd/MM/yyyy")))
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => origen.FechaRegistro.ToString("dd/MM/yyyy")));
            CreateMap<CuotaVM, Cuota>();

            #region Plan de Pago DTO
            CreateMap<PlanDePago, PlanDePagoDTO>().ReverseMap();
            CreateMap<Cuota, CuotaDTO>().ReverseMap();
            CreateMap<Pago, PagoDTO>().ReverseMap();

            #endregion


        }
    }
}

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
                           .ForMember(destino => destino.SecMenuPadre, opt => opt.MapFrom(origen => origen.SecMenuPadre))
                           .ForMember(destino => destino.SubMenu, opt => opt.Ignore());

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
                                                                               $" Recorrido: {origen.Recorrido} -" +
                                                                               $" Sbr. Recorrido: {origen.SobreRecorrido} -" +
                                                                               $" Ing. Frontales: {origen.IngresosFrontales} -" +
                                                                               $" Ing. Posteriores: {origen.IngresosPosteriores} -" +
                                                                               $" Dime Entrada: {origen.DimencionEntrada} -" +
                                                                               $" Alt Entre Pisos: {origen.AlturaEntrePisos} -" +
                                                                               $" Energia: {origen.Energia} -" +
                                                                               $" Puertas: {origen.MaterialPuertas} -" +
                                                                               $" Num. Paradas{origen.NumeroParadas}-" +
                                                                               $" Nomb. Paradas{origen.NombresParadas}-" +
                                                                               $" Num Personas:{origen.NumeroPersonas} "))
                                                                      .ForMember(destino =>
                                                                                 destino.DescripcionImpresa,
                                                                                     opt =>
                                                                                     opt.MapFrom(origen =>
                                                                                         $" Sistema del Equipo: {ConstantesEquipos.Sistema.ObtenerValores().GetValueOrDefault(origen.Sistema, origen.Sistema)} " + Environment.NewLine +
                                                                                         $" Tipo Equipo: {ConstantesEquipos.TipoEquipo.ObtenerValores().GetValueOrDefault(origen.TipoEquipo, origen.TipoEquipo)} " + Environment.NewLine +
                                                                                         $" Marca del Equipo: {ConstantesEquipos.Marca.ObtenerValores().GetValueOrDefault(origen.Marca, origen.Marca)} " + Environment.NewLine +
                                                                                         $" Capacidad: {origen.Capacidad} " + Environment.NewLine +
                                                                                         $" Velocidad: {origen.Velocidad} " + Environment.NewLine +
                                                                                         $" Sala Maquina: {ConstantesEquipos.SalaMaquinas.ObtenerValores().GetValueOrDefault(origen.SalaMaquinas, origen.SalaMaquinas)} " + Environment.NewLine +
                                                                                         $" Motor: {ConstantesEquipos.TipoMotor.ObtenerValores().GetValueOrDefault(origen.TipoMotor, origen.TipoMotor)} " + Environment.NewLine +
                                                                                         $" Embarque: {ConstantesEquipos.TipoEmbarque.ObtenerValores().GetValueOrDefault(origen.Embarque, origen.Embarque)} " + Environment.NewLine +
                                                                                         $" Tipo Ducto: {ConstantesEquipos.TipoDucto.ObtenerValores().GetValueOrDefault(origen.TipoDucto, origen.TipoDucto)} " + Environment.NewLine +
                                                                                         $" Medidas Ducto Ancho Fondo: {origen.MedidasAfducto} " + Environment.NewLine +
                                                                                         $" Foso: {origen.Foso} " + Environment.NewLine +
                                                                                         $" Recorrido: {origen.Recorrido} " + Environment.NewLine +
                                                                                         $" Sobre Recorrido: {origen.SobreRecorrido} " + Environment.NewLine +
                                                                                         $" Ingresos Frontales: {origen.IngresosFrontales} " + Environment.NewLine +
                                                                                         $" Ingresos Posteriores: {origen.IngresosPosteriores} " + Environment.NewLine +
                                                                                         $" Dimención Entrada: {origen.DimencionEntrada} " + Environment.NewLine +
                                                                                         $" Altura Entre Pisos: {origen.AlturaEntrePisos} " + Environment.NewLine +
                                                                                         $" Energía: {ConstantesEquipos.TipoEnergia.ObtenerValores().GetValueOrDefault(origen.Energia, origen.Energia)} " + Environment.NewLine +
                                                                                         $" Material de Puertas: {ConstantesEquipos.MaterialPuertas.ObtenerValores().GetValueOrDefault(origen.MaterialPuertas, origen.MaterialPuertas)} " + Environment.NewLine +
                                                                                         $" Numero de Paradas: {origen.NumeroParadas}" + Environment.NewLine +
                                                                                         $" Nombres de Paradas: {origen.NombresParadas}" + Environment.NewLine +
                                                                                         $" Numero de Personas: {origen.NumeroPersonas} "))
                            .ForMember(destino =>
                                       destino.DetalleEspecifico, opt =>
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
                                                   $" Num Personas:{origen.NumeroPersonas} "));


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

            CreateMap<FormaPago, FormaPagoVM>()
                .ForMember(destino => destino.EstaActivo,
                           opt => opt.MapFrom(origen => origen.EstaActivo == 1)); // short a bool
            CreateMap<FormaPagoVM, FormaPago>()
                .ForMember(destino => destino.EstaActivo,
                           opt => opt.MapFrom(origen => (short)(origen.EstaActivo ? 1 : 0))); // bool a short

            CreateMap<PlantillaPreContrato, PlantillaPreContratoVM>()
                .ForMember(destino => destino.DescripcionTipoDocumento,
                           opt => opt.MapFrom(origen => origen.SecTipoDocumentoNavigation.Descripcion))
                .ReverseMap();

            CreateMap<PlantillaPreContratoParrafo, PlantillaPreContratoParrafoVM>();

            CreateMap<PlantillaPreContratoParrafoVM, PlantillaPreContratoParrafo>();





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
                    opt => opt.MapFrom(origen => origen.SecUsuarioModificaNavigation != null ? origen.SecUsuarioModificaNavigation.Nombre : "N/A")); ;

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
                    opt => opt.MapFrom(origen => origen.ValorCompra))
                .ForMember(destino =>
                    destino.MargenGanancia,
                    opt => opt.MapFrom(origen => origen.MargenGanancia))
                .ForMember(destino =>
                    destino.Total,
                    opt => opt.MapFrom(origen => origen.Total))
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
                .ForMember(dest => dest.NombreProyecto,
                           opt => opt.MapFrom(src => src.IdCotizacionNavigation.SecVisitaNavigation.Nombre ?? "Sin Obra Asociada"))
                .ForMember(dest => dest.NombreCliente,
                           opt => opt.MapFrom(src => src.SecClienteNavigation != null && src.SecClienteNavigation.SecConstructoraNavigation != null ? src.SecClienteNavigation.SecConstructoraNavigation.Nombre : "N/A"))
                .ForMember(dest => dest.NombreUsuarioCarga,
                           opt => opt.MapFrom(src => src.IdUsuarioCargaNavigation.Nombre))
                .ForMember(dest => dest.FechaFirma,
                           opt => opt.MapFrom(src => src.FechaFirma.ToString("dd/MM/yyyy")));

            CreateMap<ContratoVM, Contrato>()
                .ForMember(dest => dest.FechaFirma,
                           opt => opt.MapFrom(src => DateTime.ParseExact(src.FechaFirma, "dd/MM/yyyy", new System.Globalization.CultureInfo("es-ES"))))
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

            CreateMap<PlanDePago, PlanDePagoVM>()
                .ForMember(destino => destino.DescripcionFormaPago,
                           opt => opt.MapFrom(origen => origen.SecFormaPagoNavigation.Descripcion))
                .ForMember(destino => destino.FechaAnticipo,
                           opt => opt.MapFrom(origen => origen.FechaAnticipo.HasValue ? origen.FechaAnticipo.Value.ToString("dd/MM/yyyy") : null))
                .ForMember(destino => destino.FechaPrimeraCuota,
                           opt => opt.MapFrom(origen => origen.FechaPrimeraCuota.HasValue ? origen.FechaPrimeraCuota.Value.ToString("dd/MM/yyyy") : null))
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => origen.FechaRegistro.ToString("dd/MM/yyyy")))
                .ForMember(destino => destino.EstaActivo,
                           opt => opt.MapFrom(origen => origen.EstaActivo))
                .ForMember(destino => destino.Cuotas,
                           opt => opt.MapFrom(origen => origen.Cuotas)); // Mapear la colección de cuotas

            CreateMap<PlanDePagoVM, PlanDePago>()
                .ForMember(destino => destino.SecFormaPagoNavigation, opt => opt.Ignore())
                .ForMember(destino => destino.IdContratoNavigation, opt => opt.Ignore())
                .ForMember(destino => destino.Pagos, opt => opt.Ignore())
                .ForMember(destino => destino.Cuotas, opt => opt.Ignore()) // Las cuotas se manejan por separado
                .ForMember(destino => destino.FechaAnticipo,
                           opt => opt.MapFrom(origen => !string.IsNullOrEmpty(origen.FechaAnticipo) ? DateTime.ParseExact(origen.FechaAnticipo, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?)null))
                .ForMember(destino => destino.FechaPrimeraCuota,
                           opt => opt.MapFrom(origen => !string.IsNullOrEmpty(origen.FechaPrimeraCuota) ? DateTime.ParseExact(origen.FechaPrimeraCuota, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : (DateTime?)null))
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => DateTime.ParseExact(origen.FechaRegistro, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)));

            CreateMap<Cuota, CuotaVM>()
                .ForMember(destino => destino.IdPlanDePago, opt => opt.MapFrom(origen => origen.IdPlanDePago))
                .ForMember(destino => destino.MontoPagado, opt => opt.MapFrom(origen => origen.MontoPagado)) // Map nullable decimal
                .ForMember(destino => destino.FechaVencimiento,
                           opt => opt.MapFrom(origen => origen.FechaVencimiento.ToString("dd/MM/yyyy")))
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => origen.FechaRegistro.ToString("dd/MM/yyyy")));
            CreateMap<CuotaVM, Cuota>()
                .ForMember(destino => destino.IdPlanDePago, opt => opt.MapFrom(origen => origen.IdPlanDePago))
                .ForMember(destino => destino.MontoPagado, opt => opt.MapFrom(origen => origen.MontoPagado)) // Map nullable decimal
                .ForMember(destino => destino.FechaVencimiento,
                           opt => opt.MapFrom(origen => !string.IsNullOrEmpty(origen.FechaVencimiento) ? DateTime.ParseExact(origen.FechaVencimiento, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue))
                .ForMember(destino => destino.FechaRegistro,
                           opt => opt.MapFrom(origen => !string.IsNullOrEmpty(origen.FechaRegistro) ? DateTime.ParseExact(origen.FechaRegistro, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture) : DateTime.MinValue));

            #region Plan de Pago DTO
            CreateMap<PlanDePago, PlanDePagoDTO>().ReverseMap();
            CreateMap<Cuota, CuotaDTO>().ReverseMap();
            CreateMap<Pago, PagoDTO>().ReverseMap();
            
            // DTO to VM Mappings
            CreateMap<PlanDePagoDTO, PlanDePagoVM>();
            CreateMap<CuotaDTO, CuotaVM>();
            CreateMap<PlanPagoDashboardDTO, PlanPagoDashboardVM>();
            CreateMap<DetallePlanPagoDTO, DetallePlanPagoVM>();

            // VM to DTO Mappings
            CreateMap<PlanDePagoVM, PlanDePagoDTO>();
            CreateMap<CuotaVM, CuotaDTO>();
            #endregion

            #region Auditoria
            CreateMap<AuditoriaEvento, AuditoriaEventoVM>()
                .ForMember(dest => dest.FechaHora, opt => opt.MapFrom(src => src.FechaHora.ToString("dd/MM/yyyy HH:mm:ss")));
            #endregion

            #region PreContratoCompromisoPago
            CreateMap<PreContratoCompromisoPago, PreContratoCompromisoPagoVM>()
                .ForMember(destino => destino.FechaVencimiento,
                           opt => opt.MapFrom(origen => origen.FechaVencimiento.ToString("dd/MM/yyyy")));
            CreateMap<PreContratoCompromisoPagoVM, PreContratoCompromisoPago>()
                .ForMember(destino => destino.FechaVencimiento,
                           opt => opt.MapFrom(origen => DateTime.ParseExact(origen.FechaVencimiento, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)));
            #endregion

            #region Pagos
            CreateMap<RegistrarPagoVM, PagoDTO>()
                .ForMember(destino => destino.FechaPago,
                           opt => opt.MapFrom(origen => origen.FechaPago)) // Direct DateTime mapping
                .ForMember(destino => destino.RegistradoPorUsuarioId, opt => opt.MapFrom(origen => origen.RegistradoPorUsuarioId))
                .ForMember(destino => destino.IdPlanDePago, opt => opt.MapFrom(origen => origen.IdPlanDePago))
                .ForMember(destino => destino.Monto, opt => opt.MapFrom(origen => origen.Monto))
                .ForMember(destino => destino.ComprobanteUrl, opt => opt.MapFrom(origen => origen.ComprobanteUrl))
                .ForMember(destino => destino.ComprobanteNombre, opt => opt.MapFrom(origen => origen.ComprobanteNombre))
                .ForMember(destino => destino.EstaActivo, opt => opt.MapFrom(origen => true)) // Los pagos nuevos siempre están activos por defecto
                .ForMember(destino => destino.FechaRegistro, opt => opt.MapFrom(origen => DateTime.Now)); // Fecha de registro actual
            #endregion

        }
    }
}

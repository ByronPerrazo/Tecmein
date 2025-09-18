using BLL.Implementacion;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Implementacion;
using DAL.Interfaces;
using Entity; // Added this line
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbContext<TecmeindbContext>(options =>
                {
                    options
                    .UseMySql(configuration.GetConnectionString("ConexionDB"),
                              Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"),
                              o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
                });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSingleton<IDatosGlobalesServices, DatosGlobalesServicio>();

            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<IRolServices, RolServices>();
            services.AddScoped<IStorageServices, StorageServices>();
            services.AddScoped<IUtilidadesServices, UtilidadesServices>();
            services.AddScoped<ICorreoServices, CorreoServices>();
            services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
            services.AddScoped<IEmpresaStorageServices, EmpresaStorageServices>();
            services.AddScoped<IEmpresaServices, EmpresaServices>();
            services.AddScoped<IMenuServices, MenuServices>();
            services.AddScoped<IProvinciaServices, ProvinciaServices>();
            services.AddScoped<ICantonServices, CantonServices>();
            services.AddScoped<IParroquiaServices, ParroquiaServices>();
            services.AddScoped<ICatalogoServices, CatalogoServices>();
            services.AddScoped<IConstructoraServices, ConstructoraServices>();
            services.AddScoped<IContactoServices, ContactoServices>();
            services.AddScoped<IContactoVisitaServices, ContactoVisitaServices>();
            services.AddScoped<IEquiposVisitaServices, EquiposVisitaServices>();
            services.AddScoped<IDashBoardServices, DashBoardServices>(provider =>
                new DashBoardServices(
                    provider.GetRequiredService<IGenericRepository<Visita>>(),
                    provider.GetRequiredService<IGenericRepository<Equiposvisita>>()
                ));
            services.AddScoped<IPermisosRolServices, PermisosRolServices>();
            services.AddScoped<IPermisoServices, PermisoServices>();
            services.AddScoped<IRolMenuServices, RolMenuServices>();
            services.AddScoped<IValidacionServices, ValidacionServices>();
            services.AddScoped<IMenusHijosDesplegables, MenusHijosDesplegables>();
            services.AddScoped<IImpuestoServices, ImpuestoServices>();
            services.AddScoped<ITipoImpuestoServices, TipoImpuestoServices>();
            services.AddScoped<ICotizacionServices, CotizacionServices>(provider =>
                new CotizacionServices(
                    provider.GetRequiredService<IGenericRepository<Cotizacion>>(),
                    provider.GetRequiredService<IGenericRepository<Cotizaciondetalle>>(),
                    provider.GetRequiredService<IGenericRepository<ImpuestoCotizacion>>(),
                    provider.GetRequiredService<IImpuestoServices>(),
                    provider.GetRequiredService<IVisitaServices>(),
                    provider.GetRequiredService<ITipoImpuestoServices>(),
                    provider.GetRequiredService<IEquiposVisitaServices>()
                ));
            services.AddScoped<IEtapaServices, EtapaServices>();
            services.AddScoped<IVisitaServices, VisitaServices>();
            services.AddScoped<IClienteServices, ClienteServices>();
            services.AddScoped<IFormatoNumeroClienteServices, FormatoNumeroClienteServices>();
            services.AddScoped<ISeguimientoServices, SeguimientoServices>(provider =>
                new SeguimientoServices(
                    provider.GetRequiredService<IGenericRepository<Seguimiento>>(),
                    provider.GetRequiredService<ICotizacionServices>(),
                    provider.GetRequiredService<IVisitaServices>(),
                    provider.GetRequiredService<IGenericRepository<Cotizacion>>(),
                    provider.GetRequiredService<IGenericRepository<PreContrato>>()
                ));
            services.AddScoped<AutorizacionService>();
            services.AddScoped<IGenericRepository<Contactovisita>, GenericRepository<Contactovisita>>();
            services.AddScoped<IGenericRepository<Equiposvisita>, GenericRepository<Equiposvisita>>();
            services.AddScoped<IGenericRepository<PreContratoParrafo>, GenericRepository<PreContratoParrafo>>();
            services.AddScoped<IPreContratoServices, PreContratoServices>(provider =>
                new PreContratoServices(
                    provider.GetRequiredService<IGenericRepository<PreContrato>>(),
                    provider.GetRequiredService<ICotizacionServices>(),
                    provider.GetRequiredService<IGenericRepository<PreContratoParrafo>>(),
                    provider.GetRequiredService<IPreContratoGeneratorService>()
                ));
            services.AddScoped<IFormaPagoServices, FormaPagoServices>();
            services.AddScoped<IPreContratoGeneratorService, PreContratoGeneratorService>(provider =>
                new PreContratoGeneratorService(
                    provider.GetRequiredService<TecmeindbContext>(),
                    provider.GetRequiredService<IGenericRepository<DiccionarioParametro>>()
                ));

            services.AddScoped<IPlantillaPreContratoServices, PlantillaPreContratoServices>();
            services.AddScoped<IPlantillaPreContratoParrafoServices, PlantillaPreContratoParrafoServices>();
            services.AddScoped<IDiccionarioParametroService, DiccionarioParametroService>();
            services.AddScoped<ITipoDocumentoServices, TipoDocumentoServices>();
            services.AddScoped<IPolizaGarantiaServices, PolizaGarantiaServices>();
            services.AddScoped<IGenericRepository<PolizaGarantia>, GenericRepository<PolizaGarantia>>();

            // Registro del patrón Strategy para la generación de documentos
            services.AddScoped<IEstrategiaGeneradorDocumento, EstrategiaPreContrato>();

            services.AddScoped<IGeneradorDocumentoService, GeneradorDocumentoService>();


        }
    }
}
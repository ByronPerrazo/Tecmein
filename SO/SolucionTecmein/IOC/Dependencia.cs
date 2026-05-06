using AutoMapper;
using BLL.Implementacion;
using BLL.Implementacion.ContractEngine;
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
                              o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .EnableSensitiveDataLogging();
                });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IGenericRepository<Cuota>, GenericRepository<Cuota>>(); // Corrected: Cuota is Cuota entity
            services.AddScoped<IGenericRepository<Pago>, GenericRepository<Pago>>(); // Register Pago repository
            services.AddScoped<IGenericRepository<PlanDePago>, GenericRepository<PlanDePago>>(); // Register PlanDePago repository
            services.AddScoped<IGenericRepository<RolPermiso>, GenericRepository<RolPermiso>>(); // New: Register RolPermiso repository
            services.AddSingleton<IDatosGlobalesServices, DatosGlobalesServicio>();

            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<IRolServices, RolServices>();
            services.AddScoped<IStorageServices, LocalStorageService>();
            services.AddScoped<IUtilidadesServices, UtilidadesServices>();
            services.AddScoped<ICorreoServices, CorreoServices>();
            services.AddScoped<ISmtpClientWrapper, SmtpClientWrapper>();
            services.AddScoped<IEmpresaStorageServices, EmpresaStorageServices>();
            services.AddScoped<IEmpresaServices, EmpresaServices>();
            services.AddScoped<IMenuServices, MenuServices>();
            services.AddScoped<IMenuDiscoveryService, MenuDiscoveryService>();
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
                    provider.GetRequiredService<IGenericRepository<Equiposvisita>>(),
                    provider.GetRequiredService<IGenericRepository<Contrato>>(),
                    provider.GetRequiredService<IGenericRepository<Cliente>>(),
                    provider.GetRequiredService<IGenericRepository<Cuota>>(),
                    provider.GetRequiredService<IGenericRepository<PlanDePago>>()
                ));

            services.AddScoped<IPermisoServices, PermisoServices>();
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
                    provider.GetRequiredService<IEquiposVisitaServices>(),
                    provider.GetRequiredService<IAuditService>(),
                    provider.GetRequiredService<IUsuarioServices>(),
                    provider.GetRequiredService<IMapper>(),
                    provider.GetRequiredService<IUnitOfWork>()
                ));
            services.AddScoped<IEtapaServices, EtapaServices>();
            services.AddScoped<IVisitaServices, VisitaServices>();
            services.AddScoped<IClienteServices, ClienteServices>(provider =>
                new ClienteServices(
                    provider.GetRequiredService<IGenericRepository<Cliente>>(),
                    provider.GetRequiredService<IGenericRepository<FormatoNumeroCliente>>(),
                    provider.GetRequiredService<IGenericRepository<Visita>>(),
                    provider.GetRequiredService<IGenericRepository<Cotizacion>>(),
                    provider.GetRequiredService<IGenericRepository<Contrato>>(),
                    provider.GetRequiredService<IGenericRepository<PlanDePago>>(),
                    provider.GetRequiredService<IGenericRepository<Etapa>>(),
                    provider.GetRequiredService<IGenericRepository<FormaPago>>(),
                    provider.GetRequiredService<IConstructoraServices>(),
                    provider.GetRequiredService<IStorageServices>(),
                    provider.GetRequiredService<IGenericRepository<Cuota>>(),
                    provider.GetRequiredService<IUnitOfWork>()
                ));
            services.AddScoped<IFormatoNumeroClienteService, FormatoNumeroClienteService>();
            services.AddScoped<ISeguimientoServices, SeguimientoServices>(provider =>
                new SeguimientoServices(
                    provider.GetRequiredService<IGenericRepository<Seguimiento>>(),
                    provider.GetRequiredService<ICotizacionServices>(),
                    provider.GetRequiredService<IVisitaServices>(),
                    provider.GetRequiredService<IGenericRepository<Cotizacion>>(),
                    provider.GetRequiredService<IGenericRepository<PreContrato>>(),
                    provider.GetRequiredService<IUnitOfWork>()
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
                    provider.GetRequiredService<IPreContratoGeneratorService>(),
                    provider.GetRequiredService<IVisitaServices>(),
                    provider.GetRequiredService<IGenericRepository<TipoDocumento>>(),
                    provider.GetRequiredService<IGenericRepository<PlantillaPreContrato>>(),
                    provider.GetRequiredService<IGenericRepository<PreContratoCompromisoPago>>(),
                    provider.GetRequiredService<IUsuarioServices>(),
                    provider.GetRequiredService<IMapper>(),
                    provider.GetRequiredService<IStorageServices>(),
                    provider.GetRequiredService<IUnitOfWork>()
                ));
            services.AddScoped<IFormaPagoServices, FormaPagoServices>();
            
            // Motores de resolución de plantillas
            services.AddScoped<IPlaceholderProvider, FinancialPlaceholderProvider>();
            services.AddScoped<IPlaceholderProvider, GeneralPlaceholderProvider>();
            services.AddScoped<IPlaceholderProvider, EquipmentPlaceholderProvider>();
            
            services.AddScoped<IPreContratoGeneratorService, PreContratoGeneratorService>();

            services.AddScoped<IPlantillaPreContratoServices, PlantillaPreContratoServices>();
            services.AddScoped<IPlantillaPreContratoParrafoServices, PlantillaPreContratoParrafoServices>();
            services.AddScoped<IDiccionarioParametroService, DiccionarioParametroService>();
            services.AddScoped<ITipoDocumentoServices, TipoDocumentoServices>();
            services.AddScoped<IActivoClienteService, ActivoClienteService>(); // Añadido
            services.AddScoped<IPolizaGarantiaServices, PolizaGarantiaServices>();
            services.AddScoped<IGenericRepository<PolizaGarantia>, GenericRepository<PolizaGarantia>>();
            services.AddScoped<IPlanDePagoService, PlanDePagoService>(); // Registro para la interfaz correcta
            services.AddScoped<IPagoService, PagoService>(); // Simple registration
            services.AddScoped<IContratoService, ContratoService>(provider =>
                new ContratoService(
                    provider.GetRequiredService<IGenericRepository<Contrato>>(),
                    provider.GetRequiredService<IGenericRepository<Cotizacion>>(),
                    provider.GetRequiredService<IGenericRepository<Visita>>(),
                    provider.GetRequiredService<IClienteServices>(),
                    provider.GetRequiredService<IStorageServices>(),
                    provider.GetRequiredService<IGenericRepository<PreContrato>>(),
                    provider.GetRequiredService<IGenericRepository<Etapa>>(),
                    provider.GetRequiredService<TecmeindbContext>(),
                    provider.GetRequiredService<IGenericRepository<PlanDePago>>(),
                    provider.GetRequiredService<IGenericRepository<Cuota>>(),
                    provider.GetRequiredService<IGenericRepository<PreContratoCompromisoPago>>(),
                    provider.GetRequiredService<ITipoDocumentoServices>(),
                    provider.GetRequiredService<IActivoClienteService>(),
                    provider.GetRequiredService<IUnitOfWork>()
                ));

            // Registro del patrón Strategy para la generación de documentos
            services.AddScoped<IEstrategiaGeneradorDocumento, EstrategiaPreContrato>();

            services.AddScoped<IGeneradorDocumentoService, GeneradorDocumentoService>();

            // Nuevo Servicio de Auditoría
            services.AddScoped<IAuditService, AuditService>();

            // Registro de Unit of Work para Atomicidad
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Registro de IMemoryCache

            services.AddMemoryCache();



            // --- INICIO MCP (Model Context Protocol) ---

            services.AddHttpClient(); // Registra IHttpClientFactory

            services.AddScoped<DAL.Mcp.IMySqlRepository, DAL.Mcp.MySqlRepository>();

            services.AddScoped<BLL.Mcp.IMcpService, BLL.Mcp.McpService>();

            // --- FIN MCP ---



        }

    }

}


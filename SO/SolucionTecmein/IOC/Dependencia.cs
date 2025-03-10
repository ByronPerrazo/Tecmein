using BLL.Implementacion;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Implementacion;
using DAL.Interfaces;
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
                    .UseMySql(configuration.GetConnectionString("ConexionDBAWS"),
                              Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));
                });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddSingleton<IDatosGlobalesServices, DatosGlobalesServicio>();

            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<IRolServices, RolServices>();
            services.AddScoped<IStorageServices, StorageServices>();
            services.AddScoped<IUtilidadesServices, UtilidadesServices>();
            services.AddScoped<ICorreoServices, CorreoServices>();
            services.AddScoped<IEmpresaStorageServices, EmpresaStorageServices>();
            services.AddScoped<IEmpresaServices, EmpresaServices>();
            services.AddScoped<IMenuServices, MenuServices>();
            services.AddScoped<IProvinciaServices, ProvinciaServices>();
            services.AddScoped<ICantonServices, CantonServices>();
            services.AddScoped<IParroquiaServices, ParroquiaServices>();
            services.AddScoped<ITipoProductoServices, TipoProductoServices>();
            services.AddScoped<IProductoServices, ProductoServices>();
            services.AddScoped<IVisitaServices, VisitaServices>();
            services.AddScoped<ICatalogoServices, CatalogoServices>();
            services.AddScoped<IConstructoraServices, ConstructoraServices>();
            services.AddScoped<IContactoServices, ContactoServices>();
            services.AddScoped<IContactoVisitaServices, ContactoVisitaServices>();
            services.AddScoped<IEquiposVisitaServices, EquiposVisitaServices>();
            services.AddScoped<IDashBoardServices, DashBoardServices>();



        }
    }
}

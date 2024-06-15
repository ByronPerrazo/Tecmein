using BLL.Implementacion;
using BLL.Interfaces;
using DAL.DBContext;
using DAL.Implementacion;
using DAL.Interfaces;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                              Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql"));
                });

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<IRolServices, RolServices>();
            services.AddScoped<IStorageServices,StorageServices>();
            services.AddScoped<IUtilidadesServices, UtilidadesServices>();
            services.AddScoped<ICorreoServices, CorreoServices> ();
            services.AddScoped<IEmpresaStorageServices, EmpresaStorageServices> ();
            services.AddScoped<IEmpresaServices, EmpresaServices> ();
            services.AddScoped<IMenuServices, MenuServices>();
        }
    }
}

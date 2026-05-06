
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DAL.DBContext
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TecmeindbContext>
    {
        public TecmeindbContext CreateDbContext(string[] args)
        {
            // Esta es una ruta de ejemplo. Debería apuntar a tu appsettings.json en el proyecto web.
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TecmeinAplicacionWeb"))
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<TecmeindbContext>();
            var connectionString = configuration.GetConnectionString("ConexionDB");

            builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new TecmeindbContext(builder.Options, null!);
        }
    }
}

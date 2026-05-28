using BLL.Mcp;
using IOC;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using QuestPDF.Infrastructure;
using Serilog;
using TecmeinAplicacionWeb.Utilidades.AutoMapper;
using TecmeinWebApp.Utilidades.Filters;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    });

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Configuracion de Data Protection para persistir keys
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"C:\TecmeinKeys\"))
        .SetApplicationName("Tecmein");

    builder.Services.AddControllersWithViews(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
        options.Filters.Add<GlobalExceptionFilter>(); // Registro global
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(op =>
        {
            op.LoginPath = "/Acceso/Login";
            op.AccessDeniedPath = "/Home/AccessDenied";
            op.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            op.Cookie.HttpOnly = true;
            op.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            op.Cookie.SameSite = SameSiteMode.Strict;
        });

    // --- INICIO NUEVA CONFIGURACIÓN DE AUTORIZACIÓN ---
    builder.Services.AddAuthorization(options =>
    {
        // Políticas genéricas basadas en acciones
        options.AddPolicy("CanConsult", policy => policy.RequireClaim("Permission", "READ"));
        options.AddPolicy("CanModify", policy => policy.RequireClaim("Permission", "UPDATE"));
        options.AddPolicy("CanEliminar", policy => policy.RequireClaim("Permission", "DELETE"));
        options.AddPolicy("CanCreate", policy => policy.RequireClaim("Permission", "CREATE"));

        // Políticas específicas para controladores o acciones concretas
        // El claim "Roles.Administrar" se asigna directamente al rol en la pantalla de gestión.
        options.AddPolicy("Roles.Administrar", policy => policy.RequireClaim("Permission", "Roles.Administrar"));

        // Futuras políticas específicas se pueden añadir aquí...
    });
    // --- FIN NUEVA CONFIGURACIÓN DE AUTORIZACIÓN ---

    // Configure Gemini Settings
    builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiSettings"));
    builder.Services.AddHttpClient(); // Add HttpClientFactory
    builder.Services.AddLogging(); // Added to ensure ILogger is available

    builder.Services.InyectarDependencia(builder.Configuration);
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    QuestPDF.Settings.License = LicenseType.Community;

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // Serve files from C:\TecmeinFiles under /uploads locally
    var localFilesPath = @"C:\TecmeinFiles";
    if (!System.IO.Directory.Exists(localFilesPath))
    {
        System.IO.Directory.CreateDirectory(localFilesPath);
    }
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(localFilesPath),
        RequestPath = "/uploads"
    });

    app.UseStatusCodePages();

    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Acceso}/{action=login}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during bootstrapping");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
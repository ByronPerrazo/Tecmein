using IOC;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;
using Serilog;
using System;
using TecmeinWebApp.Utilidades.AutoMapper;

// Use the classic two-stage initialization for Serilog.
// This allows logging during startup, before the host is built.
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add services to the container.
    builder.Services.AddControllersWithViews().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(
            op =>
            {
                op.LoginPath = "/Acceso/Login";
                op.ExpireTimeSpan = TimeSpan.FromMinutes(20);
            }
        );

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CanConsult", policy => policy.RequireClaim("CanConsult", "True"));
        options.AddPolicy("CanModify", policy => policy.RequireClaim("CanModify", "True"));
        options.AddPolicy("CanDelete", policy => policy.RequireClaim("CanDelete", "True"));
    });

    builder.Services.InyectarDependencia(builder.Configuration);
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    QuestPDF.Settings.License = LicenseType.Community;

    var app = builder.Build();

    // This must be one of the first middleware.
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }
    app.UseStaticFiles();

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

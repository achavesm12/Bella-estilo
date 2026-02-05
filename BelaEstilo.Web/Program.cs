using BelaEstilo.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;
using System.Text;
using BelaEstilo.Web.Middleware;
using BelaEstilo.Infraestructure.Repository.Interfaces;
using BelaEstilo.Infraestructure.Repository.Implementations;
using BelaEstilo.Application.Services.Interfaces;
using BelaEstilo.Application.Profiles;
using BelaEstilo.Application.Services.Implementations;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using BelaEstilo.Application.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Requerido para usar HttpContext.Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Configurar D.I.
//Repository
builder.Services.AddTransient<IRepositoryProducto, RepositoryProducto>();
builder.Services.AddTransient<IRepositoryResena, RepositoryResena>();
builder.Services.AddTransient<IRepositoryPromocion, RepositoryPromocion>();
builder.Services.AddTransient<IRepositoryPedido, RepositoryPedido>();
builder.Services.AddTransient<IRepositoryCategoria, RepositoryCategoria>();
builder.Services.AddTransient<IRepositoryUsuario, RepositoryUsuario>();
builder.Services.AddTransient<IRepositoryEtiqueta, RepositoryEtiqueta>();
builder.Services.AddTransient<IRepositoryPedidoPersonalizado, RepositoryPedidoPersonalizado>();
builder.Services.AddTransient<IRepositoryPedidoProducto, RepositoryPedidoProducto>();


//Services
builder.Services.AddTransient<IServiceProducto, ServiceProducto>();
builder.Services.AddTransient<IServiceResena, ServiceResena>();
builder.Services.AddTransient<IServicePromocion, ServicePromocion>();
builder.Services.AddTransient<IServicePedido, ServicePedido>();
builder.Services.AddTransient<IServiceCategoria, ServiceCategoria>();
builder.Services.AddTransient<IServiceUsuario, ServiceUsuario>();
builder.Services.AddTransient<IServiceEtiqueta, ServiceEtiqueta>();
builder.Services.AddTransient<IServicePedidoPersonalizado, ServicePedidoPersonalizado>();
builder.Services.AddTransient<IServicePedidoProducto, ServicePedidoProducto>();
builder.Services.AddTransient<IServiceFactura, ServiceFactura>();
builder.Services.AddTransient<IServiceEmail, ServiceEmail>();

// Configuración de correo electrónico
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));


//Automapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<ProductoProfile>();
    config.AddProfile<ResenaProfile>();
    config.AddProfile<PromocionProfile>();
    config.AddProfile<PedidoProfile>();
    config.AddProfile<PedidoPersonalizadoProfile>();
    config.AddProfile<CategoriaProfile>();
    config.AddProfile<UsuarioProfile>();
    config.AddProfile<EtiquetaProfile>();
    config.AddProfile<PedidoProductoProfile>();

});

//// Configuar Conexión a la Base de Datos SQL 
builder.Services.AddDbContext<BelaEstiloContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerDataBase"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

//Configuración Serilog 
// Logger. P.E. Verbose = muestra SQl Statement 
var logger = new LoggerConfiguration()
// Limitar la información de depuración 
.MinimumLevel.Override("Microsoft", LogEventLevel.Error)
.Enrich.FromLogContext()
// Log LogEventLevel.Verbose muestra mucha información, pero no es necesaria solo para el proceso de depuración 
.WriteTo.Console(LogEventLevel.Information)
.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level ==
LogEventLevel.Information).WriteTo.File(@"Logs\Info-.log", shared: true, encoding:
Encoding.ASCII, rollingInterval: RollingInterval.Day))
.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level ==
LogEventLevel.Debug).WriteTo.File(@"Logs\Debug-.log", shared: true, encoding:
System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level ==
LogEventLevel.Warning).WriteTo.File(@"Logs\Warning-.log", shared: true, encoding:
System.Text.Encoding.ASCII, rollingInterval: RollingInterval.Day))
.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level ==
LogEventLevel.Error).WriteTo.File(@"Logs\Error-.log", shared: true, encoding: Encoding.ASCII,
rollingInterval: RollingInterval.Day))
.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level ==
LogEventLevel.Fatal).WriteTo.File(@"Logs\Fatal-.log", shared: true, encoding: Encoding.ASCII,
rollingInterval: RollingInterval.Day))
.CreateLogger();
builder.Host.UseSerilog(logger);
//*************************** 

//Configurar la cultura 
var supportedCultures = new[] { "es-CR", "en-US" };
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var cultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.DefaultRequestCulture = new RequestCulture("es-CR"); // español por defecto
    options.SupportedCultures = cultures;
    options.SupportedUICultures = cultures;
});

// Configuración de autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login/AccesoDenegado";
    });


var app = builder.Build();

//Configuracion de localización
var localizationOptions = app.Services
    .GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

app.UseRequestLocalization(localizationOptions);


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // Error control Middleware 
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

//Activar soporte a la solicitud de registro con SERILOG 
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization(); 
app.UseAntiforgery();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

using MiniLenguajeLexico.Application.Interfaces;
using MiniLenguajeLexico.Application.Services;
using MiniLenguajeLexico.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepositorioConfiguracionLexica, RepositorioConfiguracionLexicaSql>();
builder.Services.AddScoped<IServicioAnalizadorLexico, ServicioAnalizadorLexico>();
builder.Services.AddScoped<IServicioPalabrasReservadas, ServicioPalabrasReservadas>();
builder.Services.AddScoped<IServicioBaulErrores, ServicioBaulErrores>();
builder.Services.AddScoped<IServicioDelimitadores, ServicioDelimitadores>();
builder.Services.AddScoped<IRepositorioAnalisis, RepositorioAnalisisSql>();

var app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");
    var repositorioConfiguracionLexica = scope.ServiceProvider.GetRequiredService<IRepositorioConfiguracionLexica>();

    await repositorioConfiguracionLexica.InicializarCatalogosAsync();
    logger.LogInformation("La configuracion lexica fue validada e inicializada en la base de datos.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Analizador}/{action=Index}/{id?}");

app.Run();

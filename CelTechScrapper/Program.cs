
using Aplicacion.CasosDeUso.CalcularConectividad;
using Aplicacion.Servicios;
using CelTechScrapper.Aplicacion.Servicios.Geolocalizador;
using CelTechScrapper.CasosDeUso.ObtenerIndiceSaturacion;
using CelTechScrapper.Aplicacion.CasosDeUso.ObtenerPropiedades;
using CelTechScrapper.CasosDeUso.SimuladorDeAlquiler;
using CelTechScrapper.Infraestructura.ServiciosGeolocalizacion;
using CelTechScrapper.Infraestructura.ServiciosScraper;
using Infraestructura.ServiciosConectividad;
using Infraestructura.ServiciosOverpass;
using WebApi.Controladores;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


// Controladores
builder.Services.AddControllers();

// Inyección de dependencias
builder.Services.AddScoped<IPropiedadScraperService, ArgenpropScraperService>();
builder.Services.AddScoped<ManejadorObtenerPropiedades>();
builder.Services.AddScoped<ManejadorSimuladorAlquiler>();
builder.Services.AddScoped<ManejadorIndiceSaturacion>();
builder.Services.AddScoped<IGeolocalizacionService, GeolocalizacionService>();
builder.Services.AddScoped<IConectividadService, ConectividadService>();
builder.Services.AddScoped<ManejadorConectividad>();
builder.Services.AddScoped<IOverpassService, OverpassService>();
var app = builder.Build();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CelScrapper v1");
    c.RoutePrefix = "swagger"; // deja la UI en /swagger
});

app.UseCors();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok("CelScrapper API up"));
app.MapGet("/health", () => Results.Ok("ok"));
app.Run();

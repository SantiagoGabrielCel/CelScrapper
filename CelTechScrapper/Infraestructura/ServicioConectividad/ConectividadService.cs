using Aplicacion.Servicios;
using CelTechScrapper.Dominio.Modelos;

namespace Infraestructura.ServiciosConectividad;

public class ConectividadService : IConectividadService
{

    private readonly IOverpassService _overpass;

    public ConectividadService(IOverpassService overpass)
    {
        _overpass = overpass;
    }
    public async Task<double> CalcularScoreAsync(Coordenada coordenada)
    {
        
        var categoriasGenerales = new List<string>
        {
            "shop=supermarket",
            "amenity=pharmacy",
            "amenity=school",
            "amenity=bank",
            "amenity=hospital",
            "leisure=park",
            "public_transport=platform",
            "amenity=bus_station",
            "railway=station",
            "highway=bus_stop"
        };

        
        var categoriasTransporte = new List<string>
        {
            "public_transport=platform",
            "amenity=bus_station",
            "railway=station",
            "highway=bus_stop"
        };

        
        var categoriasSubte = new List<string>
        {
            "railway=subway_entrance",
            "station=subway"
        };

        
        int totalPOIs = await _overpass.ObtenerCantidadPOIsAsync(coordenada, 500, categoriasGenerales);
        int transportePOIs = await _overpass.ObtenerCantidadPOIsAsync(coordenada, 500, categoriasTransporte);
        int subtePOIs = await _overpass.ObtenerCantidadPOIsAsync(coordenada, 700, categoriasSubte); 

        
        double ponderado = (subtePOIs * 5) + (transportePOIs * 2) + (totalPOIs - transportePOIs - subtePOIs);
        double score = Math.Min(10, Math.Round(ponderado / 5.0, 1)); 

        Console.WriteLine($"[Conectividad] Total: {totalPOIs}, Transporte: {transportePOIs}, Subte: {subtePOIs}, Score: {score}");

        return score;
    }

    private double Distancia(Coordenada a, Coordenada b)
    {
        double R = 6371; // radio de la Tierra en km
        double dLat = GradosARadianes(b.Latitud - a.Latitud);
        double dLon = GradosARadianes(b.Longitud - a.Longitud);
        double lat1 = GradosARadianes(a.Latitud);
        double lat2 = GradosARadianes(b.Latitud);

        double haversine = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                           Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        double c = 2 * Math.Atan2(Math.Sqrt(haversine), Math.Sqrt(1 - haversine));

        return R * c;
    }

    private double GradosARadianes(double grados) => grados * (Math.PI / 180);
}

using Aplicacion.Servicios;
using CelTechScrapper.Aplicacion.Servicios.Geolocalizador;
using CelTechScrapper.Dominio.Modelos;
using Flurl;
using Flurl.Http;
using System.Globalization;
namespace CelTechScrapper.Infraestructura.ServiciosGeolocalizacion
{
    public class GeolocalizacionService : IGeolocalizacionService
    {
        public async Task<Coordenada?> ObtenerCoordenadasAsync(string direccion)
        {
            try
            {
                var resultados = await "https://nominatim.openstreetmap.org/search"
                    .SetQueryParams(new
                    {
                        q = direccion,
                        format = "json",
                        limit = 1
                    })
                    .WithHeader("User-Agent", "CelTechScraper") // Nominatim lo requiere
                    .GetJsonAsync<List<NominatimResponse>>();

                var resultado = resultados.FirstOrDefault();

                if (resultado == null) return null;
                double latitud = double.Parse(resultado.lat, CultureInfo.InvariantCulture);
                double longitud = double.Parse(resultado.lon, CultureInfo.InvariantCulture);

                Console.WriteLine($"[{direccion}] => Lat: {latitud}, Lon: {longitud}");

                return new Coordenada
                {
                    Latitud = latitud,
                    Longitud = longitud

                };;

            }
            catch
            {
                return null;
            }
        }

        private class NominatimResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }
    }
}

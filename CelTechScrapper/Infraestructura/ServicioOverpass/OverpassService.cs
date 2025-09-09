using Aplicacion.Servicios;
using CelTechScrapper.Dominio.Modelos;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Infraestructura.ServiciosOverpass;

public class OverpassService : IOverpassService
{
    private readonly HttpClient _http;

    public OverpassService()
    {
        _http = new HttpClient();
        _http.BaseAddress = new Uri("https://overpass-api.de/api/");
    }

    public async Task<int> ObtenerCantidadPOIsAsync(Coordenada coordenada, double radioMetros, List<string> categorias)
    {
        string query = ConstruirQuery(coordenada, radioMetros, categorias);

        var content = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("data", query)
    });

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "interpreter")
        {
            Content = content
        };

        HttpResponseMessage response = await _http.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[ERROR Overpass] Código: {response.StatusCode}");
            return 0;
        }

        string json = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(json);
        JsonElement root = doc.RootElement;

        return root.TryGetProperty("elements", out var elements)
            ? elements.GetArrayLength()
            : 0;
    }

    private string ConstruirQuery(Coordenada coordenada, double radio, List<string> categorias)
    {
        var sb = new StringBuilder();
        sb.AppendLine("[out:json][timeout:25];");
        sb.AppendLine("(");

        foreach (var cat in categorias)
        {
            var partes = cat.Split('=');
            if (partes.Length == 2)
            {
                string key = partes[0];
                string value = partes[1];
                sb.AppendLine(
                    $"  node[\"{key}\"=\"{value}\"](around:{radio.ToString(CultureInfo.InvariantCulture)}," +
                    $"{coordenada.Latitud.ToString(CultureInfo.InvariantCulture)}," +
                    $"{coordenada.Longitud.ToString(CultureInfo.InvariantCulture)});"
                );
            }
        }

        sb.AppendLine(");");
        sb.AppendLine("out body;");

        return sb.ToString();
    }
}
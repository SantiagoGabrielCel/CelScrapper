using HtmlAgilityPack;
using CelTechScrapper.Dominio.Entidades;
using Aplicacion.Servicios;
using System.Text.RegularExpressions;
using CelTechScrapper.Aplicacion.Servicios.Geolocalizador;


namespace CelTechScrapper.Infraestructura.ServiciosScraper
{
    public class ArgenpropScraperService : IPropiedadScraperService
    {
        private readonly HttpClient _cliente;
        private readonly IGeolocalizacionService _geo;
        private readonly IConectividadService _conectividad;
        public ArgenpropScraperService(IGeolocalizacionService geo, IConectividadService conectividad)
        {
            _cliente = new HttpClient();
            _cliente.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            _geo = geo;
            _conectividad = conectividad;
        }

        public async Task<List<Propiedad>> ObtenerPropiedadesAsync(string tipo, string operacion, string zona, int maxPaginas = 3, bool incluirScore = false)
        {
            List<Propiedad> propiedades = new List<Propiedad>();

            for (int pagina = 1; pagina <= maxPaginas; pagina++)
            {
                string url = $"https://www.argenprop.com/{tipo}/{operacion}/{zona}?pagina={pagina}";
                string html = await _cliente.GetStringAsync(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                HtmlNodeCollection? nodos = doc.DocumentNode.SelectNodes("//div[contains(@class,'listing__item')] | //div[contains(@class,'card__details-box')]");
                if (nodos == null || nodos.Count == 0) break;

                foreach (HtmlNode nodo in nodos)
                {
                    string titulo = nodo.SelectSingleNode(".//h2[contains(@class,'card__title')]")?.InnerText.Trim() ?? string.Empty;
                    string precio = nodo.SelectSingleNode(".//p[contains(@class,'card__price')]")?.InnerText.Trim() ?? string.Empty;
                    string direccion = nodo.SelectSingleNode(".//p[contains(@class,'card__address')]")?.InnerText.Trim() ?? string.Empty;
                    string descripcion = nodo.SelectSingleNode(".//p[contains(@class,'card__info')]")?.InnerText.Trim() ?? string.Empty;
                    HtmlNode? nodoLink = nodo.SelectSingleNode("ancestor::a[@href]") ?? nodo.SelectSingleNode(".//ancestor::div[contains(@class, 'listing__item')]//a[@href]");

                    string urlPropiedad = string.Empty;

                    if (nodoLink != null)
                    {
                        string href = nodoLink.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrWhiteSpace(href) && href.StartsWith("/"))
                        {
                            urlPropiedad = "https://www.argenprop.com" + href;
                        }
                    }

                    Propiedad propiedad = new Propiedad
                    {
                        Titulo = titulo,
                        Precio = precio,
                        Direccion = direccion,
                        Descripcion = descripcion,
                        Url = urlPropiedad
                    };

                    propiedad = LimpiarPropiedad(propiedad);


                    if (incluirScore)
                    {
                        var coordenadas = await _geo.ObtenerCoordenadasAsync(propiedad.Direccion);
                        if (coordenadas != null)
                        {
                            double score = await _conectividad.CalcularScoreAsync(coordenadas);
                            propiedad.ScoreConectividad = score;
                        }
                    }
                    propiedades.Add(propiedad);
                }

                await Task.Delay(1000);
            }

            return propiedades;
        }

        private string ExtraerUrlArgenpropDesdeTexto(string texto)
        {
            Match match = Regex.Match(texto, @"https:\/\/www\.argenprop\.com\/[^\s]+");
            return match.Success ? match.Value : string.Empty;
        }
        private Propiedad LimpiarPropiedad(Propiedad original)
        {
            string LimpiarTexto(string texto)
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return string.Empty;

                string limpio = System.Net.WebUtility.HtmlDecode(texto);

                limpio = limpio
                    .Replace("&plus;", "+")
                    .Replace("\n", " ")
                    .Replace("\r", " ")
                    .Replace("\t", " ");

                while (limpio.Contains("  "))
                    limpio = limpio.Replace("  ", " ");

                return limpio.Trim();
            }

            decimal? ExtraerPrecioDecimal(string texto)
            {
                if (string.IsNullOrWhiteSpace(texto)) return null;

                
                Match match = Regex.Match(texto, @"\$ *([\d\.]+)");
                if (match.Success && decimal.TryParse(match.Groups[1].Value.Replace(".", ""), out decimal monto))
                    return monto;

                return null;
            }

            return new Propiedad
            {
                Titulo = LimpiarTexto(original.Titulo),
                Precio = LimpiarTexto(original.Precio),
                Direccion = LimpiarTexto(original.Direccion),
                Descripcion = LimpiarTexto(original.Descripcion),
                Url = LimpiarTexto(original.Url),
                PrecioDecimal = ExtraerPrecioDecimal(original.Precio)
            };
        }

    }
}

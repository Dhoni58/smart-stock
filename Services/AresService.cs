using System.Text.Json;

namespace WarehouseSystem.Services;

public class AresResult
{
    public string Ico { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class AresService
{
    private readonly HttpClient _httpClient;

    public AresService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AresResult?> GetByIcoAsync(string ico)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://ares.gov.cz/ekonomicke-subjekty-v-be/rest/ekonomicke-subjekty/{ico}");

            if (!response.IsSuccessStatusCode)
                return null;
            
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var name = root.GetProperty("obchodniJmeno").GetString() ?? string.Empty;

            // Sestavení adresy 
            var sidlo = root.GetProperty("sidlo");
            var street = sidlo.TryGetProperty("nazevUlice", out var s) ? s.GetString() : null;
            var houseNum = sidlo.TryGetProperty("cisloDomovni", out var h) ? h.GetInt32().ToString() : null;
            var city = sidlo.TryGetProperty("nazevObce", out var c) ? c.GetString() : null;
            var zip = sidlo.TryGetProperty("psc", out var z) ? z.GetInt32().ToString() : null;

            var address = $"{street} {houseNum}, {zip} {city}".Trim();

            return new AresResult
            {
                Ico = ico,
                Name = name,
                Address = address
            };
        }
        catch
        {
            return null;
        }
    }
}
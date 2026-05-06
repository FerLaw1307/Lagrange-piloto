using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class EspecieService
{
    private readonly IJSRuntime _jsRuntime;
    private const string EspeciesKey = "dashboard_especies";

    public EspecieService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Especie>> GetAllEspeciesAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", EspeciesKey);
        if (string.IsNullOrEmpty(json))
        {
            // Initialize with sample data
            var sampleData = new List<Especie>
            {
                new Especie { IdEspecie = "ESP-001", UuidSistema = "220e8400-e29b-41d4-a716-446655440001", NombreEspecie = "Bovino", NombreCientifico = "Bos taurus / Bos indicus" },
                new Especie { IdEspecie = "ESP-002", UuidSistema = "220e8400-e29b-41d4-a716-446655440002", NombreEspecie = "Porcino", NombreCientifico = "Sus scrofa domesticus" },
                new Especie { IdEspecie = "ESP-003", UuidSistema = "220e8400-e29b-41d4-a716-446655440003", NombreEspecie = "Ovino", NombreCientifico = "Ovis aries" },
                new Especie { IdEspecie = "ESP-004", UuidSistema = "220e8400-e29b-41d4-a716-446655440004", NombreEspecie = "Caprino", NombreCientifico = "Capra aegagrus hircus" },
                new Especie { IdEspecie = "ESP-005", UuidSistema = "220e8400-e29b-41d4-a716-446655440005", NombreEspecie = "Equino", NombreCientifico = "Equus caballus" }
            };
            await SaveEspeciesAsync(sampleData);
            return sampleData;
        }
        return System.Text.Json.JsonSerializer.Deserialize<List<Especie>>(json) ?? new List<Especie>();
    }

    public async Task<List<Especie>> GetFilteredEspeciesAsync(string searchTerm)
    {
        var all = await GetAllEspeciesAsync();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return all;

        return all.Where(e =>
            e.IdEspecie.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            e.NombreEspecie.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            e.NombreCientifico.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    private async Task SaveEspeciesAsync(List<Especie> especies)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(especies);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", EspeciesKey, json);
    }
}
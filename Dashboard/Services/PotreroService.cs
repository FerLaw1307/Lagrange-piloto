using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class PotreroService
{
    private readonly IJSRuntime _jsRuntime;
    private const string PotrerosKey = "dashboard_potreros";

    public PotreroService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Potrero>> GetAllPotrerosAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", PotrerosKey);
        if (string.IsNullOrEmpty(json))
        {
            var sampleData = new List<Potrero>
            {
                new Potrero { IdPotrero = "POT-001", UuidSistema = "220e8400-e29b-41d4-a716-446655440301", IdRancho = "RAN-001", NombrePotrero = "Norte", Area = "12 hectáreas" },
                new Potrero { IdPotrero = "POT-002", UuidSistema = "220e8400-e29b-41d4-a716-446655440302", IdRancho = "RAN-001", NombrePotrero = "Sur", Area = "18 hectáreas" },
                new Potrero { IdPotrero = "POT-003", UuidSistema = "220e8400-e29b-41d4-a716-446655440303", IdRancho = "RAN-003", NombrePotrero = "Este", Area = "10 hectáreas" }
            };
            await SavePotrerosAsync(sampleData);
            return sampleData;
        }

        return System.Text.Json.JsonSerializer.Deserialize<List<Potrero>>(json) ?? new List<Potrero>();
    }

    public async Task<Potrero?> GetPotreroByIdAsync(string id)
    {
        return (await GetAllPotrerosAsync()).FirstOrDefault(p => p.IdPotrero == id);
    }

    public async Task<List<Potrero>> GetPotrerosByRanchoIdAsync(string ranchoId)
    {
        return (await GetAllPotrerosAsync())
            .Where(p => p.IdRancho == ranchoId)
            .ToList();
    }

    public async Task AddPotreroAsync(Potrero potrero)
    {
        var potreros = await GetAllPotrerosAsync();
        if (string.IsNullOrWhiteSpace(potrero.UuidSistema)) potrero.UuidSistema = Guid.NewGuid().ToString();
        potreros.Add(potrero);
        await SavePotrerosAsync(potreros);
    }

    private async Task SavePotrerosAsync(List<Potrero> potreros)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(potreros);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", PotrerosKey, json);
    }
}

using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class CorralService
{
    private readonly IJSRuntime _jsRuntime;
    private const string CorralesKey = "dashboard_corrales";

    public CorralService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Corral>> GetAllCorralesAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", CorralesKey);
        if (string.IsNullOrEmpty(json))
        {
            var sampleData = new List<Corral>
            {
                new Corral { IdCorral = "COR-001", UuidSistema = "220e8400-e29b-41d4-a716-446655440401", IdRancho = "RAN-001", NombreCorral = "Corral 1", Capacidad = "25 animales" },
                new Corral { IdCorral = "COR-002", UuidSistema = "220e8400-e29b-41d4-a716-446655440402", IdRancho = "RAN-001", NombreCorral = "Corral 2", Capacidad = "18 animales" },
                new Corral { IdCorral = "COR-003", UuidSistema = "220e8400-e29b-41d4-a716-446655440403", IdRancho = "RAN-003", NombreCorral = "Corral Principal", Capacidad = "30 animales" }
            };
            await SaveCorralesAsync(sampleData);
            return sampleData;
        }

        return System.Text.Json.JsonSerializer.Deserialize<List<Corral>>(json) ?? new List<Corral>();
    }

    public async Task<Corral?> GetCorralByIdAsync(string id)
    {
        return (await GetAllCorralesAsync()).FirstOrDefault(c => c.IdCorral == id);
    }

    public async Task<List<Corral>> GetCorralesByRanchoIdAsync(string ranchoId)
    {
        return (await GetAllCorralesAsync())
            .Where(c => c.IdRancho == ranchoId)
            .ToList();
    }

    public async Task AddCorralAsync(Corral corral)
    {
        var corrales = await GetAllCorralesAsync();
        if (string.IsNullOrWhiteSpace(corral.UuidSistema)) corral.UuidSistema = Guid.NewGuid().ToString();
        corrales.Add(corral);
        await SaveCorralesAsync(corrales);
    }

    private async Task SaveCorralesAsync(List<Corral> corrales)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(corrales);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", CorralesKey, json);
    }
}

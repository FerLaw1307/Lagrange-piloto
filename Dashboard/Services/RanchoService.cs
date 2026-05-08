using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class RanchoService
{
    private readonly IJSRuntime _jsRuntime;
    private const string RanchosKey = "dashboard_ranchos";

    public RanchoService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Rancho>> GetAllRanchosAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", RanchosKey);
        if (string.IsNullOrEmpty(json))
        {
            var sampleData = new List<Rancho>
            {
                new Rancho { IdRancho = "RAN-001", UuidSistema = "220e8400-e29b-41d4-a716-446655440201", IdEmpresa = "EMP-001", NombreRancho = "Rancho El Porvenir", Region = "Valle Central" },
                new Rancho { IdRancho = "RAN-002", UuidSistema = "220e8400-e29b-41d4-a716-446655440202", IdEmpresa = "EMP-001", NombreRancho = "Rancho La Esperanza", Region = "Sierra Norte" },
                new Rancho { IdRancho = "RAN-003", UuidSistema = "220e8400-e29b-41d4-a716-446655440203", IdEmpresa = "EMP-002", NombreRancho = "Rancho Vista Alegre", Region = "Llanura Sur" }
            };
            await SaveRanchosAsync(sampleData);
            return sampleData;
        }

        return System.Text.Json.JsonSerializer.Deserialize<List<Rancho>>(json) ?? new List<Rancho>();
    }

    public async Task<Rancho?> GetRanchoByIdAsync(string id)
    {
        return (await GetAllRanchosAsync()).FirstOrDefault(r => r.IdRancho == id);
    }

    public async Task<List<Rancho>> GetRanchosByEmpresaIdAsync(string empresaId)
    {
        return (await GetAllRanchosAsync())
            .Where(r => r.IdEmpresa == empresaId)
            .ToList();
    }

    public async Task AddRanchoAsync(Rancho rancho)
    {
        var ranchos = await GetAllRanchosAsync();
        if (string.IsNullOrWhiteSpace(rancho.UuidSistema)) rancho.UuidSistema = Guid.NewGuid().ToString();
        ranchos.Add(rancho);
        await SaveRanchosAsync(ranchos);
    }

    private async Task SaveRanchosAsync(List<Rancho> ranchos)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(ranchos);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", RanchosKey, json);
    }
}

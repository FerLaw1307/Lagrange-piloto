using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class EmpresaService
{
    private readonly IJSRuntime _jsRuntime;
    private const string EmpresasKey = "dashboard_empresas";

    public EmpresaService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<EmpresaGanadera>> GetAllEmpresasAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", EmpresasKey);
        if (string.IsNullOrEmpty(json))
        {
            var sampleData = new List<EmpresaGanadera>
            {
                new EmpresaGanadera { IdEmpresa = "EMP-001", UuidSistema = "220e8400-e29b-41d4-a716-446655440101", NombreEmpresa = "La Granja Azul", Ubicacion = "Valle Central" },
                new EmpresaGanadera { IdEmpresa = "EMP-002", UuidSistema = "220e8400-e29b-41d4-a716-446655440102", NombreEmpresa = "Rancho del Sol", Ubicacion = "Sierra Norte" }
            };
            await SaveEmpresasAsync(sampleData);
            return sampleData;
        }

        return System.Text.Json.JsonSerializer.Deserialize<List<EmpresaGanadera>>(json) ?? new List<EmpresaGanadera>();
    }

    public async Task<EmpresaGanadera?> GetEmpresaByIdAsync(string id)
    {
        return (await GetAllEmpresasAsync()).FirstOrDefault(e => e.IdEmpresa == id);
    }

    public async Task AddEmpresaAsync(EmpresaGanadera empresa)
    {
        var empresas = await GetAllEmpresasAsync();
        if (string.IsNullOrWhiteSpace(empresa.UuidSistema)) empresa.UuidSistema = Guid.NewGuid().ToString();
        empresas.Add(empresa);
        await SaveEmpresasAsync(empresas);
    }

    public async Task<List<EmpresaGanadera>> SearchEmpresasAsync(string searchTerm)
    {
        var all = await GetAllEmpresasAsync();
        if (string.IsNullOrWhiteSpace(searchTerm)) return all;

        return all.Where(e =>
            e.IdEmpresa.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            e.NombreEmpresa.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
            e.Ubicacion.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    private async Task SaveEmpresasAsync(List<EmpresaGanadera> empresas)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(empresas);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", EmpresasKey, json);
    }
}

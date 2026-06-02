using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class AnimalService
{
    private readonly IJSRuntime _jsRuntime;
    private const string AnimalsKey = "dashboard_animales";

    public AnimalService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Animal>> GetAllAnimalsAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", AnimalsKey);
        if (string.IsNullOrEmpty(json))
        {
            var initial = new List<Animal>();
            await SaveAnimalsAsync(initial);
            return initial;
        }
        return System.Text.Json.JsonSerializer.Deserialize<List<Animal>>(json) ?? new List<Animal>();
    }

    public async Task<Animal?> GetAnimalByAreteAsync(string arete)
    {
        var all = await GetAllAnimalsAsync();
        return all.FirstOrDefault(a => string.Equals(a.Arete?.Trim(), arete?.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<bool> IsAreteUniqueAsync(string arete)
    {
        var all = await GetAllAnimalsAsync();
        return !all.Any(a => string.Equals(a.Arete?.Trim(), arete?.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAnimalAsync(Animal animal)
    {
        var all = await GetAllAnimalsAsync();
        if (!await IsAreteUniqueAsync(animal.Arete))
            throw new InvalidOperationException("El arete ya existe en el inventario.");

        all.Add(animal);
        await SaveAnimalsAsync(all);
    }

    private async Task SaveAnimalsAsync(List<Animal> animals)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(animals);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", AnimalsKey, json);
    }
}

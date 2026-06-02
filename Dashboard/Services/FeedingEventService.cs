using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class FeedingEventService
{
    private readonly IJSRuntime _jsRuntime;
    private const string EventsKey = "dashboard_feeding_events";

    public FeedingEventService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<FeedingEvent>> GetAllEventsAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", EventsKey);
        if (string.IsNullOrEmpty(json))
        {
            var sample = new List<FeedingEvent>();
            await SaveEventsAsync(sample);
            return sample;
        }
        return System.Text.Json.JsonSerializer.Deserialize<List<FeedingEvent>>(json) ?? new List<FeedingEvent>();
    }

    public async Task<List<FeedingEvent>> GetEventsByAreteAsync(string arete)
    {
        return (await GetAllEventsAsync())
            .Where(e => string.Equals(e.Arete?.Trim(), arete?.Trim(), StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(e => e.Fecha)
            .ToList();
    }

    public async Task AddEventAsync(FeedingEvent evt)
    {
        var all = await GetAllEventsAsync();
        if (string.IsNullOrWhiteSpace(evt.IdEvent)) evt.IdEvent = Guid.NewGuid().ToString();
        all.Add(evt);
        await SaveEventsAsync(all);
    }

    private async Task SaveEventsAsync(List<FeedingEvent> evts)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(evts);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", EventsKey, json);
    }
}

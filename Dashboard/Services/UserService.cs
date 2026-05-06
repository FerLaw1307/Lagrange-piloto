using Microsoft.JSInterop;
using Dashboard.Models;

namespace Dashboard.Services;

public class UserService
{
    private readonly IJSRuntime _jsRuntime;
    private const string UsersKey = "dashboard_users";

    public UserService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UsersKey);
        if (string.IsNullOrEmpty(json))
            return new List<User>();

        return System.Text.Json.JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var users = await GetAllUsersAsync();
        return users.FirstOrDefault(u => u.Username == username);
    }

    public async Task AddUserAsync(User user)
    {
        var users = await GetAllUsersAsync();
        user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        var json = System.Text.Json.JsonSerializer.Serialize(users);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UsersKey, json);
    }

    public async Task UpdateUserAsync(User user)
    {
        var users = await GetAllUsersAsync();
        var existing = users.FirstOrDefault(u => u.Id == user.Id);
        if (existing != null)
        {
            users.Remove(existing);
            users.Add(user);
            var json = System.Text.Json.JsonSerializer.Serialize(users);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UsersKey, json);
        }
    }

    public async Task DeleteUserAsync(int id)
    {
        var users = await GetAllUsersAsync();
        users.RemoveAll(u => u.Id == id);
        var json = System.Text.Json.JsonSerializer.Serialize(users);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UsersKey, json);
    }

    public async Task<bool> ValidateLoginAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        if (user == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}
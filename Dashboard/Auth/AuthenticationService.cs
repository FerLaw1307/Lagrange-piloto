using Microsoft.JSInterop;
using Dashboard.Services;

namespace Dashboard.Auth;

public class AuthenticationService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly UserService _userService;

    public event Action? OnAuthStateChanged;

    public AuthenticationService(IJSRuntime jsRuntime, UserService userService)
    {
        _jsRuntime = jsRuntime;
        _userService = userService;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var isValid = await _userService.ValidateLoginAsync(username, password);
        if (isValid)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "isAuthenticated", "true");
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", username);
            OnAuthStateChanged?.Invoke();
        }
        return isValid;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "isAuthenticated");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
        OnAuthStateChanged?.Invoke();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var auth = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "isAuthenticated");
        return auth == "true";
    }

    public async Task<string?> GetCurrentUserAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");
    }
}
using Blazored.LocalStorage;
using MarketplaceAPI.Models;
using MarketplaceFrontend.Authentication;
using System.Net.Http.Headers;
using System.Net.Http.Json;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly CustomAuthStateProvider _customAuthStateProvider;

    public AuthService(HttpClient http, ILocalStorageService localStorage, CustomAuthStateProvider customAuthStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _customAuthStateProvider = customAuthStateProvider;
    }

    public async Task<UserResponseDto> Register(UserRegisterDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/users/register", dto);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<UserResponseDto>();

        if (result?.Token is null)
            throw new Exception("No token returned");

        await _localStorage.SetItemAsync("authToken", result.Token);

        return result;
    }


    public async Task<string> Login(UserLoginDto dto)
    {
        var response = await _http.PostAsJsonAsync("api/users/login", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Login failed");

        var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

        if (result?.Token == null)
            throw new Exception("No token returned");

        await _localStorage.SetItemAsync("authToken", result.Token);

        // Notify Blazor that user is logged in
        _customAuthStateProvider.NotifyUserAuthentication(result.Token);

        return result.Token;
    }


    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        _customAuthStateProvider.NotifyUserLogout();
    }


    public async Task<string?> GetToken() => await _localStorage.GetItemAsync<string>("authToken");
}

using Blazored.LocalStorage;
using MarketplaceFrontend.Authentication;
using MarketplaceFrontend;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//  Configure HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7110/") // <-- your API base URL
});

//  Local Storage for persisting JWT
builder.Services.AddBlazoredLocalStorage();

//  Blazor Authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

//  Your existing AuthService (uses JWT + API)
builder.Services.AddScoped<AuthService>(sp =>
{
    var http = sp.GetRequiredService<HttpClient>();
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    var authStateProvider = sp.GetRequiredService<CustomAuthStateProvider>();
    return new AuthService(http, localStorage, authStateProvider);
});


await builder.Build().RunAsync();

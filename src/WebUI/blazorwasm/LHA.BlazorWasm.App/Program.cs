using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LHA.BlazorWasm.App;
using LHA.BlazorWasm.Services.Storage;
using LHA.BlazorWasm.Services.Localization;
using LHA.BlazorWasm.Services.Theme;
using LHA.BlazorWasm.Services.Toast;
using LHA.BlazorWasm.Services.ErrorHandling;
using LHA.BlazorWasm.Components;
using LHA.BlazorWasm.Services.StatusBadge;
using LHA.BlazorWasm.HttpApi.Client.Extensions;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.App.Services;
using LHA.BlazorWasm.Services.Auth;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add custom infrastructure services
builder.Services.AddAppLocalStorage(options =>
{
    options.KeyPrefix = "app:";
});

builder.Services.AddAppLocalization(options =>
{
    options.DefaultCulture = CLanguageCode.EN;
    options.SupportedCultures = new List<CLanguageCode> { CLanguageCode.EN, CLanguageCode.VI, CLanguageCode.FR, CLanguageCode.JA, CLanguageCode.ES };
    options.ResourcePaths.Add("_content/LHA.BlazorWasm.Shared/localization/{0}.json");
});

builder.Services.AddThemeService();
builder.Services.AddToastService();
builder.Services.AddErrorReporting();
builder.Services.AddStatusBadgeServices();
builder.Services.AddBlazorWasmComponents();
builder.Services.AddAppAuthentication();

builder.Services.AddLhaHttpApiClient(options =>
{
    // Point to the API Gateway
    options.BaseAddress = "https://localhost:7100/";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.MaxRetries = 3;
});

// Mock service for testing authentication in API Client
builder.Services.AddSingleton<IAccessTokenProvider, MockAccessTokenProvider>();

// Integrate IToastService into API error handling
builder.Services.AddTransient<IApiErrorHandler, ToastApiErrorHandler>();

var host = builder.Build();

// Register module-specific enum mappings
host.Services.RegisterOrderModuleMappings();
host.Services.RegisterPaymentModuleMappings();

var themeService = host.Services.GetRequiredService<IThemeService>();
await themeService.InitializeAsync();

await host.RunAsync();

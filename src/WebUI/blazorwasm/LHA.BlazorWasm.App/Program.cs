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
using LHA.BlazorWasm.HttpApi.Client.Options;
using LHA.BlazorWasm.HttpApi.Client.Abstractions;
using LHA.BlazorWasm.UI;
using LHA.BlazorWasm.Modules.Core;
using LHA.BlazorWasm.Modules.UserManagement;
using LHA.BlazorWasm.App.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// ── Infrastructure Services ───────────────────────────────────────
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

builder.Services.AddLhaHttpApiClient(options =>
{
    options.BaseAddress = "http://localhost:5088/";
    options.Timeout = TimeSpan.FromSeconds(30);
    options.MaxRetries = 3;
});

builder.Services.AddSingleton<IAccessTokenProvider, MockAccessTokenProvider>();
builder.Services.AddTransient<IApiErrorHandler, ToastApiErrorHandler>();

// ── UI Framework ──────────────────────────────────────────────────
builder.Services.AddLhaUiFramework();

// ── UI Modules ────────────────────────────────────────────────────
builder.Services.AddUiModule<CoreUiModule>();
builder.Services.AddUiModule<UserManagementUiModule>();

// ── Build & Initialize ────────────────────────────────────────────
var host = builder.Build();

// Register module-specific enum mappings
host.Services.RegisterOrderModuleMappings();
host.Services.RegisterPaymentModuleMappings();

// Initialize UI modules (navigation, permissions, widgets)
host.Services.InitializeUiModules();

// Initialize theme
var themeService = host.Services.GetRequiredService<IThemeService>();
await themeService.InitializeAsync();

await host.RunAsync();

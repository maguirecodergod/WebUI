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
    options.DefaultCulture = LanguageCode.EN;
    options.SupportedCultures = new List<LanguageCode> { LanguageCode.EN, LanguageCode.VI, LanguageCode.FR, LanguageCode.JA, LanguageCode.ES };
});

builder.Services.AddThemeService();
builder.Services.AddToastService();
builder.Services.AddErrorReporting();
builder.Services.AddStatusBadgeServices();
builder.Services.AddBlazorWasmComponents();

var host = builder.Build();

// Register module-specific enum mappings
host.Services.RegisterOrderModuleMappings();
host.Services.RegisterPaymentModuleMappings();

var themeService = host.Services.GetRequiredService<IThemeService>();
await themeService.InitializeAsync();

await host.RunAsync();

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LHA.BlazorWasm.App;
using LHA.BlazorWasm.Services.Storage;
using LHA.BlazorWasm.Services.Localization;
using LHA.BlazorWasm.Services.Theme;
using LHA.BlazorWasm.Components;

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
    options.DefaultCulture = "en";
});

builder.Services.AddThemeService();
builder.Services.AddBlazorWasmComponents();

var host = builder.Build();

var themeService = host.Services.GetRequiredService<IThemeService>();
await themeService.InitializeAsync();

await host.RunAsync();

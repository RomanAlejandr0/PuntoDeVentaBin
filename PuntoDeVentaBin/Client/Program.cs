using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PuntoDeVentaBin.Client;
using PuntoVentaBin.Client.Helpers;
using PuntoVentaBin.Client.Repositorios;
using PuntoVentaBin.Client.Seguridad;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("PuntoDeVentaBin.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PuntoDeVentaBin.ServerAPI"));

builder.Services.AddScoped<IManager, Manager>();
builder.Services.AddScoped<IMostrarMensajes, MostrarMensajes>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ProveedorAutenticacion>();

builder.Services.AddScoped<AuthenticationStateProvider, ProveedorAutenticacion>(
    prov => prov.GetRequiredService<ProveedorAutenticacion>());

builder.Services.AddScoped<ILoginService, ProveedorAutenticacion>(
    prov => prov.GetRequiredService<ProveedorAutenticacion>());

await builder.Build().RunAsync();

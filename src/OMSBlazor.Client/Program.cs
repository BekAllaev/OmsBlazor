using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using OMSBlazor.Client;
using OMSBlazor.Client.Pages.Dashboard.CustomerStastics;
using OMSBlazor.Client.Pages.Dashboard.EmployeeStastics;
using OMSBlazor.Client.Pages.Dashboard.OrderStastics;
using OMSBlazor.Client.Pages.Dashboard.ProductStastics;
using OMSBlazor.Client.Pages.Order.Create;
using OMSBlazor.Client.Pages.Order.Journal;
using OMSBlazor.Client.Services.HubConnectionsService;
using ReactiveUI;
using Splat;

namespace OMSBlazor.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            var backendUrl = builder.Configuration["BackendUrl"];

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(backendUrl ?? throw new ArgumentNullException(nameof(backendUrl))) });
            builder.Services.AddMudServices();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            builder.Services.AddSingleton<IHubConnectionsService, HubConnectionsService>();

            // We register view models on server too because server need them for pre-render
            // Please read this for further understanding - https://stackoverflow.com/a/78535224
            builder.Services.AddScoped<CustomerStasticsViewModel>();
            builder.Services.AddScoped<EmployeeStasticsViewModel>();
            builder.Services.AddScoped<OrderStasticsViewModel>();
            builder.Services.AddScoped<ProductStasticsViewModel>();
            builder.Services.AddScoped<JournalViewModel>();
            builder.Services.AddScoped<CreateViewModel>();

            builder.AddBlazorCookies();

            await builder.Build().RunAsync();
        }
    }
}

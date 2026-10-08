using Microsoft.AspNetCore.SignalR.Client;

namespace OMSBlazor.Client.Services.HubConnectionsService
{
    public class HubConnectionsService : IHubConnectionsService, IAsyncDisposable
    {
        public HubConnectionsService(IConfiguration configuration)
        {
            ProductHubConnection = new HubConnectionBuilder()
                .WithUrl($"{configuration["BackendUrl"]}signalr-hubs/product")
                .Build();
            DashboardHubConnection = new HubConnectionBuilder()
                .WithUrl($"{configuration["BackendUrl"]}signalr-hubs/dashboard")
                .Build();
        }

        public HubConnection ProductHubConnection { get; private set; }

        public HubConnection DashboardHubConnection { get; private set; }

        // Called by the DI container when its scope ends: on the server - when the user's circuit is closed,
        // in WebAssembly - when the app is unloaded. Stops the connections so the hub receives OnDisconnected
        public async ValueTask DisposeAsync()
        {
            await ProductHubConnection.DisposeAsync();
            await DashboardHubConnection.DisposeAsync();
        }
    }
}

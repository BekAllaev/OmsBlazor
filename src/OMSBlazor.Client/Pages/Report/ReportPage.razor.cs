using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using OMSBlazor.Client.Constants;
using OMSBlazor.Client.Services.HubConnectionsService;
using OMSBlazor.Client.Services.StatisticsReader;
using Reporting.Pages.Services;
using System.Collections;
using System.Net.Http;

namespace OMSBlazor.Client.Pages.Report
{
    public partial class ReportPage : IDisposable
    {
        private readonly NavigationManager navigationManager;
        private readonly IHubConnectionsService hubConnectionsService;
        // Both are null when the page runs in WebAssembly: they are registered only on the server
        private readonly IJsonDataSourceUpdater? jsonDataSourceUpdater;
        private readonly IStatisticsDataReader? statisticsDataReader;

        private IDisposable? _updateDashboardSubscription;

        public ReportPage(
            NavigationManager navigationManager, 
            IServiceProvider serviceProvider,
            IHubConnectionsService hubConnectionsService)
        {
            this.navigationManager = navigationManager;
            this.jsonDataSourceUpdater = serviceProvider.GetService<IJsonDataSourceUpdater>();
            this.hubConnectionsService = hubConnectionsService;
            this.statisticsDataReader = serviceProvider.GetService<IStatisticsDataReader>();
        }

        // TODO: This is definitely workaround. Problem is that this component should be moved
        // to the project OMSBlazor, where only server rendering components live 
        private bool IsServerSide => jsonDataSourceUpdater is not null && statisticsDataReader is not null;

        protected override void OnInitialized()
        {
            // The report page has to run in InteractiveServer mode (see App.razor), but the render mode is picked
            // only on a full HTTP request. When we get here via client-side navigation the page is rendered by
            // the WebAssembly router, so we force a full page load to let the server render it
            if (!IsServerSide)
            {
                navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender || !IsServerSide) return;

            if (_updateDashboardSubscription is null)
            {
                _updateDashboardSubscription = hubConnectionsService.DashboardHubConnection.On("UpdateDashboard", async () =>
                {
                    var data = await statisticsDataReader!.GetData();
                    await jsonDataSourceUpdater!.UpdateDataSourceAsync(data);
                    navigationManager.NavigateTo(navigationManager.Uri, true);
                });
            }

            if (hubConnectionsService.DashboardHubConnection.State == HubConnectionState.Disconnected)
                await hubConnectionsService.DashboardHubConnection.StartAsync();
        }

        // The hub connection outlives the page, so remove the handler when the user leaves it
        public void Dispose() => _updateDashboardSubscription?.Dispose();
    }
}

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Reporting.Pages.Services;

namespace OMSBlazor.Client.Pages.Report
{
    public partial class ReportPage
    {
        private readonly NavigationManager navigationManager;
        private readonly IJsonDataSourceUpdater jsonDataSourceUpdater;
        private readonly HubConnection hub;

        private bool _subscribed;

        public ReportPage(NavigationManager navigationManager, IJsonDataSourceUpdater jsonDataSourceUpdater, HubConnection hub)
        {
            this.navigationManager = navigationManager;
            this.jsonDataSourceUpdater = jsonDataSourceUpdater;
            this.hub = hub;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            if (!_subscribed)
            {
                //hub.On<>("ReceiveAllStats", async data =>
                //{
                //    await jsonDataSourceUpdater.UpdateDataSourceAsync(data);
                //    navigationManager.NavigateTo(navigationManager.Uri, true);
                //});
                _subscribed = true;
            }

            if (hub.State == HubConnectionState.Disconnected)
                await hub.StartAsync();

        }
    }
}

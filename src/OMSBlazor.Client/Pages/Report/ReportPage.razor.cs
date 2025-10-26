using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using OMSBlazor.Client.Constants;
using OMSBlazor.Client.Services.HubConnectionsService;
using OMSBlazor.Client.Services.StatisticsReader;
using Reporting.Pages.Services;
using System.Collections;
using System.Net.Http;

namespace OMSBlazor.Client.Pages.Report
{
    public partial class ReportPage
    {
        private readonly NavigationManager navigationManager;
        private readonly IJsonDataSourceUpdater jsonDataSourceUpdater;
        private readonly IHubConnectionsService hubConnectionsService;
        private readonly IStatisticsDataReader statisticsDataReader;

        private bool _subscribed;

        public ReportPage(
            NavigationManager navigationManager, 
            IJsonDataSourceUpdater jsonDataSourceUpdater, 
            IHubConnectionsService hubConnectionsService,
            IStatisticsDataReader statisticsDataReader)
        {
            this.navigationManager = navigationManager;
            this.jsonDataSourceUpdater = jsonDataSourceUpdater;
            this.hubConnectionsService = hubConnectionsService;
            this.statisticsDataReader = statisticsDataReader;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender) return;

            if (!_subscribed)
            {
                hubConnectionsService.DashboardHubConnection.On("UpdateDashboard", async () =>
                {
                    var data = await statisticsDataReader.GetData();
                    await jsonDataSourceUpdater.UpdateDataSourceAsync(data);
                    navigationManager.NavigateTo(navigationManager.Uri, true);
                });
                _subscribed = true;
            }

            if (hubConnectionsService.DashboardHubConnection.State == HubConnectionState.Disconnected)
                await hubConnectionsService.DashboardHubConnection.StartAsync();
        }
    }
}


using OMSBlazor.Client.Constants;
using OMSBlazor.Client.Services.StatisticsReader;
using OMSBlazor.Dto.Order.Stastics;
using Reporting.Pages.Services;
using System.Net.Http;
using System.Text.Json;

namespace OMSBlazor.Services.Hosted
{
    public class JsonReportDataSourceSeeder : IHostedService
    {
        private readonly IJsonDataSourceUpdater jsonDataSourceUpdater;
        private readonly IStatisticsDataReader statisticsDataReader;

        public JsonReportDataSourceSeeder(IJsonDataSourceUpdater jsonDataSourceUpdater, IStatisticsDataReader statisticsDataReader)
        {
            this.jsonDataSourceUpdater = jsonDataSourceUpdater;
            this.statisticsDataReader = statisticsDataReader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var data = await statisticsDataReader.GetData();

            await jsonDataSourceUpdater.UpdateDataSourceAsync(data);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}

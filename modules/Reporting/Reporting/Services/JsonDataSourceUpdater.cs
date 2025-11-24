using Microsoft.AspNetCore.Hosting;
using Reporting.Pages.Services;

namespace Reporting.Services
{
    public class JsonDataSourceUpdater : IJsonDataSourceUpdater
    {
        private IWebHostEnvironment _hostingEnvironment;

        public JsonDataSourceUpdater(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task UpdateDataSourceAsync(Dictionary<string, string> dataSourceNameAndJsonMap)
        {
            foreach (var (statisticsName, json) in dataSourceNameAndJsonMap)
            {
                await UpdateAsync(json, statisticsName);
            }
        }

        private async Task UpdateAsync(string json, string statisticsName)
        {
            var filePath = _hostingEnvironment.WebRootPath + $@"\Resources\JsonDataSources\{statisticsName}.json";

            await File.WriteAllTextAsync(filePath, json);
        }
    }
}

using Reporting.Pages.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Reporting.Pages.Services
{
    public class JsonDataSourceUpdater : IJsonDataSourceUpdater
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public JsonDataSourceUpdater(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task UpdateDataSourceAsync(Dictionary<string, string> jsonDataSources)
        {
            foreach (var jsonDataSource in jsonDataSources)
            {
                await UpdateAsync(jsonDataSource.Value, jsonDataSource.Key);
            }
        }

        private async Task UpdateAsync(string json, string statisticsName)
        {
            var filePath = _hostingEnvironment.WebRootPath + $@"\Resources\JsonDataSources\{statisticsName}.json";

            await File.WriteAllTextAsync(filePath, json);
        }
    }
}

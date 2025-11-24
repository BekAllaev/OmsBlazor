using OMSBlazor.Client.Constants;
using OMSBlazor.Dto.Order.Stastics;
using System.Net.Http;
using System.Text.Json;

namespace OMSBlazor.Client.Services.StatisticsReader
{
    public class StatisticsDataReader : IStatisticsDataReader
    {
        private readonly HttpClient httpClient;

        public StatisticsDataReader(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient(Constants.Constants.BaseHttpClientTitel);
        }

        public async Task<Dictionary<string, string>> GetData()
        {
            var salesByEmployes = await httpClient.GetStringAsync(BackEndEnpointURLs.EmployeeEndpoints.SalesByEmployees);
            var purchasesByCustomers = await httpClient.GetStringAsync(BackEndEnpointURLs.CustomersEndpoints.PurchasesByCustomers);
            var customersByCountries = await httpClient.GetStringAsync(BackEndEnpointURLs.CustomersEndpoints.CustomersByCountries);
            var productsByCategories = await httpClient.GetStringAsync(BackEndEnpointURLs.ProductEndpoints.ProductByCategories);
            var summaries = await httpClient.GetStringAsync(BackEndEnpointURLs.OrderEndpoints.Summaries);

            var data = new Dictionary<string, string>
            {
                { "SalesByEmployee", salesByEmployes },
                { "PurchasesByCustomers", purchasesByCustomers },
                { "CustomersByCountry", customersByCountries },
                { "ProductsByCategory", productsByCategories },
                { "Summaries", Transform(summaries) }
            };

            return data;
        }

        string Transform(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var items = JsonSerializer.Deserialize<List<SummaryDto>>(json, options)!;

            var dict = items.ToDictionary(i => i.SummaryName, i => i.SummaryValue);

            var result = JsonSerializer.Serialize(new[] { dict },
                new JsonSerializerOptions { WriteIndented = true });

            return result;
        }
    }
}

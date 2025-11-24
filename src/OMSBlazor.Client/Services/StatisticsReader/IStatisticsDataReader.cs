namespace OMSBlazor.Client.Services.StatisticsReader
{
    /// <summary>
    /// Returns statistics data for reports.
    /// </summary>
    public interface IStatisticsDataReader
    {
        public Task<Dictionary<string, string>> GetData();
    }
}

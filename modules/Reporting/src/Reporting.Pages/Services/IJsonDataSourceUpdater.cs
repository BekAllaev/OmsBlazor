using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Pages.Services
{
    public interface IJsonDataSourceUpdater
    {
        public Task UpdateDataSourceAsync(Dictionary<string, string> jsonDataSources);
    }
}

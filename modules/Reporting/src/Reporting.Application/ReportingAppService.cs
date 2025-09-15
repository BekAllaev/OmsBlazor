using Reporting.Localization;
using Volo.Abp.Application.Services;

namespace Reporting;

public abstract class ReportingAppService : ApplicationService
{
    protected ReportingAppService()
    {
        LocalizationResource = typeof(ReportingResource);
        ObjectMapperContext = typeof(ReportingApplicationModule);
    }
}

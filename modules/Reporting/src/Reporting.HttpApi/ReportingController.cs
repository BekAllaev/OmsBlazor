using Reporting.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Reporting;

public abstract class ReportingController : AbpControllerBase
{
    protected ReportingController()
    {
        LocalizationResource = typeof(ReportingResource);
    }
}

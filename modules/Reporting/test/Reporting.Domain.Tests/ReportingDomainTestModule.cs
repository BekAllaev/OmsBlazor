using Volo.Abp.Modularity;

namespace Reporting;

[DependsOn(
    typeof(ReportingDomainModule),
    typeof(ReportingTestBaseModule)
)]
public class ReportingDomainTestModule : AbpModule
{

}

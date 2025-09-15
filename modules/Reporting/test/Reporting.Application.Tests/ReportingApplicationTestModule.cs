using Volo.Abp.Modularity;

namespace Reporting;

[DependsOn(
    typeof(ReportingApplicationModule),
    typeof(ReportingDomainTestModule)
    )]
public class ReportingApplicationTestModule : AbpModule
{

}

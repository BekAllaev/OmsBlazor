using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Reporting;

[DependsOn(
    typeof(AbpDddDomainModule),
    typeof(ReportingDomainSharedModule)
)]
public class ReportingDomainModule : AbpModule
{

}

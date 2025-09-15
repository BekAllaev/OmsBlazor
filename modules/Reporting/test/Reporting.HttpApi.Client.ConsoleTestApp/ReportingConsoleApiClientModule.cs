using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace Reporting;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ReportingHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class ReportingConsoleApiClientModule : AbpModule
{

}

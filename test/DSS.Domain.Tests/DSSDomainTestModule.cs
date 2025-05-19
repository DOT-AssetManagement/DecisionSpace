using Volo.Abp.Modularity;

namespace DSS;

[DependsOn(
    typeof(DSSDomainModule),
    typeof(DSSTestBaseModule)
)]
public class DSSDomainTestModule : AbpModule
{

}

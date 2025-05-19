using Volo.Abp.Modularity;

namespace DSS;

[DependsOn(
    typeof(DSSApplicationModule),
    typeof(DSSDomainTestModule)
)]
public class DSSApplicationTestModule : AbpModule
{

}

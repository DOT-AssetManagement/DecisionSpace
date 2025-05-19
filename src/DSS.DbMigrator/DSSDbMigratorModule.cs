using DSS.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DSS.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DSSEntityFrameworkCoreModule),
    typeof(DSSApplicationContractsModule)
    )]
public class DSSDbMigratorModule : AbpModule
{
}

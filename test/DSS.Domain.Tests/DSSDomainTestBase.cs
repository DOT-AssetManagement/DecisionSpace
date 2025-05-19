using Volo.Abp.Modularity;

namespace DSS;

/* Inherit from this class for your domain layer tests. */
public abstract class DSSDomainTestBase<TStartupModule> : DSSTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

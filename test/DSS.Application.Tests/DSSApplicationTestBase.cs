using Volo.Abp.Modularity;

namespace DSS;

public abstract class DSSApplicationTestBase<TStartupModule> : DSSTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
	// empty constructor
}

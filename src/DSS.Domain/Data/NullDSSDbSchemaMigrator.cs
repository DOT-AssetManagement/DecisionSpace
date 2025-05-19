using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace DSS.Data;

/* This is used if database provider does't define
 * IDSSTestingDbSchemaMigrator implementation.
 */
public class NullDSSDbSchemaMigrator : IDSSDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

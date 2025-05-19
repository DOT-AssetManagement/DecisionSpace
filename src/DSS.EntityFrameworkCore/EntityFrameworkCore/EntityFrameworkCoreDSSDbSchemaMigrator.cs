using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DSS.Data;
using Volo.Abp.DependencyInjection;

namespace DSS.EntityFrameworkCore;

public class EntityFrameworkCoreDSSDbSchemaMigrator
    : IDSSDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreDSSDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the DSSTestingDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DSSDbContext>()
            .Database
            .MigrateAsync();
    }
}

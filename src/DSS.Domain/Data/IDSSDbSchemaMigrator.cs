using System.Threading.Tasks;

namespace DSS.Data;

public interface IDSSDbSchemaMigrator
{
    Task MigrateAsync();
}

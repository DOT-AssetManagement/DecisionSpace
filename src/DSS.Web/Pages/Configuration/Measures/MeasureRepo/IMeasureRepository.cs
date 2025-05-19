using DSS.Web.Pages.Configuration.Measures;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMeasureRepository<TEntity, TPrimaryKey>
    where TEntity : class
{
    Task<List<Measure>> GetAllAsync();
    Task<TEntity> GetAsync(TPrimaryKey id);
    Task Create(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TPrimaryKey id);
}

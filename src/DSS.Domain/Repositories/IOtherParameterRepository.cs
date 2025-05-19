using DSS.Entities;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DSS.Repositories
{
    public interface IOtherParameterRepository : IRepository<TblOtherParameters>
    {
        Task<TblOtherParameters> GetAsync(string code);
        Task<TblOtherParameters> UpdateAsync(TblOtherParameters entity);
    }
}

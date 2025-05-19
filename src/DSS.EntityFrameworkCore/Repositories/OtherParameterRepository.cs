using DSS.Entities;
using DSS.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class OtherParameterRepository : EfCoreRepository<DSSDbContext, TblOtherParameters>, IOtherParameterRepository
    {
        private readonly DSSDbContext _dbContext;
        public OtherParameterRepository(IDbContextProvider<DSSDbContext> dbContextProvider, DSSDbContext dbContext)
            : base(dbContextProvider)
        {
            _dbContext = dbContext;
        }
        public async Task<TblOtherParameters> GetAsync(string code)
        {
            return await DbContext.Set<TblOtherParameters>()
                                  .AsNoTracking() 
                                  .FirstOrDefaultAsync(op => op.Code == code);
        }

        public async Task<TblOtherParameters> UpdateAsync(TblOtherParameters entity)
        {
            try
            {
                _dbContext.Update(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        
    }
}

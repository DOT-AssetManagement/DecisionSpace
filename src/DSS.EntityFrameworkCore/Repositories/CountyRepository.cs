using DSS.Entities;
using DSS.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class CountyRepository : EfCoreRepository<DSSDbContext, County>, ICountyRepository
    {
        private readonly string _connectionString = string.Empty;
        private readonly DSSDbContext _dbContext;
        public CountyRepository(IDbContextProvider<DSSDbContext> dbContextProvider, DSSDbContext dbContext, IConfiguration configuration) : base(dbContextProvider)
        {
            _dbContext = dbContext;
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<List<County>> GetAllDistricts()
        {

            var districts = await _dbContext.Set<County>()
                                           .AsNoTracking()
                                           .Select(c => c.District)
                                           .Distinct()
                                           .ToListAsync();

            var districtDtos = districts.Select(d => new County { District = d }).ToList();

            return districtDtos;
        }

        public async Task<List<County>> GetCountiesByDistrict(int id)
        {
            var counties = await _dbContext.Set<County>()
                                           .AsNoTracking()
                                           .Where(c => c.District == id)
                                           .ToListAsync();

            return counties;
        }

        public async Task<List<County>> GetCounties()
        {
            var counties = await _dbContext.Set<County>()
                                           .AsNoTracking()
                                           .ToListAsync();

            return counties;
        }

    }
}

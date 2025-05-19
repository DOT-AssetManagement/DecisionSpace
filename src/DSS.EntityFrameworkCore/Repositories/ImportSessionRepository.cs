using DSS.Entities;
using DSS.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;

public class ImportSessionRepository : EfCoreRepository<DSSDbContext, ImportSession, Guid>, IImportSessionRepository
{
    private readonly string _connectionString = string.Empty;
    private readonly ICurrentUser _curruntUser;
    private readonly DSSDbContext _dbContext;
    public ImportSessionRepository(IDbContextProvider<DSSDbContext> dbContextProvider, IConfiguration configuration, ICurrentUser curruntUser, DSSDbContext dbContext)
        : base(dbContextProvider)
    {
        _connectionString = configuration.GetConnectionString("Default");
        _curruntUser = curruntUser;
        _dbContext = dbContext;
    }

    public async Task<ImportSession> GetByPoolId(Guid poolId)
    {
        var result = await _dbContext.ImportSessions.AsNoTracking()
        .FirstOrDefaultAsync(x => x.PoolId == poolId);
        return result;
    }

}

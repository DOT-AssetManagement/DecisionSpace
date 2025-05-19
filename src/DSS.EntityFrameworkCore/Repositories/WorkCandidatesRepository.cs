using DSS.Entities;
using DSS.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using DSS.Utilities;

public class WorkCandidatesRepository : EfCoreRepository<DSSDbContext, WorkCandidate, Guid>, IWorkCandidatesRepository
{
    private readonly string _connectionString = string.Empty;
    private readonly DSSDbContext _dbContext;
    public WorkCandidatesRepository(IDbContextProvider<DSSDbContext> dbContextProvider, DSSDbContext dbContext, IConfiguration configuration)
        : base(dbContextProvider)
    {
        _dbContext = dbContext;
        _connectionString = configuration.GetConnectionString("Default");
    }

    public async Task<List<WorkCandidate>> GetListByPoolOrProjectId(Guid poolId, Guid projectId)
    {
        var results = await _dbContext.WorkCandidates
            .Where(x => (x.PoolId == poolId || x.ProjectCandidateId == projectId))
            .ToListAsync();

        return results;
    }

    public async Task<WorkCandidate> GetByProjectId(Guid projectId)
    {
        var result = await _dbContext.WorkCandidates
        .FirstOrDefaultAsync(x => x.ProjectCandidateId == projectId);

        return result;
    }


    public async Task<bool> UpdatePool(Guid poolId, bool scoreParametersUpdated)
    {
        string errors;
        var response = DataManager.UpdatePool(_connectionString, poolId, scoreParametersUpdated, out errors);

        return response;
    }

    public async Task<bool> CreateWorkCandidate(WorkCandidate workCandidate)
    {
        string? errors;
        Guid? workCandidateId;
        Guid? projectCandidateId;
        var response = DataManager.CreateNewWorkCandidate(_connectionString, workCandidate, out workCandidateId, out projectCandidateId, out errors);

        return response;
    }

    public async Task<bool> UpdateWorkCandidate(Guid workCandidateId)
    {
        string errors;
        var response = DataManager.UpdateWorkCandidate(_connectionString, workCandidateId, out errors);

        return response;
    }
}

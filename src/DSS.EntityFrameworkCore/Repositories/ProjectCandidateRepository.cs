using DSS.Entities;
using DSS.EntityFrameworkCore;
using DSS.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;

public class ProjectCandidateRepository : EfCoreRepository<DSSDbContext, ProjectCandidate, Guid>, IProjectCandidateRepository
{
    private readonly DSSDbContext _dbContext;
    private readonly ICurrentUser _curruntUser;
    private readonly string _connectionString = string.Empty;
    public ProjectCandidateRepository(IDbContextProvider<DSSDbContext> dbContextProvider, IConfiguration configuration, DSSDbContext dbContext, ICurrentUser curruntUser)
        : base(dbContextProvider)
    {
        _dbContext = dbContext;
        _curruntUser = curruntUser;
        _connectionString = configuration.GetConnectionString("Default");
    }

    public async Task<List<ProjectCandidate>> GetAll()
    {

        var projects = await (from pc in _dbContext.ProjectCandidates.AsNoTracking()
                              join c in _dbContext.Counties.AsNoTracking()
                              on pc.NumberOfWorkCandidates equals c.Cnty
                              select new ProjectCandidate
                              {
                                  PoolId = pc.PoolId,
                                  District = pc.District,
                                  Cnty = pc.Cnty,
                                  CountyName = c.CountyName,
                                  Description = pc.Description,
                                  NumberOfWorkCandidates = pc.NumberOfWorkCandidates,
                                  Treatment = pc.Treatment,
                                  YearEarliest = pc.YearEarliest,
                                  YearLatest = pc.YearLatest,
                                  TotalCost = pc.TotalCost,
                                  TotalScaledBenefit = pc.TotalScaledBenefit,
                                  SafetyScore = pc.SafetyScore,
                                  MobilityAndEconomyScore = pc.MobilityAndEconomyScore,
                                  EquityAndAccessScore = pc.EquityAndAccessScore,
                                  ResilienceAndEnvironmentScore = pc.ResilienceAndEnvironmentScore,
                                  ConditionAndPerformanceScore = pc.ConditionAndPerformanceScore,
                                  TotalScore = pc.TotalScore,
                                  RelativeEfficiency = pc.RelativeEfficiency,
                                  RelativeEfficiencySetAt = pc.RelativeEfficiencySetAt,
                              }).ToListAsync();

        return projects;
        //return await _dbContext.ProjectCandidates.AsNoTracking().ToListAsync();
    }

    public async Task<List<string>> GetDistinctDescriptions()
    {
        var distinctDescriptions = await _dbContext.ProjectCandidates
                                                  .AsNoTracking()
                                                  .Select(pc => pc.Description)
                                                  .Distinct()
                                                  .ToListAsync();

        return distinctDescriptions;
    }

    //public async Task<List<ProjectCandidate>> GetProjectsByScenarioId(Guid scenarioId)
    //{
    //    var scenario = await _dbContext.Scenarios.AsNoTracking()
    //        .FirstOrDefaultAsync(x => x.Id == scenarioId);
    //    if (scenario == default)
    //    {
    //        throw new Exception("No Scenario Found against ID");
    //    }
    //    var projects = await _dbContext.ProjectCandidates.AsNoTracking()
    //        .Where(pc => pc.PoolId == scenario.PoolId)
    //        .ToListAsync();
    //    return projects;
    //}

    public async Task<List<ProjectCandidate>> GetProjectsByPoolId(Guid poolId)
    {
        /*var projects = await _dbContext.ProjectCandidates.AsNoTracking()
            .Where(pc => pc.PoolId == poolId)
            .ToListAsync();

        return projects;*/

        var projects = await (from pc in _dbContext.ProjectCandidates.AsNoTracking()
                                   join c in _dbContext.Counties.AsNoTracking()
                                   on pc.NumberOfWorkCandidates equals c.Cnty
                                   where pc.PoolId == poolId
                                   select new ProjectCandidate
                                   {
                                       Id = pc.Id,
                                       PoolId = pc.PoolId,
                                       District = pc.District,
                                       Cnty = pc.Cnty,
                                       CountyName = c.CountyName,
                                       Description = pc.Description,
                                       NumberOfWorkCandidates = pc.NumberOfWorkCandidates,
                                       Treatment = pc.Treatment,
                                       YearEarliest = pc.YearEarliest,
                                       YearLatest = pc.YearLatest,
                                       TotalCost = pc.TotalCost,
                                       TotalScaledBenefit = pc.TotalScaledBenefit,
                                       SafetyScore = pc.SafetyScore,
                                       MobilityAndEconomyScore = pc.MobilityAndEconomyScore,
                                       EquityAndAccessScore = pc.EquityAndAccessScore,
                                       ResilienceAndEnvironmentScore = pc.ResilienceAndEnvironmentScore,
                                       ConditionAndPerformanceScore = pc.ConditionAndPerformanceScore,
                                       TotalScore = pc.TotalScore,
                                       RelativeEfficiency = pc.RelativeEfficiency,
                                       RelativeEfficiencySetAt = pc.RelativeEfficiencySetAt,
                                   }).ToListAsync();

        return projects;
    }


    public async Task<string> GetGisJsonStringOutput(Guid scenarioId, double RelativeEfficiencyThreshold)
    {
        string errors;
        string json;
        var response = DataManager.GetGisJsonStringOutput(_connectionString, scenarioId, RelativeEfficiencyThreshold, true, out json, out errors);

        return json;
    }
}

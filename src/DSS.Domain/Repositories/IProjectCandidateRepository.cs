using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

public interface IProjectCandidateRepository : IBasicRepository<ProjectCandidate, Guid>
{
    Task<List<ProjectCandidate>> GetAll();
    Task<List<ProjectCandidate>> GetProjectsByPoolId(Guid poolId);
   // Task<List<ProjectCandidate>> GetProjectsByScenarioId(Guid scenarioId);
    Task<List<string>> GetDistinctDescriptions();
    Task<string> GetGisJsonStringOutput(Guid scenarioId, double RelativeEfficiencyThreshold);
}

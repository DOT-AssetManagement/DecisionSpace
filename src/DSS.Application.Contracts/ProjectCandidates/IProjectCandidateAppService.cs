using DSS.Pool.Dtos;
using DSS.ProjectCandidates.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.ProjectCandidates
{
    public interface IProjectCandidateAppService : ICrudAppService<ProjectCandidateDto, Guid, GetProjectCandidateInput, ProjectCandidateCreateDto, ProjectCandidateUpdateDto>
    {
        Task<List<ProjectCandidateDto>> GetAllAsync();
        Task<List<ProjectCandidateDto>> GetAllProjectsAsync(Guid scenarioId);
        Task<List<string>> GetDistinctDescriptions();
        Task<string> GetGisJsonStringOutput(Guid scenarioId, double RelativeEfficiencyThreshold);
    }
}

using DSS.Pool;
using DSS.Pool.Dtos;
using DSS.ProjectCandidates;
using DSS.ProjectCandidates.Dtos;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Outputs.Charts
{
    public class ProjectsAndGoalsActualContributionModel : PageModel
    {
        private readonly IProjectCandidateAppService _projectCandidateAppService;
        private readonly IScenarioAppService _scenarioAppService;
        private readonly IPoolAppService _poolAppService;

        public List<ScenarioDto> Scenarios { get; set; }
        public List<PoolDto> Pools { get; set; }
        public string PoolName { get; set; }
        public Guid PoolId { get; set; }

        public string json { get; set; }
        public List<ProjectCandidateDto> ProjectCandidates { get; set; }
        public ProjectsAndGoalsActualContributionModel(IProjectCandidateAppService projectCandidateAppService, IScenarioAppService scenarioAppService, IPoolAppService poolAppService)
        {
            _projectCandidateAppService = projectCandidateAppService;
            _scenarioAppService = scenarioAppService;
            _poolAppService = poolAppService;
        }
        public async Task OnGet(Guid? poolId)
        {
            if (poolId != null)
            {
                ProjectCandidates = await _projectCandidateAppService.GetAllProjectsAsync((Guid)poolId);

                var pool = await _poolAppService.GetAsync(poolId.Value);
                PoolId = poolId.Value;
                PoolName = pool.Name;

                json = System.Text.Json.JsonSerializer.Serialize(ProjectCandidates, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }

            Scenarios = await _scenarioAppService.GetAllAsync("");
            Pools = await _poolAppService.GetAllAsync("", "");
        }
    }
}

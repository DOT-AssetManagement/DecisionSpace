using DSS.Entities;
using DSS.ProjectCandidates;
using DSS.Scenarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using DSS.Scenarios.Dtos;
using DSS.ProjectCandidates.Dtos;
using System.Collections.Generic;
using System.Text.Json;
using DSS.Pool;
using DSS.Pool.Dtos;
using System.Linq;


namespace DSS.Web.Pages.Outputs.Charts
{
    public class ProjectEfficiencyAndCostModel : PageModel
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

        public ProjectEfficiencyAndCostModel(IProjectCandidateAppService projectCandidateAppService, IScenarioAppService scenarioAppService, IPoolAppService poolAppService)
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

                var extractedCandidates = ProjectCandidates.Select(candidate => new
                {
                    Description = candidate.Description,
                    RelativeEfficiency = candidate.RelativeEfficiency,
                    TotalCost = candidate.TotalCost
                }).ToList();



                json = System.Text.Json.JsonSerializer.Serialize(extractedCandidates, new JsonSerializerOptions
                {
                    WriteIndented = true // For pretty printing
                });
            }

            Scenarios = await _scenarioAppService.GetAllAsync("");
            Pools = await _poolAppService.GetAllAsync("", "");
        }
    }
}

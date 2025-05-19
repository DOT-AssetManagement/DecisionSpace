using DSS.Entities;
using DSS.ProjectCandidates;
using DSS.Scenarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System;
using DSS.Scenarios.Dtos;
using System.Collections.Generic;
using DSS.ProjectCandidates.Dtos;
using System.Linq;
using Microsoft.AspNetCore.Routing.Matching;
using Newtonsoft.Json;
using DSS.Pool;
using DSS.Pool.Dtos;
using System.Text.Json;

namespace DSS.Web.Pages.Outputs.Charts
{
    public class ProjectToScoreArcMapping : PageModel
    {
        private readonly IProjectCandidateAppService _projectCandidateAppService;
        private readonly IScenarioAppService _scenarioAppService;
        private readonly IPoolAppService _poolAppService;

        public List<ScenarioDto> Scenarios { get; set; }
        public List<PoolDto> Pools { get; set; }
        public string PoolName { get; set; }
        public List<byte?> Districts { get; set; }
        public List<byte?> Counties { get; set; }

        public Guid PoolId { get; set; }
        public string json { get; set; }
        public List<ProjectCandidateDto> ProjectCandidates { get; set; }

        public ProjectToScoreArcMapping(IProjectCandidateAppService projectCandidateAppService, IScenarioAppService scenarioAppService, IPoolAppService poolAppService)
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

                // Calculate and sort by total score
                var sortedProjectCandidates = ProjectCandidates
                    .Select(p => new
                    {
                        Project = p,
                        TotalScore = p.SafetyScore + p.MobilityAndEconomyScore +
                                     p.EquityAndAccessScore + p.ResilienceAndEnvironmentScore +
                                     p.ConditionAndPerformanceScore
                    })
                    .OrderBy(p => p.TotalScore)
                    .ToList();

                // Arrange in inverse bell curve order in order to make the name labels look better
                var inverseBellCurveList = new List<ProjectCandidateDto>();
                bool addToFront = true;

                foreach (var item in sortedProjectCandidates)
                {
                    if (addToFront)
                        inverseBellCurveList.Insert(0, item.Project); // Add to front
                    else
                        inverseBellCurveList.Add(item.Project); // Add to end

                    addToFront = !addToFront; // Alternate between adding to the front and to the end
                }

                // Serialize the sorted list
                json = System.Text.Json.JsonSerializer.Serialize(inverseBellCurveList, new JsonSerializerOptions
                {
                    WriteIndented = true // For pretty printing
                });
            }

            Scenarios = await _scenarioAppService.GetAllAsync("");
            Pools = await _poolAppService.GetAllAsync("", "");
        }
    }
}
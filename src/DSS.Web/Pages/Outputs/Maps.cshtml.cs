using DSS.Pool;
using DSS.Pool.Dtos;
using DSS.ProjectCandidates;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Outputs
{
    [Authorize]
    public class MapsModel : PageModel
    {
        private readonly IProjectCandidateAppService _projectCandidateAppService;
        private readonly IScenarioAppService _scenarioAppService;
        private readonly IPoolAppService _poolAppService;

        public string GisJsonString { get; set; }
        public List<ScenarioDto> Scenarios { get; set; }
        public List<PoolDto> Pools { get; set; }
        public string PoolName { get; set; }
        public string ScenarioName { get; set; }
        public Guid PoolId { get; set; }

        public ScenarioDto scenario { get; set; }

        public MapsModel(IProjectCandidateAppService projectCandidateAppService, IScenarioAppService scenarioAppService, IPoolAppService poolAppService)
        {
            _projectCandidateAppService = projectCandidateAppService;
            _scenarioAppService = scenarioAppService;
            _poolAppService = poolAppService;
        }
        public async Task OnGet(Guid? poolId)
        {
            if (poolId != null)
            {

                scenario = await _scenarioAppService.GetScenarioByPoolId((Guid)poolId);
                if (scenario != null)
                {
                    GisJsonString = await _projectCandidateAppService.GetGisJsonStringOutput((Guid)scenario.Id, 0);
                    ScenarioName = scenario.ScenName;
                }

                var pool = await _poolAppService.GetAsync(poolId.Value);
                PoolId = poolId.Value;
                PoolName = pool.Name;
            }

            Scenarios = await _scenarioAppService.GetAllAsync("");
            Pools = await _poolAppService.GetAllAsync("", "");
        }

    }
}

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

namespace DSS.Web.Pages.Outputs.Charts
{
    public class ProjectsAndGoalsFlowDiagramModel : PageModel
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

        public ProjectsAndGoalsFlowDiagramModel(IProjectCandidateAppService projectCandidateAppService, IScenarioAppService scenarioAppService, IPoolAppService poolAppService)
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
             

                var sankeyDiagram = TransformToSankeyDiagram(ProjectCandidates);
                json = JsonConvert.SerializeObject(sankeyDiagram, Formatting.Indented);
                json = json.ToLower();
            }

            Scenarios = await _scenarioAppService.GetAllAsync("");
            Pools = await _poolAppService.GetAllAsync("", "");
        }

        public static SankeyDiagram TransformToSankeyDiagram(List<ProjectCandidateDto> candidates)
        {
            var diagram = new SankeyDiagram();

            var nodeNames = new HashSet<string>();
            foreach (var candidate in candidates)
            {
                nodeNames.Add(candidate.Description);
                nodeNames.Add("SafetyScore");
                nodeNames.Add("MobilityAndEconomyScore");
                nodeNames.Add("EquityAndAccessScore");
                nodeNames.Add("ResilienceAndEnvironmentScore");
                nodeNames.Add("ConditionAndPerformanceScore");
            }
            diagram.Nodes.AddRange(nodeNames.Select(name => new SankeyNode { Name = name }));

            foreach (var candidate in candidates)
            {
                int descIndex = diagram.Nodes.FindIndex(n => n.Name == candidate.Description);
                int safetyIndex = diagram.Nodes.FindIndex(n => n.Name == "SafetyScore");
                int mobilityIndex = diagram.Nodes.FindIndex(n => n.Name == "MobilityAndEconomyScore");
                int equityIndex = diagram.Nodes.FindIndex(n => n.Name == "EquityAndAccessScore");
                int resilienceIndex = diagram.Nodes.FindIndex(n => n.Name == "ResilienceAndEnvironmentScore");
                int conditionIndex = diagram.Nodes.FindIndex(n => n.Name == "ConditionAndPerformanceScore");

                diagram.Links.Add(new SankeyLink { Source = descIndex, Target = safetyIndex, Value = candidate.SafetyScore });
                diagram.Links.Add(new SankeyLink { Source = descIndex, Target = mobilityIndex, Value = candidate.MobilityAndEconomyScore });
                diagram.Links.Add(new SankeyLink { Source = descIndex, Target = equityIndex, Value = candidate.EquityAndAccessScore });
                diagram.Links.Add(new SankeyLink { Source = descIndex, Target = resilienceIndex, Value = candidate.ResilienceAndEnvironmentScore });
                diagram.Links.Add(new SankeyLink { Source = descIndex, Target = conditionIndex, Value = candidate.ConditionAndPerformanceScore });
            }

            return diagram;
        }
    }

    public class SankeyNode
    {
        public string Name { get; set; }
    }

    public class SankeyLink
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public double? Value { get; set; }
    }

    public class SankeyDiagram
    {
        public List<SankeyNode> Nodes { get; set; } = new List<SankeyNode>();
        public List<SankeyLink> Links { get; set; } = new List<SankeyLink>();
    }
}

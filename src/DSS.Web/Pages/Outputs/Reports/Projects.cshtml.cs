using DSS.Pool;
using DSS.Pool.Dtos;
using DSS.ProjectCandidates;
using DSS.ProjectCandidates.Dtos;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using DSS.WorkCandidates;
using DSS.WorkCandidates.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Outputs
{
    [Authorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class ProjectsModel : PageModel
    {
        private readonly IProjectCandidateAppService _projectCandidateAppService;
        private readonly IWorkCandidatesAppService _WorkCandidateAppService;
        private readonly IPoolAppService _poolsappservice;
        public List<ProjectCandidateDto> ProjectCandidates { get; set; }
        public List<PoolDto> Pools { get; set; }
        public Guid? ScenarioId { get;  set; }
        public Guid? PoolId { get;  set; }
        public string SearchString { get; set; }
        public string ScenarioName { get;  set; }
        public string PoolName { get;  set; }

        public WorkCandidatesDto project { get; set; }
        public ProjectsModel(IProjectCandidateAppService projectCandidateAppService, IScenarioAppService scenarioAppService, IPoolAppService poolsappservice, IWorkCandidatesAppService WorkCandidateAppService)
        {
            _projectCandidateAppService = projectCandidateAppService;
            _WorkCandidateAppService = WorkCandidateAppService;
            _poolsappservice = poolsappservice;
        }

     
        public async Task OnGet(Guid? poolId, string searchString = "")
        {
            SearchString = searchString;

            if (poolId.HasValue)
            {
                ProjectCandidates = await _projectCandidateAppService.GetAllProjectsAsync(poolId.Value);
                var pool = await _poolsappservice.GetAsync(poolId.Value);
                PoolId = poolId.Value;
                PoolName = pool.Name;

                foreach (var projectCandidate in ProjectCandidates)
                {
                    project = await _WorkCandidateAppService.GetByProjectId(projectCandidate.Id);
                    if (project != null)
                    {
                        projectCandidate.AssetType = project.AssetType;
                    }
                }
            }
            else
            {
                ProjectCandidates = await _projectCandidateAppService.GetAllAsync();
                PoolId = ProjectCandidates.FirstOrDefault()?.PoolId;
            }


            



            Pools = await _poolsappservice.GetAllAsync("","");
        }



        public async Task<IActionResult> OnPostUpdateProject(ProjectCandidateUpdateDto input)
        {
            await _projectCandidateAppService.UpdateAsync(input.Id, input);
            return RedirectToPage("/OutPuts/Projects", new { scenarioId = ScenarioId });
        }

        public async Task<IActionResult> OnPostCreateProject(ProjectCandidateCreateDto input)
        {
            await _projectCandidateAppService.CreateAsync(input);
            return RedirectToPage("/Outputs/Projects", new { scenarioId = ScenarioId });
        }
    }
}

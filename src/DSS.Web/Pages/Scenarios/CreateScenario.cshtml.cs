using Azure;
using DSS.Pool;
using DSS.Pool.Dtos;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace DSS.Web.Pages.Scenarios
{
    [Authorize]
    public class CreateScenarioModel : PageModel
    {
        private readonly IPoolAppService _poolAppService;
        private readonly IScenarioAppService _scenarioAppService;
        private readonly ICurrentUser _currentUser;

        public CreateScenarioModel(IPoolAppService poolAppService, IScenarioAppService scenarioAppService, ICurrentUser currentUser)
        {
            _poolAppService = poolAppService;
            _scenarioAppService = scenarioAppService;
            _currentUser = currentUser;
        }

        public List<PoolDto> Pools { get; set; }
        public async Task OnGet()
        {
            Pools = await _poolAppService.GetAllAsync("", "");
        }

        public async Task<IActionResult> OnGetCheckName(string name)
        {
            bool response = await _scenarioAppService.CheckName(name);
            return new JsonResult(new { success = response });
        }


        public async Task<IActionResult> OnPostCreateAsync(ScenarioCreateDto newScenario)
        {           
            await _scenarioAppService.Create(newScenario);
            return RedirectToPage("/Scenarios/Index");
        }
    }
}

using DSS.Entities;
using DSS.Goals;
using DSS.Goals.Dtos;
using DSS.Web.Pages.Configuration.Measures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Configuration.Goals
{
    [Authorize]
    public class ListModel : PageModel
    {
        private readonly IGoalAppService _goalAppService;
        public ListModel(IGoalAppService goalAppService)
        {
            _goalAppService = goalAppService;
        }

        public List<GoalDto> Goals { get; set; }

        public async Task OnGetAsync()
        {
            Goals = await _goalAppService.GetAllAsync();
        }
        public async Task<IActionResult> OnPostDeleteResult(Guid id)
        {
            try
            {
                await _goalAppService.DeleteAsync(id);
                return RedirectToPage();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}

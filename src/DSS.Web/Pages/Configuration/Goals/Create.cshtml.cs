using DSS.Goals;
using DSS.Goals.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Configuration.Goals
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IGoalAppService _goalAppService;

        public CreateModel(IGoalAppService goalAppService)
        {
            _goalAppService = goalAppService;
        }

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostCreate(GoalCreateDto goalCreateDto)

        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _goalAppService.CreateAsync(goalCreateDto);
            return RedirectToPage("/Configuration/Goals/List");
        }
    }
}

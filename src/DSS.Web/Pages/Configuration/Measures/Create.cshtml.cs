using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Configuration.Measures
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IMeasureRepository<Measure, Guid> _measureRepository;
        public CreateModel(IMeasureRepository<Measure, Guid> measureRepository)
        {
            _measureRepository = measureRepository;
        }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostCreate(Measure measure)
        {
          
            if (!ModelState.IsValid)
            {
                return Page();
            }
          
            measure.Id = Guid.NewGuid();

            await _measureRepository.Create(measure);
            return RedirectToPage("/Configuration/Measures/List");
        }


    }
}

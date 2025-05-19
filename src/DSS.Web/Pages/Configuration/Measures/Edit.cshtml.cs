using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace DSS.Web.Pages.Configuration.Measures
{
    [Authorize]
    public class EditModel : PageModel
    {
        public Measure MeasureEntityEdit { get; set; }
        private readonly IMeasureRepository<Measure, Guid> _measureRepository;

        public EditModel(IMeasureRepository<Measure, Guid> measureRepository)
        {
            _measureRepository = measureRepository;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            MeasureEntityEdit = await _measureRepository.GetAsync(id);
            if (MeasureEntityEdit == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(Measure MeasureEntityEdit)
        
        {
            await _measureRepository.UpdateAsync(MeasureEntityEdit);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (MeasureEntityEdit == null)
            {
                return NotFound();
            }

            
            return RedirectToPage("/Configuration/Measures/List");
        }

    }
}

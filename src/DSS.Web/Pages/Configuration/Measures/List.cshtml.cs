using DSS.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DSS.Web.Pages.Configuration.Measures
{
    [Authorize]
    public class ListModel : PageModel
    {
        private readonly IMeasureRepository<Measure, Guid> _measureRepository;

        public ListModel(IMeasureRepository<Measure, Guid> measureRepository)
        {
            _measureRepository = measureRepository;
        }

        public List<Measure> MeasureEntities { get; set; } 

        public async Task OnGetAsync() 
        {
            MeasureEntities = await _measureRepository.GetAllAsync();
        }

        public async Task<IActionResult> GetAsync(Guid id)
        {
            var measure = await _measureRepository.GetAsync(id);
            return RedirectToPage("/Configuration/Measures/Edit", new { id });
        }

        public async Task<IActionResult> OnPostDeleteResult(Guid id)
        {
            try
            {
                await _measureRepository.DeleteAsync(id);
                return RedirectToPage();
            }
            catch (Exception ex)
            {

                throw;
            }
        }



    }
}

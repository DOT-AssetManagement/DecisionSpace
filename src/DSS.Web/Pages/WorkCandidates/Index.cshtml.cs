using DSS.PoolScoreParameters;
using DSS.PoolScoreParameters.Dtos;
using DSS.WorkCandidates;
using DSS.WorkCandidates.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DSS.Web.Pages.WorkCandidates
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IWorkCandidatesAppService _workCandidatesAppService;
        public List<WorkCandidatesDto> WorkCandidates { get; set; }

        public string PoolId { get; set; }
        public string ProjectCandidateId { get; set; }

        public IndexModel(IWorkCandidatesAppService workCandidatesAppService)
        {
            _workCandidatesAppService = workCandidatesAppService;
        }
        public async Task OnGet(Guid id,Guid Projectid)
        {
            WorkCandidates = await _workCandidatesAppService.GetAllByPoolOrProjectIdAsync(id, Projectid);
            PoolId = id.ToString();
        }
    

        public async Task<IActionResult> OnPostCreate(WorkCandidatesCreateDto input, Guid PoolId)
        {
            await _workCandidatesAppService.CreateWorkCandidateAsync(input, PoolId);
            return RedirectToPage("/WorkCandidates/Index", new { id = PoolId });
        }

        public async Task<IActionResult> OnPostUpdate(Guid id, WorkCandidatesUpdateDto input, Guid PoolId)
        {
            await _workCandidatesAppService.UpdateWorkCandidateAsync(id, input, PoolId);
            return RedirectToPage("/WorkCandidates/Index", new { id = PoolId });
        }

        public async Task<IActionResult> OnGetDelete(Guid id, Guid PoolId)
        {
            await _workCandidatesAppService.DeleteAsync(id);
            return RedirectToPage("/WorkCandidates/Index", new { id = PoolId });
        }
    }
}

using DSS.Entities;
using DSS.OthersParameters.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System;

namespace DSS.Web.Pages.SetUp.OtherParameter
{
    public class EditcshtmlModel : PageModel
    {
        private readonly IOtherParameterAppService _appService;

        public EditcshtmlModel(IOtherParameterAppService appService)
        {
            _appService = appService;
        }

        public TblOtherParameterDto otherParameterDto { get; set; }
        public async Task<IActionResult> OnGetAsync(string code)
         {
            otherParameterDto = await _appService.GetParameterAsync(code);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string code, TblOtherParameterUpdateDto input)
        {
            if (!ModelState.IsValid)
            {
                return Page(); 
            }

            var result = await _appService.UpdateAsync(code, input);

            if (result == null)
            {
                return NotFound();
            }

            return RedirectToPage("/SetUp/OtherParameter/List");
        }
    }
}

using DSS.Entities;
using DSS.OthersParameters.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages.SetUp.OtherParameter
{
    public class ListModel : PageModel
    {
        private readonly IOtherParameterAppService _otherparameterservice;

        public ListModel(IOtherParameterAppService otherparameterservice)
        {
            _otherparameterservice = otherparameterservice;
        }
        public List<TblOtherParameterDto> otherParameterDtos { get; set; }

        public async Task OnGet()
        {
            otherParameterDtos = await _otherparameterservice.GetAllAsync();
        }
        public async Task<IActionResult> OnPostDeleteParameter(string code)
        {
            await _otherparameterservice.DeleteOtherParameterAsync(code);
            return RedirectToPage("/SetUp/OtherParameter/List");
        }

    }
}

using DSS.Counties;
using DSS.Counties.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages.SetUp
{
    public class CountiesModel : PageModel
    {
        private readonly ICountyAppService _countyAppService;
        public List<CountyDto> Counties { get; set; }
        public CountiesModel(ICountyAppService countyAppService)
        {
            _countyAppService = countyAppService;
        }
        public async Task OnGet()
        {
            Counties = await _countyAppService.GetCounties();
        }
    }
}

using DSS.Pool;
using DSS.Pool.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly IPoolAppService _poolAppService;
        public List<PoolDto> Pools { get; set; }

        public DashboardModel(IPoolAppService poolAppService)
        {
            _poolAppService = poolAppService;
        }
        public async Task OnGet()
        {
            Pools = await _poolAppService.GetAllAsync("", "");
        }
    }
}

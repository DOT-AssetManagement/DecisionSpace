using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DSS.Web.Pages.Outputs
{
    [Authorize]
    public class PrioritizationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

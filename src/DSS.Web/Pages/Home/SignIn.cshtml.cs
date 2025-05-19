using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DSS.Web.Pages.Home
{
    [Authorize]
    public class SignInModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

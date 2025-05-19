using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DSS.Web.Pages;

public class IndexModel : DSSPageModel
{
    public virtual Task<IActionResult> OnGet()
    {
        if (CurrentUser.IsAuthenticated)
        {
            return Task.FromResult<IActionResult>(Redirect("~/Dashboard"));
        }
        return Task.FromResult<IActionResult>(Page());
    }
}

using DSS.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace DSS.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class DSSPageModel : AbpPageModel
{
    protected DSSPageModel()
    {
        LocalizationResourceType = typeof(DSSResource);
    }
}

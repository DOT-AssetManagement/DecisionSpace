using DSS.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DSS.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DSSController : AbpControllerBase
{
    protected DSSController()
    {
        LocalizationResource = typeof(DSSResource);
    }
}

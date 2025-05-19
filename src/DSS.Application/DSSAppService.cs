using System;
using System.Collections.Generic;
using System.Text;
using DSS.Localization;
using Volo.Abp.Application.Services;

namespace DSS;

/* Inherit your application services from this class.
 */
public abstract class DSSAppService : ApplicationService
{
    protected DSSAppService()
    {
        LocalizationResource = typeof(DSSResource);
    }
}

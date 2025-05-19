using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace DSS.Web;

[Dependency(ReplaceServices = true)]
public class DSSBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "DSS";
}

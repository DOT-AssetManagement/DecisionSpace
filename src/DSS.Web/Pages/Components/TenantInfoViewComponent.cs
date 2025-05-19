using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using Volo.Abp.Users;

public class TenantInfoViewComponent : ViewComponent
{
    private readonly ICurrentUser _currentUser;
    private readonly ITenantStore _tenantStore;

    public TenantInfoViewComponent(ICurrentUser currentUser, ITenantStore tenantStore)
    {
        _currentUser = currentUser;
        _tenantStore = tenantStore;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var tenantName = _currentUser.TenantId.HasValue
            ? (await _tenantStore.FindAsync(_currentUser.TenantId.Value))?.Name
            : null;

        var model = new TenantInfoViewModel
        {
            UserName = _currentUser.Name,
            UserRole = _currentUser.Roles.FirstOrDefault(),
            TenantName = tenantName
        };

        return View(model);
    }
}

public class TenantInfoViewModel
{
    public string UserName { get; set; }
    public string UserRole { get; set; }
    public string TenantName { get; set; }
}

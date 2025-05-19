using System.Threading.Tasks;
using DSS.Localization;
using DSS.MultiTenancy;
using DSS.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace DSS.Web.Menus;

public class DSSMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        administration.Order = 8;
        var l = context.GetLocalizer<DSSResource>();

   
        var importScreenMenuItem = new ApplicationMenuItem(
               DSSMenus.Import,
               l["Import"],
               "/importScreen/Index",
               icon: "fas fa-home",
               order: 0,
               requiredPermissionName: DSSPermissions.ImportPermission.Default
           );

        var setUpMenuItem = new ApplicationMenuItem(
        "DSSMenus.SetUp",
        l["SetUp"],
         "~/",
        icon: "fas fa-gear",
        order: 0,
         requiredPermissionName: DSSPermissions.SetUp.Default

      );
        var assetTypeParameterMenuItem = new ApplicationMenuItem(
                   DSSMenus.AssetTypeparameter,
                   l["AssetTypeparameter"],
                   "/SetUp/AssetTypeParameter/List",
                   icon: "fas fa-list",
                   order: 0,
                   requiredPermissionName: DSSPermissions.AssetTypeParameterPermissions.Default
               );
        var otherParameterMenuItem = new ApplicationMenuItem(
                       DSSMenus.OtherParameter,
                       l["OtherParameter"],
                       "/SetUp/OtherParameter/List",
                       icon: "fas fa-list",
                       order: 0,
                       requiredPermissionName: DSSPermissions.OtherParameterPermissions.Default
                   );

        administration.Items.Add(setUpMenuItem);
        setUpMenuItem.Items.Add(assetTypeParameterMenuItem);
        setUpMenuItem.Items.Add(otherParameterMenuItem);
        administration.Items.Add(importScreenMenuItem);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);

        return Task.CompletedTask;
    }
}

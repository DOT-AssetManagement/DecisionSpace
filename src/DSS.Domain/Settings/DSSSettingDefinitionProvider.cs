using Volo.Abp.Settings;

namespace DSS.Settings;

public class DSSSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DSSTestingSettings.MySetting1));
    }
}

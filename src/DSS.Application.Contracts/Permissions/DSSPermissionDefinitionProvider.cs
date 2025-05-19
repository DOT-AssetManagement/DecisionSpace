using DSS.Localization;
using System.Net;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;


namespace DSS.Permissions;

public class DSSPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DSSPermissions.GroupName);

        var administrationPermission = myGroup.AddPermission(DSSPermissions.SetUp.Default, L("Set Up"));

        // Asset Type Parameter Permissions
        var assetTypeParameterPermission = administrationPermission
            .AddChild(DSSPermissions.AssetTypeParameterPermissions.Default, L("Asset Type Parameter"));
        assetTypeParameterPermission.AddChild(DSSPermissions.AssetTypeParameterPermissions.Create, L("Create"));
        assetTypeParameterPermission.AddChild(DSSPermissions.AssetTypeParameterPermissions.Edit, L("Edit"));
        assetTypeParameterPermission.AddChild(DSSPermissions.AssetTypeParameterPermissions.Delete, L("Delete"));

        // Other Parameter Permissions
        var otherParameterPermission = administrationPermission
            .AddChild(DSSPermissions.OtherParameterPermissions.Default, L("Other Parameter"));
        otherParameterPermission.AddChild(DSSPermissions.OtherParameterPermissions.Create, L("Create"));
        otherParameterPermission.AddChild(DSSPermissions.OtherParameterPermissions.Edit, L("Edit"));
        otherParameterPermission.AddChild(DSSPermissions.OtherParameterPermissions.Delete, L("Delete"));

        var ImportPermission = myGroup.AddPermission(DSSPermissions.ImportPermission.Default, L("Import"));


        var CandidatePoolsActions = myGroup.AddPermission(CandidatePoolsPermissions.PoolsPageActions.Default, L("Candidate Pools"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.ImportWorkCandidates, L("Import Work Candidates"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.ImportConfiguration, L("Import Configuration"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.WorkCandidates, L("Work Candidates"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.Configuration, L("Configuration"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.Run, L("Run"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.OutPut, L("Outputs"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.Projects, L("Projects"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.ExportProjectsOrCandidate, L("Export Projects & Candidates"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.Edit, L("Edit"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.Clear, L("Clear"));
        CandidatePoolsActions.AddChild(CandidatePoolsPermissions.PoolsPageActions.Create, L("Create"));


            

        var ConfigurationActions = myGroup.AddPermission(ConfigurationPermissions.ConfigurationPageActions.Default, L("Configuration"));
        ConfigurationActions.AddChild(ConfigurationPermissions.ConfigurationPageActions.Create, L("Create"));
        ConfigurationActions.AddChild(ConfigurationPermissions.ConfigurationPageActions.Edit, L("Edit"));
        ConfigurationActions.AddChild(ConfigurationPermissions.ConfigurationPageActions.Import, L("Import"));


        var ProjectsActions = myGroup.AddPermission(PoolProjectPermissions.PoolsProjectPageActions.Default, L("Projects"));
        ProjectsActions.AddChild(PoolProjectPermissions.PoolsProjectPageActions.Edit, L("Edit"));
        ProjectsActions.AddChild(PoolProjectPermissions.PoolsProjectPageActions.Export, L("Export to Excel"));


        var ProjectWorkCandidatesActions = myGroup.AddPermission(ProjectWorkCandidatePermissions.ProjectWorkCandidatePageActions.Default, L("Work Candidates"));
        ProjectWorkCandidatesActions.AddChild(ProjectWorkCandidatePermissions.ProjectWorkCandidatePageActions.Create, L("Create"));
        ProjectWorkCandidatesActions.AddChild(ProjectWorkCandidatePermissions.ProjectWorkCandidatePageActions.Edit, L("Edit"));
        ProjectWorkCandidatesActions.AddChild(ProjectWorkCandidatePermissions.ProjectWorkCandidatePageActions.Delete, L("Delete"));

        var Dashboard = myGroup.AddPermission(DSSPermissions.TopBarPermissions.Dashboard, L("Dashboard"));
        var OutPuts = myGroup.AddPermission(DSSPermissions.TopBarPermissions.Outputs, L("Outputs"));

        var ChartsActions = myGroup.AddPermission(ChartsPermissions.Charts.Default, L("Charts"));
        ChartsActions.AddChild(ChartsPermissions.Charts.ProjectsAndGoalsFlowDiagram, L("Projects And Goals Flow Diagram"));
        ChartsActions.AddChild(ChartsPermissions.Charts.ProjectEfficiencyAndCost, L("Project Efficiency and Cost"));
        ChartsActions.AddChild(ChartsPermissions.Charts.ProjectsAndGoalsPercentageContribution, L("Projects and Goals Percentage Contribution"));
        ChartsActions.AddChild(ChartsPermissions.Charts.ProjectsAndGoalsActualContribution, L("Projects and Goals Actual Contribution"));
        ChartsActions.AddChild(ChartsPermissions.Charts.ProjectToScoreArcMapping, L("Project to Score Arc Mapping"));



        var ReportsActions = myGroup.AddPermission(ReportsPermissions.Reports.Default, L("Reports"));
        ReportsActions.AddChild(ReportsPermissions.Reports.Projects, L("Projects"));
        ReportsActions.AddChild(ReportsPermissions.Reports.WorkCandidateSummary, L("Work Candidate Summary"));
        ReportsActions.AddChild(ReportsPermissions.Reports.WorkCandidateDetail, L("Work Candidate Detail"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DSSResource>(name);
    }
}




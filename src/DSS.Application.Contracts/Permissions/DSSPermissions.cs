
namespace DSS.Permissions;

public static class DSSPermissions
{
    public const string GroupName = "DSS";

    public static class SetUp
    {
        public const string Default = GroupName + ".SetUp";

    }
    public static class Administrarion
    {
        public const string Default = GroupName + ".SetUp";
    }

    public static class AssetTypeParameterPermissions
    {
        public const string Default = "AssetTypeParameter";
        public const string AssetType = Default + ".AssetTypeParameter";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class OtherParameterPermissions
    {
        public const string Default = "OtherParameter";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
    public static class ImportPermission
    {
        public const string Default = "Import";
        public const string Import = Default + ".Import";
    }
    public static class TopBarPermissions
    {
        public const string Default = "Topbar";
        public const string Dashboard = Default + ".Dashboard";
        public const string Outputs = Default + ".Outputs";
    }

}

public static class CandidatePoolsPermissions
{
    public static class PoolsPageActions
    {
        public const string Default = "Candidate Pools";
        public const string Create = Default + ".Create";
        public const string Clear = Default + ".Clear";
        public const string ImportWorkCandidates = Default + ".ImportWorkCandidates";
        public const string ImportConfiguration = Default + ".ImportConfiguration";
        public const string WorkCandidates = Default + ".WorkCandidates";
        public const string Configuration = Default + ".Configuration";
        public const string Run = Default + ".Run";
        public const string OutPut = Default + ".Outputs";
        public const string Projects = Default + ".Projects";
        public const string ExportProjectsOrCandidate = Default + ".ExportProjectsOrCandidates";
        public const string Edit = Default + ".Edit";
    }
}

public static class ConfigurationPermissions
{
    public static class ConfigurationPageActions
    {
        public const string Default = "ConfigurationPageAction";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Import = Default + ".Import";
    }
}


public static class PoolProjectPermissions
{
    public static class PoolsProjectPageActions
    {
        public const string Default = "PoolsProjectPageActions";
        public const string Edit = Default + ".Edit";
        public const string Export = Default + ".Export";
    }
}

public static class ProjectWorkCandidatePermissions
{
    public static class ProjectWorkCandidatePageActions
    {
        public const string Default = "ProjectWorkCandidatePageActions";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}


public static class ChartsPermissions
{
    public static class Charts
    {
        public const string Default = "Charts";
        public const string ProjectsAndGoalsFlowDiagram = Default + ".ProjectsAndGoalsFlowDiagram";
        public const string ProjectEfficiencyAndCost = Default + ".ProjectEfficiencyAndCost";
        public const string ProjectsAndGoalsPercentageContribution = Default + ".ProjectsAndGoalsPercentageContribution";
        public const string ProjectsAndGoalsActualContribution = Default + ".ProjectsAndGoalsActualContribution";
        public const string ProjectToScoreArcMapping = Default + ".ProjectToScoreArcMapping";
    }
}

public static class ReportsPermissions
{
    public static class Reports
    {
        public const string Default = "Reports";
        public const string Projects = Default + ".Projects";
        public const string WorkCandidateSummary = Default + ".WorkCandidateSummary";
        public const string WorkCandidateDetail = Default + ".WorkCandidateDetail";
    
    }
}






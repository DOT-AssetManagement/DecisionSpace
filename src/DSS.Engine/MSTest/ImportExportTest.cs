using log4net;
using log4net.Appender;
using log4net.Layout;
using System.Security.Cryptography;
using DSS.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DSS.Utilities
{
    [TestClass]
    public class ImportExportTest
    {

        private Guid defaultPoolId = Guid.Parse("281DA274-C446-45B5-9FD7-F8100934E69A");
        private string connectionString = @"Server=XPS8960\MSS2022;Database=DSS;Trusted_Connection=True;MultipleActiveResultSets=True;";
        private string workCandidatesExcel = @"C:\Projects\FHWA-SpyPond\PennDOT-2022\DSS\SpoofProjects_NorthwestRPO_YearsAdded_KE_05222024.xlsx";
        private string b2pExcel = @"C:\Projects\FHWA-SpyPond\PennDOT-2022\DSS\BridgeToPavements.xlsx";
        private string masExcel = @"C:\Projects\FHWA-SpyPond\PennDOT-2022\DSS\MaintainableAssetSegmentation.xlsx";

        #region LOG      
        private static ILog _log = ConfigureLogger();
        private static log4net.ILog ConfigureLogger()
        {
            // Programmatic configuration

            // follows (with some streamlining) the example from Brendan Long and Ron Grabowski
            // org.apache.logging.log4net-user
            // These config statements create a RollingFile Appender.  Rolling File Appenders rollover on each execution of the test harness, 
            // in this case, following the Composite RollingMode.  Alternative log4net appenders may be added  or replace this default appender at the programmer's discretion.

            // PatternLayout layout = new PatternLayout("%d [%t] %-5p %c - %m%n");

            PatternLayout layout = new PatternLayout("%d %-5p %c - %m%n");
            log4net.Appender.RollingFileAppender appender = new RollingFileAppender
            {
                Layout = layout,
                AppendToFile = true,
                MaxFileSize = 10000000,
                RollingStyle = RollingFileAppender.RollingMode.Composite,
                StaticLogFileName = true,

                File = @"C:\GIT_Sandbox\_DEA\DEALib\Logs\ImportExportTest.log" // all logs will be created in the subdirectory logs 
            };

            // Configure filter to accept log messages of any level.
            log4net.Filter.LevelMatchFilter traceFilter = new log4net.Filter.LevelMatchFilter
            {
#if DEBUG
                LevelToMatch = log4net.Core.Level.Debug
#else
                LevelToMatch = log4net.Core.Level.Info
#endif
            };
            appender.ClearFilters();
            appender.AddFilter(traceFilter);

            appender.ImmediateFlush = true;
            appender.ActivateOptions();

            // Attach appender into hierarchy
            log4net.Repository.Hierarchy.Logger root =
                ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root;
            root.AddAppender(appender);
            root.Repository.Configured = true;

            log4net.ILog log = log4net.LogManager.GetLogger("IMPORT-EXPORT");

            log.Debug("LOGGER created.");

            return log;
        }
        #endregion



        [TestMethod]
        public void ImportWorkCandidates()
        {
            DataManager.Log = _log;
            bool ok = DataManager.ImportDSSWorkCandidatesFromExcel(connectionString, 
                    workCandidatesExcel, "Northwest RPO", "WorkCandidates", 
                    defaultPoolId, false, out Guid? sessionId, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void ImportBridgeToPavements()
        {
            DataManager.Log = _log;
            bool ok = DataManager.ImportBridgeToPavementFromExcelToDSS(connectionString, b2pExcel,
                  out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void ImportScoreParameters()
        {
            DataManager.Log = _log;
            bool ok = DataManager.ImportScoreParameters(connectionString, defaultPoolId,
                  @"C:\Projects\FHWA-SpyPond\PennDOT-2022\DSS\SpoofProjects_NorthwestRPO_ScoresWeights_KE_0520.xlsx",
                  "ScoresWeights",
                  out string? errorMessage, true, true);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void UpdateWorkCandidate()
        {
            DataManager.Log = _log;
            Guid id = Guid.Parse("31D45587-16E0-4115-9DB4-C333E161ED5E");   // Such work candidate must exist!
            bool ok = DataManager.UpdateWorkCandidate(connectionString, id, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void RunScenario()
        {
            DataManager.Log = _log;
            Guid scenId = Guid.Parse("94F05DBF-6854-4BFC-8F00-05DC90C9C9B7");  // Such scenario must exist
            bool ok = DataManager.RunScenario(connectionString, scenId, out string? errorMessage); 
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void WriteDSSOutputUsingSingleSProc()
        {
            ExcelHandler.Log = _log;
            bool ok = ExcelHandler.WriteDSSOutputUsingSingleSProc(connectionString, null, null,
                    @"Foo.xlsx",
                    new List<string> { "Projects", "Pavements", "Bridges" }, "dbo.UspGetScenarioOutput",
                    out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void ExportScenarioData()
        {
            DataManager.Log = _log;
            Guid? scdenId = Guid.Parse("94F05DBF-6854-4BFC-8F00-05DC90C9C9B7"); // Scenario must exist
            bool ok = DataManager.ExportScenarioData(connectionString, scdenId, 0.0, "FooS.xlsx", out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void ExportPoolData()
        {
            DataManager.Log = _log;
            Guid? poolId = Guid.Parse("281DA274-C446-45B5-9FD7-F8100934E69A"); // Pool must exist
            bool ok = DataManager.ExportScenarioData(connectionString, poolId, 0.0, "FooP.xlsx", out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void UpdatePool()
        {
            DataManager.Log = _log; 
            bool ok = DataManager.UpdatePool(connectionString, defaultPoolId, true, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void CreateScenario()
        {
            DataManager.Log = _log;
            bool ok = DataManager.CreateScenario(connectionString, "DIG Scenario", "Dmitry's scenario",
                    defaultPoolId, null, null, out Guid? scenId, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void ImportMASFromExcel()
        {
            DataManager.Log = _log;
            bool ok = DataManager.ImportMaintainableAssetSegmentation(connectionString, masExcel, null, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void ClearPoolWorkCandidateData()
        {
            DataManager.Log = _log;
            bool ok = DataManager.ClearPoolWorkCandidateData(connectionString, defaultPoolId, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void CreateUserPool()
        {
            DataManager.Log = _log;
            Guid userId = Guid.Parse("AFC9A23C-7C9A-4A71-BB35-1569A5B6FD2E"); // Asher Muneer
            bool ok = DataManager.CreateUserPool(connectionString, "Asher's pool", userId, out Guid? poolId, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void GetGisJsonOutput()
        {
            DataManager.Log = _log;
            Guid scenId = Guid.Parse("94F05DBF-6854-4BFC-8F00-05DC90C9C9B7");
            bool ok = DataManager.GetGisJsonOutput(connectionString, scenId, 0.0, out DssGisOutput? gis, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }

        [TestMethod]
        public void GetGisJsonStringOutput()
        {
            DataManager.Log = _log;
            Guid scenId = Guid.Parse("94F05DBF-6854-4BFC-8F00-05DC90C9C9B7");
            bool ok = DataManager.GetGisJsonStringOutput(connectionString, scenId, 0.99, true, out string? gisJson, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
            if (ok)
            {
                File.WriteAllText(@"C:\Projects\FHWA-SpyPond\PennDOT-2022\DSS\SampleDSSOutputForGIS_2024-07-16.json", gisJson);
            }
        }

        [TestMethod]
        public void CreateNewWorkCandidate()
        {
            DataManager.Log = _log;
            WorkCandidate wc = new WorkCandidate()
            {
                PoolId = Guid.Parse("281DA274-C446-45B5-9FD7-F8100934E69A"),
                ProjectName = "20240704",
                District=10,
                Cnty=16,
                Route=322,
                AssetType = "P",
                FromSection = 10,
                ToSection = "110",
                Description = "Independence Day, 04/07/2024",
                Cost=123490,
                HSIP_Eligible = 0,
                Excess_Safety_Value = 0.2,
                Supported_by_RSA_Safety_Study = 0,
                Degree_of_Safety_Improvement=1,
                Supports_Hazard_Mitigation=1,
                Improves_Access_to_Emergency_Services=0,
                Travel_Time_Savings=700,
                Operating_Cost_Savings=19460,
                Facilitates_Transit_Use=0,
                Bike_Lanes_Constructed=1000,
                Sidewalks_and_Curb_Ramps_Constructed_or_Improved=800,
                Reduced_Fuel_Consumption=20000,
                Degree_of_Environmental_Improvement=1,
                Pavement_Rehabilitated_or_Reconstructed=1,
                Sidewalks_Rehabilitated_or_Reconstructed=0,
                ADT=2000,
                Percent_Trucks=0.05,
                Population_Density=69,
                Bike_Commute_Share=0.01,
                BPN="3",
                Length=10,
                Speed_Limit=35,
                Detour_Distance=5,
                LFAR_BOF_Eligible=0,
                Treatment= "Mobility",
                Year=2026,
                MinYear=2026,
                MaxYear=2026
            };

            bool ok = DataManager.CreateNewWorkCandidate(connectionString, wc, out Guid? wcId, out Guid? projId, out string? errorMessage);
            Assert.IsTrue(ok, errorMessage);
        }
    }
}
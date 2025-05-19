using DEALib;
using log4net;
using log4net.Appender;
using log4net.Layout;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using DSS.Entities;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using static System.Collections.Specialized.BitVector32;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

namespace DSS.Utilities
{
    public class DataManager
    {

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

                File = @".\Logs\DSS.NET.log" // all logs will be created in the subdirectory logs 
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

            log4net.ILog log = log4net.LogManager.GetLogger("DSS-LOGGER");

            log.Debug("DSS-LOGGER created.");

            return log;
        }

        private static ILog? _log = ConfigureLogger();

        public static ILog? Log
        {
            get
            {
                return _log;
            }

            set
            {
                _log = value;
            }
        }

        private static void Generic_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Log.Info("T-SQL messages:\n" + e.Message);
        }

        /// <summary>
        /// Imports Bridge-to-Pavement Crosswalk Excel file
        /// </summary>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="excelFilePath">Pathname of the input Excel file</param>
        /// <param name="errorMessage">(out) error messagem], null if no errors</param>
        /// <returns>True on success, False on failure</returns>
        public static bool ImportBridgeToPavementFromExcelToDSS(string dbConnectionString, string excelFilePath, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            ExcelHandler.Log = Log;

            ok = ExcelHandler.ImportBridgeToPavementFromExcelToDSS(dbConnectionString, excelFilePath, out errorMessage);

            return ok;
        }

        /// <summary>
        /// Imports work candidates
        /// </summary>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="poolId">Guid of the target user pool (library) Id</param>
        /// <param name="excelFilePath">Full path name of the Excel file.</param>
        /// <param name="worksheetName">Name of the worksheet, like "WorkCandidates". May be left as null if it is the first worksheet.</param>
        /// <param name="errorMessage">(out) error message, null if no errors.</param>
        public static bool ImportScoreParameters(string dbConnectionString, Guid poolId, string excelFilePath, string? worksheetName, out string? errorMessage, bool fromScratch, bool keepUserCreated)
        {
            bool ok = true;
            errorMessage = null;

            ExcelHandler.Log = Log;

            ok = ExcelHandler.ImportScoreParameters(dbConnectionString, poolId, excelFilePath, worksheetName, out errorMessage, fromScratch, keepUserCreated);

            return ok;
        }

        public static bool ImportDSSWorkCandidatesFromExcel(string dbConnectionString,
                string excelFilePath,
                string? importSourceName,
                string? worksheetName,
                Guid poolId,
                bool forgiving,
                out Guid? guidSession,
                out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;
            guidSession = null;

            ExcelHandler.Log = Log;

            ok = ExcelHandler.ImportDSSWorkCandidatesFromExcel(dbConnectionString, excelFilePath,
                importSourceName, worksheetName, poolId, forgiving, out guidSession, out errorMessage);

            return ok;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnectionString">Connection string</param>
        /// <param name="workCandidateId">Guid Id of the recored in TblWorkCandidates table which must exist.</param>
        /// <param name="errorMessage">(out) error message, null if no errors</param>
        /// <returns>True on success, False on failure</returns>
        public static bool UpdateWorkCandidate(string dbConnectionString, Guid workCandidateId, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            Log.Info("UpdateWorkCandidate - started...");
            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    connection.Open();
                    connection.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.UspUpdateWorkCandidate";
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 12000;
                        command.Parameters.Add("@WorkCandidateId", SqlDbType.UniqueIdentifier).Value = workCandidateId;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
            }

            Log.Info("UpdateWorkCandidate - finished.");

            return ok;
        }

        public static bool RunScenario(string dbConnectionString, Guid scenarioId, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;

            Log.Info($"Run scenario [{scenarioId}] started...");
            Guid? runId = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    connection.Open();
                    connection.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.UspStartScenarioRun";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ScenId", SqlDbType.UniqueIdentifier).Value = scenarioId;
                        SqlParameter parmId = command.Parameters.Add("@Guid", SqlDbType.UniqueIdentifier);
                        parmId.Direction = ParameterDirection.Output;
                        command.ExecuteNonQuery();
                        runId = parmId.Value as Guid?;
                    }

                    DEAContext.Log = Log;
                    DEAContext.ConnectionString = connection.ConnectionString;
                    DEAContext? dea = null;

                    ok = DEAContext.LoadFromDatabase(scenarioId, out dea, out errorMessage);

                    if (!ok)
                    {
                        throw new Exception(errorMessage);
                    }

                    ok = dea.RunDEA(out errorMessage);

                    if (!ok)
                    {
                        throw new Exception(errorMessage);
                    }

                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = $@"DELETE FROM TblScenarioProjectObjectiveValues 
WHERE ScenId=CONVERT(UNIQUEIDENTIFIER,'{scenarioId}')";
                        cmd.CommandType = CommandType.Text;
                        int N = cmd.ExecuteNonQuery();
                        Log.Info($"Number of old records deleted from TblScenarioProjectObjectiveValues: {N}");
                    }

                    using (BulkInserter bulkInserter = new BulkInserter())
                    {
                        bulkInserter.Configure(connection, "TblScenarioProjectObjectiveValues", "DT", null, null, out errorMessage);
                        if (!ok)
                        {
                            throw new Exception(errorMessage);
                        }
                        foreach (Project p in dea.ProjectList)
                        {
                            DataRow r = bulkInserter.NewRow();
                            r["ScenId"] = scenarioId;
                            r["ProjectId"] = Guid.Parse(p.Name);
                            r["RelativeEfficiency"] = p.R;
                            ok = bulkInserter.AddRow(r, out errorMessage);
                            if (!ok)
                            {
                                throw new Exception();
                            }
                        }
                    }

                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "dbo.UspUpdateRelativeEfficiencies";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 24000;
                        cmd.Parameters.Add("@ScenId", SqlDbType.UniqueIdentifier).Value = scenarioId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
            }
            finally
            {
                if (runId != null)
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();
                        connection.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);

                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "dbo.UspCompleteScenarioRun";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@RunId", SqlDbType.UniqueIdentifier).Value = runId;
                            if (errorMessage != null)
                            {
                                cmd.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar, 4000).Value = errorMessage;
                            }
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            Log.Info($"Run scenario [{scenarioId}] finished. Ok={ok}, ErrorMessage: '{errorMessage}'");

            return ok;
        }


        /// <summary>
        /// Exports to Excel projects and work candidates for pavements and bridges
        /// associated with a scenario.
        /// </summary>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="scenId">ScenId; may be null, in which case data for all scenarios is exported</param>
        /// <param name="effThreshold">Relative efficiency threshold.  May be null, in which casde no efficiency threshold is applided</param>
        /// <param name="xlsxFilePath">Path name of the Excel file to be created.</param>
        /// <param name="errorMessage">(out) Error message.  Null if no errors.</param>
        /// <returns>True on success, false on failure</returns>
        public static bool ExportScenarioData(string dbConnectionString, Guid? scenId, double? effThreshold, string xlsxFilePath, out string? errorMessage)
        {
            errorMessage = null;
            bool ok = true;
            ExcelHandler.Log = _log;

            Log.Info($"ExportScenarioData (scenId=[{scenId}] , XLSX=[{xlsxFilePath}], effThreshold=({effThreshold})]) - started...");
            try
            {
                Guid? poolId = null;
                string sprocName = "dbo.UspGetScenarioOutput";

                if (scenId != null)
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        connection.Open();
                        connection.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);

                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT PoolId FROM TblScenarios WITH (NOLOCK) WHERE Id=CONVERT(UNIQUEIDENTIFIER, '{scenId}')";
                            cmd.CommandType = CommandType.Text;
                            object o = cmd.ExecuteScalar();
                            if (o != null && o != DBNull.Value)
                            {
                                poolId = Guid.Parse(o.ToString());
                            }
                        }

                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT DeaOutputSproc FROM TblScenarios WITH (NOLOCK) WHERE Id=CONVERT(UNIQUEIDENTIFIER, '{scenId}')";
                            cmd.CommandType = CommandType.Text;
                            object o = cmd.ExecuteScalar();
                            if (o != null)
                            {
                                sprocName = o.ToString();
                            }
                        }
                    }
                }

                ok = ExcelHandler.WriteDSSOutputUsingSingleSProc(dbConnectionString, poolId
                    , effThreshold
                    , xlsxFilePath
                    , new List<string> { "Projects", "Pavements", "Bridges" }
                    , sprocName
                    , out errorMessage);
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
            }

            Log.Info($"ExportScenarioData (scenId=[{scenId}] , XLSX=[{xlsxFilePath}], effThreshold=({effThreshold})]) - finished.  OK={ok}, ErrorMessage: '{errorMessage}'");
            return ok;
        }

        /// <summary>
        /// Exports to Excel projects and work candidates for pavements and bridges
        /// associated with a scenario.
        /// </summary>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="scenId">ScenId; may be null, in which case data for all scenarios is exported</param>
        /// <param name="effThreshold">Relative efficiency threshold.  May be null, in which casde no efficiency threshold is applided</param>
        /// <param name="xlsxFilePath">Path name of the Excel file to be created.</param>
        /// <param name="errorMessage">(out) Error message.  Null if no errors.</param>
        /// <returns>True on success, false on failure</returns>
        public static bool ExportPoolData(string dbConnectionString, Guid? poolId, double? effThreshold, string xlsxFilePath, out string? errorMessage)
        {
            errorMessage = null;
            bool ok = true;
            ExcelHandler.Log = _log;

            Log.Info($"ExportPoolData (poolId=[{poolId}] , XLSX=[{xlsxFilePath}], effThreshold=({effThreshold})]) - started...");
            string sprocName = "dbo.UspGetScenarioOutput";  // Default sproc

            ok = ExcelHandler.WriteDSSOutputUsingSingleSProc(dbConnectionString, poolId
                 , effThreshold
                 , xlsxFilePath
                 , new List<string> { "Projects", "Pavements", "Bridges" }
                 , sprocName
                 , out errorMessage);

            Log.Info($"ExportPoolData (poolId=[{poolId}] , XLSX=[{xlsxFilePath}], effThreshold=({effThreshold})]) - finished.  OK={ok}, ErrorMessage: '{errorMessage}'");
            return ok;
        }

        /// <summary>
        /// This function must be called by the UI each time users save their changes in work candidate data
        /// (table TblWorkCandidates) or TblPoolScoreParameters
        /// </summary>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="poolId">PoolId</param>
        /// <param name="scoreParametersUpdated">True if function is being called after changes in TblPoolScoreParameters, otherwise false.</param>
        /// <param name="errorMessage">(out) error message, null if no errors</param>
        /// <param name="bDoCreate">By default is true, can be passed as false if method is caalled if called after a change of parameters or if no deletes or additions happened among the work candidates.  Leave it as True if uncertain.</param>
        /// <returns>True on success, False on failure</returns>
        public static bool UpdatePool(string dbConnectionString, Guid poolId, bool scoreParametersUpdated, out string? errorMessage, bool bDoCreate = true)
        {
            errorMessage = null;
            bool ok = true;

            Log.Info($"UpdatePool [{poolId}] - started....");

            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnectionString))
                {
                    conn.Open();
                    conn.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);

                    if (scoreParametersUpdated)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "dbo.UspCreateScoreSQL";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                            cmd.CommandTimeout = 12000;
                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.UspCalculateBenefits";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                        cmd.CommandTimeout = 12000;
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.UspCalculateScores";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                        cmd.CommandTimeout = 12000;
                        cmd.ExecuteNonQuery();
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.UspUpdateProjectCandidates";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                        cmd.Parameters.Add("@CreateOrUpdate", SqlDbType.Char, 1).Value = "C";
                        cmd.CommandTimeout = 12000;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
            }

            Log.Info($"UpdatePool [{poolId}] - finished.");

            return (ok);
        }


        /// <summary>
        /// Creates a new scenario records in TblScenarios
        /// </summary>
        /// <param name="connectionString">database connection string</param>
        /// <param name="scenarioName">Scenario name</param>
        /// <param name="description">(optional) description, may be left null</param>
        /// <param name="poolId">Pool Id to be associated with (must exist)</param>
        /// <param name="deaInputSproc">DEA input sproc. May be left null to use default</param>
        /// <param name="deaOutputSproc">DEA output sproc. May be left null to use default</param>
        /// <param name="scenId">(out) generated Id of newly created scenario</param>
        /// <param name="errorMessage">(out) error message, null if no errors.</param>
        /// <returns>True on success, False on failure</returns>
        public static bool CreateScenario(string connectionString, string scenarioName, string? description, Guid poolId,
            string? deaInputSproc, string? deaOutputSproc,
            out Guid? scenId, out string? errorMessage)
        {
            bool ok = true;
            scenId = null;
            errorMessage = null;

            Log.Info($"Create scenario [{scenarioName}] - started...");
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    conn.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT TOP 1 ScenName FROM TblScenarios WITH (NOLOCK) WHERE UPPER(ScenName)='{scenarioName.ToUpper()}'";
                        object o = cmd.ExecuteScalar();
                        if (o != null && o != DBNull.Value)
                        {
                            throw new Exception($"Scenario [{o}] already exists.");
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT TOP 1 UserId FROM TblUserPools WITH (NOLOCK) WHERE Id=CONVERT(UNIQUEIDENTIFIER,'{poolId}')";
                        object o = cmd.ExecuteScalar();
                        if (o == null || o == DBNull.Value)
                        {
                            throw new Exception($"PoolId [{poolId}] is not associated with any user in the TblUserPools table.");
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT TOP 1 Measure FROM TblPoolScoreParameters WITH (NOLOCK) WHERE PoolId=CONVERT(UNIQUEIDENTIFIER,'{poolId}')";
                        object o = cmd.ExecuteScalar();
                        if (o == null || o == DBNull.Value)
                        {
                            throw new Exception($"There score calculation parameters (weights and scaling) for PoolId [{poolId}].");
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT TOP 1 ScoreName FROM TblPoolScoreSQL WITH (NOLOCK) WHERE PoolId=CONVERT(UNIQUEIDENTIFIER,'{poolId}')";
                        object o = cmd.ExecuteScalar();
                        if (o == null || o == DBNull.Value)
                        {
                            throw new Exception($"Score calculation SQL has not ben generated for PoolId [{poolId}].");
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"SELECT TOP 1 Id FROM TblWorkCandidates WITH (NOLOCK) WHERE PoolId=CONVERT(UNIQUEIDENTIFIER,'{poolId}')";
                        object o = cmd.ExecuteScalar();
                        if (o != null && o != DBNull.Value)
                        {
                            throw new Exception($"Work candidate data already exists for PoolId [{poolId}].");
                        }
                    }

                    int N = 0;

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"INSERT INTO TblScenarios (ScenName, Description, DeaInputSproc, DeaOutputSproc, PoolId)
 VALUES (@ScenName, @Description, @DeaInputSproc, @DeaOutputSproc, @PoolId)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@ScenName", SqlDbType.NVarChar, 50).Value = scenarioName;

                        if (description != null)
                        {
                            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = description;
                        }
                        else
                        {
                            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 255).Value = DBNull.Value;
                        }

                        if (deaInputSproc != null)
                        {
                            cmd.Parameters.Add("@DeaInputSproc", SqlDbType.NVarChar, 255).Value = deaInputSproc;
                        }
                        else
                        {
                            cmd.Parameters.Add("@DeaInputSproc", SqlDbType.NVarChar, 255).Value = "dbo.UspGetProjectCandidates";
                        }

                        if (deaOutputSproc != null)
                        {
                            cmd.Parameters.Add("@DeaOutputSproc", SqlDbType.NVarChar, 255).Value = deaOutputSproc;
                        }
                        else
                        {
                            cmd.Parameters.Add("@DeaOutputSproc", SqlDbType.NVarChar, 255).Value = "dbo.UspGetScenarioOutput";
                        }

                        cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;

                        N = cmd.ExecuteNonQuery();
                    }

                    if (N > 0)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = $"SELECT TOP 1 Id FROM TblScenarios WITH (NOLOCK) WHERE PoolId=CONVERT(UNIQUEIDENTIFIER,'{poolId}') ORDER BY ScenNo DESC";
                            object o = cmd.ExecuteScalar();
                            if (o == null || o == DBNull.Value)
                            {
                                throw new Exception("Scenario Id could nt be retrieved from the TblScenarios table.");
                            }

                            scenId = Guid.Parse(o.ToString());

                            Log.Info($"The Id of the newly created scenario is [{scenId}]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                // Log.Error(errorMessage);
            }

            // Log.Info($"Create scenario [{scenarioName}] - finished.");

            return (ok);
        }

        /// <summary>
        /// Imports Maintainable Asset Segmentation Excel file
        /// </summary>
        /// <param name="dbConnectionString">Database connection string</param>
        /// <param name="excelFilePath">Pathname of the input Excel file</param>
        /// <param name="worksheetName">Name of the worksheet.  May be null if Excel documentc ontains only one sheet.</param>
        /// <param name="errorMessage">(out) error message, null if no errors</param>
        /// <returns>True on success, False on failure</returns>
        public static bool ImportMaintainableAssetSegmentation(string dbConnectionString, string excelFilePath, string? worksheetName, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;
            ExcelHandler.Log = Log;
            ok = ExcelHandler.ImportMASFromExcelToDSS(dbConnectionString, excelFilePath, worksheetName, out errorMessage);

            return (ok);
        }

        public static bool ClearPoolWorkCandidateData(string dbConnectionString, Guid poolId, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;


            try
            {
                Log.Info($"ClearPoolWorkCandidateData [{poolId}]  - started...");
                using (SqlConnection conn = new SqlConnection(dbConnectionString))
                {
                    conn.Open();
                    conn.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "UspDeletePoolWorkData";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 12000;
                        cmd.Parameters.Add("PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
            }
            Log.Info($"ClearPoolWorkCandidateData [{poolId}]  - finished.  Ok={ok}, ErrorMessage: '{errorMessage}'");
            return ok;
        }

        /// <summary>
        /// Creates new user pool and populates with default parameters
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="poolName">Name of the pool to be created. 50 characters maximum/</param>
        /// <param name="userId">UserId of the user for the pool to be associated with. User must exist/</param>
        /// <param name="poolId">(out) Id of the created pool.</param>
        /// <param name="errorMessage">(out) error message, null if no errors.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool CreateUserPool(string connectionString, string poolName, Guid userId, out Guid? poolId, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;
            poolId = null;

            Log.Info($"CreateUserPool [{poolName}] - started...");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    conn.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"SELECT TOP 1 Id FROM TblUserPools WITH (NOLOCK)
 WHERE UPPER([Name]) = @PoolName AND UserId=@UserId";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add("@PoolName", SqlDbType.NVarChar, 50).Value = poolName;
                        cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                        object o = cmd.ExecuteScalar();
                        if (o != null && o != DBNull.Value)
                        {
                            throw new Exception($"Pool [{poolName}] of the user [{userId}] already exists.");
                        }
                    }

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "dbo.UspCreateUserPool";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PoolName", SqlDbType.NVarChar, 50).Value = poolName;
                        cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;
                        SqlParameter parmPoolId = cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier);
                        parmPoolId.Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        poolId = Guid.Parse(parmPoolId.Value.ToString());
                        Log.Info($"Newly created poolId=[{poolId}]");
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
            }

            Log.Info($"CreateUserPool [{poolName}] - finished.  Ok={ok}, ErrorMessage: '{errorMessage}'");

            return ok;
        }

        /// <summary>
        /// Returns DssGisOutput object populated from scenario data
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="scenId">Scenario Id</param>
        /// <param name="RelativeEfficiencyThreshold">Relative efficiency lower threshold</param>
        /// <param name="gis">(out) populated DssGisOutput object</param>
        /// <param name="errorMessage">(out) error message, null if no errors</param>
        /// <returns>True on syccess, False on failure</returns>
        public static bool GetGisJsonOutput(string connectionString, Guid scenId, double RelativeEfficiencyThreshold, out DssGisOutput? gis, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;
            gis = null;

             Log.Info($"GetGisJsonOutput scenId=[{scenId}] - started...");

            try
            {
                Dictionary<Guid,short> map = new Dictionary<Guid,short>();
                Dictionary<Guid,int> projMap = new Dictionary<Guid,int>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    DataSet ds = new DataSet();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "dbo.UspGetScenarioResultsForGIS";
                        cmd.CommandTimeout = 12000;
                        cmd.Parameters.Add("@ScenId", SqlDbType.UniqueIdentifier).Value = scenId;
                        cmd.Parameters.Add("@EffThreshold", SqlDbType.Float).Value = RelativeEfficiencyThreshold;
                        SqlDataAdapter a = new SqlDataAdapter(cmd);
                        a.Fill(ds);
                    }

                    // DIG 7/5/2024
                    // Attention!  Commented out lines are expected to be restored once the receieving GIS API has been adjusted.

                    gis = new DssGisOutput();
                
                    gis.Scenario.EfficiencyThreshold = RelativeEfficiencyThreshold;

                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        AssignInt(r, "ScenNo", ref gis.Scenario.ScenId);
                        AssignString(r, "ScenName", ref gis.Scenario.Name);
                        // AssignString(r, "Description", ref gis.Scenario.Description);
                        AssignString(r, "PoolName", ref gis.Scenario.PoolName);
                        AssignString(r, "User", ref gis.Scenario.User);
                        AssignDateTime(r, "LastRun", ref gis.Scenario.LastRun);
                        break;  // There should be only one record 
                    }

                    foreach (DataRow r in ds.Tables[1].Rows)
                    {
                        DssProject p = new DssProject();

                        Guid? projectId = Guid.Parse(r["ProjectId"].ToString());
                        AssignInt(r, "ProjectNo", ref p.Id);
                    
                        /*
                        AssignString(r, "PlanningPartner", ref p.Partner);
                        AssignInt(r, "District", ref p.District);
                        AssignInt(r, "Cnty", ref p.Cnty);
                        AssignString(r, "County", ref p.County);
                        AssignString(r, "Description", ref p.Description);
                    
                        AssignInt(r, "MinYear", ref p.MinYear);
                        AssignInt(r, "MaxYear", ref p.MaxYear);
                        */
                        AssignInt(r, "MinYear", ref p.Year);        // DIG 7/5/2024 For now
                        AssignInt(r, "NumPave", ref p.NumPave);
                        AssignInt(r, "NumBridge", ref p.NumBridge);
                        AssignDouble(r, "Cost", ref p.Cost);
                        /*
                        AssignDouble(r, "Benefit", ref p.Benefit);
                        AssignDouble(r, "Efficiency", ref p.Efficiency);
                        */


                        if (p.NumBridge >0)
                        {
                            if (p.NumPave > 0)
                            {
                                p.ProjectType = 3;
                            }
                            else
                            {
                                p.ProjectType = 1;
                            }
                        }
                        else
                        {
                            p.ProjectType = 2;
                        }

                        if (projectId != null && p.Id != null)
                        {
                            map.Add(projectId.Value, p.ProjectType);
                            projMap.Add(projectId.Value, p.Id.Value);
                        }

                        gis.Projects.Add(p);
                    }

                    foreach (DataRow r in ds.Tables[2].Rows)
                    {
                        DssWorkCandidate w = new DssWorkCandidate();

                        Guid? projectId = Guid.Parse(r["ProjId"].ToString());
                    
                        if (projectId != null)
                        {
                            w.ProjectType = map[projectId.Value];
                            w.ProjectId = projMap[projectId.Value];
                        }

                        AssignInt(r, "TreatNo", ref w.TreatmentId);
                        AssignString(r, "AssetType", ref w.AssetType);
                        AssignString(r, "TreatmentType", ref w.TreatmentType);
                        AssignString(r, "TreatmentName", ref w.TreatmentName);

                        AssignString(r, "Partner", ref w.Partner);
                        AssignInt(r, "District", ref w.District);
                        AssignInt(r, "Cnty", ref w.Cnty);
                        AssignString(r, "County", ref w.County);
                        AssignInt(r, "Route", ref w.Route);
                        AssignInt(r, "Direction", ref w.Direction);
                        AssignInt(r, "FromSection", ref w.FromSection);
                        AssignInt(r, "ToSection", ref w.ToSection);
                        if (w.AssetType == "B")
                        {
                            AssignString(r, "BRKEY", ref w.BRKEY);
                            AssignLong(r, "BMSID", ref w.BRIDGE_ID);
                        }
                        /*
                        if (w.AssetType == "P")
                        {
                            AssignString(r, "CRS", ref w.CRS);
                        }
                        */
                        AssignInt(r, "Year", ref w.Year);
                        AssignDouble(r, "Cost", ref w.Cost);
                        AssignDouble(r, "Benefit", ref w.Benefit);
                        AssignDouble(r, "BCRatio", ref w.BCRatio);
                        gis.WorkCandidates.Add(w);
                    }
                }
            }
            catch (Exception ex)
            {
                ok = false;
                errorMessage = ex.Message;
                Log.Error(errorMessage);
                gis = null;
            }

            Log.Info($"GetGisJsonOutput scenId=[{scenId}] - finished.  Ok={ok}, ErrorMessage: '{errorMessage}'");
            return ok;
        }


        /// <summary>
        /// Returns DssGisOutput object populated from scenario data as a JSON string
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="scenId">Scenario Id</param>
        /// <param name="RelativeEfficiencyThreshold">Relative efficiency lower threshold</param>
        /// <param name="gisJson">(out) JSON string /param>
        /// <param name="errorMessage">(out) error message, null if no errors</param>
        /// <returns>True on syccess, False on failure</returns>
        public static bool GetGisJsonStringOutput(string connectionString, Guid scenId, double RelativeEfficiencyThreshold, bool Indented, out string? gisJson, out string? errorMessage)
        {
            bool ok = true;
            errorMessage = null;
            gisJson = null;

            ok = GetGisJsonOutput(connectionString, scenId, RelativeEfficiencyThreshold,
                out DssGisOutput? gisOutput, out errorMessage);

            if (ok && gisOutput != null)
            {
                try
                {
                    gisJson = JsonConvert.SerializeObject(gisOutput, Indented ? Formatting.Indented : Formatting.None);
                }
                catch (Exception ex)
                {
                    ok = false;
                    errorMessage = ex.Message;
                    // Log.Error(errorMessage);
                    gisJson = null;
                }
            }

            return ok;
        }

        private static void AssignString(DataRow r, string colName, ref string? target)
        {
            if (r[colName] != null && r[colName] != DBNull.Value)
            {
                target = r[colName].ToString();
            }
        }

        private static void AssignDateTime(DataRow r, string colName, ref DateTime? target)
        {
            if (r[colName] != null && r[colName] != DBNull.Value)
            {
                target = Convert.ToDateTime(r[colName]);
            }
        }

        private static void AssignGuid(DataRow r, string colName, ref Guid? target)
        {
            if (r[colName] != null && r[colName] != DBNull.Value)
            {
                target = Guid.Parse(r[colName].ToString());
            }
        }

        private static void AssignInt(DataRow r, string colName, ref int? target)
        {
            if (r[colName] != null && r[colName] != DBNull.Value)
            {
                target = Convert.ToInt32(r[colName]);
            }
        }

        private static void AssignLong(DataRow r, string colName, ref long? target)
        {
            if (r[colName] != null && r[colName] != DBNull.Value)
            {
                target = Convert.ToInt64(r[colName]);
            }
        }

        private static void AssignDouble(DataRow r, string colName, ref double? target)
        {
            if (r[colName] != null && r[colName] != DBNull.Value)
            {
                target = Convert.ToDouble(r[colName]);
            }
        }

        /// <summary>
        /// Saves newky created work candidate and updates projects appropriately
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="wc">WorkCandidate object to be saved</param>
        /// <param name="workCandidateId">(out) Id of the newly created work candidate</param>
        /// <param name="projectCandidateId">(out) Id of the project to which work candidate belongs</param>
        /// <param name="errorMessage">(out) error message, null if no errors</param>
        /// <returns>True on success, false on failure</returns>
        public static bool CreateNewWorkCandidate(string connectionString, WorkCandidate wc, out Guid? workCandidateId, out Guid? projectCandidateId, out string? errorMessage)
        {
            errorMessage = null;
            bool ok = true;
            workCandidateId = null;
            projectCandidateId = null;

            Log.Info("CreateNewWorkCandidate - started...");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    conn.InfoMessage += new SqlInfoMessageEventHandler(Generic_InfoMessage);

                    if (wc.ProjectCandidateId == null)
                    {
                        if (wc.ProjectName == null)
                        {
                            throw new Exception("Provided WorkCandidate object has no ProjectName specified. (ProjectName memeber is null).");
                        }

                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = $@"SELECT Id FROM TblProjectCandidates WITH (NOLOCK) WHERE
PoolId=@PoolId AND (Description=@Name OR Description=@PName)";
                            cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = wc.PoolId;
                            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 250).Value = wc.ProjectName;
                            cmd.Parameters.Add("@PName", SqlDbType.NVarChar, 250).Value = "Project " + wc.ProjectName;
                            object o = cmd.ExecuteScalar();
                            if (o == null || o == DBNull.Value)
                            {
                                wc.ProjectCandidateId = Guid.NewGuid();
                                Log.Info($"Project '{wc.ProjectName}' does not exist. It will have to be created.  Its Id will be '{wc.ProjectCandidateId}'.");
                            }
                            else
                            {
                                wc.ProjectCandidateId = Guid.Parse(o.ToString());
                            }
                        }

                        using (BulkInserter bi = new BulkInserter())
                        {
                            ok = bi.Configure(conn, "TblWorkCandidates", "TWC", null, null, out errorMessage);
                            if (!ok)
                            {
                                throw new Exception(errorMessage);
                            }

                            DataRow r = bi.NewRow();

                            workCandidateId = wc.Id;
                            if (workCandidateId == Guid.Empty)
                            {
                                workCandidateId = Guid.NewGuid();
                            }

                            projectCandidateId = wc.ProjectCandidateId;

                            r["Id"] = workCandidateId;
                            r["ImportSessionId"] = Guid.Empty;
                            r["PoolId"] = wc.PoolId;
                            r["ImportTimeGeneratedId"] = Guid.Empty;

                            r["ProjectCandidateId"] = wc.ProjectCandidateId;
                            r["ProjectName"] = wc.ProjectName;

                            if (wc.District != null) { r["District"] = wc.District; }
                            if (wc.Cnty != null) { r["Cnty"] = wc.Cnty; }
                            if (wc.Route != null) { r["Route"] = wc.Route; }
                            if (wc.AssetType != null) { r["AssetType"] = wc.AssetType; }
                            if (wc.FromSection != null) { r["FromSection"] = wc.FromSection; }
                            if (wc.ToSection != null) { r["ToSection"] = wc.ToSection; }
                            if (wc.BRKEY != null) { r["BRKEY"] = wc.BRKEY; }
                            if (wc.BRIDGE_ID != null) { r["BRIDGE_ID"] = wc.BRIDGE_ID; }
                            if (wc.Description != null) { r["Description"] = wc.Description; }
                            if (wc.Cost != null) { r["Cost"] = wc.Cost; }
                            if (wc.HSIP_Eligible != null) { r["HSIP_Eligible"] = wc.HSIP_Eligible; }
                            if (wc.Excess_Safety_Value != null) { r["Excess_Safety_Value"] = wc.Excess_Safety_Value; }
                            if (wc.Supported_by_RSA_Safety_Study != null) { r["Supported_by_RSA_Safety_Study"] = wc.Supported_by_RSA_Safety_Study; }
                            if (wc.Degree_of_Safety_Improvement != null) { r["Degree_of_Safety_Improvement"] = wc.Degree_of_Safety_Improvement; }
                            if (wc.Supports_Hazard_Mitigation != null) { r["Supports_Hazard_Mitigation"] = wc.Supports_Hazard_Mitigation; }
                            if (wc.Improves_Access_to_Emergency_Services != null) { r["Improves_Access_to_Emergency_Services"] = wc.Improves_Access_to_Emergency_Services; }
                            if (wc.Travel_Time_Savings != null) { r["Travel_Time_Savings"] = wc.Travel_Time_Savings; }
                            if (wc.Operating_Cost_Savings != null) { r["Operating_Cost_Savings"] = wc.Operating_Cost_Savings; }
                            if (wc.Degree_of_Mobility_Improvement != null) { r["Degree_of_Mobility_Improvement"] = wc.Degree_of_Mobility_Improvement; }
                            if (wc.Freight_Improvement != null) { r["Freight_Improvement"] = wc.Freight_Improvement; }
                            if (wc.Facilitates_Transit_Use != null) { r["Facilitates_Transit_Use"] = wc.Facilitates_Transit_Use; }
                            if (wc.Facilitates_Active_Transportation != null) { r["Facilitates_Active_Transportation"] = wc.Facilitates_Active_Transportation; }
                            if (wc.Bike_Lanes_Constructed != null) { r["Bike_Lanes_Constructed"] = wc.Bike_Lanes_Constructed; }
                            if (wc.New_Cyclists != null) { r["New_Cyclists"] = wc.New_Cyclists; }
                            if (wc.Support_for_Job_Growth != null) { r["Support_for_Job_Growth"] = wc.Support_for_Job_Growth; }
                            if (wc.Support_for_Tourism != null) { r["Support_for_Tourism"] = wc.Support_for_Tourism; }
                            if (wc.Support_for_Economic_Goals != null) { r["Support_for_Economic_Goals"] = wc.Support_for_Economic_Goals; }
                            if (wc.Support_for_Recreation != null) { r["Support_for_Recreation"] = wc.Support_for_Recreation; }
                            if (wc.Consistency_with_Existing_Plans != null) { r["Consistency_with_Existing_Plans"] = wc.Consistency_with_Existing_Plans; }
                            if (wc.Sidewalks_and_Curb_Ramps_Constructed_or_Improved != null) { r["Sidewalks_and_Curb_Ramps_Constructed_or_Improved"] = wc.Sidewalks_and_Curb_Ramps_Constructed_or_Improved; }
                            if (wc.Change_in_Accessible_Destinations != null) { r["Change_in_Accessible_Destinations"] = wc.Change_in_Accessible_Destinations; }
                            if (wc.Percent_of_Acessibilty_Improvement_for_Disadvantaged_Populations != null) { r["Percent_of_Acessibilty_Improvement_for_Disadvantaged_Populations"] = wc.Percent_of_Acessibilty_Improvement_for_Disadvantaged_Populations; }
                            if (wc.Support_for_Environmental_Justice != null) { r["Support_for_Environmental_Justice"] = wc.Support_for_Environmental_Justice; }
                            if (wc.Predicted_Equity_and_Access_Benefit != null) { r["Predicted_Equity_and_Access_Benefit"] = wc.Predicted_Equity_and_Access_Benefit; }
                            if (wc.Equity_and_Access_Score != null) { r["Equity_and_Access_Score"] = wc.Equity_and_Access_Score; }
                            if (wc.Reduced_Flood_Closure_Risk != null) { r["Reduced_Flood_Closure_Risk"] = wc.Reduced_Flood_Closure_Risk; }
                            if (wc.Wetlands_Improved != null) { r["Wetlands_Improved"] = wc.Wetlands_Improved; }
                            if (wc.Wildlife_Crossings != null) { r["Wildlife_Crossings"] = wc.Wildlife_Crossings; }
                            if (wc.Reduced_Fuel_Consumption != null) { r["Reduced_Fuel_Consumption"] = wc.Reduced_Fuel_Consumption; }
                            if (wc.Degree_of_Environmental_Improvement != null) { r["Degree_of_Environmental_Improvement"] = wc.Degree_of_Environmental_Improvement; }
                            if (wc.Consistency_with_Land_Use_Plans != null) { r["Consistency_with_Land_Use_Plans"] = wc.Consistency_with_Land_Use_Plans; }
                            if (wc.Pavement_Rehabilitated_or_Reconstructed != null) { r["Pavement_Rehabilitated_or_Reconstructed"] = wc.Pavement_Rehabilitated_or_Reconstructed; }
                            if (wc.Bridges_Rehabilitated_or_Replaced != null) { r["Bridges_Rehabilitated_or_Replaced"] = wc.Bridges_Rehabilitated_or_Replaced; }
                            if (wc.Culverts_Rehabilitated_or_Replaced != null) { r["Culverts_Rehabilitated_or_Replaced"] = wc.Culverts_Rehabilitated_or_Replaced; }
                            if (wc.Guardrail_Rehabilitated_or_Replaced != null) { r["Guardrail_Rehabilitated_or_Replaced"] = wc.Guardrail_Rehabilitated_or_Replaced; }
                            if (wc.Geotechnical_Assets_Rehabilitated_or_Replaced != null) { r["Geotechnical_Assets_Rehabilitated_or_Replaced"] = wc.Geotechnical_Assets_Rehabilitated_or_Replaced; }
                            if (wc.Facilities_Rehabilitated_or_Reconstructed != null) { r["Facilities_Rehabilitated_or_Reconstructed"] = wc.Facilities_Rehabilitated_or_Reconstructed; }
                            if (wc.Sidewalks_Rehabilitated_or_Reconstructed != null) { r["Sidewalks_Rehabilitated_or_Reconstructed"] = wc.Sidewalks_Rehabilitated_or_Reconstructed; }
                            if (wc.ADT != null) { r["ADT"] = wc.ADT; }
                            if (wc.Percent_Trucks != null) { r["Percent_Trucks"] = wc.Percent_Trucks; }
                            if (wc.Population_Density != null) { r["Population_Density"] = wc.Population_Density; }
                            if (wc.Bike_Commute_Share != null) { r["Bike_Commute_Share"] = wc.Bike_Commute_Share; }
                            if (wc.BPN != null) { r["BPN"] = wc.BPN; }
                            if (wc.Length != null) { r["Length"] = wc.Length; }
                            if (wc.Speed_Limit != null) { r["Speed_Limit"] = wc.Speed_Limit; }
                            if (wc.Detour_Distance != null) { r["Detour_Distance"] = wc.Detour_Distance; }
                            if (wc.Functional_Classification != null) { r["Functional_Classification"] = wc.Functional_Classification; }
                            if (wc.National_Highway_System != null) { r["National_Highway_System"] = wc.National_Highway_System; }
                            if (wc.Pennsylvania_Byway != null) { r["Pennsylvania_Byway"] = wc.Pennsylvania_Byway; }
                            if (wc.Bicycle_PA_Route != null) { r["Bicycle_PA_Route"] = wc.Bicycle_PA_Route; }
                            if (wc.Interstate_Emergency_Detour != null) { r["Interstate_Emergency_Detour"] = wc.Interstate_Emergency_Detour; }
                            if (wc.LFAR_BOF_Eligible != null) { r["LFAR_BOF_Eligible"] = wc.LFAR_BOF_Eligible; }
                            if (wc.Treatment != null) { r["Treatment"] = wc.Treatment; }
                            if (wc.Year != null) { r["Year"] = wc.Year; }
                            if (wc.MinYear != null) { r["MinYear"] = wc.MinYear; }
                            if (wc.MaxYear != null) { r["MaxYear"] = wc.MaxYear; }

                            ok = bi.AddRow(r, out errorMessage);
                            if (!ok)
                            {
                                throw new Exception(errorMessage);
                            }
                        }

                        using(SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "dbo.UspUpdateWorkCandidate";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@WorkCandidateId", SqlDbType.UniqueIdentifier).Value = workCandidateId;
                            cmd.CommandTimeout = 12000;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                ok = false;
                Log.Error(errorMessage);
            }

            Log.Info($"CreateNewWorkCandidate - ended.  Ok={ok}, ErrorMessage: '{errorMessage}'");
            return ok;
        }
    }
}

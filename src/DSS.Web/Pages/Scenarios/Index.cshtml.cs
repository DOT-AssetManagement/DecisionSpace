using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using DSS.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DSS.Web.Pages.Scenarios
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICurrentUser _currentUser;
        private readonly IScenarioAppService _scenarioAppService;
        private readonly string _connectionString = string.Empty;

        public IndexModel(IScenarioAppService scenarioAppService, IConfiguration configuration, ICurrentUser currentUser = null)
        {
            _scenarioAppService = scenarioAppService;
            _connectionString = configuration.GetConnectionString("Default");
            _currentUser = currentUser;
        }

        public List<ScenarioDto> Scenarios { get; set; }
        public ScenarioDto Scenario { get; set; }
        public string SearchString { get; set; }
        public async Task OnGet(string searchString = "")
        {
            SearchString = searchString;
            Scenarios = await _scenarioAppService.GetAllAsync(searchString);
        }

        public async Task<IActionResult> OnGetRunScenario(Guid id)
        {
            var response = await _scenarioAppService.RunScenario(id);
            HttpContext.Session.SetString("SessionMsg", "Scenario ran successfully. You can check Charts and Reports now");
            return RedirectToPage("/Pools/Index");
        }

        public async Task<IActionResult> OnGetProjects(Guid? poolId, double? effThreshold)
        {
            string storedProcedure = "dbo.UspGetScenarioOutput";
            DataTable dataTable = new DataTable();
            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand();

                    command.Connection = connection;
                    command.CommandText = storedProcedure;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 24000;
                    if (poolId != null)
                    {
                        command.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                    }
                    if (effThreshold != null)
                    {
                        command.Parameters.Add("@RelativeEfficiencyThreshold", SqlDbType.Float).Value = effThreshold;
                    }

                    using (var dataAdapter = new SqlDataAdapter(command))
                    {
                        dataAdapter.Fill(dataTable);
                    }
                }
            }

            string jsonResult = JsonConvert.SerializeObject(dataTable);

            return new JsonResult(jsonResult);
        }
        public async Task<IActionResult> OnPostExport(Guid? poolId, double? effThreshold)
        {
            string xlsxFilePath = Path.Combine(Path.GetTempPath(), $"Export_{Guid.NewGuid()}.xlsx");

            try
            {
                byte result = _scenarioAppService.ExportScenariosData(poolId, effThreshold, xlsxFilePath);

                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, "Export failed.");

                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(xlsxFilePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Export.xlsx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                if (System.IO.File.Exists(xlsxFilePath))
                {
                    System.IO.File.Delete(xlsxFilePath);
                }
            }
        }
    }
}

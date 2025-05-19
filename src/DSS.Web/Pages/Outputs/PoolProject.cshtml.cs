using DSS.Pool;
using DSS.Pool.Dtos;
using DSS.ProjectCandidates;
using DSS.ProjectCandidates.Dtos;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.IdentityModel.Tokens;


namespace DSS.Web.Pages.Outputs
{
    [Authorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class PoolProjectModel : PageModel
    {
        private readonly IProjectCandidateAppService _projectCandidateAppService;
        private readonly IPoolAppService _poolAppService;

        public List<ProjectCandidateDto> ProjectCandidates { get; set; }
        public List<PoolDto> Pools { get; set; }
        public List<ScenarioDto> Scenarios { get; set; }
        public Guid? ScenarioId { get; set; }
        public Guid? PoolId { get; set; }
        public string SearchString { get; set; }
        public string ScenarioName { get; set; }
        public string PoolName { get; set; }
        public List<byte?> Districts { get; set; }
        public List<byte?> Counties { get; set; }
        public List<int?> YearEarliests { get; set; }
        public List<int?> YearLatests { get; set; }

        public PoolProjectModel(
            IProjectCandidateAppService projectCandidateAppService,
            IPoolAppService poolAppService,
            IScenarioAppService scenarioAppService)
        {
            _projectCandidateAppService = projectCandidateAppService;
            _poolAppService = poolAppService;
        }

        public async Task<IActionResult> OnGetAsync(Guid? poolId, byte? district, byte? county, string searchString = "", int? minYear = null, int? maxYear = null)
        {
            try
            {
                if (poolId.HasValue)
                {

                    ProjectCandidates = await _projectCandidateAppService.GetAllProjectsAsync(poolId.Value);
                    var pool = await _poolAppService.GetAsync(poolId.Value);
                    PoolId = poolId.Value;
                    PoolName = pool?.Name;
                   await OnGetDropDownValue(poolId.Value, district, county, minYear, maxYear);
                }
                else
                {
                    ProjectCandidates = await _projectCandidateAppService.GetAllAsync();
                    Pools = await _poolAppService.GetAllAsync(searchString, poolId?.ToString());
                    PoolId = ProjectCandidates.FirstOrDefault()?.PoolId;
                   await OnGetDropDownValue(poolId.Value, district, county, minYear, maxYear);
                }

                await ApplyFilter(county, district, maxYear, minYear);

                return Page();
            }
            catch (Exception ex)
            {
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnGetDropDownValue(Guid poolId, byte? district, byte? county, int? minYear, int? maxYear)
        {
            ProjectCandidates = await _projectCandidateAppService.GetAllProjectsAsync(poolId);
            Districts = ProjectCandidates.Select(pc => pc.District).Distinct().OrderBy(d => d).ToList();
            if (district != null)
            {
                var counties = ProjectCandidates.Where(pc => pc.District == district && pc.Cnty.HasValue).Select(pc => (byte?)pc.Cnty.Value).Distinct().OrderBy(c => c).ToList();
                Counties = counties;
            }
            if (county != null)
            {
                var yearEarliests = ProjectCandidates.Where(pc => pc.Cnty == county && pc.YearEarliest.HasValue).Select(pc => (int?)pc.YearEarliest.Value).Distinct().OrderBy(year => year).ToList();
                YearEarliests = yearEarliests;
            }
            if (minYear != null)
            {
                var yearLatest = ProjectCandidates.Where(pc => pc.YearEarliest == minYear).Select(pc => pc.YearLatest).Distinct().OrderBy(c => c).ToList();
                YearLatests = yearLatest;
            }
            return new JsonResult(new

            { Districts, Counties, YearEarliests, YearLatests });
        }
        private async Task ApplyFilter(byte? county, byte? district, int? yearLatest, int? yearEarliest)
        {
            if (district != null)
            {
                ProjectCandidates = ProjectCandidates.Where(pc => pc.District == district).ToList();
            }
            if (county != null)
            {
                ProjectCandidates = ProjectCandidates.Where(pc => pc.Cnty == county).ToList();
            }
            if (yearEarliest != null)
            {
                ProjectCandidates = ProjectCandidates.Where(pc => pc.YearEarliest == yearEarliest).ToList();
            }
            if (yearLatest != null)
            {
                ProjectCandidates = ProjectCandidates.Where(pc => pc.YearLatest == yearLatest).ToList();
            }
        }

        public async Task<IActionResult> OnPostExport(Guid? poolId)
        {
            if (!poolId.HasValue)
            {
                return BadRequest("PoolId parameter is required.");
            }

            List<ProjectCandidateDto> ProjectCandidates = await _projectCandidateAppService.GetAllProjectsAsync(poolId.Value);

            if (ProjectCandidates == null || !ProjectCandidates.Any())
            {
                return NotFound("No data found for the specified poolId.");
            }

            var dataForExport = ProjectCandidates.Select(pc => new
            {
                PoolId = pc.PoolId,
                District = pc.District,
                Cnty = pc.Cnty,
                Description = pc.Description,
                NumberOfWorkCandidates = pc.NumberOfWorkCandidates,
                Treatment = pc.Treatment,
                YearEarliest = pc.YearEarliest,
                YearLatest = pc.YearLatest,
                TotalCost = pc.TotalCost,
                TotalScaledBenefit = pc.TotalScaledBenefit,
                SafetyScore = pc.SafetyScore,
                MobilityAndEconomyScore = pc.MobilityAndEconomyScore,
                EquityAndAccessScore = pc.EquityAndAccessScore,
                ResilienceAndEnvironmentScore = pc.ResilienceAndEnvironmentScore,
                ConditionAndPerformanceScore = pc.ConditionAndPerformanceScore,
                TotalScore = pc.TotalScore,
                RelativeEfficiency = pc.RelativeEfficiency,
                RelativeEfficiencySetAt = pc.RelativeEfficiencySetAt?.ToString("yyyy-MM-dd")
            }).ToList();

            string fileName = $"Export_PoolProject_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            try
            {
                using (var excelPackage = new ExcelPackage())
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("PoolProjectData");

                    var headers = typeof(ProjectCandidateDto).GetProperties().Select(p => p.Name).ToList();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                    }

                    for (int i = 0; i < dataForExport.Count; i++)
                    {
                        var item = dataForExport[i];
                        for (int j = 0; j < headers.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1].Value = item.GetType().GetProperty(headers[j])?.GetValue(item);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await excelPackage.SaveAsAsync(fileStream);
                    }
                }

                byte[] excelBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting PoolProject data: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to export data.");
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }


    }
}

using DSS.Pool;
using DSS.Pool.Dtos;
using DSS.Pools;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace DSS.Web.Pages.Pools
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IPoolAppService _poolAppService;
        private readonly IScenarioAppService _scenarioAppService;
        private readonly ICurrentUser _currentUser;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public List<PoolDto> Pools { get; set; }
        public IndexModel(IPoolAppService poolAppService, ICurrentUser currentUser, IWebHostEnvironment hostingEnvironment, IScenarioAppService scenarioAppService)
        {
            _poolAppService = poolAppService;
            _scenarioAppService = scenarioAppService;
            _currentUser = currentUser;
            _hostingEnvironment = hostingEnvironment;
        }

        public string SearchString { get; set; }
        public string PoolId { get; set; }
        public string Import { get; set; }
        public string PoolName { get; set; }
        public string SessionMsg { get; set; }
        public PoolDto PoolData { get; set; }
        public ScenarioDto Scenario { get; set; }

        public ScenarioCreateDto ScenarioCreateDto { get; set; }

        public ScenarioUpdateDto ScenarioUpdateDto { get; set; }
        public async Task OnGet(String searchString = "", String poolId = "", String import = "false")
        {
            SessionMsg = HttpContext.Session.GetString("SessionMsg");
            HttpContext.Session.Remove("SessionMsg");


            SearchString = searchString;
            Pools = await _poolAppService.GetAllAsync(searchString, poolId);

            foreach (var pool in Pools)
            {
                Scenario = await _scenarioAppService.GetScenarioByPoolId(pool.Id);
                if (Scenario != null)
                {
                    pool.ScenarioId = Scenario.Id;
                    pool.ScenarioName = Scenario.ScenName;
                }
            }


            PoolId = poolId;
            Import = import;
            PoolName = "";
            if (poolId != "")
            {
                Guid poolIdNew = Guid.Parse(poolId);
                PoolData = await _poolAppService.GetAsync(poolIdNew);
                PoolName = PoolData.Name;
            }

        }

        [BindProperty]
        public PoolDto Pool { get; set; }
        public async Task<IActionResult> OnGetGetPool(Guid id)
        {
            Pool = await _poolAppService.GetAsync(id);
            return new JsonResult(Pool);
        }

        public async Task<IActionResult> OnPostCreateAsync(PoolCreateDto newPool)
        {
            if (ModelState.IsValid)
            {

                Pool = await _poolAppService.CheckName(newPool.Name);
                if (Pool != null)
                {                   
                    var result = new { success = false, message = "Candidate Pool name exists. Please try with new name." };
                    return new JsonResult(result);

                }
                else
                {
                    Guid? poolId = await _poolAppService.Create(newPool);

                    if (ScenarioCreateDto == null)
                    {
                        ScenarioCreateDto = new ScenarioCreateDto();
                    }

                    ScenarioCreateDto.ScenName = newPool.Name;
                    ScenarioCreateDto.Description = newPool.Description;
                    ScenarioCreateDto.PoolId = (Guid)poolId;
                    await _scenarioAppService.Create(ScenarioCreateDto);

                    HttpContext.Session.SetString("SessionMsg", "Candidate pool created. You can import your work candidates now.");

                    var result = new { success = true };
                    return new JsonResult(result);
                }

                
            }
            else
            {
                var result = new { success = true };
                return new JsonResult(result);
            }
        }

        public async Task<IActionResult> OnGetClearPool(Guid id)
        {
            await _poolAppService.ClearAsync(id);
            var result = new { success = true };
            return new JsonResult(result);
        }



        public async Task<IActionResult> OnPostUpdatePoolAsync(PoolUpdateDto input)
        {
            Pool = await _poolAppService.CheckName(input.Name);
            if(Pool == null)
            {
                await _poolAppService.UpdateAsync(input.Id, input);

                ScenarioUpdateDto = new ScenarioUpdateDto();
                ScenarioUpdateDto.ScenName = input.Name;
                ScenarioUpdateDto.Description = input.Description;

                Scenario = await _scenarioAppService.GetScenarioByPoolId(input.Id);
                if (Scenario != null)
                {
                    ScenarioUpdateDto.Id = Scenario.Id;
                    ScenarioUpdateDto.PoolId = input.Id;
                    await _scenarioAppService.UpdateAsync(Scenario.Id, ScenarioUpdateDto);
                }

                HttpContext.Session.SetString("SessionMsg", "Candidate pool updated.");

                return new JsonResult(new { success = true });
            }
            else if(Pool.Id == input.Id)
            {
                await _poolAppService.UpdateAsync(input.Id, input);

                ScenarioUpdateDto = new ScenarioUpdateDto();
                ScenarioUpdateDto.ScenName = input.Name;
                ScenarioUpdateDto.Description = input.Description;

                Scenario = await _scenarioAppService.GetScenarioByPoolId(input.Id);
                if (Scenario != null)
                {
                    ScenarioUpdateDto.Id = Scenario.Id;
                    ScenarioUpdateDto.PoolId = input.Id;
                    await _scenarioAppService.UpdateAsync(Scenario.Id, ScenarioUpdateDto);
                }

                HttpContext.Session.SetString("SessionMsg", "Candidate pool updated.");

                return new JsonResult(new { success = true });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Candidate Pool name exists. Please try with new name." });
            }

        }



        public async Task<IActionResult> OnPostImportWorkCandidatesAsync(IFormFile? excelfile, Guid PoolId, string tabname)
        {
            if (excelfile == null || excelfile.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            string filePath = await SaveUploadedFile(excelfile);

            if (string.IsNullOrEmpty(filePath))
            {
                return StatusCode(500, new { success = false, message = "File upload failed" });
            }

            
            _poolAppService.ImportWorkCandidates(filePath, "Northwest RPO", tabname, PoolId);

            HttpContext.Session.SetString("SessionMsg", "Work Candidates are imported. You can run the candidate pool now.");

            return RedirectToPage("/Pools/Index");
        }

        private async Task<string> SaveUploadedFile(IFormFile file)
        {
            try
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }


        public async Task<IActionResult> OnPostExportAsync(Guid? poolId, double? effThreshold)
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



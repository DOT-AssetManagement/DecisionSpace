using DSS.PoolScoreParameters;
using DSS.PoolScoreParameters.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Users;


namespace DSS.Web.Pages.Pools
{
    [Authorize]
    [IgnoreAntiforgeryToken(Order = 1001)]
    public class PoolScoreParametersModel : PageModel
    {
        private readonly IPoolScoreParametersAppService _poolScoreParametersAppService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICurrentUser _currentUser;

        public List<PoolScoreParametersDto> PoolScoreParameters { get; set; }
        public Guid PoolId { get; set; }
        public string? CreatedBy { get; set; }
        public string Measure { get; set; }
        public string Import { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public PoolScoreParametersModel(IPoolScoreParametersAppService poolScoreParametersAppService, IWebHostEnvironment hostingEnvironment, ICurrentUser currentUser)
        {
            _poolScoreParametersAppService = poolScoreParametersAppService;
            _hostingEnvironment = hostingEnvironment;
            _currentUser = currentUser;
        }
        public async Task OnGet(Guid poolId, string import = "false")
        {
            PoolScoreParameters = await _poolScoreParametersAppService.GetAllByPoolIdAsync(poolId);
            PoolId = poolId;
            Import = import;
        }

        public async Task<IActionResult> OnPostUpdatePoolScoreParameterAsync(PoolScoreParametersUpdateDto input)
        {

            await _poolScoreParametersAppService.UpdatePoolScoreParameterAsync(input);
            return RedirectToPage("/Pools/PoolScoreParameters", new { poolId = input.PoolId });
        }

        public async Task<IActionResult> OnPostCreatePoolScoreParameter(PoolScoreParametersCreateDto input)
        {
            input.CreatedBy = _currentUser.UserName;
            await _poolScoreParametersAppService.CreateAsync(input);
            return RedirectToPage("/Pools/PoolScoreParameters", new { poolId = input.PoolId });
        }
        public async Task<IActionResult> OnPostCheckMeasureExists(PoolScoreParametersCreateDto input)
        {
            var existingMeasure = await _poolScoreParametersAppService.GetAllByPoolIdAndMeasureAsync(input.PoolId, input.Measure);
            if (existingMeasure != null)
            {
                return new JsonResult(new { exists = existingMeasure != null });
            }
            else
            {
                return new JsonResult(new { exists = false });
            }
        }

        public async Task<IActionResult> OnPostImportPoolScoreCandidatesAsync(IFormFile? excelfile, Guid PoolId, string tabname, bool deleteAll, bool keepAll, bool keepUserCreated)
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

            _poolScoreParametersAppService.ImportPoolScoreParameter(filePath, tabname, PoolId, !keepAll, keepUserCreated);


            return RedirectToPage("/Pools/PoolScoreParameters", new { poolId = PoolId });
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

    }
}

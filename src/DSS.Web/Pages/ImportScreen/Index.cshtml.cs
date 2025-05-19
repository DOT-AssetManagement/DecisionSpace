using DSS.Pool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace DSS.Web.Pages.ImportScreen
{
    public class IndexModel : PageModel
    {
        private readonly IPoolAppService _poolAppService;
        public IndexModel(IPoolAppService poolAppService)
        {
            _poolAppService = poolAppService;
        }
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostImportBridgePavmentsAsync(IFormFile? excelfile)
        {
            if (excelfile == null || excelfile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var filePath = Path.GetTempFileName();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await excelfile.CopyToAsync(stream);
                }

                bool result =  _poolAppService.ImportBridgeToPavements(filePath);

                if (!result)
                {
                    return StatusCode(500, "Import failed.");
                }

                return default;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<IActionResult> OnPostImportAssetSegmentationAsync(IFormFile? excelfile)
        {
            if (excelfile == null || excelfile.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var filePath = Path.GetTempFileName();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await excelfile.CopyToAsync(stream);
                }

                string fileName = Path.GetFileNameWithoutExtension(excelfile.FileName);
                string worksheetName = fileName;

                bool result = await _poolAppService.ImportAssetSegmentationAsync(filePath, worksheetName);

                if (!result)
                {
                    return StatusCode(500, "Import failed.");
                }

                return default;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



    }
}

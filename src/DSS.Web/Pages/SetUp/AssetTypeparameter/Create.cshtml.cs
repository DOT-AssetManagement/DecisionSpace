using DSS.AssetTypeParametersData;
using DSS.AssetTypeParametersData.Dtos;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages.SetUp
{
    public class CreateModel : PageModel
    {
        private readonly IAssetTypeParameterDataAppService _assetTypeParameterDataAppService;
        private readonly IScenarioAppService _scenarioAppService;

        public List<ScenarioDto> Scenarios { get; set; }
        public string SearchString { get; set; }

        public CreateModel(IAssetTypeParameterDataAppService assetTypeParameterDataAppService, IScenarioAppService scenarioAppService)
        {
            _assetTypeParameterDataAppService = assetTypeParameterDataAppService;
            _scenarioAppService = scenarioAppService;
        }

        public async Task OnGet(string searchString = "")
        {
            Scenarios = await _scenarioAppService.GetAllAsync(searchString);
        }

        public async Task<IActionResult> OnPostCreateAsset(AssetTypeParameterDataCreateDto asset, string selectedScenarioId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Guid.TryParse(selectedScenarioId, out var scenarioGuid))
            {
                return BadRequest("Invalid ScenarioId");
            }

            asset.ScenId = scenarioGuid;

            await _assetTypeParameterDataAppService.CreateAsync(asset);

            return RedirectToPage("./List");
        }

    }
}

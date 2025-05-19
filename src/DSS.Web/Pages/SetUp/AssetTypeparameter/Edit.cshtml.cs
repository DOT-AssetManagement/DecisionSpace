using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSS.AssetTypeParametersData;
using DSS.AssetTypeParametersData.Dtos;
using DSS.Scenarios;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DSS.Web.Pages.SetUp.AssetTypeparameter
{
    public class EditModel : PageModel
    {
        private readonly IAssetTypeParameterDataAppService _assetTypeParameterDataAppService;
        private readonly IScenarioAppService _scenarioAppService;

        public List<ScenarioDto> Scenarios { get; set; }
        public AssetTypeParameterDataDto AssetEdit { get; set; }
        public string SearchString { get; set; }

        public EditModel(IAssetTypeParameterDataAppService assetTypeParameterDataAppService, IScenarioAppService scenarioAppService)
        {
            _assetTypeParameterDataAppService = assetTypeParameterDataAppService;
            _scenarioAppService = scenarioAppService;
        }

        public async Task OnGetAsync(Guid id,string  searchString = "")
        {
            AssetEdit = await _assetTypeParameterDataAppService.GetAsync(id);
            Scenarios = await _scenarioAppService.GetAllAsync( searchString);
        }
        public async Task<IActionResult> OnPostAsync(Guid id, AssetTypeParameterDataUpdateDto asset)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var updatedAsset = await _assetTypeParameterDataAppService.UpdateAsync(id, asset);

            if (updatedAsset != null)
            {
                return RedirectToPage("./List");
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }
        public async Task DelteAsset(Guid id)
        {
            await _assetTypeParameterDataAppService.DeleteAsync(id);

        }
    }
}

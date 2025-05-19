using DSS.AssetTypeParameters;
using DSS.AssetTypeParametersData;
using DSS.AssetTypeParametersData.Dtos;
using DSS.Scenarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSS.Web.Pages.SetUp.AssetTypeparameter
{
    public class ListModel : PageModel
    {
        private readonly IAssetTypeParameterDataAppService _assetTypeParameterDataAppService;
        private readonly IScenarioAppService _scenarioAppService;

        public ListModel(IAssetTypeParameterDataAppService assetTypeParameterDataAppService, IScenarioAppService scenarioAppService)
        {
            _assetTypeParameterDataAppService = assetTypeParameterDataAppService;
            _scenarioAppService = scenarioAppService;
        }
        public class AssetTypeParameterViewModel
        {
            public AssetTypeParameterDataDto AssetTypeParameter { get; set; }
            public string ScenarioName { get; set; }
        }

        public List<AssetTypeParameterViewModel> AssetTypeParameterViewModels { get; set; }
        public List<AssetTypeParameterDataDto> AssetTypeParameterDataDtos { get; set; }

        public async Task OnGet()
        {
            AssetTypeParameterDataDtos = await _assetTypeParameterDataAppService.GetAllAsync();
            AssetTypeParameterViewModels = new List<AssetTypeParameterViewModel>();

            foreach (var dto in AssetTypeParameterDataDtos)
            {
                var assetTypeParameterViewModel = new AssetTypeParameterViewModel
                {
                    AssetTypeParameter = dto
                };

                if (dto.ScenId.HasValue)
                {
                    assetTypeParameterViewModel.ScenarioName = (await _scenarioAppService.GetAsync(dto.ScenId.Value))?.ScenName;
                }

                AssetTypeParameterViewModels.Add(assetTypeParameterViewModel);
            }
        }

        public async Task<IActionResult> OnPostDeleteAsset(Guid id)
        {
            await _assetTypeParameterDataAppService.DeleteAsync(id);
            return RedirectToPage("/SetUp/AssetTypeparameter/List");
        }
    }
}


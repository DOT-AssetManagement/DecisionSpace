using DSS.AssetTypeParametersData.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.AssetTypeParametersData
{
    public interface IAssetTypeParameterDataAppService: ICrudAppService<AssetTypeParameterDataDto, Guid, AssetTypeParameterDataInput, AssetTypeParameterDataCreateDto, AssetTypeParameterDataUpdateDto>
    {
        Task<List<AssetTypeParameterDataDto>> GetAllAsync();
    }
}

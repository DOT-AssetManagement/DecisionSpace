using DSS.AssetTypeParametersData;
using DSS.AssetTypeParametersData.Dtos;
using DSS.Entities;
using DSS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DSS.AssetTypeParameters
{
    public class AssetTypeParameterDataAppService : ApplicationService, IAssetTypeParameterDataAppService
    {
        private readonly IAssetTypeParameterRepository _assetTypeParameterRepository;
        public AssetTypeParameterDataAppService(IAssetTypeParameterRepository assetTypeParameterRepository)
        {
            _assetTypeParameterRepository = assetTypeParameterRepository;
        }

         public async Task<AssetTypeParameterDataDto> CreateAsync(AssetTypeParameterDataCreateDto input)
        {
            var asset = ObjectMapper.Map<AssetTypeParameterDataCreateDto,Entities.AssetTypeParametersData > (input);
            var response = await _assetTypeParameterRepository.InsertAsync(asset);
            return ObjectMapper.Map<Entities.AssetTypeParametersData, AssetTypeParameterDataDto>(response);
        }

        public async Task DeleteAsync(Guid id)
        {
             _assetTypeParameterRepository.DeleteAsync(id);
        }

        public async Task<List<AssetTypeParameterDataDto>> GetAllAsync()
        {
            try
            {
                var goals = await _assetTypeParameterRepository.GetListAsync();
                return ObjectMapper.Map<List<Entities.AssetTypeParametersData>, List<AssetTypeParameterDataDto>>(goals);

            }
            catch(Exception ex)
            {
                throw;
            }
           
        }

        public async Task<AssetTypeParameterDataDto> GetAsync(Guid id)
        {
            var asset = await _assetTypeParameterRepository.GetAsync(id);
            return ObjectMapper.Map<Entities.AssetTypeParametersData, AssetTypeParameterDataDto>(asset);
        }

        public Task<PagedResultDto<AssetTypeParameterDataDto>> GetListAsync(AssetTypeParameterDataInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<AssetTypeParameterDataDto> UpdateAsync(Guid id, AssetTypeParameterDataUpdateDto input)
        {
            var asset = await _assetTypeParameterRepository.GetAsync(id);
            var updatedGoal = ObjectMapper.Map(input, asset);
            var response = await _assetTypeParameterRepository.UpdateAsync(updatedGoal);
            return ObjectMapper.Map<Entities.AssetTypeParametersData, AssetTypeParameterDataDto>(response);
        }
    }
}

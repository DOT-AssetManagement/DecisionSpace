using System;
using System.Collections.Generic;
using DSS.PoolScoreParameters.Dtos;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.PoolScoreParameters
{
    public interface IPoolScoreParametersAppService : ICrudAppService<PoolScoreParametersDto, Guid, GetPoolScoreParametersInput, PoolScoreParametersCreateDto, PoolScoreParametersUpdateDto>
    {
        Task<List<PoolScoreParametersDto>> GetAllAsync();
        Task<bool> InsertScoreParametersForPool(Guid poolId);
        Task<List<PoolScoreParametersDto>> GetAllByPoolIdAsync(Guid id);

        Task<PoolScoreParametersDto> GetAllByPoolIdAndMeasureAsync(Guid id, string measure);

        Task<PoolScoreParametersDto> UpdatePoolScoreParameterAsync(PoolScoreParametersUpdateDto input);
        bool ImportPoolScoreParameter(string excelFilePath, string? worksheetName, Guid poolId, bool fromScratch, bool keepUserCreated);
    }
}

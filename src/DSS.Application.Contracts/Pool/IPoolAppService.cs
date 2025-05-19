using DSS.Pool.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.Pool
{
    public interface IPoolAppService : ICrudAppService<PoolDto, Guid, GetPoolInput, PoolCreateDto, PoolUpdateDto>
    {
        Task<PoolDto> CheckName(string poolName);
        Task<List<PoolDto>> GetAllAsync(string name, String poolId);
        Task<Guid?> Create(PoolCreateDto input);
        Task ClearAsync(Guid id);
        bool ImportBridgeToPavements(string excelFilePath);
        Task<bool> ImportAssetSegmentationAsync(string excelFilePath, string worksheetName);

        bool ImportWorkCandidates(string excelFilePath, string? importSourceName, string? worksheetName, Guid poolId, bool forgiving = false);
    }
}

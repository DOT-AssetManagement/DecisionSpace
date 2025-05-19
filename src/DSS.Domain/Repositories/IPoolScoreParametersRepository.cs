using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;


public interface IPoolScoreParametersRepository : IBasicRepository<PoolScoreParameter>
{
   
    bool CreatePoolScoreParameters(Guid poolId);
    Task<List<PoolScoreParameter>> GetListByPoolId(Guid id);

    Task<PoolScoreParameter> GetByPoolIdAndMeasure(Guid id, string measure);
    Task<bool> UpdateByPoolIdAndMeasureAsync(Guid PoolId, PoolScoreParameter input);
    bool Import(Guid poolId, string excelFilePath, string? worksheetName, bool fromScratch, bool keepUserCreated);
}


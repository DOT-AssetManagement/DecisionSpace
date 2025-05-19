using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

public interface IPoolRepository : IBasicRepository<CandidatePool, Guid>
{
    Task<CandidatePool> CheckName(string poolName);
    Guid? CreatePool(CandidatePool pool);
    bool ClearPool(Guid poolId);
    List<CandidatePool> GetPoolsByUserId(String name, String poolId);
    bool Import(string excelFilePath, string? importSourceName, string? worksheetName, Guid poolId, bool forgiving);

    bool ImportBridgeToPavements(string excelFilePath);
    bool ImportAssetSegmentation( string excelFilePath, string? worksheetName);

}

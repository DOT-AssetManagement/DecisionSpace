using DSS.Entities;
using DSS.EntityFrameworkCore;
using DSS.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Users;

public class PoolRepository : EfCoreRepository<DSSDbContext, CandidatePool, Guid>, IPoolRepository
{
    private readonly string _connectionString = string.Empty;
    private readonly ICurrentUser _curruntUser;
    private readonly DSSDbContext _dbContext;
    public PoolRepository(IDbContextProvider<DSSDbContext> dbContextProvider, IConfiguration configuration, ICurrentUser curruntUser, DSSDbContext dbContext)
        : base(dbContextProvider)
    {
        _connectionString = configuration.GetConnectionString("Default");
        _curruntUser = curruntUser;
        _dbContext = dbContext;
    }
    public async Task<CandidatePool> CheckName(string poolName)
    {
        //var result = await _dbContext.TblUserPools.AsNoTracking().AnyAsync(x => x.Name.ToLower() == poolName.ToLower());

        var result = await _dbContext.TblUserPools.AsNoTracking()
        .FirstOrDefaultAsync(x => x.Name.ToLower() == poolName.ToLower());
        return result;
    }

    public bool ClearPool(Guid poolId)
    {
        string errors;
        var result = DataManager.ClearPoolWorkCandidateData(_connectionString, poolId, out errors);

        if (!result)
            throw new Exception(errors);
        return true;
    }

    public Guid? CreatePool(CandidatePool pool)
    {
        string errors;
        Guid? poolId;
        var result = DataManager.CreateUserPool(_connectionString, pool.Name, pool.UserId, out poolId, out errors);

        if (!result)
            throw new Exception(errors);

        return poolId;
    }

    public List<CandidatePool> GetPoolsByUserId(string name, string poolId)
    {
        var userId = _curruntUser.Id.Value;
        //var pools = _dbContext.TblUserPools.Where(pool => pool.UserId == userId && (name == "" || pool.Name == name)).ToList();

        var pools = _dbContext.TblUserPools
        .Where(pool => pool.UserId == userId
                       && (string.IsNullOrEmpty(name) || pool.Name == name)
                       && (string.IsNullOrEmpty(poolId) || pool.Id == Guid.Parse(poolId)))
        .OrderByDescending(pool => pool.CreatedAt)
        .ToList();
        return pools;
    }

    public bool Import(string excelFilePath, string? importSourceName, string? worksheetName, Guid poolId, bool forgiving)
    {
        string errors;
        Guid? session;

        var result = DataManager.ImportDSSWorkCandidatesFromExcel(_connectionString, excelFilePath, importSourceName, worksheetName, poolId, forgiving, out session, out errors);

        if (!result)
            throw new Exception(errors);

        return true;
    }

    public bool ImportBridgeToPavements(string excelFilePath)
    {
        string? errorMessage;
        bool result;

        result = DataManager.ImportBridgeToPavementFromExcelToDSS(_connectionString, excelFilePath, out errorMessage);

        if (!result)
        {
            throw new Exception(errorMessage ?? "An error occurred during the import process.");
        }

        return true;
    }


    public bool ImportAssetSegmentation( string excelFilePath, string? worksheetName)
    {
        string? errorMessage;
        bool result;

        result = DataManager.ImportMaintainableAssetSegmentation(_connectionString, excelFilePath, worksheetName, out errorMessage);

        if (!result)
        {
            throw new Exception(errorMessage ?? "An error occurred during the import process.");
        }

        return true;
    }
}
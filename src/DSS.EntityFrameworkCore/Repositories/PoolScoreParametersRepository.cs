using DSS.Entities;
using DSS.EntityFrameworkCore;
using DSS.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

public class PoolScoreParametersRepository : EfCoreRepository<DSSDbContext, PoolScoreParameter>, IPoolScoreParametersRepository
{
    private readonly string _connectionString = string.Empty;
    private readonly DSSDbContext _dbContext;
    public PoolScoreParametersRepository(IDbContextProvider<DSSDbContext> dbContextProvider, DSSDbContext dbContext, IConfiguration configuration)
        : base(dbContextProvider)
    {
        _dbContext = dbContext;
        _connectionString = configuration.GetConnectionString("Default");
    }
    public async Task<List<PoolScoreParameter>> GetListByPoolId(Guid id)
    {
        var results = await _dbContext.PoolScoreParameters
         .Where(x => x.PoolId == id)
         .ToListAsync();

        return results;
    }

    public bool CreatePoolScoreParameters(Guid poolId)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "dbo.CreateTblPoolScoreParameters";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@PoolId", SqlDbType.UniqueIdentifier).Value = poolId;
                SqlParameter RowCount = cmd.Parameters.Add("@RowCount", SqlDbType.Int);
                RowCount.Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                int totalRows = int.Parse(RowCount.Value.ToString());
                if (totalRows == 0)
                {
                    Console.WriteLine("No Record Inserted");
                }
                else
                {
                    Console.WriteLine($"{totalRows} Records Inserted");
                }
            }
        }
        return true;
    }

    public async Task<PoolScoreParameter> GetByPoolIdAndMeasure(Guid id, string measure)
    {
        return await _dbContext.PoolScoreParameters.AsNoTracking()
         .FirstOrDefaultAsync(x => x.PoolId == id && x.Measure.ToLower() == measure.ToLower());
    }


    public async Task<bool> UpdateByPoolIdAndMeasureAsync(Guid PoolId, PoolScoreParameter input)
    {

        var existingRecord = await _dbContext.PoolScoreParameters.AsNoTracking()
        .FirstOrDefaultAsync(x => x.PoolId == PoolId && x.Measure.ToLower() == input.Measure.ToLower());

        if (existingRecord == null)
        {
            return false;
        }

        _dbContext.PoolScoreParameters.Update(input);
        await _dbContext.SaveChangesAsync();

        return true;
    }
    public bool Import(Guid poolId, string excelFilePath, string? worksheetName, bool fromScratch, bool keepUserCreated)
    {
        string? errorMessage;

        var result = DataManager.ImportScoreParameters(_connectionString, poolId, excelFilePath, worksheetName, out errorMessage, fromScratch, keepUserCreated);

        if (!result)
            throw new Exception(errorMessage);

        return true;
    }

}
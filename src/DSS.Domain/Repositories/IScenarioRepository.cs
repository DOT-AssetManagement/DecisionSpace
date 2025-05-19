using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

public interface IScenarioRepository : IBasicRepository<Scenario, Guid>
{
    Task<bool> CheckName(string scenarioName);
    Task<Guid?> Create(Scenario scenario);
    Task<Scenario> GetScenarioByPoolId(Guid poolId);
    byte Export(Guid? scenId, double? effThreshold, string xlsxFilePath);

    Task<List<Scenario>> GetScenarioByUserId(String name);
    Task<bool> RunScenario(Guid id);
}

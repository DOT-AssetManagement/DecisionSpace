using DSS.Scenarios.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.Scenarios
{
    public interface IScenarioAppService: ICrudAppService<ScenarioDto, Guid, GetScenarioInput, ScenarioCreateDto, ScenarioUpdateDto>
    {
        Task<List<ScenarioDto>> GetAllAsync(String name);
        Task<Guid?> Create(ScenarioCreateDto input);
        Task<bool> CheckName(string scenarioName);
        Task<ScenarioDto> GetScenarioByPoolId(Guid poolId);
        byte ExportScenariosData(Guid? poolId, double? effThreshold, string xlsxFilePath);

        Task<bool> RunScenario(Guid id);
    }
}

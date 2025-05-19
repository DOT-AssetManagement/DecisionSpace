using DSS.Entities;
using DSS.Pool;
using DSS.Scenarios.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace DSS.Scenarios
{
    public class ScenarioAppService : ApplicationService, IScenarioAppService
    {
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IPoolAppService _poolAppService;


        public ScenarioAppService(IScenarioRepository Scenariorepository, IPoolAppService poolAppService)
        {
            _scenarioRepository = Scenariorepository;
            _poolAppService = poolAppService;
        }

        public async Task<ScenarioDto> CreateAsync(ScenarioCreateDto input)
        {
            var Scenario = ObjectMapper.Map<ScenarioCreateDto, Scenario>(input);
            var response = await _scenarioRepository.InsertAsync(Scenario);
            return ObjectMapper.Map<Scenario, ScenarioDto>(response);
        }

        [HttpPost("api/app/Scenario/Create")]
        public async Task<Guid?> Create(ScenarioCreateDto input)
        {
            var Scenario = ObjectMapper.Map<ScenarioCreateDto, Scenario>(input);
            return await _scenarioRepository.Create(Scenario);
        }
        [HttpPost("api/app/scenario/exportscenario")]
        public byte ExportScenariosData(Guid? poolId, double? effThreshold, string xlsxFilePath)
        {
            var result =_scenarioRepository.Export(poolId, effThreshold, xlsxFilePath);
            return result;
        }
        public async Task DeleteAsync(Guid id)
        {
            await _scenarioRepository.DeleteAsync(id);
        }

        [HttpGet("api/app/Scenario/getAll")]
        public async Task<List<ScenarioDto>> GetAllAsync(String name)
        {

            var userScenarios = await _scenarioRepository.GetScenarioByUserId(name);
            return ObjectMapper.Map<List<Scenario>, List<ScenarioDto>>(userScenarios);
        }

        [HttpGet("api/app/Scenario/checkName")]
        public async Task<bool> CheckName(string scenarioName)
        {
            return await _scenarioRepository.CheckName(scenarioName);
        }

        public async Task<ScenarioDto> GetAsync(Guid id)
        {
            var Scenario = await _scenarioRepository.GetAsync(id);
            return ObjectMapper.Map<Scenario, ScenarioDto>(Scenario);
        }

        public async Task<ScenarioDto> GetScenarioByPoolId(Guid poolId)
        {
            var Scenario = await _scenarioRepository.GetScenarioByPoolId(poolId);
            return ObjectMapper.Map<Scenario, ScenarioDto>(Scenario);
        }

        

        public async Task<PagedResultDto<ScenarioDto>> GetListAsync(GetScenarioInput input)
        {
            var Scenarios = await _scenarioRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);
            var ScenariosDtos = ObjectMapper.Map<List<Scenario>, List<ScenarioDto>>(Scenarios);

            return new PagedResultDto<ScenarioDto>(ScenariosDtos.Count, ScenariosDtos);

        }

        public async Task<ScenarioDto> UpdateAsync(Guid id, ScenarioUpdateDto input)
        {
            /*var Scenario = await _scenarioRepository.GetAsync(id);
            var updatedScenario = ObjectMapper.Map(input, Scenario);
            var response = await _scenarioRepository.UpdateAsync(updatedScenario);

            return ObjectMapper.Map<Scenario, ScenarioDto>(response);*/


            var Scenario = await _scenarioRepository.GetAsync(id);
            Scenario.ScenName = input.ScenName;
            Scenario.Description = input.Description;
            Scenario.PoolId = input.PoolId;
            var response = await _scenarioRepository.UpdateAsync(Scenario);

            return ObjectMapper.Map<Scenario, ScenarioDto>(response);
        }

        public async Task<bool> RunScenario(Guid id)
        {
            return await _scenarioRepository.RunScenario(id);
        }
    }
}

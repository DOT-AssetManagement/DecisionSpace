using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using DSS.Entities;
using DSS.PoolScoreParameters.Dtos;
using Volo.Abp.Users;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DSS.PoolScoreParameters
{

    public class PoolScoreParametersAppService : ApplicationService, IPoolScoreParametersAppService
    {
        private readonly IPoolScoreParametersRepository _poolScoreParametersRepository;
        private readonly IWorkCandidatesRepository _workCandidatesRepository;
        private readonly ICurrentUser _curruntUser;

        public PoolScoreParametersAppService(IPoolScoreParametersRepository poolScoreParametersRepository, IWorkCandidatesRepository workCandidatesRepository, ICurrentUser curruntUser)
        {
            _poolScoreParametersRepository = poolScoreParametersRepository;
            _workCandidatesRepository = workCandidatesRepository;
            _curruntUser = curruntUser;
        }

        public async Task<PoolScoreParametersDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("api/app/pool-score-parameters/getAll")]

        public async Task<List<PoolScoreParametersDto>> GetAllByPoolIdAsync(Guid id)
        {
            var PoolScoreParameters = await _poolScoreParametersRepository.GetListByPoolId(id);
            return ObjectMapper.Map<List<PoolScoreParameter>, List<PoolScoreParametersDto>>(PoolScoreParameters);
        }
        public async Task<PoolScoreParametersDto> GetAllByPoolIdAndMeasureAsync(Guid id, string measure)
        {
            var PoolScoreParameters = await _poolScoreParametersRepository.GetByPoolIdAndMeasure(id, measure);
            return ObjectMapper.Map<PoolScoreParameter, PoolScoreParametersDto>(PoolScoreParameters);
        }

        [HttpPut("api/app/Pool/import-poolscore-parameter")]
        public bool ImportPoolScoreParameter(string excelFilePath, string? worksheetName, Guid poolId, bool fromScratch, bool keepUserCreated)
        {
            var result = _poolScoreParametersRepository.Import(poolId, excelFilePath, worksheetName, fromScratch, keepUserCreated);
            return result;
        }

        [HttpGet("api/app/pool-score-parameters/all")]
        public async Task<List<PoolScoreParametersDto>> GetAllAsync()
        {
            var PoolScoreParameters = await _poolScoreParametersRepository.GetListAsync();
            return ObjectMapper.Map<List<PoolScoreParameter>, List<PoolScoreParametersDto>>(PoolScoreParameters);
        }
        [HttpGet("api/app/pool-score-parameters/list")]
        public async Task<PagedResultDto<PoolScoreParametersDto>> GetListAsync(GetPoolScoreParametersInput input)
        {
            var Pools = await _poolScoreParametersRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);
            var PoolsDtos = ObjectMapper.Map<List<PoolScoreParameter>, List<PoolScoreParametersDto>>(Pools);

            return new PagedResultDto<PoolScoreParametersDto>(PoolsDtos.Count, PoolsDtos);

        }

        public async Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> InsertScoreParametersForPool(Guid poolId)
        {
            var result = _poolScoreParametersRepository.CreatePoolScoreParameters(poolId);

            return result;
        }


        public async Task<PoolScoreParametersDto> CreateAsync(PoolScoreParametersCreateDto input)
        {
            var pool = ObjectMapper.Map<PoolScoreParametersCreateDto, PoolScoreParameter>(input);
           
            var response = await _poolScoreParametersRepository.InsertAsync(pool);

            await _workCandidatesRepository.UpdatePool(pool.PoolId, false);
            return ObjectMapper.Map<PoolScoreParameter, PoolScoreParametersDto>(response);
        }

        public async Task<PoolScoreParametersDto> UpdateAsync(Guid PoolId, PoolScoreParametersUpdateDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<PoolScoreParametersDto> UpdatePoolScoreParameterAsync(PoolScoreParametersUpdateDto input)
        {
            var poolScoreParameter = await _poolScoreParametersRepository.GetByPoolIdAndMeasure(input.PoolId, input.Measure);

            ObjectMapper.Map(input, poolScoreParameter);

            await _poolScoreParametersRepository.UpdateByPoolIdAndMeasureAsync(input.PoolId, poolScoreParameter);

            await _workCandidatesRepository.UpdatePool(input.PoolId, false);
            return ObjectMapper.Map<PoolScoreParameter, PoolScoreParametersDto>(poolScoreParameter);
        }

    }
}

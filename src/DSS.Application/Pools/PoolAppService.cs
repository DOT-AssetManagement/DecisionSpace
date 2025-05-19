using Azure;
using DSS.Entities;
using DSS.Pool;
using DSS.Pool.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Users;

namespace DSS.Pools
{
    public class PoolAppService : ApplicationService, IPoolAppService
    {
        private readonly IPoolRepository _poolRepository;
        private readonly ICurrentUser _curruntUser;

        public PoolAppService(IPoolRepository poolRepository, ICurrentUser curruntUser)
        {
            _poolRepository = poolRepository;
            _curruntUser = curruntUser;
        }

        public async Task<PoolDto> CheckName(string poolName)
        {
            var response = await _poolRepository.CheckName(poolName);
            return ObjectMapper.Map<CandidatePool, PoolDto>(response);
        }


        public async Task<PoolDto> CreateAsync(PoolCreateDto input)
        {
            var pool = ObjectMapper.Map<PoolCreateDto, CandidatePool>(input);
            pool.UserId = _curruntUser.Id.Value;
            pool.CreatedBy = _curruntUser.UserName;
            pool.UpdatedBy = _curruntUser.UserName;
            var response = await _poolRepository.InsertAsync(pool);
            return ObjectMapper.Map<CandidatePool, PoolDto>(response);
        }

        [HttpPost("api/app/pool/create")]
        public async Task<Guid?> Create(PoolCreateDto input)
        {
            var pool = ObjectMapper.Map<PoolCreateDto, CandidatePool>(input);
            pool.UserId = _curruntUser.Id.Value;
            pool.CreatedBy = _curruntUser.UserName;
            pool.UpdatedBy = _curruntUser.UserName;
            var response = _poolRepository.CreatePool(pool);

            var Pool = await _poolRepository.GetAsync((Guid)response);
            Pool.Description = input.Description;
            await _poolRepository.UpdateAsync(Pool);

            return response;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _poolRepository.DeleteAsync(id);
        }

        public async Task ClearAsync(Guid id)
        {
            _poolRepository.ClearPool(id);
        }

        [HttpGet("api/app/Pool/getAll")]
        public async Task<List<PoolDto>> GetAllAsync(string name, string poolId)
        {
            var pools = _poolRepository.GetPoolsByUserId(name, poolId);  
            return ObjectMapper.Map<List<CandidatePool>, List<PoolDto>>(pools);
        }

        [HttpPut("api/app/Pool/import-work-candidates")]
        public bool ImportWorkCandidates(string excelFilePath, string? importSourceName, string? worksheetName, Guid poolId, bool forgiving = false)
        {
            var result = _poolRepository.Import(excelFilePath, importSourceName, worksheetName, poolId, forgiving);

            return result;
        }
        public bool ImportBridgeToPavements(string excelFilePath)
        {
            string? errorMessage;
            bool result;

            result = _poolRepository.ImportBridgeToPavements(excelFilePath);

            if (!result)
            {
                throw new Exception("An error occurred during the import process.");
            }

            return true;
        }

        public async Task<bool> ImportAssetSegmentationAsync(string excelFilePath, string worksheetName)
        {
            string errorMessage;
            bool result;

            try
            {
                result = _poolRepository.ImportAssetSegmentation(excelFilePath, worksheetName);

                if (!result)
                {
                    throw new Exception("An error occurred during the import process.");
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred during the import process.", ex);
            }
        }


        public async Task<PoolDto> GetAsync(Guid id)
        {
            var Pool = await _poolRepository.GetAsync(id);
            return ObjectMapper.Map<CandidatePool, PoolDto>(Pool);
        }

        public async Task<PagedResultDto<PoolDto>> GetListAsync(GetPoolInput input)
        {
            var Pools = await _poolRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);
            var PoolsDtos = ObjectMapper.Map<List<CandidatePool>, List<PoolDto>>(Pools);

            return new PagedResultDto<PoolDto>(PoolsDtos.Count, PoolsDtos);

        }

        public async Task<PoolDto> UpdateAsync(Guid id, PoolUpdateDto input)
        {
            var Pool = await _poolRepository.GetAsync(id);
            Pool.Name = input.Name;
            Pool.Description = input.Description;
            Pool.IsSharedLibrary = input.IsSharedLibrary;
            Pool.UpdatedAt = DateTime.Now;
            Pool.UpdatedBy = _curruntUser.Name;
            var response = await _poolRepository.UpdateAsync(Pool);

            return ObjectMapper.Map<CandidatePool, PoolDto>(response);
        }


    }
}

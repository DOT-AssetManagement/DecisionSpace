using DSS.Entities;
using DSS.OthersParameters.Dtos;
using DSS.Repositories;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DSS.OthersParameters
{
    public class OtherParameterAppService : ApplicationService, IOtherParameterAppService
    {
        private readonly IOtherParameterRepository _otherParameterRepository;

        public OtherParameterAppService(IOtherParameterRepository otherParameterRepository)
        {
            _otherParameterRepository = otherParameterRepository;
        }

        public Task<TblOtherParameterDto> CreateAsync(TblOtherParameterUpdateDto input)
        {
            throw new NotImplementedException();
        }
        public async Task<TblOtherParameterDto> CreateParameterAsync(TblOtherParameterCreateDto input)
        {
            try
            {
                var parameters = ObjectMapper.Map<TblOtherParameterCreateDto, TblOtherParameters>(input);
                var response = await _otherParameterRepository.InsertAsync(parameters);
                return ObjectMapper.Map<TblOtherParameters, TblOtherParameterDto>(response);
            }
            catch
            {
                throw;
            }

        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteOtherParameterAsync(string code)
        {
            var entityToDelete = await _otherParameterRepository.GetAsync(code);

            if (entityToDelete != null)
            {
                await _otherParameterRepository.DeleteAsync(entityToDelete);
            }
        }
        public async Task<List<TblOtherParameterDto>> GetAllAsync()
        {
            var parameters = await _otherParameterRepository.GetListAsync();
            return ObjectMapper.Map<List<TblOtherParameters>, List<TblOtherParameterDto>>(parameters);
        }
        public async Task<TblOtherParameterDto> GetParameterAsync(string code)
        {
            var parameter = await _otherParameterRepository.GetAsync(code);
            return ObjectMapper.Map<TblOtherParameters, TblOtherParameterDto>(parameter);
        }
   
        public Task<TblOtherParameterDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<TblOtherParameterDto>> GetListAsync(TblOtherParameterCreateDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<TblOtherParameterDto> UpdateAsync(string code, TblOtherParameterUpdateDto input)
        {
            var existingEntity = await _otherParameterRepository.GetAsync(code);
            if (existingEntity == null)
            {
                return null;
            }

            ObjectMapper.Map(input, existingEntity);
            try
            {
                var updatedEntity = await _otherParameterRepository.UpdateAsync(existingEntity);
                return ObjectMapper.Map<TblOtherParameters, TblOtherParameterDto>(updatedEntity);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Task<TblOtherParameterDto> UpdateAsync(Guid id, GetOtherParameterInput input)
        {
            throw new NotImplementedException();
        }
    }
}

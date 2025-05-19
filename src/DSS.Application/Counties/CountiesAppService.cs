
using DSS.Counties.Dtos;
using DSS.Entities;
using DSS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.ObjectMapping;

namespace DSS.Counties
{
    public class CountiesAppService : ApplicationService, ICountyAppService
    {
        private readonly ICountyRepository _countyRepository;

        public CountiesAppService(ICountyRepository countyRepository)
        {
            _countyRepository = countyRepository;
        }

        public async Task<List<CountyDto>> GetAllDistricts()
        {
            var districts = await _countyRepository.GetAllDistricts();
            return ObjectMapper.Map<List<County>, List<CountyDto>>(districts);
        }

        public async Task<List<CountyDto>> GetCountiesByDistrict(int id)
        {
            var districts = await _countyRepository.GetCountiesByDistrict(id);
            return ObjectMapper.Map<List<County>, List<CountyDto>>(districts);
        }

        public async Task<List<CountyDto>> GetCounties()
        {
            var counties = await _countyRepository.GetCounties();
            return ObjectMapper.Map<List<County>, List<CountyDto>>(counties);
        }



        public Task<PagedResultDto<CountyDto>> GetListAsync(GetCountyInput input)
        {
            throw new NotImplementedException();
        }

        public Task<CountyDto> CreateAsync(CountyCreateDto input)
        {
            throw new NotImplementedException();
        }

        public Task<CountyDto> UpdateAsync(Guid id, CountyUpdateDto input)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CountyDto> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}

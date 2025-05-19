using DSS.Counties.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.Counties
{
    public interface ICountyAppService : ICrudAppService<CountyDto, Guid, GetCountyInput, CountyCreateDto, CountyUpdateDto>
    {
        Task<List<CountyDto>> GetAllDistricts();
        Task<List<CountyDto>> GetCountiesByDistrict(int id);

        Task<List<CountyDto>> GetCounties();

    }
}

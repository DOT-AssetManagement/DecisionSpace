using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;


public interface ICountyRepository : IBasicRepository<County>
{
    Task<List<County>> GetAllDistricts();
    Task<List<County>> GetCountiesByDistrict(int id);

    Task<List<County>> GetCounties();
}


using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace DSS.Repositories
{
    public interface IAssetTypeParameterRepository : IBasicRepository<AssetTypeParametersData, Guid>
    {
    }
}

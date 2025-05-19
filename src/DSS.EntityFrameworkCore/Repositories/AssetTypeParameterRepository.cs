using DSS.Entities;
using DSS.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace DSS.Repositories
{
    public class AssetTypeParameterRepository: EfCoreRepository<DSSDbContext, AssetTypeParametersData, Guid>, IAssetTypeParameterRepository
    {
        public AssetTypeParameterRepository(IDbContextProvider<DSSDbContext> dbContextProvider)
       : base(dbContextProvider)
        {
        }
    }
}

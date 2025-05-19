using DSS.Entities;
using DSS.Repositories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class OtherParameterRepositoryTest : DSSTestingEntityFrameworkCoreTestBase
    {
        private readonly IOtherParameterRepository _otherParameterRepository;

        public OtherParameterRepositoryTest()
        {
            _otherParameterRepository = GetRequiredService<IOtherParameterRepository>();
        }

        [Fact]
        public async Task Test_GetAllAsync()
        {
            var parameter = await _otherParameterRepository.GetListAsync();
        }
    }
}

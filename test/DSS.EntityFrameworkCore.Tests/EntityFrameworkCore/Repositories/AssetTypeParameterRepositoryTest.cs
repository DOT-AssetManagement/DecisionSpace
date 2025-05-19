using DSS.Entities;
using DSS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class AssetTypeParameterRepositoryTest: DSSTestingEntityFrameworkCoreTestBase
    {
        private readonly IBasicRepository<Entities.AssetTypeParametersData, Guid> _assetTypeParameterRepository;
        public AssetTypeParameterRepositoryTest()
        {
            _assetTypeParameterRepository = GetRequiredService<IAssetTypeParameterRepository>();
        }
        [Fact]
        public async Task Should_Query_AssetTypeParameter()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Pool = (await _assetTypeParameterRepository.GetListAsync());

            });
        }
        [Fact]
        public async Task Test_CreateAsset()
        {
            Guid AssetId = Guid.NewGuid();
            var asset = new Entities.AssetTypeParametersData()
            {
                AssetType = "B"
            };

            var newasset = _assetTypeParameterRepository.InsertAsync(asset);
        }
        [Fact]
        public async Task Test_GetAsset()
        {
            Guid AssetId = new Guid("05ACD553-778D-B8B3-042C-3A13186EA53F");
            var asset = _assetTypeParameterRepository.GetAsync(AssetId);
        }

    }
}

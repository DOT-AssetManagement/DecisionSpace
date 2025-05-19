using Xunit;

namespace DSS.EntityFrameworkCore;

[CollectionDefinition(DSSTestConsts.CollectionDefinitionName)]
public class DSSEntityFrameworkCoreCollection : ICollectionFixture<DSSEntityFrameworkCoreFixture>
{

}

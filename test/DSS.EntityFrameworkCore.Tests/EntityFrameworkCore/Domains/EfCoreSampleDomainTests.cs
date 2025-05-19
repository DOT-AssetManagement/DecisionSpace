using DSS.Samples;
using Xunit;

namespace DSS.EntityFrameworkCore.Domains;

[Collection(DSSTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<DSSEntityFrameworkCoreTestModule>
{

}

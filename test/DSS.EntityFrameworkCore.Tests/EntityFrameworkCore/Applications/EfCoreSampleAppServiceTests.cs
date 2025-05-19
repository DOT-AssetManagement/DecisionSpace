using DSS.Samples;
using Xunit;

namespace DSS.EntityFrameworkCore.Applications;

[Collection(DSSTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<DSSEntityFrameworkCoreTestModule>
{

}

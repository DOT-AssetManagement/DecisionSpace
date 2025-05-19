using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace DSS.Pages;

public class Index_Tests : DSSTestingWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}

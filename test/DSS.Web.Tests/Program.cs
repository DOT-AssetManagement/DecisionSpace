using Microsoft.AspNetCore.Builder;
using DSS;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
await builder.RunAbpModuleAsync<DSSTestingWebTestModule>();

public partial class Program
{
}

using Volo.Abp.Application.Dtos;

namespace DSS.Scenarios.Dtos
{
    public class GetScenarioInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }

}

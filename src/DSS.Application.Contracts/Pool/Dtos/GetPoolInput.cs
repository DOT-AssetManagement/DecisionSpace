using Volo.Abp.Application.Dtos;

namespace DSS.Pool.Dtos
{
    public class GetPoolInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }

}

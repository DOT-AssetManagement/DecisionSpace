using Volo.Abp.Application.Dtos;

namespace DSS.Goals.Dtos
{
    public class GetGoalInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }

}

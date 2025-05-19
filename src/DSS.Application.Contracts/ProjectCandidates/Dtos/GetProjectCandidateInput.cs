using Volo.Abp.Application.Dtos;

namespace DSS.ProjectCandidates.Dtos
{
    public class GetProjectCandidateInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.WorkCandidates.Dtos
{
    public class GetWorkCandidatesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}

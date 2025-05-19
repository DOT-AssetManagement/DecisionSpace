using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.Counties.Dtos
{
    public class GetCountyInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.PoolScoreParameters.Dtos
{
    public class GetPoolScoreParametersInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}

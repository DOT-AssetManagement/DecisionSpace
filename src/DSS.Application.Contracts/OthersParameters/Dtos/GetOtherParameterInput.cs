using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.OthersParameters.Dtos
{
    public class GetOtherParameterInput: PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}

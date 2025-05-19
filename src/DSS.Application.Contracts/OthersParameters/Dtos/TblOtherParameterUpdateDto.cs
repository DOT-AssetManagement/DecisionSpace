using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.OthersParameters.Dtos
{
    public class TblOtherParameterUpdateDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Val { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DSS.OthersParameters.Dtos
{
    public class TblOtherParameterCreateDto
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Val { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
    }
}

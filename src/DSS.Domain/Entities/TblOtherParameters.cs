using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    public class TblOtherParameters:Entity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public double Val { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.Now;
        public override object?[] GetKeys()
        {
            return new object[] { Code };
        }
    }
}

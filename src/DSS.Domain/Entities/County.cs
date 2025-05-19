using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    [Table("TblCounties")]
    public class County : Entity
    {

        public int District { get; set; }
        public int Cnty { get; set; }

        [Column("County")]
        public string CountyName { get; set; }

        public override object?[] GetKeys()
        {
            return new object[] { District, Cnty };
        }
    }
}

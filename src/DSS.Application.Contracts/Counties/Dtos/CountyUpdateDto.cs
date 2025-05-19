using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.Counties.Dtos
{
    public class CountyUpdateDto : EntityDto<Guid>
    {
        public int District { get; set; }
        public int Cnty { get; set; }

        [Column("County")]
        public string CountyName { get; set; }
    }
}

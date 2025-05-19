using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DSS.Counties.Dtos
{
    public class CountyCreateDto
    {
        public int District { get; set; }
        public int Cnty { get; set; }

        [Column("County")]
        public string CountyName { get; set; }
    }
}

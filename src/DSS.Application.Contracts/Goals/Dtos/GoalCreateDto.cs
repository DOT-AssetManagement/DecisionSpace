using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.Goals.Dtos
{
    public class GoalCreateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte Status { get; set; }
    }

}

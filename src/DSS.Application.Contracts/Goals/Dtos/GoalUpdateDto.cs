using System;
using Volo.Abp.Application.Dtos;

namespace DSS.Goals.Dtos
{
    public class GoalUpdateDto: EntityDto<Guid>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public byte Status { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace DSS.Entities
{
    [Table("_DssGoals")]
    public class Goal : Entity<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public Guid? CreatorId { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}

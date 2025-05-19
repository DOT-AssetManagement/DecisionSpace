using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace DSS.Web.Pages.Configuration.Measures
{
    [Table("_DssMeasures")]
    public class Measure : Entity<Guid>
    {
        public Guid Id { get; set; } 

        [Column("MeasureName")]
        public string? Name { get; set; }

        [Column("MeasureDescription")]
        public string? Description { get; set; }

        public byte Status { get; set; }
        public Guid? CreatorId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public virtual object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
}

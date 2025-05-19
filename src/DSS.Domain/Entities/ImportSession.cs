using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    [Table("TblImportSessions")]
    public class ImportSession : Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionNo { get; set; }
        public string? ImportSourceType { get; set; }
        public string? ImportSourceName { get; set; }
        public string DataSourceType { get; set; }
        public string DataSourceName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? CompletedStatus { get; set; }
        public string? Notes { get; set; }
        public Guid? PoolId { get; set; }
    }
}

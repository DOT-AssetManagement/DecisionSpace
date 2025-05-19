using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    [Table("TblScenarios")]
    public class Scenario : Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ScenNo { get; set; }
        public string ScenName { get; set; }
        public string Description { get; set; }
        public string DeaInputSproc { get; set; }
        public string DeaOutputSproc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public CandidatePool Pool { get; set; }
        public Guid PoolId { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    public class CandidatePool: Entity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LibNo { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public bool? IsActive { get; set; }
        public string? UpdatedBy { get; set; } 
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public bool IsSharedLibrary { get; set; }
        public Guid? SourcePoolId { get; set; }
    }
}

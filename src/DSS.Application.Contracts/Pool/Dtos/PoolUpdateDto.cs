using System;
using Volo.Abp.Application.Dtos;

namespace DSS.Pool.Dtos
{
    public class PoolUpdateDto: EntityDto<Guid>
    {
        public int LibNo { get; set; }
        public Guid? UserId { get; set; }
        public string? Name { get; set; }
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

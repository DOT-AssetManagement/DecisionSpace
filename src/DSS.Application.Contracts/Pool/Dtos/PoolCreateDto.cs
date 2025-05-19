using System;

namespace DSS.Pool.Dtos
{
    public class PoolCreateDto
    {
        public int LibNo { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public bool IsSharedLibrary { get; set; }
        public Guid? SourcePoolId { get; set; }
    }

}

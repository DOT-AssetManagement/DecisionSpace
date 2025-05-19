using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    public class AssetTypeParametersData : Entity<Guid>
    {
        public string AssetType { get; set; }
        public double BenefitScalingFactor { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public Guid? ScenId { get; set; }
    }
}

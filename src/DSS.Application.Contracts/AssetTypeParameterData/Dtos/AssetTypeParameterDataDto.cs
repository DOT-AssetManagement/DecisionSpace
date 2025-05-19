using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.AssetTypeParametersData.Dtos
{
    public class AssetTypeParameterDataDto: EntityDto<Guid>
    {
        public string AssetType { get; set; }
        public double BenefitScalingFactor { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public Guid? ScenId { get; set; }
    }
}

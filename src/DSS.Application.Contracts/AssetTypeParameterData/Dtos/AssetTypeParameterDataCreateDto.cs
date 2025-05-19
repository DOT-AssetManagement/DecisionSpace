using System;
using System.Collections.Generic;
using System.Text;

namespace DSS.AssetTypeParametersData.Dtos
{
    public class AssetTypeParameterDataCreateDto
    {
        public string AssetType { get; set; }
        public double BenefitScalingFactor { get; set; }
        public string? CreatedBy { get; set; }
        public Guid? ScenId { get; set; }
    }
}

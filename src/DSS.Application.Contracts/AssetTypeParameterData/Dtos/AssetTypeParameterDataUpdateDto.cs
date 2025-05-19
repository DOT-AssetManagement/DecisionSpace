using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.AssetTypeParametersData.Dtos
{
    public class AssetTypeParameterDataUpdateDto: EntityDto<Guid>
    {
        public string AssetType { get; set; }
        public double BenefitScalingFactor { get; set; }
        public Guid? ScenId { get; set; }
    }
}

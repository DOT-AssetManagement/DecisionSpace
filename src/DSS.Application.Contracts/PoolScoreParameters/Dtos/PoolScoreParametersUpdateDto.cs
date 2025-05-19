using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.PoolScoreParameters.Dtos
{
    public class PoolScoreParametersUpdateDto : EntityDto<Guid>
    {
        public Guid PoolId { get; set; }
        public string Measure { get; set; }
        public float Min { get; set; }
        public float ScoreAtMin { get; set; }
        public float Max { get; set; }
        public float ScoreAtMax { get; set; }
        public float Slope { get; set; }
        public float SafetyWeight { get; set; }
        public float MobilityWeight { get; set; }
        public float EquityWeight { get; set; }
        public float EnvWeight { get; set; }
        public float CondWeight { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}

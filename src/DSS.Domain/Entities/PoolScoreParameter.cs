﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    [Table("TblPoolScoreParameters")]
    public class PoolScoreParameter : Entity
    {

        public Guid PoolId { get; set; }
        public string Measure { get; set; }
        public double Min { get; set; }
        public double ScoreAtMin { get; set; }
        public double Max { get; set; }
        public double ScoreAtMax { get; set; }
        public double Slope { get; set; }
        public double SafetyWeight { get; set; }
        public double MobilityWeight { get; set; }
        public double EquityWeight { get; set; }
        public double EnvWeight { get; set; }
        public double CondWeight { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public override object?[] GetKeys()
        {
            return new object[] { PoolId, Measure};
        }

    }
}

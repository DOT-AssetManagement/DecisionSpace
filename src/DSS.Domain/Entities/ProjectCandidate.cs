using System;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities;

namespace DSS.Entities
{
    [Table("TblProjectCandidates")]
    public class ProjectCandidate : Entity<Guid>
    {
        public Guid Id { get; set; }
        public Byte? District { get; set; }
        public Byte? Cnty { get; set; }

        public String? CountyName { get; set; }
        public string? Description { get; set; }
        public int? NumberOfWorkCandidates { get; set; }
        public string? Treatment { get; set; }
        public int? YearEarliest { get; set; }
        public int? YearLatest { get; set; }
        public double? TotalCost { get; set; }
        public double? TotalScaledBenefit { get; set; }
        public double? SafetyScore { get; set; }
        public double? MobilityAndEconomyScore { get; set; }
        public double? EquityAndAccessScore { get; set; }
        public double? ResilienceAndEnvironmentScore { get; set; }
        public double? ConditionAndPerformanceScore { get; set; }
        public double? TotalScore { get; set; }
        public double? RelativeEfficiency { get; set; }
        public string? CreatedBy { get; set; }
        public Guid? PoolId { get; set; }
        public CandidatePool Pool { get; set; }
        public DateTime? RelativeEfficiencySetAt { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}

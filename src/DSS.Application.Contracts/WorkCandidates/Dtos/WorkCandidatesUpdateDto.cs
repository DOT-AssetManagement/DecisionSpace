﻿using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace DSS.WorkCandidates.Dtos
{
    public class WorkCandidatesUpdateDto : EntityDto<Guid>
    {
        public Guid ImportSessionId { get; set; }
        public Guid PoolId { get; set; }
        public Guid ImportTimeGeneratedId { get; set; }
        public Guid? ProjectCandidateId { get; set; }
        public string? ProjectName { get; set; }
        public Byte? District { get; set; }
        public Byte? Cnty { get; set; }
        public int? Route { get; set; }
        public string? AssetType { get; set; }
        public int? FromSection { get; set; }
        public string? ToSection { get; set; }
        public string? BRKEY { get; set; }
        public string? BRIDGE_ID { get; set; }
        public string? Description { get; set; }
        public double? Cost { get; set; }
        public double? Predicted_Scaled_Benefit { get; set; }
        public double? Total_Score { get; set; }
        public double? HSIP_Eligible { get; set; }
        public double? Excess_Safety_Value { get; set; }
        public int? Supported_by_RSA_Safety_Study { get; set; }
        public double? Degree_of_Safety_Improvement { get; set; }
        public int? Supports_Hazard_Mitigation { get; set; }
        public int? Improves_Access_to_Emergency_Services { get; set; }
        public double? Predicted_Safety_Benefit { get; set; }
        public double? Safety_Score { get; set; }
        public double? Travel_Time_Savings { get; set; }
        public double? Operating_Cost_Savings { get; set; }
        public double? Degree_of_Mobility_Improvement { get; set; }
        public double? Freight_Improvement { get; set; }
        public int? Facilitates_Transit_Use { get; set; }
        public int? Facilitates_Active_Transportation { get; set; }
        public int? Bike_Lanes_Constructed { get; set; }
        public int? New_Cyclists { get; set; }
        public int? Support_for_Job_Growth { get; set; }
        public int? Support_for_Tourism { get; set; }
        public int? Support_for_Economic_Goals { get; set; }
        public int? Support_for_Recreation { get; set; }
        public int? Consistency_with_Existing_Plans { get; set; }
        public double? Predicted_Mobility_Benefit { get; set; }
        public double? Mobility_and_Access_Score { get; set; }
        public double? Sidewalks_and_Curb_Ramps_Constructed_or_Improved { get; set; }
        public Int16? Change_in_Accessible_Destinations { get; set; }
        public double? Percent_of_Acessibilty_Improvement_for_Disadvantaged_Populations { get; set; }
        public int? Support_for_Environmental_Justice { get; set; }
        public double? Predicted_Equity_and_Access_Benefit { get; set; }
        public double? Equity_and_Access_Score { get; set; }
        public double? Reduced_Flood_Closure_Risk { get; set; }
        public double? Wetlands_Improved { get; set; }
        public double? Wildlife_Crossings { get; set; }
        public double? Reduced_Fuel_Consumption { get; set; }

        public int? Degree_of_Environmental_Improvement { get; set; }
        public double? Predicted_Environmental_Benefit { get; set; }
        public double? Resilience_and_Environment_Score { get; set; }
        public int? Consistency_with_Land_Use_Plans { get; set; }
        public double? Pavement_Rehabilitated_or_Reconstructed { get; set; }
        public double? Bridges_Rehabilitated_or_Replaced { get; set; }
        public double? Culverts_Rehabilitated_or_Replaced { get; set; }
        public double? Guardrail_Rehabilitated_or_Replaced { get; set; }
        public double? Geotechnical_Assets_Rehabilitated_or_Replaced { get; set; }
        public double? Facilities_Rehabilitated_or_Reconstructed { get; set; }
        public double? Sidewalks_Rehabilitated_or_Reconstructed { get; set; }
        public double? Predicted_Condition_and_Performance_Benefit { get; set; }
        public double? Condition_and_Performance_Score { get; set; }
        public double? ADT { get; set; }
        public double? Percent_Trucks { get; set; }
        public double? Population_Density { get; set; }
        public double? Bike_Commute_Share { get; set; }
        public string? BPN { get; set; }
        public double? Length { get; set; }
        public double? Speed_Limit { get; set; }
        public double? Detour_Distance { get; set; }
        public int? Functional_Classification { get; set; }
        public int? National_Highway_System { get; set; }
        public int? Pennsylvania_Byway { get; set; }
        public int? Bicycle_PA_Route { get; set; }
        public int? Interstate_Emergency_Detour { get; set; }
        public int? LFAR_BOF_Eligible { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Treatment { get; set; }
        public DateTime? BenefitsComputedAt { get; set; }
        public DateTime? ScoresComputedAt { get; set; }
        public int? Year { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
    }
}

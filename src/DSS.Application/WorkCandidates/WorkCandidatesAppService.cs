using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using DSS.Entities;
using DSS.WorkCandidates.Dtos;
using Volo.Abp.Users;
using Volo.Abp.Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using static Volo.Abp.UI.Navigation.DefaultMenuNames.Application;
using Azure;
using Volo.Abp.ObjectMapping;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace DSS.WorkCandidates
{
    public class WorkCandidatesAppService : ApplicationService, IWorkCandidatesAppService
    {
        private readonly IWorkCandidatesRepository _workCandidatesRepository;
        private readonly ICurrentUser _curruntUser;

        public WorkCandidatesAppService(IWorkCandidatesRepository workCandidatesRepository, ICurrentUser curruntUser)
        {
            _workCandidatesRepository = workCandidatesRepository;
            _curruntUser = curruntUser;
        }

        public async Task<WorkCandidatesDto> GetAsync(Guid id)
        {
            var workCandidate = await _workCandidatesRepository.GetAsync(id);
            return ObjectMapper.Map<WorkCandidate, WorkCandidatesDto>(workCandidate);
        }

        public async Task<PagedResultDto<WorkCandidatesDto>> GetListAsync(GetWorkCandidatesInput input)
        {
            throw new NotImplementedException();
        }

        public async Task<List<WorkCandidatesDto>> GetAllByPoolOrProjectIdAsync(Guid id,Guid projectId)
        {
            var workCandidates = await _workCandidatesRepository.GetListByPoolOrProjectId(id, projectId);
            return ObjectMapper.Map<List<WorkCandidate>, List<WorkCandidatesDto>>(workCandidates);
        }

        public async Task<WorkCandidatesDto> GetByProjectId(Guid projectId)
        {
            var workCandidate = await _workCandidatesRepository.GetByProjectId(projectId);
            return ObjectMapper.Map<WorkCandidate, WorkCandidatesDto>(workCandidate);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _workCandidatesRepository.DeleteAsync(id);
        }

        public async Task<WorkCandidatesDto> CreateAsync(WorkCandidatesCreateDto input)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> CreateWorkCandidateAsync(WorkCandidatesCreateDto input, Guid PoolId)
        {
            var workCandidate = ObjectMapper.Map<WorkCandidatesCreateDto, WorkCandidate>(input);
           
            var response = await _workCandidatesRepository.CreateWorkCandidate(workCandidate);


            await _workCandidatesRepository.UpdatePool(PoolId, false);

            return response;
        }

        public async Task<WorkCandidatesDto> UpdateAsync(Guid id, WorkCandidatesUpdateDto input)
        {
            throw new NotImplementedException();
        }
        public async Task<WorkCandidatesDto> UpdateWorkCandidateAsync(Guid id, WorkCandidatesUpdateDto input, Guid PoolId)
        {
       
            var workCandidate = await _workCandidatesRepository.GetAsync(id);


            workCandidate.District = input.District;
            workCandidate.Cnty = input.Cnty;
            workCandidate.Route = input.Route;
            workCandidate.AssetType = input.AssetType;
            workCandidate.FromSection = input.FromSection;          
            workCandidate.ToSection = input.ToSection;
            workCandidate.Description = input.Description;
            workCandidate.BRKEY = input.BRKEY;
            workCandidate.BRIDGE_ID = input.BRIDGE_ID;
            workCandidate.Cost = input.Cost;
            workCandidate.HSIP_Eligible = input.HSIP_Eligible;
            workCandidate.Excess_Safety_Value = input.Excess_Safety_Value;
            workCandidate.Supported_by_RSA_Safety_Study = input.Supported_by_RSA_Safety_Study;
            workCandidate.Degree_of_Safety_Improvement = input.Degree_of_Safety_Improvement;
            workCandidate.Supports_Hazard_Mitigation = input.Supports_Hazard_Mitigation;
            workCandidate.Improves_Access_to_Emergency_Services = input.Improves_Access_to_Emergency_Services;
            workCandidate.Travel_Time_Savings = input.Travel_Time_Savings;
            workCandidate.Operating_Cost_Savings = input.Operating_Cost_Savings;
            workCandidate.Degree_of_Mobility_Improvement = input.Degree_of_Mobility_Improvement;
            workCandidate.Freight_Improvement = input.Freight_Improvement;
            workCandidate.Facilitates_Transit_Use = input.Facilitates_Transit_Use;
            workCandidate.Facilitates_Active_Transportation = input.Facilitates_Active_Transportation;
            workCandidate.Bike_Lanes_Constructed = input.Bike_Lanes_Constructed;
            workCandidate.New_Cyclists = input.New_Cyclists;
            workCandidate.Support_for_Job_Growth = input.Support_for_Job_Growth;
            workCandidate.Support_for_Tourism = input.Support_for_Tourism;
            workCandidate.Support_for_Economic_Goals = input.Support_for_Economic_Goals;
            workCandidate.Support_for_Recreation = input.Support_for_Recreation;
            workCandidate.Consistency_with_Existing_Plans = input.Consistency_with_Existing_Plans;
            workCandidate.Sidewalks_and_Curb_Ramps_Constructed_or_Improved = input.Sidewalks_and_Curb_Ramps_Constructed_or_Improved;
            workCandidate.Change_in_Accessible_Destinations = input.Change_in_Accessible_Destinations;
            workCandidate.Percent_of_Acessibilty_Improvement_for_Disadvantaged_Populations = input.Percent_of_Acessibilty_Improvement_for_Disadvantaged_Populations;
            workCandidate.Support_for_Environmental_Justice = input.Support_for_Environmental_Justice;
            workCandidate.Reduced_Flood_Closure_Risk = input.Reduced_Flood_Closure_Risk;
            workCandidate.Wetlands_Improved = input.Wetlands_Improved;
            workCandidate.Wildlife_Crossings = input.Wildlife_Crossings;
            workCandidate.Reduced_Fuel_Consumption = input.Reduced_Fuel_Consumption;
            workCandidate.Degree_of_Environmental_Improvement = input.Degree_of_Environmental_Improvement;
            workCandidate.Consistency_with_Land_Use_Plans = input.Consistency_with_Land_Use_Plans;
            workCandidate.Pavement_Rehabilitated_or_Reconstructed = input.Pavement_Rehabilitated_or_Reconstructed;
            workCandidate.Bridges_Rehabilitated_or_Replaced = input.Bridges_Rehabilitated_or_Replaced;
            workCandidate.Culverts_Rehabilitated_or_Replaced = input.Culverts_Rehabilitated_or_Replaced;
            workCandidate.Guardrail_Rehabilitated_or_Replaced = input.Guardrail_Rehabilitated_or_Replaced;
            workCandidate.Geotechnical_Assets_Rehabilitated_or_Replaced = input.Geotechnical_Assets_Rehabilitated_or_Replaced;
            workCandidate.Facilities_Rehabilitated_or_Reconstructed = input.Facilities_Rehabilitated_or_Reconstructed;
            workCandidate.Sidewalks_Rehabilitated_or_Reconstructed = input.Sidewalks_Rehabilitated_or_Reconstructed;
            workCandidate.ADT = input.ADT;
            workCandidate.Percent_Trucks = input.Percent_Trucks;
            workCandidate.Population_Density = input.Population_Density;
            workCandidate.Bike_Commute_Share = input.Bike_Commute_Share;
            workCandidate.BPN = input.BPN;
            workCandidate.Length = input.Length;
            workCandidate.Speed_Limit = input.Speed_Limit;
            workCandidate.Detour_Distance = input.Detour_Distance;
            workCandidate.Functional_Classification = input.Functional_Classification;
            workCandidate.National_Highway_System = input.National_Highway_System;
            workCandidate.Pennsylvania_Byway = input.Pennsylvania_Byway;
            workCandidate.Bicycle_PA_Route = input.Bicycle_PA_Route;
            workCandidate.Interstate_Emergency_Detour = input.Interstate_Emergency_Detour;
            workCandidate.LFAR_BOF_Eligible = input.LFAR_BOF_Eligible;
            workCandidate.Treatment = input.Treatment;
            workCandidate.BenefitsComputedAt = input.BenefitsComputedAt;
            workCandidate.Year = input.Year;
            workCandidate.MinYear = input.MinYear;
            workCandidate.MaxYear = input.MaxYear;



            var response = await _workCandidatesRepository.UpdateAsync(workCandidate);

            
            await _workCandidatesRepository.UpdateWorkCandidate(id);
            await _workCandidatesRepository.UpdatePool(PoolId, false);

            return ObjectMapper.Map<WorkCandidate, WorkCandidatesDto>(response);
        }
    }
}

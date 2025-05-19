using DSS.Entities;
using DSS.ProjectCandidates.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;

namespace DSS.ProjectCandidates
{
    public class ProjectCandidateAppService : ApplicationService, IProjectCandidateAppService
    {
        private readonly IProjectCandidateRepository _ProjectCandidateRepository;
        private readonly IWorkCandidatesRepository _WorkCandidateRepository;
        private readonly ICurrentUser _curruntUser;

        public ProjectCandidateAppService(IProjectCandidateRepository ProjectCandidateRepository, IWorkCandidatesRepository WorkCandidateRepository, ICurrentUser curruntUser)
        {
            _ProjectCandidateRepository = ProjectCandidateRepository;
            _WorkCandidateRepository = WorkCandidateRepository;
            _curruntUser = curruntUser;
        }

        public async Task<ProjectCandidateDto> CreateAsync(ProjectCandidateCreateDto input)
        {
            var ProjectCandidate = ObjectMapper.Map<ProjectCandidateCreateDto, ProjectCandidate>(input);

            ProjectCandidate.CreatedBy = _curruntUser.UserName;
            var response = await _ProjectCandidateRepository.InsertAsync(ProjectCandidate);
            return ObjectMapper.Map<ProjectCandidate, ProjectCandidateDto>(response);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _ProjectCandidateRepository.DeleteAsync(id);
        }

        [HttpGet("api/app/project-candidate/getAll")]
        public async Task<List<ProjectCandidateDto>> GetAllAsync()
        {
            var ProjectCandidates = await _ProjectCandidateRepository.GetAll();
            return ObjectMapper.Map<List<ProjectCandidate>, List<ProjectCandidateDto>>(ProjectCandidates);
        }
        public async Task<List<ProjectCandidateDto>> GetAllProjectsAsync(Guid poolid)
        {
            var projectCandidates = await _ProjectCandidateRepository.GetProjectsByPoolId(poolid);
            return ObjectMapper.Map<List<ProjectCandidate>, List<ProjectCandidateDto>>(projectCandidates);
        }


        public async Task<ProjectCandidateDto> GetAsync(Guid id)
        {
            var ProjectCandidate = await _ProjectCandidateRepository.GetAsync(id);
            return ObjectMapper.Map<ProjectCandidate, ProjectCandidateDto>(ProjectCandidate);
        }

        public async Task<PagedResultDto<ProjectCandidateDto>> GetListAsync(GetProjectCandidateInput input)
        {
            var ProjectCandidates = await _ProjectCandidateRepository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, input.Sorting);
            var ProjectCandidatesDtos = ObjectMapper.Map<List<ProjectCandidate>, List<ProjectCandidateDto>>(ProjectCandidates);

            return new PagedResultDto<ProjectCandidateDto>(ProjectCandidatesDtos.Count, ProjectCandidatesDtos);

        }

        public async Task<List<string>> GetDistinctDescriptions()
        {
            return await _ProjectCandidateRepository.GetDistinctDescriptions();
        }

        public async Task<ProjectCandidateDto> UpdateAsync(Guid id, ProjectCandidateUpdateDto input)
        {
            var candidateProject = ObjectMapper.Map<ProjectCandidateUpdateDto, ProjectCandidate>(input);
            candidateProject.CreatedBy = _curruntUser.UserName;
            var response = await _ProjectCandidateRepository.UpdateAsync(candidateProject);

            return ObjectMapper.Map<ProjectCandidate, ProjectCandidateDto>(response);
        }

        public async Task<string> GetGisJsonStringOutput(Guid scenarioId, double RelativeEfficiencyThreshold)
        {
            return await _ProjectCandidateRepository.GetGisJsonStringOutput(scenarioId, RelativeEfficiencyThreshold);
        }
    }
}

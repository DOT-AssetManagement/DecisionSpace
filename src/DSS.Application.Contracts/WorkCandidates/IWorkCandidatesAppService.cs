using DSS.Pool.Dtos;
using DSS.WorkCandidates.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DSS.WorkCandidates
{
    public interface IWorkCandidatesAppService : ICrudAppService<WorkCandidatesDto, Guid, GetWorkCandidatesInput, WorkCandidatesCreateDto, WorkCandidatesUpdateDto>
    {
        Task<List<WorkCandidatesDto>> GetAllByPoolOrProjectIdAsync(Guid id, Guid projectId);

        Task<WorkCandidatesDto> GetByProjectId(Guid projectId);
        Task<WorkCandidatesDto> UpdateWorkCandidateAsync(Guid id, WorkCandidatesUpdateDto input, Guid PoolId);

        Task<bool> CreateWorkCandidateAsync(WorkCandidatesCreateDto input, Guid PoolId);

    }
}

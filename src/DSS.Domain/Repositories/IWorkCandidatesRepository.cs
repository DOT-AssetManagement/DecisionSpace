using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

public interface IWorkCandidatesRepository : IBasicRepository<WorkCandidate, Guid>
{
    Task<List<WorkCandidate>> GetListByPoolOrProjectId(Guid id, Guid Projectid);

    Task<WorkCandidate> GetByProjectId(Guid projectId);
    Task<bool> CreateWorkCandidate(WorkCandidate workCandidate);

    Task<bool> UpdateWorkCandidate(Guid workCandidateId);

    Task<bool> UpdatePool(Guid poolId, bool scoreParametersUpdated);
}

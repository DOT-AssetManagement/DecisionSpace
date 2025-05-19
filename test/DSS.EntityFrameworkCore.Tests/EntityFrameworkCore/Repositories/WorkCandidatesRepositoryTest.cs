using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class WorkCandidatesRepositoryTest: DSSTestingEntityFrameworkCoreTestBase
    {
        private readonly IWorkCandidatesRepository _workcandidaterepository;
        public WorkCandidatesRepositoryTest()
        {
            _workcandidaterepository = GetRequiredService<IWorkCandidatesRepository>();
        }
        [Fact]
        public async Task Should_Query_WorkCandidates()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Pool = (await _workcandidaterepository.GetListAsync());

            });
        }
        [Fact]
        public async Task Test_CreateWorkcandidate()
        {
            Guid WorkCandidateId = Guid.NewGuid();
            var candidate = new WorkCandidate()
            {
                ProjectName = "WorkcandidateTest"
            };

            var newcandidate = _workcandidaterepository.InsertAsync(candidate);
        }
        [Fact]
        public async Task Test_GetWorkCandidate()
        {
            Guid WorkCandidateId = new Guid("CA2C6AB9-D473-41D1-91BF-019F9BBE4402");
            var candidate = _workcandidaterepository.GetAsync(WorkCandidateId);
        }
        [Fact]
        public async Task Test_GetWorkCandidateByPoolOrProjectId()
        {
            Guid PoolId = new Guid("24CB81F0-35A2-46BA-B814-360C0B2630E0");
            Guid ProjectId = new Guid("1A651F01-9DF2-419B-95C8-0118BCA68BED");
            var candidate = _workcandidaterepository.GetListByPoolOrProjectId(PoolId,ProjectId);
        }
        [Fact]
        public async Task Test_GetByProjectId()
        {
            Guid ProjectId = new Guid("1A651F01-9DF2-419B-95C8-0118BCA68BED");
            var candidate = _workcandidaterepository.GetByProjectId(ProjectId);
        }

        [Fact]
        public async Task Test_GetByProjectId_NonExisting()
        {
            Guid ProjectId = new Guid("00000000-0000-0000-0000-000000000000");
            var candidate = _workcandidaterepository.GetByProjectId(ProjectId);
            if (candidate == null)
            {
                throw new Exception("There are no candidates related to Id");
            }
        }
    }
}

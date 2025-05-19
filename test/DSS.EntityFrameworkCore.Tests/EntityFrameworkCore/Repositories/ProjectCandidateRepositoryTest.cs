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
    public class ProjectCandidateRepositoryTest : DSSTestingEntityFrameworkCoreTestBase
    {
        //private readonly IBasicRepository<ProjectCandidate, Guid> _projectcandidaterepository;
        private readonly IProjectCandidateRepository _projectcandidaterepository;
        public ProjectCandidateRepositoryTest()
        {
            _projectcandidaterepository = GetRequiredService<IProjectCandidateRepository>();
        }
        [Fact]
        public async Task Should_Query_ProjectCandidate()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Pool = (await _projectcandidaterepository.GetAll());

            });
        }

        [Fact]
        public async Task Test_CreateProjectCandidate()
        {
            Guid CandidateId = Guid.NewGuid();
            var candidate = new ProjectCandidate()
            {
                Description = "GoalTest"
            };

            var newcandidate = _projectcandidaterepository.InsertAsync(candidate);
        }
        [Fact]
        public async Task Test_GetProjectCandidate()
        {
            Guid CandidateId = new Guid("DDA3BFB5-72C6-4535-B61E-045B48777D0B");
            var candidate = _projectcandidaterepository.GetAsync(CandidateId);
        }
        [Fact]
        public async Task Test_GetProjectByPoolId()
        {
            Guid PoolId = new Guid("DDA3BFB5-72C6-4535-B61E-045B48777D0B");
            var candidate = _projectcandidaterepository.GetProjectsByPoolId(PoolId);
        }
        [Fact]
        public async Task Test_GetProjectBy_PoolIdNotExist()
        {
            // Arrange
            Guid nonExistingPoolId = new Guid("00000000-0000-0000-0000-000000000000");

            // Act
            var resultTask = _projectcandidaterepository.GetProjectsByPoolId(nonExistingPoolId);
            var result = await resultTask;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task Test_GetDistinctDescriptions()
        {
            // Act
            var resultTask = _projectcandidaterepository.GetDistinctDescriptions();
            var result = await resultTask;

        }

        [Fact]
        public async Task Test_GetGisJsonStringOutput()
        {
            // Arrange
            Guid scenarioId = Guid.NewGuid(); 
            double relativeEfficiencyThreshold = 0.5; 

            // Act
            var resultTask = _projectcandidaterepository.GetGisJsonStringOutput(scenarioId, relativeEfficiencyThreshold);
            var result = await resultTask;

            // Assert
            Assert.NotNull(result); 
            Assert.NotEmpty(result);

        }

    }
}

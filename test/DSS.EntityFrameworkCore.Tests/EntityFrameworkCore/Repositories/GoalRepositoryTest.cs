using DSS.Entities;
using DSS.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class GoalRepositoryTest: DSSTestingEntityFrameworkCoreTestBase
    {

        private readonly IBasicRepository<Goal, Guid> _goalrepository;
        public GoalRepositoryTest()
        {
            _goalrepository = GetRequiredService<IGoalRepository>();
        }
        [Fact]
        public async Task Should_Query_Goal()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Pool = (await _goalrepository.GetListAsync());

            });
        }
        [Fact]
        public async Task Test_CreateGoal()
        {
            Guid GoalId = Guid.NewGuid();
            var goal = new Entities.Goal()
            {
                Name = "GoalTest"
            };

            var newgoal = _goalrepository.InsertAsync(goal);
        }
        [Fact]
        public async Task Test_GetGoal()
        {
            Guid GoalId = new Guid("05ACD553-778D-B8B3-042C-3A13186EA53F");
            var goal = _goalrepository.GetAsync(GoalId);
        }
    }
}

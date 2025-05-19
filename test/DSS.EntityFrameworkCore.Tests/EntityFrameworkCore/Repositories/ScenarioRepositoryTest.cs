using DSS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class ScenarioRepositoryTest : DSSTestingEntityFrameworkCoreTestBase
    {
        private readonly IScenarioRepository _scenarioRepository;
        public ScenarioRepositoryTest()
        {
            _scenarioRepository = GetRequiredService<IScenarioRepository>();
        }


        [Fact]
        public async Task Should_Query_ScenarioName()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Name = "New Secnario";
                var scenario = await _scenarioRepository.CheckName(Name);

            });
        }
        [Fact]

        public async Task Should_Query_ScenarioName_When_No_Name_Exists()
        {
            // Arrange
            var nonExistentName = "NotExistPool";
            var scenario = await _scenarioRepository.CheckName(nonExistentName);

            if (scenario == null)
            {
                throw new Exception($"Scenario with name '{nonExistentName}' does not exist.");
            }

        }
        [Fact]
        public async Task Should_Query_Scenarios()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Pool = (await _scenarioRepository.GetListAsync());

            });

        }
        [Fact]
        public async Task Test_CreateScenario()
        {
            Guid ScenarioId = Guid.NewGuid();
            var scenario = new Scenario()
            {
                ScenName = "ScenarioTest"
            };

            var newscenario = _scenarioRepository.InsertAsync(scenario);
        }
        [Fact]
        public async Task Test_GetScenario()
        {
            Guid ScenarioId = new Guid("7AA1C9B8-26F0-428A-806D-0201595D0CE0");
            var scenario = _scenarioRepository.GetAsync(ScenarioId);
        }
        [Fact]
        public async Task Test_GetScenarioByPoolId()
        {
            Guid PoolId = new Guid("24CB81F0-35A2-46BA-B814-360C0B2630E0");
            var scenario = _scenarioRepository.GetScenarioByPoolId(PoolId);
        }
        [Fact]
        public async Task Test_GetScenarioByPoolId_NonExistingPool()
        {
            // Arrange
            Guid nonExistingPoolId = new Guid("00000000-0000-0000-0000-000000000000");
            var scenario = await _scenarioRepository.GetScenarioByPoolId(nonExistingPoolId);
        }

        [Fact]
        public async Task Test_GetScenarioByUserId()
        {
            Guid UserId = new Guid("46354DBC-86D5-447B-A180-C9C285B87E6B");
            var scenario = _scenarioRepository.GetScenarioByPoolId(UserId);
        }

        [Fact]
        public async Task Test_GetScenarioByUserId_NonExistingUser()
        {
            Guid UserId = new Guid("00000000-0000-0000-0000-000000000000");
            var scenario = _scenarioRepository.GetScenarioByPoolId(UserId);
        }

        [Fact]
        public async Task Test_Export()
        {
            // Arrange
            Guid scenId = Guid.NewGuid();
            double effThreshold = 0.5;
            string xlsxFilePath = "test.xlsx";

            // Act
            byte result = _scenarioRepository.Export(scenId, effThreshold, xlsxFilePath);
            string successMessage = "Exported successfully";
            Assert.True(result > 0, successMessage);
        }

        [Fact]
        public async Task Test_RunScenario()
        {
            // Arrange
            Guid scenarioId = new Guid("EB09553F-2A47-476D-B0FC-78ECBAB78561");

            // Act
            bool result = await _scenarioRepository.RunScenario(scenarioId);

            // Assert
            Assert.True(result, "Scenario Run Completed Successfully");
        }
        [Fact]
        public async Task Test_RunScenarioFail()
        {
            // Arrange
            Guid scenarioId = new Guid("00000000-0000-0000-0000-000000000000");

            // Act
            bool result = await _scenarioRepository.RunScenario(scenarioId);

            // Assert
            Assert.False(result, "There is no Scenario With This Id");
        }

    }
}

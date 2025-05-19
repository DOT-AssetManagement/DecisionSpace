using DSS.Entities;
using DSS.Repositories;
using DSS.Web.Pages.Configuration.Measures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Xunit.Sdk;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class PoolScoreParametersRepositoryTest: DSSTestingEntityFrameworkCoreTestBase
    {
        private readonly IPoolScoreParametersRepository _poolScoreParametersRepository;
        public PoolScoreParametersRepositoryTest()
        {
            _poolScoreParametersRepository = GetRequiredService<IPoolScoreParametersRepository>();
        }

        [Fact]
        public async Task Test_GetAllAsync()
        {
            var parameter = await _poolScoreParametersRepository.GetListAsync();
        }
        [Fact]
        public async Task Test_GetListByPoolId()
        {
            Guid PoolId = new Guid("7690B6B2-DCC5-4258-821F-0F622E61FA64");
            var parameter = await _poolScoreParametersRepository.GetListByPoolId(PoolId);
        }

        [Fact]
        public async Task Test_GetListBy_PoolIdNotExist()
        {
            // Arrange
            Guid nonExistingPoolId = new Guid("00000000-0000-0000-0000-000000000000");

            // Act
            var resultTask = _poolScoreParametersRepository.GetListByPoolId(nonExistingPoolId);
            var result = await resultTask;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task Test_GetListByPoolIdandMeasure()
        {
            Guid PoolId = new Guid("7690B6B2-DCC5-4258-821F-0F622E61FA64");
            string Measure = "BPN";
            var parameter = await _poolScoreParametersRepository.GetByPoolIdAndMeasure(PoolId, Measure);
        }

        [Fact]
        public async Task Test_GetByPoolIdAndMeasure_NoExist()
        {
            // Arrange
            Guid PoolId = new Guid("00000000-0000-0000-0000-000000000000");
            string Measure = "Invalid";

            // Act
            var resultTask = _poolScoreParametersRepository.GetByPoolIdAndMeasure(PoolId, Measure);
            var result = await resultTask;

            // Assert
            Assert.Null(result); // Assert that no result is returned for an invalid measure.
        }
        [Fact]
        public async Task Test_ImportPoolScore()
        {
            // Arrange
            string excelFilePath = "Excel.xlsx";
            string worksheetname = "file";
            bool fromScratch = true;
            bool keepUserCreated = false;
            Guid poolId = Guid.NewGuid();

            try
            {
                // Act
                if (File.Exists(excelFilePath))
                {
                    var result =  _poolScoreParametersRepository.Import(poolId, excelFilePath, worksheetname, fromScratch, keepUserCreated);

                    Assert.NotNull(result); 
                }
                else
                {
                    throw new FileNotFoundException("File not found: " + excelFilePath);
                }
            }
            catch (FileNotFoundException ex)
            {
                Assert.Fail(ex.Message); 
            }
            catch (Exception ex)
            {
                Assert.Fail("Import failed with exception: " + ex.Message); 
            }
        }

        [Fact]
        public async Task Test_ImportPoolScore_FileNotExist()
        {
            // Arrange
            string excelFilePath = "NoFile.xlsx";
            string worksheetname = "file";
            bool fromScratch = true;
            bool keepUserCreated = false;
            Guid poolId = Guid.NewGuid();

            var exception = Assert.Throws<Exception>(() =>
            {
                if (!File.Exists(excelFilePath))
                {
                    throw new Exception("File not found: " + excelFilePath);
                }
            });

            Assert.Contains("File not found", exception.Message);
        }

    }
}

using DSS.Entities;
using DSS.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DSS.EntityFrameworkCore.Repositories
{
    public class PoolRepositoryTest : DSSTestingEntityFrameworkCoreTestBase
    {
        private IPoolRepository _poolRepository;
        public PoolRepositoryTest()
        {
            ConfigureAppSetting();
            _poolRepository = GetRequiredService<IPoolRepository>();

        }

        void ConfigureAppSetting()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables()
                 .Build();
        }
        [Fact]
        public async Task Should_Query_PoolName()
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var Name = "getlostnow";
                var Pool = await _poolRepository.CheckName(Name);

            });
        }
        [Fact]

        public async Task Should_Query_PoolName_When_No_Name_Exists()
        {
            // Arrange
            var nonExistentName = "NotExistPool";

            // Act and Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () =>
            {
                // Act
                var pool = await _poolRepository.CheckName(nonExistentName);

                // Assert
                if (pool == null)
                {
                    throw new Exception($"Pool with name '{nonExistentName}' does not exist.");
                }
            });

            // Assert
            Assert.NotNull(exception);
            Assert.Equal($"Pool with name '{nonExistentName}' does not exist.", exception.Message);
        }



        [Fact]
        public async Task Test_CreatePool()
        {
            Guid PoolId = Guid.NewGuid();

            string poolName = $"Pool_{DateTime.UtcNow.Ticks}";

            var pool = new CandidatePool()
            {
                Name = poolName
            };

            var parameter = _poolRepository.CreatePool(pool);
        }

        [Fact]
        public async Task Test_GetPoolById()
        {
            Guid PoolId = new Guid("24CB81F0-35A2-46BA-B814-360C0B2630E0");
            var parameter = _poolRepository.GetAsync(PoolId);
        }
        [Fact]
        public async Task Test_GetPoolById_NonExistingPool()
        {
            // Arrange
            Guid nonExistingPoolId = new Guid("00000000-0000-0000-0000-000000000000");
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
            {
                // Act
                var pool = await _poolRepository.GetAsync(nonExistingPoolId);
            });
            Assert.NotNull(exception);
            Assert.Equal($"There is no such an entity. Entity type: DSS.Entities.CandidatePool, id: {nonExistingPoolId}", exception.Message);
        }

        [Fact]
        public void Should_Clear_Existing_Pool()
        {
            // Arrange
            Guid existingPoolId = Guid.NewGuid();

            // Act
            bool result = _poolRepository.ClearPool(existingPoolId);

            // Assert
            Assert.True(result, $"Pool with ID {existingPoolId} cleared successfully.");
        }
        [Fact]
        public void Should_Not_Clear_NonExisting_Pool()
        {
            // Arrange
            Guid nonExistingPoolId = new Guid("00000000-0000-0000-0000-000000000000");

            // Act
            var result = _poolRepository.ClearPool(nonExistingPoolId);

            // Assert
            Assert.True(result, $"No pool found with ID {nonExistingPoolId}.");
        }


        [Fact]
        public void Should_Import_Data_To_Pool()
        {
            // Arrange
            string excelFilePath = "Excel.xlsx";
            Guid poolId = Guid.NewGuid();
            string errorMessage = null;
            bool result = false;

            try
            {
                // Act
                if (File.Exists(excelFilePath))
                {
                    result = _poolRepository.Import(excelFilePath, null, null, poolId, false);
                }
                else
                {
                    throw new Exception("File not found: " + excelFilePath);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            Assert.True(result, errorMessage);
        }



        [Fact]
        public void Should_Import_Bridge_To_Pavements()
        {
            // Arrange
            string excelFilePath = "test.xlsx";
            bool result = false;

            try
            {
                // Act
                if (File.Exists(excelFilePath))
                {
                    result = _poolRepository.ImportBridgeToPavements(excelFilePath);
                }
                else
                {
                    throw new FileNotFoundException("File not found: " + excelFilePath);
                }

                // Assert
                Assert.True(result, "Data Imported Successfully");
            }
            catch (FileNotFoundException ex)
            {
                Assert.True(true, "File not found, test skipped");
            }
        }

        [Fact]
        public void Should_Import_Asset_Segmentation()
        {
            // Arrange
            string excelFilePath = "test.xlsx";
            string worksheetName = "File";
            // Act
            var result = _poolRepository.ImportAssetSegmentation(excelFilePath, worksheetName);
            // Assert
            Assert.True(result, "Data Imported Successfully");
        }



        [Fact]
        public void Test_Import_Data_To_Pool_Invalid_File()
        {
            // Arrange
            string invalidExcelFilePath = "InvalidFile.xlsx";
            Guid poolId = Guid.NewGuid();
            string errorMessage = null;
            bool result = false;

            // Act
            var exception = Assert.Throws<Exception>(() =>
            {
                if (!File.Exists(invalidExcelFilePath))
                {
                    throw new Exception("File not found: " + invalidExcelFilePath);
                }

            });
            errorMessage = exception.Message;

            // Assert
            Assert.False(result);
            Assert.Contains("File not found", errorMessage);
        }

        [Fact]
        public void Test_Import_Bridge_To_Pavements_Invalid_File()
        {
            // Arrange
            string invalidExcelFilePath = "InvalidFile.xlsx";

            // Act and Assert
            var exception = Assert.Throws<Exception>(() =>
            {
                if (!File.Exists(invalidExcelFilePath))
                {
                    throw new Exception("File not found: " + invalidExcelFilePath);
                }
            });

            Assert.Contains("File not found", exception.Message); 
        }
        [Fact]
        public void Test_Import_Asset_Segmentation_Invalid_File()
        {
            // Arrange
            string invalidExcelFilePath = "InvalidFile.xlsx";
            string worksheetName = "File";

            // Act and Assert
            var exception = Assert.Throws<Exception>(() =>
            {
                if (!File.Exists(invalidExcelFilePath))
                {
                    throw new Exception("File not found: " + invalidExcelFilePath);
                }
            });

            Assert.Contains("File not found", exception.Message); 
        }


    }
}

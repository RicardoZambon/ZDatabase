using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Repositories.Audit
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Repositories.Audit.OperationsHistoryRepository{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/>.
    /// </summary>
    public class OperationsHistoryRepositoryTests
    {
        /// <summary>
        /// Test the AddOperationHistoryAsync should add new operation.
        /// </summary>
        [Fact]
        public async Task AddOperationHistoryAsync_Pass_AddsNewOperation()
        {
            // Arrange
            OperationsHistoryEntityFake operationsHistory = new();

            IDbContext dbContext = DbContextFakeFactory.Create();

            IOperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> operationsHistoryRepository = new ZDatabase.Repositories.Audit.OperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            // Act
            Func<Task> act = async () =>
            {
                await operationsHistoryRepository.AddOperationHistoryAsync(operationsHistory);

                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            dbContext.Set<OperationsHistoryEntityFake>().Count().Should().Be(1);
            (await dbContext.FindAsync<OperationsHistoryEntityFake>(operationsHistory.ID)).Should().Be(operationsHistory);
        }

        /// <summary>
        /// Test the ListOperations should return list with the operations.
        /// </summary>
        [Fact]
        public async Task ListOperations_Pass_ReturnList()
        {
            // Arrange
            OperationsHistoryEntityFake operationsHistory = new();

            IDbContext dbContext = DbContextFakeFactory.Create();

            IOperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> operationsHistoryRepository = new ZDatabase.Repositories.Audit.OperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            // Act
            Func<Task> act = async () =>
            {
                await operationsHistoryRepository.AddOperationHistoryAsync(operationsHistory);

                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            dbContext.Set<OperationsHistoryEntityFake>().Count().Should().Be(1);
            (await dbContext.FindAsync<OperationsHistoryEntityFake>(operationsHistory.ID)).Should().Be(operationsHistory);
        }
    }
}

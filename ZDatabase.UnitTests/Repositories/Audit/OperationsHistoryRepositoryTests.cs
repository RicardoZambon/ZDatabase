using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit;
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
        /// Test the AddOperationHistoryAsync should add new history operation.
        /// </summary>
        [Fact]
        public async Task AddOperationHistoryAsync_Pass_AddsNewOperation()
        {
            // Arrange
            OperationsHistoryEntityFake operationHistory = new();

            IDbContext dbContext = DbContextFakeFactory.Create();

            IOperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> operationsHistoryRepository = new OperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            // Act
            Func<Task> act = async () =>
            {
                await operationsHistoryRepository.AddOperationHistoryAsync(operationHistory);

                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            dbContext.Set<OperationsHistoryEntityFake>().Count().Should().Be(1);
            (await dbContext.FindAsync<OperationsHistoryEntityFake>(operationHistory.ID)).Should().Be(operationHistory);
        }

        /// <summary>
        /// Test the ListOperations should not return list with the history operations for invalid service history identifier.
        /// </summary>
        [Fact]
        public async Task ListOperations_Pass_NotReturnListForInvalidServiceHistoryID()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();

            IOperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> operationsHistoryRepository = new OperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            ServicesHistoryEntityFake serviceHistory = new();
            await dbContext.AddAsync(serviceHistory);

            AuditableEntityFake auditableEntity = new();
            await dbContext.AddAsync(auditableEntity);

            await dbContext.SaveChangesAsync();

            IQueryable<OperationsHistoryEntityFake>? operationsHistory = null;

            // Act
            Action act = () =>
            {
                operationsHistory = operationsHistoryRepository.ListOperations(serviceHistory.ID + 1);
            };

            // Assert
            act.Should().NotThrow();

            operationsHistory.Should().NotBeNull();
            operationsHistory!.Should().BeEmpty();
        }

        /// <summary>
        /// Test the ListOperations should return list with the history operations.
        /// </summary>
        [Fact]
        public async Task ListOperations_Pass_ReturnList()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();

            IOperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> operationsHistoryRepository = new OperationsHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            ServicesHistoryEntityFake serviceHistory = new();
            await dbContext.AddAsync(serviceHistory);

            AuditableEntityFake auditableEntity = new();
            await dbContext.AddAsync(auditableEntity);

            await dbContext.SaveChangesAsync();

            IQueryable<OperationsHistoryEntityFake>? operationsHistory = null;

            // Act
            Action act = () =>
            {
                operationsHistory = operationsHistoryRepository.ListOperations(serviceHistory.ID);
            };

            // Assert
            act.Should().NotThrow();

            operationsHistory.Should().NotBeNull();
            operationsHistory!.Count().Should().Be(1);
            operationsHistory!.All(x => x.ServiceHistoryID == serviceHistory.ID).Should().BeTrue();
            operationsHistory!.All(x => x.EntityID == auditableEntity.ID).Should().BeTrue();
            operationsHistory!.All(x => x.EntityName == nameof(AuditableEntityFake)).Should().BeTrue();
        }
    }
}
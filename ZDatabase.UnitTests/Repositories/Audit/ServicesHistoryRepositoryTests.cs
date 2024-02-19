using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit;
using ZDatabase.Repositories.Audit.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Repositories.Audit
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Repositories.Audit.ServicesHistoryRepository{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/>.
    /// </summary>
    public class ServicesHistoryRepositoryTests
    {
        /// <summary>
        /// Test the AddServiceHistoryAsync should add new history service.
        /// </summary>
        [Fact]
        public async Task AddServiceHistoryAsync_Pass_AddsNewOperation()
        {
            // Arrange
            ServicesHistoryEntityFake serviceHistory = new();

            IDbContext dbContext = DbContextFakeFactory.Create();

            IServicesHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> servicesHistoryRepository = new ServicesHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            // Act
            Func<Task> act = async () =>
            {
                await servicesHistoryRepository.AddServiceHistoryAsync(serviceHistory);

                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            dbContext.Set<ServicesHistoryEntityFake>().Count().Should().Be(1);
            (await dbContext.FindAsync<ServicesHistoryEntityFake>(serviceHistory.ID)).Should().Be(serviceHistory);
        }

        /// <summary>
        /// Test the ListServicesAsync should throw an exception when the entity identifier is invalid.
        /// </summary>
        [Fact]
        public async Task ListServicesAsync_Fail_ThrowExceptionWhenInvalidEntityID()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();

            IServicesHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> servicesHistoryRepository = new ServicesHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            ServicesHistoryEntityFake serviceHistory = new();
            await servicesHistoryRepository.AddServiceHistoryAsync(serviceHistory);

            AuditableEntityFake auditableEntity = new();
            await dbContext.AddAsync(auditableEntity);

            await dbContext.SaveChangesAsync();

            // Act
            Func<Task> act = async () =>
            {
                IQueryable<ServicesHistoryEntityFake>? servicesHistory = await servicesHistoryRepository.ListServicesAsync<AuditableEntityFake>(auditableEntity.ID + 1);
            };

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException<AuditableEntityFake>>();
        }

        /// <summary>
        /// Test the ListServicesAsync should return list with the history services.
        /// </summary>
        [Fact]
        public async Task ListServicesAsync_Pass_ReturnList()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();

            IServicesHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long> servicesHistoryRepository = new ServicesHistoryRepository<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>(dbContext);

            // First service
            ServicesHistoryEntityFake serviceHistory1 = new();
            await servicesHistoryRepository.AddServiceHistoryAsync(serviceHistory1);

            AuditableEntityFake auditableEntity1 = new();
            await dbContext.AddAsync(auditableEntity1);

            await dbContext.SaveChangesAsync();

            // Second service
            ServicesHistoryEntityFake serviceHistory2 = new();
            await servicesHistoryRepository.AddServiceHistoryAsync(serviceHistory2);

            AuditableEntityFake auditableEntity2 = new();
            await dbContext.AddAsync(auditableEntity2);

            await dbContext.SaveChangesAsync();

            IQueryable<ServicesHistoryEntityFake>? servicesHistory = null;

            // Act
            Func<Task> act = async () =>
            {
                servicesHistory = await servicesHistoryRepository.ListServicesAsync<AuditableEntityFake>(auditableEntity1.ID);
            };

            // Assert
            await act.Should().NotThrowAsync();

            servicesHistory.Should().NotBeNull();
            servicesHistory!.Count().Should().Be(1);
            servicesHistory!.First().Should().Be(serviceHistory1);
        }
    }
}
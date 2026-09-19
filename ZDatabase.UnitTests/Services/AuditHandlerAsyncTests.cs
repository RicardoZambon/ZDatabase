using Microsoft.Extensions.DependencyInjection;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.UnitTests.Fakes.ServicesFake;

namespace ZDatabase.UnitTests.Services
{
    /// <summary>
    /// Unit tests for the service-history and after-save members of
    /// <see cref="ZDatabase.Services.AuditHandler{TServicesHistory, TOperationsHistory, TUsers, TUsersKey}"/>.
    /// </summary>
    /// <remarks>
    /// These tests drive the handler directly rather than through
    /// <see cref="ZDatabase.ZDbContext{TDbContext}"/>, because the context override already runs the whole
    /// audit cycle and would leave nothing for the after-save methods to process.
    /// </remarks>
    public class AuditHandlerAsyncTests
    {
        /// <summary>
        /// Test the HasPreviousServiceHistory should be false before anything has been saved.
        /// </summary>
        [Fact]
        public void HasPreviousServiceHistory_Pass_FalseWhenNothingSaved()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();

            // Act
            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Assert
            auditHandler.HasPreviousServiceHistory.Should().BeFalse();
        }

        /// <summary>
        /// Test the HasPreviousServiceHistory should be true once the after-save step has retained the history.
        /// </summary>
        [Fact]
        public void HasPreviousServiceHistory_Pass_TrueAfterOperationsSaved()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.Add(new AuditableEntityFake { ID = 1 });

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            // Act
            auditHandler.AddOperationEntitiesAfterSaved();

            // Assert
            auditHandler.HasPreviousServiceHistory.Should().BeTrue();
        }

        /// <summary>
        /// Test the ClearServiceHistory should drop the retained service history.
        /// </summary>
        [Fact]
        public void ClearServiceHistory_Pass_DropsRetainedHistory()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.Add(new AuditableEntityFake { ID = 1 });

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);
            auditHandler.AddOperationEntitiesAfterSaved();

            // Act
            auditHandler.ClearServiceHistory();

            // Assert
            auditHandler.HasPreviousServiceHistory.Should().BeFalse();
        }

        /// <summary>
        /// Test the AddOperationEntitiesAfterSaved should track operations for the audited entries.
        /// </summary>
        [Fact]
        public void AddOperationEntitiesAfterSaved_Pass_TracksAuditedEntries()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.Add(new AuditableEntityFake { ID = 1 });

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            // Act
            auditHandler.AddOperationEntitiesAfterSaved();

            // Assert
            dbContext.Set<OperationsHistoryEntityFake>().Should().NotBeEmpty();
        }

        /// <summary>
        /// Test the AddOperationEntitiesAfterSaved should do nothing when no entry was audited.
        /// </summary>
        [Fact]
        public void AddOperationEntitiesAfterSaved_Pass_NoOperationsWhenNothingAudited()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake());

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            // Act
            Action act = auditHandler.AddOperationEntitiesAfterSaved;

            // Assert
            act.Should().NotThrow();
            dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>().Should().BeEmpty();
        }

        /// <summary>
        /// Test the AddOperationEntitiesAfterSavedAsync should track operations for the audited entries.
        /// </summary>
        [Fact]
        public async Task AddOperationEntitiesAfterSavedAsync_Pass_TracksAuditedEntries()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.Add(new AuditableEntityFake { ID = 1 });

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            // Act
            await auditHandler.AddOperationEntitiesAfterSavedAsync();

            // Assert
            dbContext.Set<OperationsHistoryEntityFake>().Should().NotBeEmpty();
        }

        /// <summary>
        /// Test the AddOperationEntitiesAfterSavedAsync should do nothing when no entry was audited.
        /// </summary>
        [Fact]
        public async Task AddOperationEntitiesAfterSavedAsync_Pass_NoOperationsWhenNothingAudited()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake());

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            // Act
            Func<Task> act = auditHandler.AddOperationEntitiesAfterSavedAsync;

            // Assert
            await act.Should().NotThrowAsync();
            dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>().Should().BeEmpty();
        }

        /// <summary>
        /// Test the AddOperationEntitiesAfterSaved should track operations for audited related entries.
        /// </summary>
        [Fact]
        public void AddOperationEntitiesAfterSaved_Pass_TracksAuditedRelatedEntries()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            IAuditHandler auditHandler = SeedManyToManyRelatedEntry(dbContext);

            // Act
            auditHandler.AddOperationEntitiesAfterSaved();

            // Assert
            dbContext.Set<OperationsHistoryEntityFake>().Should().NotBeEmpty();
        }

        /// <summary>
        /// Test the AddOperationEntitiesAfterSavedAsync should track operations for audited related entries.
        /// </summary>
        [Fact]
        public async Task AddOperationEntitiesAfterSavedAsync_Pass_TracksAuditedRelatedEntries()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            IAuditHandler auditHandler = SeedManyToManyRelatedEntry(dbContext);

            // Act
            await auditHandler.AddOperationEntitiesAfterSavedAsync();

            // Assert
            dbContext.Set<OperationsHistoryEntityFake>().Should().NotBeEmpty();
        }

        /// <summary>
        /// Test the AddOperationEntitiesBeforeSavingAsync should track operations for audited related entries.
        /// </summary>
        [Fact]
        public async Task AddOperationEntitiesBeforeSavingAsync_Pass_TracksAuditedRelatedEntries()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            IAuditHandler auditHandler = SeedManyToManyRelatedEntry(dbContext);

            // Act
            await auditHandler.AddOperationEntitiesBeforeSavingAsync();

            // Assert
            dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>().Should().NotBeEmpty();
        }

        /// <summary>
        /// Test the RefreshAuditedEntries should reuse the service history retained by a previous save.
        /// </summary>
        [Fact]
        public void RefreshAuditedEntries_Pass_ReusesPreviousServiceHistory()
        {
            // Arrange
            IDbContext dbContext = CreateDbContext();
            dbContext.Add(new ServicesHistoryEntityFake { Name = "First" });
            dbContext.Add(new AuditableEntityFake { ID = 1 });

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);
            auditHandler.AddOperationEntitiesAfterSaved();

            auditHandler.HasPreviousServiceHistory.Should().BeTrue();

            dbContext.Add(new AuditableEntityFake { ID = 2 });

            // Act
            Action act = () => auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            // Assert
            // No new service history was added, so the retained one has to be reused rather than
            // raising MissingServiceHistoryException.
            act.Should().NotThrow();
            auditHandler.HasPreviousServiceHistory.Should().BeTrue();
        }

        private static IDbContext CreateDbContext()
        {
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            return DbContextFakeFactory.Create(serviceCollection);
        }

        /// <summary>
        /// Tracks a many-to-many link between two auditable entities.
        /// </summary>
        /// <remarks>
        /// The implicit join entity is itself not auditable but is referenced by skip navigations marked
        /// with <see cref="ZDatabase.Attributes.AuditableRelationAttribute"/>, which is the only shape that
        /// lands an entry in the handler's related-entry list rather than its ordinary audited-entry list.
        /// </remarks>
        private static IAuditHandler SeedManyToManyRelatedEntry(IDbContext dbContext)
        {
            AuditableEntityFake principal = new();
            dbContext.Add(principal);
            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            ChildAuditableEntityFake child = new()
            {
                AuditableEntity = principal,
                AuditableEntityID = principal.ID,
            };
            dbContext.Add(child);
            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            dbContext.Add(new ManyToManyChildAuditableEntityFake
            {
                ChildAuditableEntities = [child],
            });
            dbContext.Add(new ServicesHistoryEntityFake());

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);
            auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

            return auditHandler;
        }
    }
}

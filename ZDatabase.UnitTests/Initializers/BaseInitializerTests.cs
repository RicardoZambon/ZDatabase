using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.UnitTests.Fakes.InitializersFake;

namespace ZDatabase.UnitTests.Initializers
{
    /// <summary>
    /// Unit tests for <see cref="ZDatabase.Initializers.BaseInitializer{TServicesHistory}"/>.
    /// </summary>
    public class BaseInitializerTests
    {
        /// <summary>
        /// Test the Initialize should run the concrete implementation.
        /// </summary>
        [Fact]
        public void Initialize_Pass_RunsConcreteImplementation()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            // Act
            initializer.Initialize();

            // Assert
            initializer.InitializeWasCalled.Should().BeTrue();
        }

        /// <summary>
        /// Test the AuditHandler should resolve the handler registered on the context.
        /// </summary>
        [Fact]
        public void AuditHandler_Pass_ResolvesFromContext()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            // Act
            IAuditHandler? auditHandler = initializer.ExposedAuditHandler;

            // Assert
            auditHandler.Should().NotBeNull();
        }

        /// <summary>
        /// Test the AuditHandler should be null when the context is not a <see cref="DbContext"/>.
        /// </summary>
        [Fact]
        public void AuditHandler_Fail_WhenContextIsNotDbContext()
        {
            // Arrange
            IDbContext dbContext = Substitute.For<IDbContext>();
            BaseInitializerFake initializer = new(dbContext);

            // Act
            IAuditHandler? auditHandler = initializer.ExposedAuditHandler;

            // Assert
            auditHandler.Should().BeNull();
        }

        /// <summary>
        /// Test the BeginTransaction should start a transaction and register a named service history.
        /// </summary>
        [Fact]
        public void BeginTransaction_Pass_RegistersServiceHistory()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            // Act
            using IDbContextTransaction transaction = initializer.ExposedBeginTransaction("SeedMethod");

            // Assert
            transaction.Should().NotBeNull();

            ServicesHistoryEntityFake serviceHistory = dbContext.ChangeTracker
                .Entries<ServicesHistoryEntityFake>()
                .Select(x => x.Entity)
                .Single();

            serviceHistory.Name.Should().Be($"{nameof(BaseInitializerFake)}\\SeedMethod");
        }

        /// <summary>
        /// Test the SaveContext should persist added entities.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_AddsEntities()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake[] entities =
            [
                new() { ID = 1, Name = "First" },
                new() { ID = 2, Name = "Second" },
            ];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_AddsEntities));

            // Assert
            // Checked before querying the set, which would re-track the saved entities.
            dbContext.ChangeTracker.Entries().Should().BeEmpty();
            dbContext.Set<NamedEntityFake>().Should().HaveCount(2);
        }

        /// <summary>
        /// Test the SaveContext should update entities when the state is modified.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_UpdatesEntities()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake entity = new() { ID = 1, Name = "Original" };
            initializer.ExposedSaveContext([entity], "Seed");

            entity.Name = "Renamed";

            // Act
            initializer.ExposedSaveContext([entity], nameof(SaveContext_Pass_UpdatesEntities), EntityState.Modified);

            // Assert
            dbContext.Set<NamedEntityFake>().Single().Name.Should().Be("Renamed");
        }

        /// <summary>
        /// Test the SaveContext should remove entities when the state is deleted.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_RemovesEntities()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake entity = new() { ID = 1, Name = "Doomed" };
            initializer.ExposedSaveContext([entity], "Seed");

            // Act
            initializer.ExposedSaveContext([entity], nameof(SaveContext_Pass_RemovesEntities), EntityState.Deleted);

            // Assert
            dbContext.Set<NamedEntityFake>().Should().BeEmpty();
        }

        /// <summary>
        /// Test the SaveContext should toggle identity insert around an identity-forced save.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_TogglesIdentityInsert()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake[] entities = [new() { ID = 1, Name = "First" }];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_TogglesIdentityInsert), EntityState.Added, forceIdentityInsert: true);

            // Assert
            initializer.IdentityInsertToggles.Should().Equal(true, false);
        }

        /// <summary>
        /// Test the SaveContext should not toggle identity insert when it is not forced.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_SkipsIdentityInsertWhenNotForced()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake[] entities = [new() { ID = 1, Name = "First" }];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_SkipsIdentityInsertWhenNotForced), EntityState.Added, forceIdentityInsert: false);

            // Assert
            initializer.IdentityInsertToggles.Should().BeEmpty();
        }

        /// <summary>
        /// Test the default SetIdentityInsert implementation should do nothing and leave the save intact.
        /// </summary>
        [Fact]
        public void SetIdentityInsert_Pass_DefaultImplementationDoesNothing()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            DefaultIdentityInsertInitializerFake initializer = new(dbContext);

            NamedEntityFake[] entities = [new() { ID = 1, Name = "First" }];

            // Act
            Action act = () => initializer.ExposedSaveContext(entities, nameof(SetIdentityInsert_Pass_DefaultImplementationDoesNothing), EntityState.Added, forceIdentityInsert: true);

            // Assert
            act.Should().NotThrow();
            dbContext.Set<NamedEntityFake>().Should().HaveCount(1);
        }

        /// <summary>
        /// Test the SaveContext should roll back and rethrow when saving fails.
        /// </summary>
        [Fact]
        public void SaveContext_Fail_RollsBackAndRethrows()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake duplicated = new() { ID = 1, Name = "Duplicated" };
            initializer.ExposedSaveContext([duplicated], "Seed");

            // Adding a second entity with an identifier already tracked makes SaveChanges throw.
            NamedEntityFake[] conflicting = [new() { ID = 1, Name = "Conflict" }];

            // Act
            Action act = () => initializer.ExposedSaveContext(conflicting, nameof(SaveContext_Fail_RollsBackAndRethrows));

            // Assert
            act.Should().Throw<Exception>();
            dbContext.Set<NamedEntityFake>().Should().HaveCount(1);
        }

        /// <summary>
        /// Test the SaveContext should assign sequential identifiers to entities that have none.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_AssignsIdentifiersToUnmatchedEntities()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake[] entities =
            [
                new() { ID = 0, Name = "First" },
                new() { ID = 0, Name = "Second" },
            ];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_AssignsIdentifiersToUnmatchedEntities), x => x.Name);

            // Assert
            entities.Select(x => x.ID).Should().Equal(1L, 2L);
            dbContext.Set<NamedEntityFake>().Should().HaveCount(2);
        }

        /// <summary>
        /// Test the SaveContext should continue numbering above the highest identifier already stored.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_ContinuesFromHighestStoredIdentifier()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            initializer.ExposedSaveContext([new NamedEntityFake { ID = 10, Name = "Existing" }], "Seed");

            NamedEntityFake[] entities = [new() { ID = 0, Name = "Fresh" }];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_ContinuesFromHighestStoredIdentifier), x => x.Name);

            // Assert
            entities.Single().ID.Should().Be(11L);
        }

        /// <summary>
        /// Test the SaveContext should skip entities whose checked property already exists.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_SkipsEntitiesAlreadyPresent()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            initializer.ExposedSaveContext([new NamedEntityFake { ID = 1, Name = "Existing" }], "Seed");

            NamedEntityFake[] entities =
            [
                new() { ID = 5, Name = "Existing" },
                new() { ID = 0, Name = "Brand new" },
            ];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_SkipsEntitiesAlreadyPresent), x => x.Name);

            // Assert
            entities[0].ID.Should().Be(0L, "an entity whose property already exists is skipped");
            entities[1].ID.Should().BeGreaterThan(0L);

            dbContext.Set<NamedEntityFake>().Select(x => x.Name).Should().BeEquivalentTo(["Existing", "Brand new"]);
        }

        /// <summary>
        /// Test the SaveContext should start numbering above the highest identifier in the incoming set.
        /// </summary>
        [Fact]
        public void SaveContext_Pass_StartsAboveHighestIncomingIdentifier()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            BaseInitializerFake initializer = new(dbContext);

            NamedEntityFake[] entities =
            [
                new() { ID = 20, Name = "Explicit" },
                new() { ID = 0, Name = "Generated" },
            ];

            // Act
            initializer.ExposedSaveContext(entities, nameof(SaveContext_Pass_StartsAboveHighestIncomingIdentifier), x => x.Name);

            // Assert
            entities[1].ID.Should().Be(21L);
        }
    }
}

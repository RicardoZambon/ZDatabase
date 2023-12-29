using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.UnitTests.Fakes.EntitiesFake;
using ZDatabase.UnitTests.Fakes.ServicesFake;

namespace ZDatabase.UnitTests.Services
{
    public class AuditHandlerTests
    {
        [Theory]
        [InlineData(new long[] { 1 })]
        [InlineData(new long[] { 2, 3 })]
        public async Task AddOperationEntitiesBeforeSaving_Fail_WhenThereIsNoServiceHistory(long[] entityIds)
        {
            // Arrange
            ServicesHistoryEntityFake historyService = new();

            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(historyService);
            foreach (long entityId in entityIds)
            {
                await dbContext.AddAsync(new AuditableEntityFake() { ID = entityId });
            }

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Act
            Action act = () =>
            {
                auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

                auditHandler.AddOperationEntitiesBeforeSaving();
            };

            // Assert
            act.Should().NotThrow();

            IEnumerable<EntityEntry<OperationsHistoryEntityFake>> operationsHistory = dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>();
            operationsHistory.Should().HaveCount(entityIds.Length);

            foreach (EntityEntry<AuditableEntityFake> entityEntry in dbContext.ChangeTracker.Entries<AuditableEntityFake>())
            {
                OperationsHistoryEntityFake operationHistory = operationsHistory.Where(x => x.Entity.EntityID == entityEntry.Entity.ID).First().Entity;
                operationHistory.Should().NotBeNull();

                operationHistory.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
                operationHistory.ID.Should().Be(operationHistory.ID);
                operationHistory.NewValues.Should().Be(JsonSerializer.Serialize(GetNewValues(entityEntry, EntityState.Added)));
                operationHistory.OldValues.Should().Be(JsonSerializer.Serialize(GetOldValues(entityEntry, EntityState.Added)));
                operationHistory.OperationType = EntityState.Added.ToString();
                operationHistory.ServiceHistoryID.Should().Be(historyService.ID);
                operationHistory.TableName.Should().Be(entityEntry.Metadata.DisplayName());
            }
        }

        [Theory]
        [InlineData(new long[] { 1 })]
        [InlineData(new long[] { 2, 3 })]
        public async Task AddOperationEntitiesBeforeSaving_Pass_OperationsForAddedEntities(long[] entityIds)
        {
            // Arrange
            ServicesHistoryEntityFake historyService = new();

            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(historyService);
            foreach (long entityId in entityIds)
            {
                await dbContext.AddAsync(new AuditableEntityFake() { ID = entityId });
            }

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Act
            Action act = () =>
            {
                auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

                auditHandler.AddOperationEntitiesBeforeSaving();
            };

            // Assert
            act.Should().NotThrow();

            IEnumerable<EntityEntry<OperationsHistoryEntityFake>> operationsHistory = dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>();
            operationsHistory.Should().HaveCount(entityIds.Length);

            foreach (EntityEntry<AuditableEntityFake> entityEntry in dbContext.ChangeTracker.Entries<AuditableEntityFake>())
            {
                OperationsHistoryEntityFake operationHistory = operationsHistory.Where(x => x.Entity.EntityID == entityEntry.Entity.ID).First().Entity;
                operationHistory.Should().NotBeNull();

                operationHistory.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
                operationHistory.ID.Should().Be(operationHistory.ID);
                operationHistory.NewValues.Should().Be(JsonSerializer.Serialize(GetNewValues(entityEntry, EntityState.Added)));
                operationHistory.OldValues.Should().Be(JsonSerializer.Serialize(GetOldValues(entityEntry, EntityState.Added)));
                operationHistory.OperationType = EntityState.Added.ToString();
                operationHistory.ServiceHistoryID.Should().Be(historyService.ID);
                operationHistory.TableName.Should().Be(entityEntry.Metadata.DisplayName());
            }
        }

        [Theory]
        [InlineData(new long[] { 1 })]
        [InlineData(new long[] { 2, 3 })]
        public async Task AddOperationEntitiesBeforeSaving_Pass_OperationsForDeletedEntities(long[] entityIds)
        {
            // Arrange
            ServicesHistoryEntityFake historyService = new() { ID = 2 };

            // Add entities to database
            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            foreach (long entityId in entityIds)
            {
                await dbContext.AddAsync(new AuditableEntityFake() { ID = entityId });
            }
            dbContext.SaveChanges();

            // Then modify them
            await dbContext.AddAsync(historyService);

            foreach (long entityId in entityIds)
            {
                AuditableEntityFake? entity = await dbContext.FindAsync<AuditableEntityFake>(entityId);
                dbContext.Remove(entity!);
            }

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Act
            Action act = () =>
            {
                auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

                auditHandler.AddOperationEntitiesBeforeSaving();
            };

            // Assert
            act.Should().NotThrow();

            IEnumerable<EntityEntry<OperationsHistoryEntityFake>> operationsHistory = dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>()
                .Where(x => x.State == EntityState.Added);

            operationsHistory.Should().HaveCount(entityIds.Length);

            foreach (EntityEntry<AuditableEntityFake> entityEntry in dbContext.ChangeTracker.Entries<AuditableEntityFake>())
            {
                OperationsHistoryEntityFake operationHistory = operationsHistory.Where(x => x.Entity.EntityID == entityEntry.Entity.ID).First().Entity;
                operationHistory.Should().NotBeNull();

                operationHistory.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
                operationHistory.ID.Should().Be(operationHistory.ID);
                operationHistory.NewValues.Should().Be(JsonSerializer.Serialize(GetNewValues(entityEntry, EntityState.Deleted)));
                operationHistory.OldValues.Should().Be(JsonSerializer.Serialize(GetOldValues(entityEntry, EntityState.Deleted)));
                operationHistory.OperationType = EntityState.Deleted.ToString();
                operationHistory.ServiceHistoryID.Should().Be(historyService.ID);
                operationHistory.TableName.Should().Be(entityEntry.Metadata.DisplayName());
            }
        }

        [Theory]
        [InlineData(new long[] { 1 })]
        [InlineData(new long[] { 2, 3 })]
        public async Task AddOperationEntitiesBeforeSaving_Pass_OperationsForUpdatedEntities(long[] entityIds)
        {
            // Arrange
            ServicesHistoryEntityFake historyService = new() { ID = 2 };

            // Add entities to database
            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            foreach (long entityId in entityIds)
            {
                await dbContext.AddAsync(new AuditableEntityFake() { ID = entityId });
            }
            dbContext.SaveChanges();

            // Then modify them
            await dbContext.AddAsync(historyService);
            IDictionary<long, Guid[]> changedGuids = new Dictionary<long, Guid[]>();

            foreach (long entityId in entityIds)
            {
                AuditableEntityFake? entity = await dbContext.FindAsync<AuditableEntityFake>(entityId);

                Guid newGuid = Guid.NewGuid();
                changedGuids.Add(entity!.ID, new Guid[] { entity.RandomGuid, newGuid });
                entity.RandomGuid = newGuid;
                dbContext.Update(entity);
            }

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Act
            Action act = () =>
            {
                auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);

                auditHandler.AddOperationEntitiesBeforeSaving();
            };

            // Assert
            act.Should().NotThrow();

            IEnumerable<EntityEntry<OperationsHistoryEntityFake>> operationsHistory = dbContext.ChangeTracker.Entries<OperationsHistoryEntityFake>()
                .Where(x => x.State == EntityState.Added);

            operationsHistory.Should().HaveCount(entityIds.Length);

            foreach (EntityEntry<AuditableEntityFake> entityEntry in dbContext.ChangeTracker.Entries<AuditableEntityFake>())
            {
                entityEntry.OriginalValues[nameof(AuditableEntityFake.RandomGuid)].Should().Be(changedGuids[entityEntry.Entity.ID][0]);
                entityEntry.CurrentValues[nameof(AuditableEntityFake.RandomGuid)].Should().Be(changedGuids[entityEntry.Entity.ID][1]);

                OperationsHistoryEntityFake operationHistory = operationsHistory.Where(x => x.Entity.EntityID == entityEntry.Entity.ID).First().Entity;
                operationHistory.Should().NotBeNull();

                operationHistory.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
                operationHistory.ID.Should().Be(operationHistory.ID);
                operationHistory.NewValues.Should().Be(JsonSerializer.Serialize(GetNewValues(entityEntry, EntityState.Modified)));
                operationHistory.OldValues.Should().Be(JsonSerializer.Serialize(GetOldValues(entityEntry, EntityState.Modified)));
                operationHistory.OperationType = EntityState.Modified.ToString();
                operationHistory.ServiceHistoryID.Should().Be(historyService.ID);
                operationHistory.TableName.Should().Be(entityEntry.Metadata.DisplayName());
            }
        }

        [Fact]
        public async Task RefreshAuditedEntries_Fail_DuplicatedHistoryService()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.AddAsync(new AuditableEntityFake());
            await dbContext.AddAsync(new ServicesHistoryEntityFake());

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Act
            Action act = () =>
            {
                auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);
            };

            // Assert
            act.Should().Throw<DuplicatedServiceAuditHistoryException>();
        }

        [Fact]
        public async Task RefreshAuditedEntries_Pass_HasHistoryService()
        {
            // Arrange
            AuditableEntityFake auditableEntityFake = new();
            auditableEntityFake.Children.Add(new ChildAuditableEntityFake());

            IDbContext dbContext = DbContextFakeFactory.Create();
            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.AddAsync(auditableEntityFake);

            IAuditHandler auditHandler = new AuditHandlerFake(dbContext);

            // Act
            Action act = () =>
            {
                auditHandler.RefreshAuditedEntries(dbContext.ChangeTracker);
            };

            // Assert
            act.Should().NotThrow();
        }

        #region Private methods

        private static Dictionary<string, object?> GetOldValues(EntityEntry entityEntry, EntityState state)
        {
            return state switch
            {
                EntityState.Deleted => entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.OriginalValue),
                EntityState.Modified => entityEntry.Properties.Where(x => IsValueModified(x)).ToDictionary(k => k.Metadata.Name, v => v.OriginalValue),
                _ => new Dictionary<string, object?>(),
            };
        }

        private static Dictionary<string, object?> GetNewValues(EntityEntry entityEntry, EntityState state)
        {
            return state switch
            {
                EntityState.Added => entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.CurrentValue),
                EntityState.Modified => entityEntry.Properties.Where(x => IsValueModified(x)).ToDictionary(k => k.Metadata.Name, v => v.CurrentValue),
                _ => new Dictionary<string, object?>(),
            };
        }

        private static bool IsValueModified(PropertyEntry property)
        {
            return property.IsModified
            && (
                property.OriginalValue != null && property.CurrentValue == null
                || property.OriginalValue == null && property.CurrentValue != null
                || (
                    property.OriginalValue != null && property.CurrentValue != null
                    && !property.OriginalValue.Equals(property.CurrentValue)
                )
            );
        }

        #endregion
    }
}
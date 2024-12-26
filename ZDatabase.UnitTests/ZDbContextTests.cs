using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Factories;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="ZDbContext{TDbContext}"/>.
    /// </summary>
    public class ZDbContextTests
    {
        /// <summary>
        /// Test the CreateProxy should create proxy entries.
        /// </summary>
        [Fact]
        public void CreateProxy_Pass_CreateProxyEntry()
        {
            // Arrange
            EntityFake? entityFake = null;

            IDbContext dbContext = DbContextFakeFactory.Create();

            // Act
            Action act = () =>
            {
                entityFake = dbContext.CreateProxy<EntityFake>(x => { });
            };

            // Assert
            act.Should().NotThrow();

            entityFake.Should().NotBeNull();
        }

        /// <summary>
        /// Test the SaveChanges method should fail when saving auditable entity and is missing service history.
        /// </summary>
        [Fact]
        public void SaveChanges_Fail_WhenThereIsNoServiceHistory()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();

            dbContext.Add(new AuditableEntityFake());

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().Throw<MissingServiceHistoryException>();
        }

        /// <summary>
        /// Test the SaveChanges method should have added audited entries.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedAuditedEntries()
        {
            // Arrange
            long expectedCurrentUserID = 1;
            DateTime startRunningDateTime = DateTime.UtcNow;

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(expectedCurrentUserID);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.Add(new AuditableEntityFake());

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            dbContext.Set<AuditableEntityFake>().Should().HaveCount(1);

            AuditableEntityFake? auditableEntity = dbContext.Set<AuditableEntityFake>().FirstOrDefault();
            auditableEntity.Should().NotBeNull();
            auditableEntity!.ID.Should().BeGreaterThan(0);
            auditableEntity!.CreatedByID.Should().Be(expectedCurrentUserID);
            auditableEntity!.CreatedOn.Should().BeAfter(startRunningDateTime);
            auditableEntity!.CreatedOn.Should().BeBefore(DateTime.UtcNow);
        }

        /// <summary>
        /// Test the SaveChanges method should have added operations history when deleting.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedOperationsHistoryWhenDeleting()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = dbContext.Add(entity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Delete the auditable entity.
            entity.RandomGuid = Guid.NewGuid();
            entityEntry = dbContext.Remove(entity);

            dbContext.Add(new ServicesHistoryEntityFake());

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.EntityID == entity.ID && x.OperationType == EntityState.Deleted.ToString());
            operationsHistory.Should().HaveCount(1);

            OperationsHistoryEntityFake? operationHistory = operationsHistory.FirstOrDefault();
            operationHistory.Should().NotBeNull();
            operationHistory!.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
            operationHistory!.NewValues.Should().Be(JsonSerializer.Serialize(new Dictionary<string, object?>()));
            operationHistory!.OldValues.Should().Be(JsonSerializer.Serialize(entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.OriginalValue)));
            operationHistory!.TableName.Should().Be(entityEntry.Metadata.GetTableName());
        }

        /// <summary>
        /// Test the SaveChanges method should have added operations history when inserting.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedOperationsHistoryWhenInserting()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = dbContext.Add(entity);

            dbContext.Add(new ServicesHistoryEntityFake());

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.EntityID == entity.ID && x.OperationType == EntityState.Added.ToString());
            operationsHistory.Should().HaveCount(1);

            OperationsHistoryEntityFake? operationHistory = operationsHistory.FirstOrDefault();
            operationHistory.Should().NotBeNull();
            operationHistory!.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
            operationHistory!.NewValues.Should().Be(JsonSerializer.Serialize(entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.CurrentValue)));
            operationHistory!.OldValues.Should().Be(JsonSerializer.Serialize(new Dictionary<string, object?>()));
            operationHistory!.TableName.Should().Be(entityEntry.Metadata.GetTableName());
        }

        /// <summary>
        /// Test the SaveChanges method should have added operations history when updating.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedOperationsHistoryWhenUpdating()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = dbContext.Add(entity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Update the auditable entity.
            entity.RandomGuid = Guid.NewGuid();
            entityEntry = dbContext.Update(entity);

            IDictionary<string, object?> oldValuesDictionary = entityEntry.Properties.Where(x =>
                x.IsModified
                && ((x.OriginalValue != null && x.CurrentValue == null)
                    || (x.OriginalValue == null && x.CurrentValue != null)
                    || (x.OriginalValue != null && x.CurrentValue != null && !x.OriginalValue.Equals(x.CurrentValue))
                )
            ).ToDictionary(k => k.Metadata.Name, v => v.OriginalValue);
            oldValuesDictionary.Add(new KeyValuePair<string, object?>(nameof(entity.LastChangedOn), entity.LastChangedOn));

            IDictionary<string, object?> newValuesDictionary = entityEntry.Properties.Where(x =>
                x.IsModified
                && ((x.OriginalValue != null && x.CurrentValue == null)
                    || (x.OriginalValue == null && x.CurrentValue != null)
                    || (x.OriginalValue != null && x.CurrentValue != null && !x.OriginalValue.Equals(x.CurrentValue))
                )
            ).ToDictionary(k => k.Metadata.Name, v => v.CurrentValue);

            dbContext.Add(new ServicesHistoryEntityFake());

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            newValuesDictionary.Add(new KeyValuePair<string, object?>(nameof(entity.LastChangedOn), entity.LastChangedOn));

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.EntityID == entity.ID && x.OperationType == EntityState.Modified.ToString());
            operationsHistory.Should().HaveCount(1);

            string newValues = JsonSerializer.Serialize(newValuesDictionary.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value));
            string oldValues = JsonSerializer.Serialize(oldValuesDictionary.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value));

            OperationsHistoryEntityFake ? operationHistory = operationsHistory.FirstOrDefault();
            operationHistory.Should().NotBeNull();
            operationHistory!.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
            operationHistory!.NewValues.Should().Be(newValues);
            operationHistory!.OldValues.Should().Be(oldValues);
            operationHistory!.TableName.Should().Be(entityEntry.Metadata.GetTableName());
        }

        /// <summary>
        /// Test the SaveChanges method should have added related operations history when deleting auditable relations.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedRelatedOperationsHistoryWhenDeletingAuditableRelations()
        {
            // Arrange
            string serviceHistoryName = "Delete child entities";

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = dbContext.Add(entity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Insert the child auditable entity.
            ChildAuditableEntityFake childEntity = new()
            {
                Parent = entity,
                ParentID = entity.ID,
            };
            EntityEntry<ChildAuditableEntityFake> childEntityEntry = dbContext.Add(childEntity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Update the child auditable entity.
            dbContext.Remove(childEntity);

            dbContext.Add(new ServicesHistoryEntityFake() { Name = serviceHistoryName });

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            IQueryable<ServicesHistoryEntityFake> servicesHistory = dbContext.Set<ServicesHistoryEntityFake>().Where(x => x.Name == serviceHistoryName);
            servicesHistory.Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = servicesHistory.FirstOrDefault();
            serviceHistory.Should().NotBeNull();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.ServiceHistoryID == serviceHistory!.ID);
            operationsHistory.Should().HaveCount(2);

            operationsHistory.Any(x => x.EntityID == entity.ID).Should().BeTrue();
        }

        /// <summary>
        /// Test the SaveChanges method should have added related operations history when inserting auditable relations.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedRelatedOperationsHistoryWhenInsertingAuditableRelations()
        {
            // Arrange
            string serviceHistoryName = "Add child entities";

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = dbContext.Add(entity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Insert the child auditable entity.
            ChildAuditableEntityFake childEntity = new()
            {
                Parent = entity,
                ParentID = entity.ID,
            };
            EntityEntry<ChildAuditableEntityFake> childEntityEntry = dbContext.Add(childEntity);

            dbContext.Add(new ServicesHistoryEntityFake() { Name = serviceHistoryName });

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            IQueryable<ServicesHistoryEntityFake> servicesHistory = dbContext.Set<ServicesHistoryEntityFake>().Where(x => x.Name == serviceHistoryName);
            servicesHistory.Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = servicesHistory.FirstOrDefault();
            serviceHistory.Should().NotBeNull();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.ServiceHistoryID == serviceHistory!.ID);
            operationsHistory.Should().HaveCount(2);

            operationsHistory.Any(x => x.EntityID == entity.ID).Should().BeTrue();
        }

        /// <summary>
        /// Test the SaveChanges method should have added related operations history when updating auditable relations.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedRelatedOperationsHistoryWhenUpdatingAuditableRelations()
        {
            // Arrange
            string serviceHistoryName = "Update child entities";

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = dbContext.Add(entity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Insert the child auditable entity.
            ChildAuditableEntityFake childEntity = new()
            {
                Parent = entity,
                ParentID = entity.ID,
            };
            EntityEntry<ChildAuditableEntityFake> childEntityEntry = dbContext.Add(childEntity);

            dbContext.Add(new ServicesHistoryEntityFake());
            dbContext.SaveChanges();

            // Update the child auditable entity.
            childEntity.DummyValue = Guid.NewGuid().ToString();
            dbContext.Update(childEntity);

            dbContext.Add(new ServicesHistoryEntityFake() { Name = serviceHistoryName });

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            IQueryable<ServicesHistoryEntityFake> servicesHistory = dbContext.Set<ServicesHistoryEntityFake>().Where(x => x.Name == serviceHistoryName);
            servicesHistory.Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = servicesHistory.FirstOrDefault();
            serviceHistory.Should().NotBeNull();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.ServiceHistoryID == serviceHistory!.ID);
            operationsHistory.Should().HaveCount(2);

            operationsHistory.Any(x => x.EntityID == entity.ID).Should().BeTrue();
        }

        /// <summary>
        /// Test the SaveChanges method should have added service history.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveAddedServiceHistory()
        {
            // Arrange
            long expectedCurrentUserID = 1;
            string expectedServiceHistoryName = "Test service history";
            DateTime startRunningDateTime = DateTime.UtcNow;

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(expectedCurrentUserID);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            dbContext.Add(new ServicesHistoryEntityFake() { Name = expectedServiceHistoryName });

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            dbContext.Set<ServicesHistoryEntityFake>().Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = dbContext.Set<ServicesHistoryEntityFake>().FirstOrDefault();
            serviceHistory.Should().NotBeNull();
            serviceHistory!.ID.Should().BeGreaterThan(0);
            serviceHistory!.Name.Should().Be(expectedServiceHistoryName);
            serviceHistory!.ChangedByID.Should().Be(expectedCurrentUserID);
            serviceHistory!.ChangedOn.Should().BeAfter(startRunningDateTime);
            serviceHistory!.ChangedOn.Should().BeBefore(DateTime.UtcNow);
        }

        /// <summary>
        /// Test the SaveChanges method should have called audit handler methods.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldHaveCalledAuditHandlerMethods()
        {
            // Arrange
            IAuditHandler auditHandlerSubstitute = Substitute.For<IAuditHandler>();

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(auditHandlerSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            auditHandlerSubstitute.Received().RefreshAuditedEntries(Arg.Any<ChangeTracker>());
            auditHandlerSubstitute.Received().AddOperationEntitiesBeforeSaving();
            auditHandlerSubstitute.Received().AddOperationEntitiesAfterSaved();
        }

        /// <summary>
        /// Test the SaveChanges method should not require service history when only saving not auditable entities.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldNotRequireServiceHistoryWhenNotAuditable()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            EntityFake entity = new();
            EntityEntry<EntityFake> entityEntry = dbContext.Add(entity);

            // Act
            Action act = () =>
            {
                dbContext.SaveChanges();
            };

            // Assert
            act.Should().NotThrow();

            dbContext.Set<EntityFake>().FirstOrDefault().Should().NotBeNull();
        }

        /// <summary>
        /// Test the SaveChangesAsync method should fail when saving auditable entity and is missing service history.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Fail_WhenThereIsNoServiceHistory()
        {
            // Arrange
            IDbContext dbContext = DbContextFakeFactory.Create();

            dbContext.Add(new AuditableEntityFake());

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().ThrowAsync<MissingServiceHistoryException>();
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added audited entries.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedAuditedEntries()
        {
            // Arrange
            long expectedCurrentUserID = 1;
            DateTime startRunningDateTime = DateTime.UtcNow;

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(expectedCurrentUserID);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.AddAsync(new AuditableEntityFake());

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            dbContext.Set<AuditableEntityFake>().Should().HaveCount(1);

            AuditableEntityFake? auditableEntity = await dbContext.Set<AuditableEntityFake>().FirstOrDefaultAsync();
            auditableEntity.Should().NotBeNull();
            auditableEntity!.ID.Should().BeGreaterThan(0);
            auditableEntity!.CreatedByID.Should().Be(expectedCurrentUserID);
            auditableEntity!.CreatedOn.Should().BeAfter(startRunningDateTime);
            auditableEntity!.CreatedOn.Should().BeBefore(DateTime.UtcNow);
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added operations history when deleting.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedOperationsHistoryWhenInserting()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = await dbContext.AddAsync(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.EntityID == entity.ID && x.OperationType == EntityState.Added.ToString());
            operationsHistory.Should().HaveCount(1);

            OperationsHistoryEntityFake? operationHistory = await operationsHistory.FirstOrDefaultAsync();
            operationHistory.Should().NotBeNull();
            operationHistory!.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
            operationHistory!.NewValues.Should().Be(JsonSerializer.Serialize(entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.CurrentValue)));
            operationHistory!.OldValues.Should().Be(JsonSerializer.Serialize(new Dictionary<string, object?>()));
            operationHistory!.TableName.Should().Be(entityEntry.Metadata.GetTableName());
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added operations history when updating.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedOperationsHistoryWhenUpdating()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = await dbContext.AddAsync(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Update the auditable entity.
            entity.RandomGuid = Guid.NewGuid();
            entityEntry = dbContext.Update(entity);

            IDictionary<string, object?> oldValuesDictionary = entityEntry.Properties.Where(x =>
                x.IsModified
                && ((x.OriginalValue != null && x.CurrentValue == null)
                    || (x.OriginalValue == null && x.CurrentValue != null)
                    || (x.OriginalValue != null && x.CurrentValue != null && !x.OriginalValue.Equals(x.CurrentValue))
                )
            ).ToDictionary(k => k.Metadata.Name, v => v.OriginalValue);
            oldValuesDictionary.Add(new KeyValuePair<string, object?>(nameof(entity.LastChangedOn), entity.LastChangedOn));

            IDictionary<string, object?> newValuesDictionary = entityEntry.Properties.Where(x =>
                x.IsModified
                && ((x.OriginalValue != null && x.CurrentValue == null)
                    || (x.OriginalValue == null && x.CurrentValue != null)
                    || (x.OriginalValue != null && x.CurrentValue != null && !x.OriginalValue.Equals(x.CurrentValue))
                )
            ).ToDictionary(k => k.Metadata.Name, v => v.CurrentValue);

            dbContext.Add(new ServicesHistoryEntityFake());

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            newValuesDictionary.Add(new KeyValuePair<string, object?>(nameof(entity.LastChangedOn), entity.LastChangedOn));

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.EntityID == entity.ID && x.OperationType == EntityState.Modified.ToString());
            operationsHistory.Should().HaveCount(1);

            string newValues = JsonSerializer.Serialize(newValuesDictionary.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value));
            string oldValues = JsonSerializer.Serialize(oldValuesDictionary.OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value));

            OperationsHistoryEntityFake? operationHistory = await operationsHistory.FirstOrDefaultAsync();
            operationHistory.Should().NotBeNull();
            operationHistory!.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
            operationHistory!.NewValues.Should().Be(newValues);
            operationHistory!.OldValues.Should().Be(oldValues);
            operationHistory!.TableName.Should().Be(entityEntry.Metadata.GetTableName());
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added operations history when deleting.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedOperationsHistoryWhenDeleting()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = await dbContext.AddAsync(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Delete the auditable entity.
            entity.RandomGuid = Guid.NewGuid();
            entityEntry = dbContext.Remove(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.EntityID == entity.ID && x.OperationType == EntityState.Deleted.ToString());
            operationsHistory.Should().HaveCount(1);

            OperationsHistoryEntityFake? operationHistory = await operationsHistory.FirstOrDefaultAsync();
            operationHistory.Should().NotBeNull();
            operationHistory!.EntityName.Should().Be(entityEntry.Metadata.DisplayName());
            operationHistory!.NewValues.Should().Be(JsonSerializer.Serialize(new Dictionary<string, object?>()));
            operationHistory!.OldValues.Should().Be(JsonSerializer.Serialize(entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.OriginalValue)));
            operationHistory!.TableName.Should().Be(entityEntry.Metadata.GetTableName());
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added related operations history when deleting auditable relations.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedRelatedOperationsHistoryWhenDeletingAuditableRelations()
        {
            // Arrange
            string serviceHistoryName = "Delete child entities";

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = await dbContext.AddAsync(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Insert the child auditable entity.
            ChildAuditableEntityFake childEntity = new()
            {
                Parent = entity,
                ParentID = entity.ID,
            };
            EntityEntry<ChildAuditableEntityFake> childEntityEntry = await dbContext.AddAsync(childEntity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Update the child auditable entity.
            dbContext.Remove(childEntity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake() { Name = serviceHistoryName });

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            IQueryable<ServicesHistoryEntityFake> servicesHistory = dbContext.Set<ServicesHistoryEntityFake>().Where(x => x.Name == serviceHistoryName);
            servicesHistory.Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = await servicesHistory.FirstOrDefaultAsync();
            serviceHistory.Should().NotBeNull();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.ServiceHistoryID == serviceHistory!.ID);
            operationsHistory.Should().HaveCount(2);

            (await operationsHistory.AnyAsync(x => x.EntityID == entity.ID)).Should().BeTrue();
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added related operations history when inserting auditable relations.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedRelatedOperationsHistoryWhenInsertingAuditableRelations()
        {
            // Arrange
            string serviceHistoryName = "Add child entities";

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = await dbContext.AddAsync(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Insert the child auditable entity.
            ChildAuditableEntityFake childEntity = new()
            {
                Parent = entity,
                ParentID = entity.ID,
            };
            EntityEntry<ChildAuditableEntityFake> childEntityEntry = dbContext.Add(childEntity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake() { Name = serviceHistoryName });

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            IQueryable<ServicesHistoryEntityFake> servicesHistory = dbContext.Set<ServicesHistoryEntityFake>().Where(x => x.Name == serviceHistoryName);
            servicesHistory.Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = await servicesHistory.FirstOrDefaultAsync();
            serviceHistory.Should().NotBeNull();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.ServiceHistoryID == serviceHistory!.ID);
            operationsHistory.Should().HaveCount(2);

            (await operationsHistory.AnyAsync(x => x.EntityID == entity.ID)).Should().BeTrue();
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added related operations history when updating auditable relations.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedRelatedOperationsHistoryWhenUpdatingAuditableRelations()
        {
            // Arrange
            string serviceHistoryName = "Update child entities";

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            AuditableEntityFake entity = new();
            EntityEntry<AuditableEntityFake> entityEntry = await dbContext.AddAsync(entity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Insert the child auditable entity.
            ChildAuditableEntityFake childEntity = new()
            {
                Parent = entity,
                ParentID = entity.ID,
            };
            EntityEntry<ChildAuditableEntityFake> childEntityEntry = await dbContext.AddAsync(childEntity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake());
            await dbContext.SaveChangesAsync();
            dbContext.ClearAuditServiceHistory();

            // Update the child auditable entity.
            childEntity.DummyValue = Guid.NewGuid().ToString();
            dbContext.Update(childEntity);

            await dbContext.AddAsync(new ServicesHistoryEntityFake() { Name = serviceHistoryName });

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            IQueryable<ServicesHistoryEntityFake> servicesHistory = dbContext.Set<ServicesHistoryEntityFake>().Where(x => x.Name == serviceHistoryName);
            servicesHistory.Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = await servicesHistory.FirstOrDefaultAsync();
            serviceHistory.Should().NotBeNull();

            IQueryable<OperationsHistoryEntityFake> operationsHistory = dbContext.Set<OperationsHistoryEntityFake>().Where(x => x.ServiceHistoryID == serviceHistory!.ID);
            operationsHistory.Should().HaveCount(2);

            (await operationsHistory.AnyAsync(x => x.EntityID == entity.ID)).Should().BeTrue();
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have added service history.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveAddedServiceHistory()
        {
            // Arrange
            long expectedCurrentUserID = 1;
            string expectedServiceHistoryName = "Test service history";
            DateTime startRunningDateTime = DateTime.UtcNow;

            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(expectedCurrentUserID);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            await dbContext.AddAsync(new ServicesHistoryEntityFake() { Name = expectedServiceHistoryName });

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            dbContext.Set<ServicesHistoryEntityFake>().Should().HaveCount(1);

            ServicesHistoryEntityFake? serviceHistory = await dbContext.Set<ServicesHistoryEntityFake>().FirstOrDefaultAsync();
            serviceHistory.Should().NotBeNull();
            serviceHistory!.ID.Should().BeGreaterThan(0);
            serviceHistory!.Name.Should().Be(expectedServiceHistoryName);
            serviceHistory!.ChangedByID.Should().Be(expectedCurrentUserID);
            serviceHistory!.ChangedOn.Should().BeAfter(startRunningDateTime);
            serviceHistory!.ChangedOn.Should().BeBefore(DateTime.UtcNow);
        }

        /// <summary>
        /// Test the SaveChangesAsync method should have called audit handler methods.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldHaveCalledAuditHandlerMethods()
        {
            // Arrange
            IAuditHandler auditHandlerSubstitute = Substitute.For<IAuditHandler>();

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(auditHandlerSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            auditHandlerSubstitute.Received().RefreshAuditedEntries(Arg.Any<ChangeTracker>());
            await auditHandlerSubstitute.Received().AddOperationEntitiesBeforeSavingAsync();
            await auditHandlerSubstitute.Received().AddOperationEntitiesAfterSavedAsync();
        }

        /// <summary>
        /// Test the SaveChangesAsync method should not require service history when not auditable.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldNotRequireServiceHistoryWhenNotAuditable()
        {
            // Arrange
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(1);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            IDbContext dbContext = DbContextFakeFactory.Create(serviceCollection);

            // Insert the auditable entity.
            EntityFake entity = new();
            EntityEntry<EntityFake> entityEntry = await dbContext.AddAsync(entity);

            // Act
            Func<Task> act = async () =>
            {
                await dbContext.SaveChangesAsync();
            };

            // Assert
            await act.Should().NotThrowAsync();

            (await dbContext.Set<EntityFake>().FirstOrDefaultAsync()).Should().NotBeNull();
        }
    }
}
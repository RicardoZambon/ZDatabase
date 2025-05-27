using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using System.Text.Json;
using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;
using ZDatabase.SpecFlowTests.Fakes.Services;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Services
{
    [Binding]
    [Scope(Feature = "Audit Handler")]
    public class AuditHandlerSteps
    {
        private IDbContext _dbContext;
        private AuditHandlerFake _auditHandler;
        private IList<AuditableFake> _entities = [];
        private Exception? _caughtException = null;
        private ServicesHistoryFake? _serviceHistory = null;

        public AuditHandlerSteps()
        {
            _dbContext = DbContextFakeFactory.Create();
            _auditHandler = new AuditHandlerFake(_dbContext);
        }

        [Given(@"(a new|existing) ServicesHistoryFake and (\d+) new AuditableFake\(s\)")]
        public void GivenANewServicesHistoryFakeAndNewAuditableFakes(string operation, int count)
        {
            _serviceHistory = new ServicesHistoryFake();
            _dbContext.Add(_serviceHistory);

            for (int i = 0; i < count; i++)
            {
                AuditableFake entity = new() { ID = i + 1 };
                _entities.Add(entity);
                _dbContext.Add(entity);
            }

            if (operation == "existing")
            {
                _dbContext.SaveChanges();
            }
        }

        [Given(@"(\d+) new AuditableFake\(s\) and no ServicesHistoryFake")]
        public void GivenNewAuditableFakesAndNoServicesHistoryFake(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AuditableFake entity = new() { ID = i + 1 };
                _entities.Add(entity);
                _dbContext.Add(entity);
            }
        }

        [When(@"AddOperationEntitiesBeforeSaving is called")]
        public void WhenAddOperationEntitiesBeforeSavingIsCalled()
        {
            try
            {
                _auditHandler.RefreshAuditedEntries(_dbContext.ChangeTracker);
                _auditHandler.AddOperationEntitiesBeforeSaving();
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then(@"(\d+) OperationsHistoryFake\(s\) should be added for state (Added|Deleted|Modified)")]
        public void ThenOperationsHistoryShouldBeAddedForState(int count, string state)
        {
            IEnumerable<EntityEntry<OperationsHistoryFake>> entries = _dbContext.ChangeTracker.Entries<OperationsHistoryFake>()
                .Where(x => x.State == EntityState.Added)
                .ToList();

            entries.Should().HaveCount(count);

            foreach (AuditableFake entity in _entities)
            {
                EntityEntry<AuditableFake> entry = _dbContext.Entry(entity);

                OperationsHistoryFake operationHistory = entries.Where(x => x.Entity.EntityID == entity.ID && x.Entity.OperationType == state).Single().Entity;
                operationHistory.Should().NotBeNull();

                operationHistory.EntityName.Should().Be(entry.Metadata.DisplayName());
                operationHistory.ID.Should().Be(operationHistory.ID);
                operationHistory.NewValues.Should().Be(JsonSerializer.Serialize(GetNewValues(entry, state)));
                operationHistory.OldValues.Should().Be(JsonSerializer.Serialize(GetOldValues(entry, state)));
                operationHistory.ServiceHistoryID.Should().Be(_serviceHistory!.ID);
                operationHistory.TableName.Should().Be(entry.Metadata.DisplayName());
            }
        }

        [When(@"the entities are deleted and AddOperationEntitiesBeforeSaving is called")]
        public void WhenTheEntitiesAreDeletedAndAddOperationEntitiesBeforeSavingIsCalled()
        {
            _serviceHistory = new ServicesHistoryFake();
            _dbContext.Add(_serviceHistory);

            foreach (AuditableFake entity in _entities)
            {
                _dbContext.Remove(entity);
            }

            _auditHandler.RefreshAuditedEntries(_dbContext.ChangeTracker);
            _auditHandler.AddOperationEntitiesBeforeSaving();
        }

        [When(@"the entities are updated and AddOperationEntitiesBeforeSaving is called")]
        public void WhenTheEntitiesAreUpdatedAndAddOperationEntitiesBeforeSavingIsCalled()
        {
            _serviceHistory = new ServicesHistoryFake();
            _dbContext.Add(_serviceHistory);

            foreach (AuditableFake? entity in _entities)
            {
                entity.RandomGuid = Guid.NewGuid();
                _dbContext.Update(entity);
            }

            _auditHandler.RefreshAuditedEntries(_dbContext.ChangeTracker);
            _auditHandler.AddOperationEntitiesBeforeSaving();
        }

        [Given(@"two ServicesHistoryFake and one AuditableFake")]
        public void GivenTwoServicesHistoryFakeAndOneAuditableFake()
        {
            _dbContext.Add(new ServicesHistoryFake());
            _dbContext.Add(new AuditableFake());
            _dbContext.Add(new ServicesHistoryFake());
        }

        [When(@"RefreshAuditedEntries is called")]
        public void WhenRefreshAuditedEntriesIsCalled()
        {
            try
            {
                _auditHandler.RefreshAuditedEntries(_dbContext.ChangeTracker);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then(@"a (.*) should be thrown")]
        public void ThenATypeErrorShouldBeThrown(string error)
        {
            switch (error)
            {
                case "DuplicatedServiceAuditHistoryException":
                    _caughtException.Should().BeOfType<DuplicatedServiceAuditHistoryException>();
                    break;
                case "MissingServiceHistoryException":
                    _caughtException.Should().BeOfType<MissingServiceHistoryException>();
                    break;
                default:
                    throw new ArgumentException($"Unknown error type: {error}");
            }
        }

        [Given(@"one ServicesHistoryFake and one AuditableFake")]
        public void GivenOneServicesHistoryFakeAndOneAuditableFake()
        {
            _dbContext.Add(new ServicesHistoryFake());
            _dbContext.Add(new AuditableFake());
        }

        [Then(@"no exception should be thrown")]
        public void ThenNoExceptionShouldBeThrown()
        {
            _caughtException.Should().BeNull();
        }

        #region Private methods

        private static Dictionary<string, object?> GetOldValues(EntityEntry entityEntry, string state)
        {
            return state switch
            {
                "Deleted" => entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.OriginalValue),
                "Modified" => entityEntry.Properties.Where(x => IsValueModified(x)).ToDictionary(k => k.Metadata.Name, v => v.OriginalValue),
                _ => [],
            };
        }

        private static Dictionary<string, object?> GetNewValues(EntityEntry entityEntry, string state)
        {
            return state switch
            {
                "Added" => entityEntry.Properties.ToDictionary(k => k.Metadata.Name, v => v.CurrentValue),
                "Modified" => entityEntry.Properties.Where(x => IsValueModified(x)).ToDictionary(k => k.Metadata.Name, v => v.CurrentValue),
                _ => [],
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

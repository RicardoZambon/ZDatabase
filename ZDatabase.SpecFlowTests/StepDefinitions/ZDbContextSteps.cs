using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Reqnroll;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZDatabase.Entities;
using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.SpecFlowTests.Helper;

namespace ZDatabase.SpecFlowTests.StepDefinitions
{
    [Binding]
    [Scope(Feature = "ZDbContext")]
    public class ZDbContextSteps
    {
        #region Variables
        private Exception? _caughtException = null;
        private long _currentUserID = 1;
        private IDbContext _dbContext;
        private object? _proxy;
        private readonly Dictionary<string, DateTime> _savedTime = [];
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public ZDbContextSteps()
        {
            ICurrentUserProvider<long> currentUserSubstitute = Substitute.For<ICurrentUserProvider<long>>();
            currentUserSubstitute.CurrentUserID.Returns(_currentUserID);

            ServiceCollection serviceCollection = new();
            serviceCollection.AddSingleton(currentUserSubstitute);

            _dbContext = DbContextFakeFactory.Create(serviceCollection);
        }
        #endregion

        #region Public methods
        [Given(@"a new (\w+) is added to the context with properties")]
        [When(@"a new (\w+) is added to the context with properties")]
        public void GivenWhenANewEntityIsAddedToTheContextWithProperties(string entityName, Table table)
        {
            Type? entityType = TestHelper.GetEntityType(entityName);

            object? entity = Activator.CreateInstance(entityType!);
            SetEntityProperties(entityName, entityType!, entity, table);

            typeof(IDbContext).GetMethod("Add", BindingFlags.Public | BindingFlags.Instance)
               ?.MakeGenericMethod(entityType!)
               ?.Invoke(_dbContext, [entity]);
        }

        [Given("I call SaveChanges")]
        [When("I call SaveChanges")]
        public void GivenWhenICallSaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Given(@"I save the current time as (.*)")]
        [When(@"I save the current time as (.*)")]
        public void GivenWhenISaveTheCurrentTimeAs(string variableName)
        {
            _savedTime[variableName] = DateTime.UtcNow;
        }


        [When("I create a proxy for ClientsFake")]
        public void WhenICreateAProxyForClientsFake()
        {
            _proxy = _dbContext.CreateProxy<ClientsFake>(x => { });
        }

        [When(@"I delete the (\w+) with ID (.*)")]
        public void WhenIDeleteTheEntityWithID(string entityName, long entityID)
        {
            IEnumerable<object> dbSet = GetDbSet(entityName);
            object? entity = dbSet.FirstOrDefault(x => ((Entity)x).ID == entityID);
            entity.Should().NotBeNull($"Entity '{entityName}' with ID {entityID} should exist in DbSet.");

            typeof(IDbContext).GetMethod("Remove", BindingFlags.Public | BindingFlags.Instance)
                ?.MakeGenericMethod(entity!.GetType())
                ?.Invoke(_dbContext, [entity]);
        }

        [When(@"I read the (\w+) with (\w+) (.*) and update with properties")]
        public void WhenIReadTheEntityWithIDAndUpdateWithProperties(string entityName, string idProperty, long entityID, Table table)
        {
            object? entity = FindEntityById(entityName, idProperty, entityID);
            entity.Should().NotBeNull($"Entity '{entityName}' with {idProperty} {entityID} should exist in DbSet.");

            Type? entityType = TestHelper.GetEntityType(entityName);
            SetEntityProperties(entityName, entityType!, entity, table);
        }


        [Then("a (\\w+) should be thrown")]
        public void ThenAMissingServiceHistoryExceptionShouldBeThrown(string exceptionTypeName)
        {
            _caughtException.Should().NotBeNull($"An exception of type '{exceptionTypeName}' should have been thrown.");
            _caughtException.Should().BeOfType<MissingServiceHistoryException>($"Expected exception type '{exceptionTypeName}' but got '{_caughtException!.GetType().Name}'.");
        }

        [Then(@"I read the (\w+) with (\w+) (.*) should have the properties")]
        public void ThenIReadTheEntityWithIDShouldHaveTheProperties(string entityName, string idProperty, long entityID, Table table)
        {
            object? entity = FindEntityById(entityName, idProperty, entityID);
            entity.Should().NotBeNull($"Entity '{entityName}' with ID {entityID} should exist in DbSet.");

            Type? entityType = TestHelper.GetEntityType(entityName);
            foreach (DataTableRow row in table.Rows)
            {
                PropertyInfo? prop = entityType!.GetProperty(row["Property"], BindingFlags.Public | BindingFlags.Instance);
                prop.Should().NotBeNull($"Property '{row["Property"]}' should exist on '{entityName}'.");

                object? actualValue = prop!.GetValue(entity);

                switch (row["Property"])
                {
                    case "ChangedOn":
                    case "CreatedOn":
                    case "LastChangedOn":
                        ((DateTime)actualValue!).Should().BeAfter(_savedTime[row["Value"]], $"Property '{row["Property"]}' of '{entityName}' should be after {row["Value"]}.");
                        ((DateTime)actualValue!).Should().BeBefore(DateTime.UtcNow, $"Property '{row["Property"]}' of '{entityName}' should be before DateTime.UtcNow.");
                        break;

                    default:
                        if (row["Value"] == "<current user>")
                        {
                            row["Value"] = _currentUserID.ToString();
                        }

                        Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object? value = Convert.ChangeType(row["Value"], targetType);
                        actualValue.Should().Be(value, $"Property '{row["Property"]}' of '{entityName}' should match the expected value.");
                        break;
                }
            }
        }

        [Then("the proxy should be created successfully")]
        public void ThenTheProxyShouldBeCreatedSuccessfully()
        {
            _proxy.Should().NotBeNull();
        }
        #endregion

        #region Private methods
        private void AddRelationship<TEntity, TRelatedEntity>(TEntity entity, string relationship, TRelatedEntity relatedEntity)
        {
            PropertyInfo? relationshipProperty = entity!.GetType().GetProperty(relationship);
            relationshipProperty.Should().NotBeNull($"Entity should have {relationship} relationship property.");

            ICollection<TRelatedEntity> relationshipCollection = ((ICollection<TRelatedEntity>?)relationshipProperty!.GetValue(entity)) ?? [];
            relationshipCollection.Add(relatedEntity);

            relationshipProperty.SetValue(entity, relationshipCollection);
        }

        private object? FindEntityById(string entityName, string idProperty, long entityID)
        {
            IQueryable<object> dbSet = GetDbSet(entityName).IgnoreQueryFilters();

            if (dbSet.Any())
            {
                PropertyInfo? prop = dbSet.First().GetType().GetProperty(idProperty, BindingFlags.Public | BindingFlags.Instance);
                prop.Should().NotBeNull($"Property '{idProperty}' should exist on '{entityName}'.");

                foreach (object item in dbSet)
                {
                    long itemID = (long)prop!.GetValue(item)!;
                    if (itemID == entityID)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        private IQueryable<object> GetDbSet(string entityName)
        {
            Type entityType = TestHelper.GetEntityType(entityName);

            MethodInfo? method = typeof(IDbContext).GetMethod(nameof(_dbContext.Set), BindingFlags.Public | BindingFlags.Instance)
                ?.MakeGenericMethod(entityType);
            method.Should().NotBeNull($"Method 'Set<{entityName}>' should exist on DbContext.");

            IQueryable<object> dbSet = (IQueryable<object>)method!.Invoke(_dbContext, null)!;
            dbSet.Should().NotBeNull($"DbSet for '{entityName}' should exist.");

            return dbSet;
        }

        private void SetEntityProperties(string entityName, Type entityType, object? entity, Table table)
        {
            foreach (DataTableRow row in table.Rows)
            {
                PropertyInfo? prop = entityType!.GetProperty(row["Property"], BindingFlags.Public | BindingFlags.Instance);
                prop.Should().NotBeNull($"Property '{row["Property"]}' should exist on '{entityName}'.");

                if (row.ContainsKey("ForeignKey") && !string.IsNullOrWhiteSpace(row["ForeignKey"]))
                {
                    Type relatedEntityType = TestHelper.GetEntityType(row["ForeignKey"]);
                    object? relatedEntity = FindEntityById(row["ForeignKey"], row["ForeignKey Property"], Convert.ToInt64(row["Value"]));

                    GetType().GetMethod(nameof(AddRelationship), BindingFlags.Instance | BindingFlags.NonPublic)!
                        .MakeGenericMethod(entityType, relatedEntityType)
                        .Invoke(this, [entity!, row["Property"], relatedEntity!]);
                }
                else
                {
                    object? value = Convert.ChangeType(row["Value"], prop!.PropertyType);
                    prop.SetValue(entity, value);
                }
            }
        }
        #endregion
    }
}
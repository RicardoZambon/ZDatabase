using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Reqnroll;
using System.Linq.Expressions;
using ZDatabase.Interfaces;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Helper;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities
{
    [Binding]
    public class EntityConfigurationSteps
    {
        #region Variables
        private IDbContext _dbContext;
        private IEntityType? _entityType;
        #endregion

        #region Properties
        #endregion

        #region Constructors
        public EntityConfigurationSteps()
        {
            _dbContext = DbContextFakeFactory.Create();
        }
        #endregion

        #region Public methods
        [Given("the model is built for {string}")]
        public void GivenTheModelIsBuiltForEntity(string entityName)
        {
            Type entityType = TestHelper.GetEntityType(entityName);

            _entityType = _dbContext.Model.FindEntityType(entityType);
            _entityType.Should().NotBeNull($"Entity type {entityName} should be found in the model.");
        }

        [Then("the property {string} should be configured as key")]
        public void ThenThePropertyShouldBeConfiguredAsKey(string propertyName)
        {
            IProperty? property = _entityType!.FindProperty(propertyName);
            property.Should().NotBeNull($"Property {propertyName} should be found in the entity type {_entityType.Name}.");

            property!.IsKey().Should().BeTrue($"Property {propertyName} should be configured as key in the entity type {_entityType.Name}.");
        }

        [Then("the property {string} should be a foreign key to {string} with navigation {string} and delete behavior {DeleteBehavior}")]
        public void ThenThePropertyShouldBeAForeignKeyToWithNavigationAndDeleteBehaviorNoAction(string propertyName, string entityName, string navigationProperty, DeleteBehavior deleteBehavior)
        {
            IProperty? property = _entityType!.FindProperty(propertyName);
            property.Should().NotBeNull($"Property {propertyName} should be found in the entity type {_entityType.Name}.");
            property!.IsForeignKey().Should().BeTrue();

            IForeignKey? fk = property.GetContainingForeignKeys().FirstOrDefault();
            fk.Should().NotBeNull($"Foreign key for property {propertyName} should be found in the entity type {_entityType.Name}.");

            Type foreignKeyType = TestHelper.GetEntityType(entityName);
            fk!.PrincipalEntityType.ClrType.Should().Be(foreignKeyType);

            fk.DependentToPrincipal!.PropertyInfo!.Name.Should().Be(navigationProperty);
            fk.DeleteBehavior.Should().Be(deleteBehavior);
        }

        [Then("the property {string} should have default value {string}")]
        public void ThenThePropertyShouldHaveDefaultValue(string propertyName, string expectedValue)
        {
            IProperty? property = _entityType!.FindProperty(propertyName);
            property.Should().NotBeNull($"Property {propertyName} should be found in the entity type {_entityType.Name}.");

            Type targetType = Nullable.GetUnderlyingType(property!.ClrType) ?? property.ClrType;
            object? value = Convert.ChangeType(expectedValue, targetType);

            property.GetDefaultValue().Should().Be(value);
        }

        [Then("the property {string} should have {string} save behavior {PropertySaveBehavior}")]
        public void ThenThePropertyShouldHaveSaveBehavior(string propertyName, string operation, PropertySaveBehavior behavior)
        {
            IProperty? property = _entityType!.FindProperty(propertyName);
            property.Should().NotBeNull($"Property {propertyName} should be found in the entity type {_entityType.Name}.");

            if (operation == "before")
            {
                property!.GetBeforeSaveBehavior().Should().Be(behavior);
            }
            else if (operation == "after")
            {
                property!.GetAfterSaveBehavior().Should().Be(behavior);
            }
            else
            {
                throw new ArgumentException($"Invalid operation: {operation}. Expected 'before' or 'after'.");
            }
        }

        [Then("the property {string} should have value generated {ValueGenerated}")]
        public void ThenThePropertyShouldHaveValueGenerated(string propertyName, ValueGenerated valueGenerated)
        {
            IProperty? property = _entityType!.FindProperty(propertyName);
            property.Should().NotBeNull($"Property {propertyName} should be found in the entity type {_entityType.Name}.");

            property!.ValueGenerated.Should().Be(valueGenerated);
        }

        [Then("the property {string} should use the generator {string}")]
        public void ThenThePropertyShouldUseTheGenerator(string propertyName, string generatorName)
        {
            IProperty? property = _entityType!.FindProperty(propertyName);
            property.Should().NotBeNull($"Property {propertyName} should be found in the entity type {_entityType.Name}.");

            Func<IProperty, ITypeBase, ValueGenerator>? factory = property!.GetValueGeneratorFactory();
            factory.Should().NotBeNull($"Value generator factory for property {propertyName} should not be null.");

            factory!(property, _entityType).Should().NotBeNull($"Value generator for property {propertyName} should not be null.");
        }

        [Then("the query filter should check the {string} property")]
        public void ThenTheQueryFilterShouldCheckTheProperty(string propertyName)
        {
            LambdaExpression? queryFilter = _entityType!.GetQueryFilter();
            queryFilter.Should().NotBeNull();

            queryFilter!.Body.ToString().Should().Contain(propertyName);
        }
        #endregion

        #region Private methods
        #endregion
    }
}
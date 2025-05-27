using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata;
using Reqnroll;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities
{
    [Binding]
    [Scope(Feature = "Auditable Entity Base Configuration")]
    public class AuditableEntityBaseConfigurationSteps
    {
        private IModel _model;

        [Given("the model is built for AuditableFake")]
        public void GivenModelIsBuiltForAuditableFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the property {string} should have after save behavior Ignore")]
        public void ThenPropertyShouldHaveAfterSaveBehaviorIgnore(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty(propertyName);
            property.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        [Then("the property {string} should have value generated on add")]
        public void ThenPropertyShouldHaveValueGeneratedOnAdd(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty(propertyName);
            property.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }

        [Then("the property {string} should use the {string}")]
        public void ThenPropertyShouldUseGenerator(string propertyName, string generatorName)
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty(propertyName);
            var factory = property.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory(property, entityType).GetType().Name.Should().Be(generatorName);
        }

        [Then("the property {string} should have after save behavior Save")]
        public void ThenPropertyShouldHaveAfterSaveBehaviorSave(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty(propertyName);
            property.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        [Then("the property {string} should have before save behavior Save")]
        public void ThenPropertyShouldHaveBeforeSaveBehaviorSave(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty(propertyName);
            property.GetBeforeSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        [Then("the property {string} should have value generated on add or update")]
        public void ThenPropertyShouldHaveValueGeneratedOnAddOrUpdate(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty(propertyName);
            property.ValueGenerated.Should().Be(ValueGenerated.OnAddOrUpdate);
        }
    }
}

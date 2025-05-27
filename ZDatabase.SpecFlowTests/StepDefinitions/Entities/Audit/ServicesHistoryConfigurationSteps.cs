using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Reqnroll;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities.Audit
{
    [Binding]
    [Scope(Feature = "Services History Configuration")]
    public class ServicesHistoryConfigurationSteps
    {
        private IModel _model;

        [Given("the model is built for ServicesHistoryFake")]
        public void GivenModelIsBuiltForServicesHistoryFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the property {string} should have after save behavior Ignore")]
        public void ThenPropertyShouldHaveAfterSaveBehaviorIgnore(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(ServicesHistoryFake));
            var property = entityType.FindProperty(propertyName);
            property.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        [Then("the property {string} should be a foreign key to UsersFake with navigation {string} and delete behavior NoAction")]
        public void ThenPropertyShouldBeForeignKeyToUsersFake(string propertyName, string navigationName)
        {
            var entityType = _model.FindEntityType(typeof(ServicesHistoryFake));
            var property = entityType.FindProperty(propertyName);
            property.IsForeignKey().Should().BeTrue();
            var fk = property.GetContainingForeignKeys().FirstOrDefault();
            fk.Should().NotBeNull();
            fk.PrincipalEntityType.ClrType.Should().Be(typeof(UsersFake));
            fk.DependentToPrincipal.PropertyInfo.Name.Should().Be(navigationName);
            fk.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }

        [Then("the property {string} should have value generated on add")]
        public void ThenPropertyShouldHaveValueGeneratedOnAdd(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(ServicesHistoryFake));
            var property = entityType.FindProperty(propertyName);
            property.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }

        [Then("the property {string} should use the {string}")]
        public void ThenPropertyShouldUseGenerator(string propertyName, string generatorName)
        {
            var entityType = _model.FindEntityType(typeof(ServicesHistoryFake));
            var property = entityType.FindProperty(propertyName);
            var factory = property.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory(property, entityType).GetType().Name.Should().Contain(generatorName);
        }
    }
}

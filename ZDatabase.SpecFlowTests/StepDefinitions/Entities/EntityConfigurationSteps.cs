using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Reqnroll;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities
{
    [Binding]
    [Scope(Feature = "Entity Configuration")]
    public class EntityConfigurationSteps
    {
        private IModel _model;

        [Given("the model is built for EntityFake")]
        public void GivenModelIsBuiltForEntityFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the property 'ID' should be configured as key")]
        public void ThenPropertyIDShouldBeConfiguredAsKey()
        {
            var entityType = _model.FindEntityType(typeof(EntityFake));
            var idProperty = entityType.FindProperty("ID");
            idProperty.IsKey().Should().BeTrue();
        }

        [Then("the property 'ID' should have value generated on add")]
        public void ThenPropertyIDShouldHaveValueGeneratedOnAdd()
        {
            var entityType = _model.FindEntityType(typeof(EntityFake));
            var idProperty = entityType.FindProperty("ID");
            idProperty.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }

        [Then("the property 'IsDeleted' should have default value false")]
        public void ThenPropertyIsDeletedShouldHaveDefaultValueFalse()
        {
            var entityType = _model.FindEntityType(typeof(EntityFake));
            var isDeletedProperty = entityType.FindProperty("IsDeleted");
            isDeletedProperty.GetDefaultValue().Should().Be(false);
        }

        [Then("the query filter should check the 'IsDeleted' property")]
        public void ThenQueryFilterShouldCheckIsDeletedProperty()
        {
            var entityType = _model.FindEntityType(typeof(EntityFake));
            var queryFilter = entityType.GetQueryFilter();
            queryFilter.Should().NotBeNull();
            queryFilter.Body.ToString().Should().Contain("IsDeleted");
        }
    }
}

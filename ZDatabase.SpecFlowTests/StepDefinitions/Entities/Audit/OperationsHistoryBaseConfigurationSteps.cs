using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata;
using Reqnroll;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities.Audit
{
    [Binding]
    [Scope(Feature = "Operations History Base Configuration")]
    public class OperationsHistoryBaseConfigurationSteps
    {
        private IModel _model;

        [Given("the model is built for OperationsHistoryFake")]
        public void GivenModelIsBuiltForOperationsHistoryFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the property {string} should have after save behavior Ignore")]
        public void ThenPropertyShouldHaveAfterSaveBehaviorIgnore(string propertyName)
        {
            var entityType = _model.FindEntityType(typeof(OperationsHistoryFake));
            var property = entityType.FindProperty(propertyName);
            property.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }
    }
}

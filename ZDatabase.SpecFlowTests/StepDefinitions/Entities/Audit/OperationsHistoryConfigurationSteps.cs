using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Reqnroll;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities.Audit
{
    [Binding]
    [Scope(Feature = "Operations History ConfigurationServicesHistoryFake")]
    public class OperationsHistoryConfigurationSteps
    {
        private IModel _model;

        [Given("the model is built for OperationsHistoryFake")]
        public void GivenModelIsBuiltForOperationsHistoryFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the property {string} should be a foreign key to ServicesHistoryFake with navigation {string} and delete behavior NoAction")]
        public void ThenPropertyShouldBeForeignKeyToServicesHistoryFake(string propertyName, string navigationName)
        {
            var entityType = _model.FindEntityType(typeof(OperationsHistoryFake));
            var property = entityType.FindProperty(propertyName);
            property.IsForeignKey().Should().BeTrue();
            var fk = property.GetContainingForeignKeys().FirstOrDefault();
            fk.Should().NotBeNull();
            fk.PrincipalEntityType.ClrType.Should().Be(typeof(ServicesHistoryFake));
            fk.DependentToPrincipal.PropertyInfo.Name.Should().Be(navigationName);
            fk.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }
    }
}

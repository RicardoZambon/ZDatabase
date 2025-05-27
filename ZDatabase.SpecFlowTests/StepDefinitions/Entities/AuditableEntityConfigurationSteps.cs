using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Reqnroll;
using ZDatabase.SpecFlowTests.Fakes;
using ZDatabase.SpecFlowTests.Fakes.Entities;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Entities
{
    [Binding]
    public class AuditableEntityConfigurationSteps
    {
        private IModel _model;

        [Given("the model is built for AuditableFake")]
        public void GivenTheModelIsBuiltForAuditableFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the property 'CreatedByID' should have after save behavior Ignore")]
        public void ThenThePropertyCreatedByIDShouldHaveAfterSaveBehaviorIgnore()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("CreatedByID");
            property.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Ignore);
        }

        [Then("the property 'CreatedByID' should be a foreign key to UsersFake with navigation 'CreatedBy' and delete behavior NoAction")]
        public void ThenThePropertyCreatedByIDShouldBeAForeignKeyToUsersFake()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("CreatedByID");
            property.IsForeignKey().Should().BeTrue();
            var fk = property.GetContainingForeignKeys().FirstOrDefault();
            fk.Should().NotBeNull();
            fk.PrincipalEntityType.ClrType.Should().Be(typeof(UsersFake));
            fk.DependentToPrincipal.PropertyInfo.Name.Should().Be("CreatedBy");
            fk.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }

        [Then("the property 'CreatedByID' should have value generated on add")]
        public void ThenThePropertyCreatedByIDShouldHaveValueGeneratedOnAdd()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("CreatedByID");
            property.ValueGenerated.Should().Be(ValueGenerated.OnAdd);
        }

        [Then("the property 'CreatedByID' should use the CurrentUserGenerator")]
        public void ThenThePropertyCreatedByIDShouldUseTheCurrentUserGenerator()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("CreatedByID");
            var factory = property.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory(property, entityType).GetType().Name.Should().Be("CurrentUserGenerator`1");
        }

        [Then("the property 'LastChangedByID' should have after save behavior Save")]
        public void ThenThePropertyLastChangedByIDShouldHaveAfterSaveBehaviorSave()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("LastChangedByID");
            property.GetAfterSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        [Then("the property 'LastChangedByID' should have before save behavior Save")]
        public void ThenThePropertyLastChangedByIDShouldHaveBeforeSaveBehaviorSave()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("LastChangedByID");
            property.GetBeforeSaveBehavior().Should().Be(PropertySaveBehavior.Save);
        }

        [Then("the property 'LastChangedByID' should be a foreign key to UsersFake with navigation 'LastChangedBy' and delete behavior NoAction")]
        public void ThenThePropertyLastChangedByIDShouldBeAForeignKeyToUsersFake()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("LastChangedByID");
            property.IsForeignKey().Should().BeTrue();
            var fk = property.GetContainingForeignKeys().FirstOrDefault();
            fk.Should().NotBeNull();
            fk.PrincipalEntityType.ClrType.Should().Be(typeof(UsersFake));
            fk.DependentToPrincipal.PropertyInfo.Name.Should().Be("LastChangedBy");
            fk.DeleteBehavior.Should().Be(DeleteBehavior.NoAction);
        }

        [Then("the property 'LastChangedByID' should have value generated on add or update")]
        public void ThenThePropertyLastChangedByIDShouldHaveValueGeneratedOnAddOrUpdate()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("LastChangedByID");
            property.ValueGenerated.Should().Be(ValueGenerated.OnAddOrUpdate);
        }

        [Then("the property 'LastChangedByID' should use the CurrentUserGenerator")]
        public void ThenThePropertyLastChangedByIDShouldUseTheCurrentUserGenerator()
        {
            var entityType = _model.FindEntityType(typeof(AuditableFake));
            var property = entityType.FindProperty("LastChangedByID");
            var factory = property.GetValueGeneratorFactory();
            factory.Should().NotBeNull();
            factory(property, entityType).GetType().Name.Should().Be("CurrentUserGenerator`1");
        }

        [Given("the model is built for EntityFake")]
        public void GivenTheModelIsBuiltForEntityFake()
        {
            var dbContext = new DbContextFake();
            _model = dbContext.Model;
        }

        [Then("the query filter should check the 'IsDeleted' property")]
        public void ThenTheQueryFilterShouldCheckTheIsDeletedProperty()
        {
            var entityType = _model.FindEntityType(typeof(EntityFake));
            var queryFilter = entityType.GetQueryFilter();
            queryFilter.Should().NotBeNull();
            queryFilter.Body.ToString().Should().Contain("IsDeleted");
        }
    }
}

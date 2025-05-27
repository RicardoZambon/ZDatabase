using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "DateTimeYearGenerator")]
    public class DateTimeYearGeneratorSteps
    {
        private DateTimeYearGenerator _generator;
        private EntityEntry<EntityFake> _entry;
        private int? _result;

        [Given("a new DateTimeYearGenerator")]
        public void GivenANewDateTimeYearGenerator()
        {
            _generator = new DateTimeYearGenerator();
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Given("a new DateTimeYearGenerator and an EntityEntry")]
        public void GivenANewDateTimeYearGeneratorAndAnEntityEntry()
        {
            _generator = new DateTimeYearGenerator();
            var dbContext = DbContextFakeFactory.Create();
            var entity = new EntityFake();
            _entry = dbContext.Add(entity);
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            _result = _generator.Next(_entry);
        }

        [Then("the result should be the current year")]
        public void ThenTheResultShouldBeTheCurrentYear()
        {
            _result.Should().Be(DateTime.Now.Year);
        }
    }
}

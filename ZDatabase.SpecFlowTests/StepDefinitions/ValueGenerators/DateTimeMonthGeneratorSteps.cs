using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "DateTimeMonthGenerator")]
    public class DateTimeMonthGeneratorSteps
    {
        private DateTimeMonthGenerator _generator;
        private EntityEntry<EntityFake> _entry;
        private int? _result;

        [Given("a new DateTimeMonthGenerator")]
        public void GivenANewDateTimeMonthGenerator()
        {
            _generator = new DateTimeMonthGenerator();
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Given("a new DateTimeMonthGenerator and an EntityEntry")]
        public void GivenANewDateTimeMonthGeneratorAndAnEntityEntry()
        {
            _generator = new DateTimeMonthGenerator();
            var dbContext = DbContextFakeFactory.Create();
            var entity = new EntityFake();
            _entry = dbContext.Add(entity);
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            _result = _generator.Next(_entry);
        }

        [Then("the result should be the current month")]
        public void ThenTheResultShouldBeTheCurrentMonth()
        {
            _result.Should().Be(DateTime.Now.Month);
        }
    }
}

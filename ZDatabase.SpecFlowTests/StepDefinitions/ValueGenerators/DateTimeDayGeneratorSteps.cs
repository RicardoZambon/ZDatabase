using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "DateTimeDayGenerator")]
    public class DateTimeDayGeneratorSteps
    {
        private DateTimeDayGenerator _generator;
        private EntityEntry<EntityFake> _entry;
        private int? _result;

        [Given("a new DateTimeDayGenerator")]
        public void GivenANewDateTimeDayGenerator()
        {
            _generator = new DateTimeDayGenerator();
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Given("a new DateTimeDayGenerator and an EntityEntry")]
        public void GivenANewDateTimeDayGeneratorAndAnEntityEntry()
        {
            _generator = new DateTimeDayGenerator();
            var dbContext = DbContextFakeFactory.Create();
            var entity = new EntityFake();
            _entry = dbContext.Add(entity);
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            _result = _generator.Next(_entry);
        }

        [Then("the result should be the current day")]
        public void ThenTheResultShouldBeTheCurrentDay()
        {
            _result.Should().Be(DateTime.Now.Day);
        }
    }
}

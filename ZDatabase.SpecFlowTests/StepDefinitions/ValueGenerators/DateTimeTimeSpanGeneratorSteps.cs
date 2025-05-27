using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "DateTimeTimeSpanGenerator")]
    public class DateTimeTimeSpanGeneratorSteps
    {
        private DateTimeTimeSpanGenerator _generator;
        private EntityEntry<EntityFake> _entry;
        private TimeSpan? _result;
        private TimeSpan _initialTimeSpan;

        [Given("a new DateTimeTimeSpanGenerator")]
        public void GivenANewDateTimeTimeSpanGenerator()
        {
            _generator = new DateTimeTimeSpanGenerator();
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Given("a new DateTimeTimeSpanGenerator and an EntityEntry")]
        public void GivenANewDateTimeTimeSpanGeneratorAndAnEntityEntry()
        {
            _generator = new DateTimeTimeSpanGenerator();
            var dbContext = DbContextFakeFactory.Create();
            var entity = new EntityFake();
            _entry = dbContext.Add(entity);
            _initialTimeSpan = DateTime.Now.TimeOfDay;
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            _result = _generator.Next(_entry);
        }

        [Then("the result should be the current TimeSpan")]
        public void ThenTheResultShouldBeTheCurrentTimeSpan()
        {
            _result.Should().BeGreaterThan(_initialTimeSpan);
            _result.Should().BeLessThan(DateTime.Now.TimeOfDay);
        }
    }
}

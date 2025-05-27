using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "DateTimeUtcGenerator")]
    public class DateTimeUtcGeneratorSteps
    {
        private DateTimeUtcGenerator _generator;
        private EntityEntry<EntityFake> _entry;
        private DateTime? _result;
        private DateTime _initialDateTime;

        [Given("a new DateTimeUtcGenerator")]
        public void GivenANewDateTimeUtcGenerator()
        {
            _generator = new DateTimeUtcGenerator();
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Given("a new DateTimeUtcGenerator and an EntityEntry")]
        public void GivenANewDateTimeUtcGeneratorAndAnEntityEntry()
        {
            _generator = new DateTimeUtcGenerator();
            var dbContext = DbContextFakeFactory.Create();
            var entity = new EntityFake();
            _entry = dbContext.Add(entity);
            _initialDateTime = DateTime.UtcNow;
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            _result = _generator.Next(_entry);
        }

        [Then("the result should be the current UTC DateTime")]
        public void ThenTheResultShouldBeTheCurrentUtcDateTime()
        {
            _result.Should().BeAfter(_initialDateTime);
            _result.Should().BeBefore(DateTime.UtcNow);
        }
    }
}

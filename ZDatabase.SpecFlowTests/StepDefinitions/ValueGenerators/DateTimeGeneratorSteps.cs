using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Reqnroll;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "DateTimeGenerator")]
    public class DateTimeGeneratorSteps
    {
        private DateTimeGenerator _generator;
        private EntityEntry<EntityFake> _entry;
        private DateTime? _result;
        private DateTime _initialDateTime;

        [Given("a new DateTimeGenerator")]
        public void GivenANewDateTimeGenerator()
        {
            _generator = new DateTimeGenerator();
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [Given("a new DateTimeGenerator and an EntityEntry")]
        public void GivenANewDateTimeGeneratorAndAnEntityEntry()
        {
            _generator = new DateTimeGenerator();
            var dbContext = DbContextFakeFactory.Create();
            var entity = new EntityFake();
            _entry = dbContext.Add(entity);
            _initialDateTime = DateTime.Now;
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            _result = _generator.Next(_entry);
        }

        [Then("the result should be the current DateTime")]
        public void ThenTheResultShouldBeTheCurrentDateTime()
        {
            _result.Should().BeAfter(_initialDateTime);
            _result.Should().BeBefore(DateTime.Now);
        }
    }
}

using FluentAssertions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Reqnroll;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.ValueGenerators;

namespace ZDatabase.SpecFlowTests.StepDefinitions.ValueGenerators
{
    [Binding]
    [Scope(Feature = "CurrentUserGenerator")]
    public class CurrentUserGeneratorSteps
    {
        private Exception? _caughtException = null;
        private ICurrentUserProvider<long> _currentUserProvider;
        private IDbContext? _dbContext = null;
        private EntityEntry<EntityFake>? _entry = null;
        private CurrentUserGenerator<long> _generator;
        private long? _result;

        public CurrentUserGeneratorSteps()
        {
            _generator = new CurrentUserGenerator<long>();
            _currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
        }

        [Given("a CurrentUserService that throws an exception")]
        public void GivenACurrentUserServiceThatThrowsAnException()
        {
            _currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
            _currentUserProvider.CurrentUserID.Throws(new Exception("Test"));
        }

        [Given(@"a CurrentUserService that returns (\d+)")]
        public void GivenACurrentUserServiceThatReturnsAnID(long id)
        {
            _currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
            _currentUserProvider.CurrentUserID.Returns(id);
        }

        [Given(@"a CurrentUserService that returns a null")]
        public void GivenACurrentUserServiceThatReturnsANull()
        {
            _currentUserProvider = Substitute.For<ICurrentUserProvider<long>>();
            _currentUserProvider.CurrentUserID.Returns((long?)null);
        }

        [Given("a database context")]
        public void GivenADatabaseContext()
        {
            ServiceCollection? serviceCollection = null;
            if (_currentUserProvider != null)
            {
                serviceCollection = new();
                serviceCollection.AddSingleton(_currentUserProvider);

            }
            _dbContext = DbContextFakeFactory.Create(serviceCollection);
        }

        [Then("GeneratesTemporaryValues should be false")]
        public void ThenGeneratesTemporaryValuesShouldBeFalse()
        {
            _generator.GeneratesTemporaryValues.Should().BeFalse();
        }

        [When("Next is called")]
        public void WhenNextIsCalled()
        {
            try
            {
                _result = _generator.Next(_entry);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then("the result should be default")]
        public void ThenTheResultShouldBeDefault()
        {
            _result.Should().Be(default);
        }

        [Given("a new EntityEntry")]
        public void GivenANewEntityEntry()
        {
            EntityFake entity = new();
            _entry = _dbContext.Add(entity);
        }

        [Then("an exception should be thrown")]
        public void ThenAnExceptionShouldBeThrown()
        {
            _caughtException.Should().NotBeNull();
        }

        [Then("the result should be the current user ID")]
        public void ThenTheResultShouldBeTheCurrentUserID()
        {
            _result.Should().Be(123L);
        }
    }
}

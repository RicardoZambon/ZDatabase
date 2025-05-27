using FluentAssertions;
using Reqnroll;
using ZDatabase.Exceptions;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit;
using ZDatabase.Repositories.Audit.Interfaces;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Repositories
{
    [Binding]
    [Scope(Feature = "Services History Repository")]
    public class ServicesHistoryRepositorySteps
    {
        private IDbContext _dbContext;
        private IServicesHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long> _repository;
        private ServicesHistoryFake _serviceHistory1;
        private ServicesHistoryFake _serviceHistory2;
        private AuditableFake _auditableEntity1;
        private AuditableFake _auditableEntity2;
        private IQueryable<ServicesHistoryFake> _result;
        private Exception _caughtException;

        [Given("a new ServicesHistoryFake")]
        public void GivenANewServicesHistoryFake()
        {
            _dbContext = DbContextFakeFactory.Create();
            _repository = new ServicesHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>(_dbContext);
            _serviceHistory1 = new ServicesHistoryFake();
        }

        [When("AddServiceHistoryAsync is called")]
        public async Task WhenAddServiceHistoryAsyncIsCalled()
        {
            await _repository.AddServiceHistoryAsync(_serviceHistory1);
            await _dbContext.SaveChangesAsync();
        }

        [Then("the service history should be added to the database")]
        public void ThenTheServiceHistoryShouldBeAddedToTheDatabase()
        {
            _dbContext.Set<ServicesHistoryFake>().Count().Should().Be(1);
            _dbContext.Set<ServicesHistoryFake>().First().Should().Be(_serviceHistory1);
        }

        [Given("a new ServicesHistoryFake and a new AuditableFake")]
        public void GivenANewServicesHistoryFakeAndANewAuditableFake()
        {
            _dbContext = DbContextFakeFactory.Create();
            _repository = new ServicesHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>(_dbContext);
            _serviceHistory1 = new ServicesHistoryFake();
            _auditableEntity1 = new AuditableFake();
            _dbContext.Add(_serviceHistory1);
            _dbContext.Add(_auditableEntity1);
            _dbContext.SaveChanges();
        }

        [When("ListServicesAsync is called with an invalid entity ID")]
        public async Task WhenListServicesAsyncIsCalledWithAnInvalidEntityID()
        {
            try
            {
                _result = await _repository.ListServicesAsync<AuditableFake>(_auditableEntity1.ID + 1);
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        [Then("an EntityNotFoundException should be thrown")]
        public void ThenAnEntityNotFoundExceptionShouldBeThrown()
        {
            _caughtException.Should().BeOfType<EntityNotFoundException<AuditableFake>>();
        }

        [Given("two service histories and two auditable entities")]
        public void GivenTwoServiceHistoriesAndTwoAuditableEntities()
        {
            _dbContext = DbContextFakeFactory.Create();
            _repository = new ServicesHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>(_dbContext);
            _serviceHistory1 = new ServicesHistoryFake();
            _serviceHistory2 = new ServicesHistoryFake();
            _auditableEntity1 = new AuditableFake();
            _auditableEntity2 = new AuditableFake();
            _dbContext.Add(_serviceHistory1);
            _dbContext.Add(_auditableEntity1);
            _dbContext.SaveChanges();
            _dbContext.ClearAuditServiceHistory();
            _dbContext.Add(_serviceHistory2);
            _dbContext.Add(_auditableEntity2);
            _dbContext.SaveChanges();
        }

        [When("ListServicesAsync is called for the first auditable entity")]
        public async Task WhenListServicesAsyncIsCalledForTheFirstAuditableEntity()
        {
            _result = await _repository.ListServicesAsync<AuditableFake>(_auditableEntity1.ID);
        }

        [Then("only the first service history should be returned")]
        public void ThenOnlyTheFirstServiceHistoryShouldBeReturned()
        {
            _result.Should().NotBeNull();
            _result.Count().Should().Be(1);
            _result.First().Should().Be(_serviceHistory1);
        }
    }
}

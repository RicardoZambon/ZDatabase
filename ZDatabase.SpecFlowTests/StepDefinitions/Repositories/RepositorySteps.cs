using FluentAssertions;
using Reqnroll;
using ZDatabase.Interfaces;
using ZDatabase.Repositories.Audit;
using ZDatabase.SpecFlowTests.Factories;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;

namespace ZDatabase.SpecFlowTests.StepDefinitions.Repositories
{
    [Binding]
    [Scope(Feature = "Operations History Repository")]
    public class RepositorySteps
    {
        private IDbContext _dbContext;
        private OperationsHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long> _repository;
        private ServicesHistoryFake _serviceHistory;
        private AuditableFake _auditableEntity;
        private OperationsHistoryFake _operationHistory;
        private IQueryable<OperationsHistoryFake> _result;

        [Given("a new OperationsHistoryFake")]
        public void GivenANewOperationsHistoryFake()
        {
            _dbContext = DbContextFakeFactory.Create();
            _repository = new OperationsHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>(_dbContext);
            _operationHistory = new OperationsHistoryFake();
        }

        [When("AddOperationHistoryAsync is called")]
        public async Task WhenAddOperationHistoryAsyncIsCalled()
        {
            await _repository.AddOperationHistoryAsync(_operationHistory);
            await _dbContext.SaveChangesAsync();
        }

        [Then("the operation history should be added to the database")]
        public void ThenTheOperationHistoryShouldBeAddedToTheDatabase()
        {
            _dbContext.Set<OperationsHistoryFake>().Count().Should().Be(1);
            _dbContext.Set<OperationsHistoryFake>().First().Should().Be(_operationHistory);
        }

        [Given("a new ServicesHistoryFake and a new AuditableFake")]
        public void GivenANewServicesHistoryFakeAndANewAuditableFake()
        {
            _dbContext = DbContextFakeFactory.Create();
            _repository = new OperationsHistoryRepository<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>(_dbContext);
            _serviceHistory = new ServicesHistoryFake();
            _auditableEntity = new AuditableFake();
            _dbContext.Add(_serviceHistory);
            _dbContext.Add(_auditableEntity);
            _dbContext.SaveChanges();
        }

        [When("ListOperations is called with an invalid service history ID")]
        public void WhenListOperationsIsCalledWithAnInvalidServiceHistoryID()
        {
            _result = _repository.ListOperations(_serviceHistory.ID + 1);
        }

        [Then("the result should be empty")]
        public void ThenTheResultShouldBeEmpty()
        {
            _result.Should().BeEmpty();
        }

        [When("ListOperations is called with the service history ID")]
        public void WhenListOperationsIsCalledWithTheServiceHistoryID()
        {
            _result = _repository.ListOperations(_serviceHistory.ID);
        }

        [Then("the result should contain one operation history with correct entity and service history IDs and entity name")]
        public void ThenTheResultShouldContainOneOperationHistoryWithCorrectEntityAndServiceHistoryIDsAndEntityName()
        {
            _result.Should().NotBeNull();
            _result.Count().Should().Be(1);
            _result.All(x => x.ServiceHistoryID == _serviceHistory.ID).Should().BeTrue();
            _result.All(x => x.EntityID == _auditableEntity.ID).Should().BeTrue();
            _result.All(x => x.EntityName == nameof(AuditableFake)).Should().BeTrue();
        }
    }
}

using ZDatabase.Entities.Audit;
using ZDatabase.Interfaces;
using ZDatabase.Services;

namespace ZDatabase.UnitTests.Fakes.ServicesFake
{
    internal class AuditHandlerFake : AuditHandler<ServicesHistoryEntityFake, UsersEntityFake, long>
    {
        public AuditHandlerFake(IDbContext dbContext)
            : base(dbContext)
        {

        }

        public override OperationsHistory<ServicesHistoryEntityFake, UsersEntityFake, long> InstantiateOperationsHistory()
            => new OperationsHistoryEntityFake();
    }
}

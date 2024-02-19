using ZDatabase.Interfaces;
using ZDatabase.Services;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Fakes.ServicesFake
{
    internal class AuditHandlerFake : AuditHandler<ServicesHistoryEntityFake, OperationsHistoryEntityFake, UsersEntityFake, long>
    {
        public AuditHandlerFake(IDbContext dbContext)
            : base(dbContext)
        {

        }

        public override OperationsHistoryEntityFake InstantiateOperationsHistory()
            => new OperationsHistoryEntityFake();
    }
}
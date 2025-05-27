using ZDatabase.Interfaces;
using ZDatabase.Services;
using ZDatabase.SpecFlowTests.Fakes.Entities;
using ZDatabase.SpecFlowTests.Fakes.Entities.Audit;

namespace ZDatabase.SpecFlowTests.Fakes.Services
{
    internal class AuditHandlerFake : AuditHandler<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>
    {
        public AuditHandlerFake(IDbContext dbContext)
            : base(dbContext)
        {

        }

        public override OperationsHistoryFake InstantiateOperationsHistory()
            => new();
    }
}
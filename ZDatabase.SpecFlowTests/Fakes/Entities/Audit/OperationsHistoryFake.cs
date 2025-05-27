using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities.Audit;

namespace ZDatabase.SpecFlowTests.Fakes.Entities.Audit
{
    internal class OperationsHistoryFake : OperationsHistory<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>
    {
    }

    internal class OperationsHistoryFakeConfiguration : OperationsHistoryConfiguration<OperationsHistoryFake, ServicesHistoryFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<OperationsHistoryFake> builder)
        {
            base.Configure(builder);
        }
    }
}
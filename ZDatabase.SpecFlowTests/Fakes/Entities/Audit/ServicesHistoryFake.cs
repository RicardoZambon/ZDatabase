using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities.Audit;

namespace ZDatabase.SpecFlowTests.Fakes.Entities.Audit
{
    internal class ServicesHistoryFake : ServicesHistory<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>
    {
    }

    internal class ServicessHistoryFakeConfiguration : ServicesHistoryConfiguration<ServicesHistoryFake, OperationsHistoryFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<ServicesHistoryFake> builder)
        {
            base.Configure(builder);
        }
    }
}
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities.Audit;

namespace ZDatabase.UnitTests.Fakes
{
    internal class OperationsHistoryEntityFake : OperationsHistory<ServicesHistoryEntityFake, UsersEntityFake, long>
    {
    }

    internal class OperationsHistoryEntityFakeConfiguration : OperationsHistoryConfiguration<OperationsHistoryEntityFake, ServicesHistoryEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<OperationsHistoryEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
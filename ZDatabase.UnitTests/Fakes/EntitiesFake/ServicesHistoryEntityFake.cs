using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities.Audit;

namespace ZDatabase.UnitTests.Fakes
{
    internal class ServicesHistoryEntityFake : ServicesHistory<UsersEntityFake, long>
    {
    }

    internal class ServicessHistoryEntityFakeConfiguration : ServicesHistoryConfiguration<ServicesHistoryEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<ServicesHistoryEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
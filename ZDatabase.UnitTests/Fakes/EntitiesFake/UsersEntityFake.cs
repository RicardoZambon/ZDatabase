using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes
{
    internal class UsersEntityFake : AuditableEntity<UsersEntityFake, long>
    {
    }

    internal class UsersEntityFakeConfiguration : AuditableEntityConfiguration<UsersEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<UsersEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
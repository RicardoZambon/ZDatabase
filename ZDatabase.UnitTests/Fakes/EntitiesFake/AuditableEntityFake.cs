using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes
{
    internal class AuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
    }

    internal class AuditableEntityFakeConfiguration : AuditableEntityConfiguration<AuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<AuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
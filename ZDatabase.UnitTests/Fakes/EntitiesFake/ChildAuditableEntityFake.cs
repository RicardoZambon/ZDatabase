using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class ChildAuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
    }

    internal class ChildAuditableEntityFakeConfiguration : AuditableEntityConfiguration<ChildAuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<ChildAuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
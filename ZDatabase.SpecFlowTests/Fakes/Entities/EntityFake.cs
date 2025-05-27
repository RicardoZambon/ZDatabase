using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class EntityFake : AuditableEntity<UsersFake, long>
    {
    }

    internal class EntityFakeConfiguration : AuditableEntityConfiguration<EntityFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<EntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
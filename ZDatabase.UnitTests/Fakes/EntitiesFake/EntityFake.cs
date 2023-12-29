using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes
{
    internal class EntityFake : Entity
    {
    }

    internal class EntityFakeConfiguration : EntityConfiguration<EntityFake>
    {
        public override void Configure(EntityTypeBuilder<EntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
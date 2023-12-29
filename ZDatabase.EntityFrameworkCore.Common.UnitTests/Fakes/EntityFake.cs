using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes
{
    public class EntityFake : Entity
    {
    }

    public class FakeEntityConfiguration : EntityConfiguration<EntityFake>
    {
        public override void Configure(EntityTypeBuilder<EntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
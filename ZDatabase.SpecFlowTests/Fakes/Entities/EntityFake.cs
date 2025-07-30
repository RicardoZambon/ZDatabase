using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class EntityFake : Entity
    {
        public string? Display { get; set; }
    }

    internal class EntityFakeConfiguration : EntityConfiguration<EntityFake>
    {
        public override void Configure(EntityTypeBuilder<EntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
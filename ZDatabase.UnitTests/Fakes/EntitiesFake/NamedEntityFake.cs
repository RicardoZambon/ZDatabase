using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class NamedEntityFake : Entity
    {
        public string Name { get; set; } = string.Empty;
    }

    internal class NamedEntityFakeConfiguration : EntityConfiguration<NamedEntityFake>
    {
        public override void Configure(EntityTypeBuilder<NamedEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}

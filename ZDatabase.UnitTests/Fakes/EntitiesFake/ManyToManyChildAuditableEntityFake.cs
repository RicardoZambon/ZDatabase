using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class ManyToManyChildAuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
        [AuditableRelation]
        public virtual ICollection<ChildAuditableEntityFake>? ChildAuditableEntities { get; set; }

        public string? DummyValue { get; set; }
    }

    internal class ManyToManyChildAuditableEntityFakeConfiguration : AuditableEntityConfiguration<ManyToManyChildAuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<ManyToManyChildAuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
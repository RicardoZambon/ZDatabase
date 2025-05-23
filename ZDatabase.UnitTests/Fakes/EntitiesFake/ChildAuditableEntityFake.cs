using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class ChildAuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
        public virtual AuditableEntityFake? AuditableEntity { get; set; }

        public long AuditableEntityID { get; set; }

        public string? DummyValue { get; set; }

        [AuditableRelation]
        public virtual ICollection<ManyToManyChildAuditableEntityFake> ManyToManyChilds { get; set; } = [];

        [AuditableRelation]
        public virtual ICollection<RelatedChildAuditableEntityFake> RelatedChilds { get; set; } = [];
    }

    internal class ChildAuditableEntityFakeConfiguration : AuditableEntityConfiguration<ChildAuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<ChildAuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
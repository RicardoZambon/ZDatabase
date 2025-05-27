using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class RelatedChildAuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
        public virtual ChildAuditableEntityFake? ChildAuditableEntity { get; set; }

        public long ChildAuditableEntityID { get; set; }

        public string? DummyValue { get; set; }
    }

    internal class RelatedChildAuditableEntityFakeConfiguration : AuditableEntityConfiguration<RelatedChildAuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<RelatedChildAuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
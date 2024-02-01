using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class ChildAuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
        public string? DummyValue { get; set; }
        public virtual AuditableEntityFake? Parent { get; set; }
        public long ParentID { get; set; }
    }

    internal class ChildAuditableEntityFakeConfiguration : AuditableEntityConfiguration<ChildAuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<ChildAuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
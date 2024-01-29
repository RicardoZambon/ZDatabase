using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.UnitTests.Fakes.EntitiesFake
{
    internal class AuditableEntityFake : AuditableEntity<UsersEntityFake, long>
    {
        public Guid RandomGuid { get; set; }

        public virtual ICollection<ChildAuditableEntityFake> Children { get; set; } = new List<ChildAuditableEntityFake>();
    }

    internal class AuditableEntityFakeConfiguration : AuditableEntityConfiguration<AuditableEntityFake, UsersEntityFake, long>
    {
        public override void Configure(EntityTypeBuilder<AuditableEntityFake> builder)
        {
            base.Configure(builder);
        }
    }
}
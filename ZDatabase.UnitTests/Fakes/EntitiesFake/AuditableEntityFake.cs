using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Fakes
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
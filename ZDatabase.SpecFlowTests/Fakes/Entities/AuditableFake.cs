using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class AuditableFake : AuditableEntity<UsersFake, long>
    {
        public Guid RandomGuid { get; set; } = Guid.NewGuid();
    }

    internal class AuditableFakeConfiguration : AuditableEntityConfiguration<AuditableFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<AuditableFake> builder)
        {
            base.Configure(builder);
        }
    }
}
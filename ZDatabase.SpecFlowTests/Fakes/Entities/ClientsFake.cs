using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class ClientsFake : AuditableEntity<UsersFake, long>
    {
        public string? Name { get; set; }

        [AuditableRelation]
        public virtual ICollection<PurchasesFake> Purchases { get; set; } = [];
    }

    internal class ClientsFakeConfiguration : AuditableEntityConfiguration<ClientsFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<ClientsFake> builder)
        {
            base.Configure(builder);
        }
    }
}
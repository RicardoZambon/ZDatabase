using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class RolesFake : AuditableEntity<UsersFake, long>
    {
        public string? Name { get; set; }

        [AuditableRelation]
        public virtual ICollection<UsersFake> Users { get; set; } = [];
    }

    internal class RolesFakeConfiguration : AuditableEntityConfiguration<RolesFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<RolesFake> builder)
        {
            base.Configure(builder);
        }
    }
}
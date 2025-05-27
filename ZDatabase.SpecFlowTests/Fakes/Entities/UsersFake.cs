using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class UsersFake : AuditableEntity<UsersFake, long>
    {
        public virtual ClientsFake? Client { get; set; }

        public long ClientID { get; set; }

        public string? Name { get; set; }

        [AuditableRelation]
        public virtual ICollection<RolesFake> Roles { get; set; } = [];

        public string? Username { get; set; }
    }

    internal class UsersFakeConfiguration : AuditableEntityConfiguration<UsersFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<UsersFake> builder)
        {
            base.Configure(builder);
        }
    }
}
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class AssessmentsFake : AuditableEntity<UsersFake, long>
    {
        public string? Details { get; set; }

        [AuditableRelation]
        public virtual ICollection<PurchasesFake> Purchases { get; set; } = [];
    }

    internal class AssessmentsFakeConfiguration : AuditableEntityConfiguration<AssessmentsFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<AssessmentsFake> builder)
        {
            base.Configure(builder);
        }
    }
}
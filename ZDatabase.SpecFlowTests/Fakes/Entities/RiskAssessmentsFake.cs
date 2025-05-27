using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class RiskAssessmentsFake : AuditableEntity<UsersFake, long>
    {
        public string? Details { get; set; }

        [AuditableRelation]
        public virtual ICollection<PurchasesFake> Purchases { get; set; } = [];
    }

    internal class RiskAssessmentsFakeConfiguration : AuditableEntityConfiguration<RiskAssessmentsFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<RiskAssessmentsFake> builder)
        {
            base.Configure(builder);
        }
    }
}
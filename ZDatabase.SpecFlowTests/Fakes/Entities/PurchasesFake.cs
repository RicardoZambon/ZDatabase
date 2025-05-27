using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Attributes;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class PurchasesFake : AuditableEntity<UsersFake, long>
    {
        public virtual ClientsFake? Client { get; set; }

        public long ClientID { get; set; }

        [AuditableRelation]
        public virtual ICollection<PurchasesItemsFake> Items { get; set; } = [];

        [AuditableRelation]
        public virtual ICollection<RiskAssessmentsFake> RiskAssessments { get; set; } = [];
    }

    internal class PurchasesFakeConfiguration : AuditableEntityConfiguration<PurchasesFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<PurchasesFake> builder)
        {
            base.Configure(builder);
        }
    }
}
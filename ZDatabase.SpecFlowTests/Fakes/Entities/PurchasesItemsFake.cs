using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class PurchasesItemsFake : AuditableEntity<UsersFake, long>
    {
        public virtual ProductsFake? Product { get; set; }

        public long ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }

    internal class PurchaseItemsFakeConfiguration : AuditableEntityConfiguration<PurchasesItemsFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<PurchasesItemsFake> builder)
        {
            base.Configure(builder);
        }
    }
}
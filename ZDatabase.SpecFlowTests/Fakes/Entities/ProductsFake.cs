using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZDatabase.Entities;

namespace ZDatabase.SpecFlowTests.Fakes.Entities
{
    internal class ProductsFake : AuditableEntity<UsersFake, long>
    {
        public string? Name { get; set; }

        public decimal UnitPrice { get; set; }
    }

    internal class ProductsFakeConfiguration : AuditableEntityConfiguration<ProductsFake, UsersFake, long>
    {
        public override void Configure(EntityTypeBuilder<ProductsFake> builder)
        {
            base.Configure(builder);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Proxies.Internal;

namespace ZDatabase.UnitTests.Fakes
{
    public class DbContextFake : ZDbContext<DbContextFake>
    {
        public DbContextFake()
            : this(new DbContextOptionsBuilder<DbContextFake>().Options)
        {
        }

        public DbContextFake(DbContextOptions<DbContextFake> options)
            : base(options)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder
                .UseInMemoryDatabase(nameof(DbContextFake))
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

#pragma warning disable EF1001 // Internal EF Core API usage.
            ProxiesOptionsExtension extension = new ProxiesOptionsExtension();
            extension = extension.WithLazyLoading(true);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);
#pragma warning restore EF1001 // Internal EF Core API usage.
        }
    }
}
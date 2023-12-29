using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

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
        }
    }
}
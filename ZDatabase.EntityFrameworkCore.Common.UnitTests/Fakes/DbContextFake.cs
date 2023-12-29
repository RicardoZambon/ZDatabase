using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ZDatabase.EntityFrameworkCore.Common.Interfaces;

namespace ZDatabase.EntityFrameworkCore.Common.UnitTests.Fakes
{
    public class DbContextFake : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }
    }
}
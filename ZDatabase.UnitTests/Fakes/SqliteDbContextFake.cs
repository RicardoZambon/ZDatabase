using Microsoft.EntityFrameworkCore;

namespace ZDatabase.UnitTests.Fakes
{
    /// <summary>
    /// A context on a relational provider (SQLite), for behaviour the in-memory provider cannot
    /// show, such as transactions.
    /// </summary>
    public class SqliteDbContextFake : ZDbContext<SqliteDbContextFake>
    {
        public SqliteDbContextFake(DbContextOptions<SqliteDbContextFake> options)
            : base(options)
        {
        }
    }
}

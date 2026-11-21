using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Fakes;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests
{
    /// <summary>
    /// Unit tests for how <see cref="ZDbContext{TDbContext}"/> saves the data and its audit rows
    /// together, on a relational provider.
    /// </summary>
    public sealed class ZDbContextTransactionTests : IDisposable
    {
        private const string AuditFailedMessage = "Audit failed";

        private readonly SqliteConnection connection;

        public ZDbContextTransactionTests()
        {
            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            using SqliteDbContextFake context = CreateContext(Substitute.For<IAuditHandler>());
            context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        /// <summary>
        /// Test the SaveChanges method should roll the data back when saving the audit rows fails.
        /// </summary>
        [Fact]
        public void SaveChanges_Fail_ShouldRollBackTheDataWhenTheAuditSaveFails()
        {
            // Arrange
            IAuditHandler auditHandler = Substitute.For<IAuditHandler>();
            auditHandler.When(x => x.AddOperationEntitiesAfterSaved()).Do(_ => throw new InvalidOperationException(AuditFailedMessage));

            using SqliteDbContextFake context = CreateContext(auditHandler);
            context.Add(new EntityFake());

            // Act
            Action act = () => context.SaveChanges();

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage(AuditFailedMessage);
            CountSavedEntities().Should().Be(0);
        }

        /// <summary>
        /// Test the SaveChangesAsync method should roll the data back when saving the audit rows fails.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Fail_ShouldRollBackTheDataWhenTheAuditSaveFails()
        {
            // Arrange
            IAuditHandler auditHandler = Substitute.For<IAuditHandler>();
            auditHandler.AddOperationEntitiesAfterSavedAsync().Returns(Task.FromException(new InvalidOperationException(AuditFailedMessage)));

            await using SqliteDbContextFake context = CreateContext(auditHandler);
            context.Add(new EntityFake());

            // Act
            Func<Task> act = async () => await context.SaveChangesAsync();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(AuditFailedMessage);
            CountSavedEntities().Should().Be(0);
        }

        /// <summary>
        /// Test the SaveChanges method should commit the data when the audit save succeeds.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldCommitTheDataWhenTheAuditSaveSucceeds()
        {
            // Arrange
            using SqliteDbContextFake context = CreateContext(Substitute.For<IAuditHandler>());
            context.Add(new EntityFake());

            // Act
            context.SaveChanges();

            // Assert
            CountSavedEntities().Should().Be(1);
        }

        /// <summary>
        /// Test the SaveChangesAsync method should commit the data when the audit save succeeds.
        /// </summary>
        [Fact]
        public async Task SaveChangesAsync_Pass_ShouldCommitTheDataWhenTheAuditSaveSucceeds()
        {
            // Arrange
            await using SqliteDbContextFake context = CreateContext(Substitute.For<IAuditHandler>());
            context.Add(new EntityFake());

            // Act
            await context.SaveChangesAsync();

            // Assert
            CountSavedEntities().Should().Be(1);
        }

        /// <summary>
        /// Test the SaveChanges method should join the caller's transaction instead of committing on
        /// its own, so the caller's rollback still undoes the save.
        /// </summary>
        [Fact]
        public void SaveChanges_Pass_ShouldJoinTheCallersTransaction()
        {
            // Arrange
            using SqliteDbContextFake context = CreateContext(Substitute.For<IAuditHandler>());
            using IDbContextTransaction transaction = context.Database.BeginTransaction();
            context.Add(new EntityFake());

            // Act
            context.SaveChanges();
            transaction.Rollback();

            // Assert
            CountSavedEntities().Should().Be(0);
        }

        private int CountSavedEntities()
        {
            using SqliteDbContextFake context = CreateContext(Substitute.For<IAuditHandler>());
            return context.Set<EntityFake>().IgnoreQueryFilters().Count();
        }

        private SqliteDbContextFake CreateContext(IAuditHandler auditHandler)
        {
            ServiceProvider services = new ServiceCollection()
                .AddSingleton(auditHandler)
                .BuildServiceProvider();

            DbContextOptions<SqliteDbContextFake> options = new DbContextOptionsBuilder<SqliteDbContextFake>()
                .UseSqlite(connection)
                .UseApplicationServiceProvider(services)
                .Options;

            return new SqliteDbContextFake(options);
        }
    }
}

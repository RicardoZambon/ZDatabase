using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZDatabase.Entities;
using ZDatabase.Initializers;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.UnitTests.Fakes.EntitiesFake;

namespace ZDatabase.UnitTests.Fakes.InitializersFake
{
    /// <summary>
    /// Concrete <see cref="BaseInitializer{TServicesHistory}"/> exposing its protected members to tests.
    /// </summary>
    internal class BaseInitializerFake : BaseInitializer<ServicesHistoryEntityFake>
    {
        internal bool InitializeWasCalled { get; private set; }

        internal List<bool> IdentityInsertToggles { get; } = new();

        public BaseInitializerFake(IDbContext dbContext)
            : base(dbContext)
        {
        }

        internal IAuditHandler? ExposedAuditHandler => AuditHandler;

        public override void Initialize()
            => InitializeWasCalled = true;

        protected override void SetIdentityInsert<TEntity>(bool enable)
            => IdentityInsertToggles.Add(enable);

        internal IDbContextTransaction ExposedBeginTransaction(string methodName)
            => BeginTransaction(methodName);

        internal void ExposedSaveContext<TEntity, TValue>(IEnumerable<TEntity> entities, string methodName, Func<TEntity, TValue> propertyCheck)
            where TEntity : Entity
            => SaveContext(entities, methodName, propertyCheck);

        internal void ExposedSaveContext<TEntity>(IEnumerable<TEntity> entities, string methodName, EntityState entriesState = EntityState.Added, bool forceIdentityInsert = false)
            where TEntity : class
            => SaveContext(entities, methodName, entriesState, forceIdentityInsert);
    }

    /// <summary>
    /// <see cref="BaseInitializerFake"/> variant that leaves <see cref="SetIdentityInsert{TEntity}"/> at its
    /// default, do-nothing implementation.
    /// </summary>
    internal class DefaultIdentityInsertInitializerFake : BaseInitializer<ServicesHistoryEntityFake>
    {
        public DefaultIdentityInsertInitializerFake(IDbContext dbContext)
            : base(dbContext)
        {
        }

        public override void Initialize()
        {
        }

        internal void ExposedSaveContext<TEntity>(IEnumerable<TEntity> entities, string methodName, EntityState entriesState = EntityState.Added, bool forceIdentityInsert = false)
            where TEntity : class
            => SaveContext(entities, methodName, entriesState, forceIdentityInsert);
    }
}

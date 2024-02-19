using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Entities;
using ZDatabase.Entities.Audit;
using ZDatabase.Entries;
using ZDatabase.Exceptions;
using ZDatabase.ExtensionMethods;
using ZDatabase.Interfaces;
using ZDatabase.Services.Interfaces;
using ZDatabase.ValueGenerators;

namespace ZDatabase.Services
{
    /// <inheritdoc />
    /// <typeparam name="TServicesHistory">The type of the service history.</typeparam>
    /// <typeparam name="TOperationsHistory">The type of the operations history.</typeparam>
    /// <typeparam name="TUsers">The type of the users.</typeparam>
    /// <typeparam name="TUsersKey">The type of the users key.</typeparam>
    /// <seealso cref="ZDatabase.Services.Interfaces.IAuditHandler" />
    public abstract class AuditHandler<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        : IAuditHandler
        where TServicesHistory : ServicesHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TOperationsHistory : OperationsHistory<TServicesHistory, TOperationsHistory, TUsers, TUsersKey>
        where TUsers : class
        where TUsersKey : struct
    {
        #region Variables

        private readonly IDbContext dbContext;

        private EntityEntry? OldServiceHistoryEntry = null;

        #endregion

        #region Properties

        /// <inheritdoc />
        public bool HasPreviousServiceHistory
        {
            get
            {
                return OldServiceHistoryEntry != null;
            }
        }

        private List<AuditEntry> AuditedEntries { get; set; } = new();
        private List<AuditRelatedEntry> AuditedRelatedEntries { get; set; } = new();
        private TServicesHistory? ServiceHistory
        {
            get
            {
                return ServiceHistoryEntry?.Entity as TServicesHistory;
            }
        }
        private EntityEntry? ServiceHistoryEntry { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ZDatabase.Services.AuditHandler{TServiceHistory, TOperationsHistory, TUsers, TUsersKey}"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ZDatabase.Interfaces.IDbContext"/> instance.</param>
        public AuditHandler(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public void AddOperationEntitiesAfterSaved()
        {
            if (AuditedEntries.Any() || AuditedRelatedEntries.Any())
            {
                foreach (AuditEntry entry in AuditedEntries.EntriesWithoutTemporaryProperties())
                {
                    TrackAuditEntry(entry);
                }

                foreach (AuditRelatedEntry entry in AuditedRelatedEntries.EntriesWithoutTemporaryProperties())
                {
                    TrackAuditEntry(entry);
                }

                OldServiceHistoryEntry = ServiceHistoryEntry;
                dbContext.SaveChanges();
            }
        }

        /// <inheritdoc />
        public async Task AddOperationEntitiesAfterSavedAsync()
        {
            if (AuditedEntries.Any() || AuditedRelatedEntries.Any())
            {
                foreach (AuditEntry entry in AuditedEntries.EntriesWithoutTemporaryProperties())
                {
                    await TrackAuditEntryAsync(entry);
                }

                foreach (AuditRelatedEntry entry in AuditedRelatedEntries.EntriesWithoutTemporaryProperties())
                {
                    await TrackAuditEntryAsync(entry);
                }

                OldServiceHistoryEntry = ServiceHistoryEntry;
                await dbContext.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public void AddOperationEntitiesBeforeSaving()
        {
            AuditEntry[] entries = AuditedEntries.EntriesWithoutTemporaryProperties();
            foreach (AuditEntry entry in entries)
            {
                TrackAuditEntry(entry);
                AuditedEntries.Remove(entry);
            }

            AuditRelatedEntry[] relatedEntries = AuditedRelatedEntries.EntriesWithoutTemporaryProperties();
            foreach (AuditRelatedEntry entry in relatedEntries)
            {
                TrackAuditEntry(entry);
                AuditedRelatedEntries.Remove(entry);
            }
        }

        /// <inheritdoc />
        public async Task AddOperationEntitiesBeforeSavingAsync()
        {
            AuditEntry[] entries = AuditedEntries.EntriesWithoutTemporaryProperties();
            foreach (AuditEntry entry in entries)
            {
                await TrackAuditEntryAsync(entry);
                AuditedEntries.Remove(entry);
            }

            AuditRelatedEntry[] relatedEntries = AuditedRelatedEntries.EntriesWithoutTemporaryProperties();
            foreach (AuditRelatedEntry entry in relatedEntries)
            {
                await TrackAuditEntryAsync(entry);
                AuditedRelatedEntries.Remove(entry);
            }
        }

        /// <inheritdoc />
        public void ClearServicesHistory()
        {
            OldServiceHistoryEntry = null;
            ServiceHistoryEntry = null;
        }

        /// <inheritdoc />
        public void RefreshAuditedEntries(ChangeTracker changeTracker)
        {
            ServiceHistoryEntry = null;
            List<AuditEntry> auditedEntries = new();
            List<AuditRelatedEntry> auditedRelatedEntries = new();

            if (OldServiceHistoryEntry is not null)
            {
                ServiceHistoryEntry = OldServiceHistoryEntry;
            }

            changeTracker.DetectChanges();
            foreach (EntityEntry entry in changeTracker.Entries())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                if (entry.Entity is TServicesHistory serviceHistory)
                {
                    if (ServiceHistoryEntry != null)
                    {
                        throw new DuplicatedServiceAuditHistoryException();
                    }
                    ServiceHistoryEntry = entry;
                    continue;
                }

                bool shouldAudit = entry.ShouldBeAudited();
                bool hasRelations = entry.HasRelationsToBeAudited();

                if (shouldAudit || hasRelations)
                {
                    if (shouldAudit)
                    {
                        auditedEntries.Add(new AuditEntry(entry));
                    }
                    else if (hasRelations)
                    {
                        auditedRelatedEntries.Add(new AuditRelatedEntry(entry));
                    }

                    auditedEntries.AddRange(
                        entry.GetRelatedEntriesToBeAudited()
                        .Where(x => !auditedEntries.Any(a => a.Entry.Entity == x.Entity))
                        .Select(x => new AuditEntry(x))
                    );
                }
            }
            AuditedEntries = auditedEntries;
            AuditedRelatedEntries = auditedRelatedEntries;
        }

        #endregion

        #region Private methods

        private bool IsNotTrackedOperationHistory(TOperationsHistory history)
        {
            return dbContext.ChangeTracker.Entries()
                .Any(x =>
                    x.State == EntityState.Added
                    && x.Entity is TOperationsHistory historyOperation
                    && historyOperation.TableName == history.TableName
                    && historyOperation.EntityName == history.EntityName
                    && historyOperation.EntityID == history.EntityID)
            ;
        }

        private void SetEntryLastChangesProperties(AuditEntry entry)
        {
            if (entry.Entry.Entity is AuditableEntity<TUsers, TUsersKey> entity)
            {
                entity.LastChangedByID = new CurrentUserGenerator<TUsersKey>().Next(entry.Entry);
                entity.LastChangedOn = new DateTimeUtcGenerator().Next(entry.Entry);

                switch (entry.Entry.State)
                {
                    case EntityState.Added:
                        dbContext.Add(entity);
                        break;
                    case EntityState.Modified:
                        dbContext.Update(entity);
                        break;
                    case EntityState.Deleted:
                        dbContext.Remove(entity);
                        break;
                }
            }
        }

        private void TrackAuditEntry(AuditEntry entry)
        {
            if (ServiceHistoryEntry == null && AuditedEntries.Any())
            {
                throw new MissingServiceHistoryException();
            }

            if (ServiceHistory != null)
            {
                SetEntryLastChangesProperties(entry);

                TOperationsHistory operationHistory = InstantiateOperationsHistory();
                operationHistory.GenerateValues(entry, ServiceHistory);

                if ((operationHistory.NewValues?.Any() ?? false) && !IsNotTrackedOperationHistory(operationHistory))
                {
                    dbContext.Add(operationHistory);
                }
            }
        }

        private async Task TrackAuditEntryAsync(AuditEntry entry)
        {
            if (ServiceHistoryEntry == null && AuditedEntries.Any())
            {
                throw new MissingServiceHistoryException();
            }

            if (ServiceHistory != null)
            {
                SetEntryLastChangesProperties(entry);

                TOperationsHistory operationHistory = InstantiateOperationsHistory();
                operationHistory.GenerateValues(entry, ServiceHistory);

                if ((operationHistory.NewValues?.Any() ?? false) && !IsNotTrackedOperationHistory(operationHistory))
                {
                    await dbContext.AddAsync(operationHistory);
                }
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Instantiates the operations history.
        /// </summary>
        /// <returns>The operations history.</returns>
        public abstract TOperationsHistory InstantiateOperationsHistory();

        #endregion
    }
}
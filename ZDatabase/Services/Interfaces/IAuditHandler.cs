using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZDatabase.Services.Interfaces
{
    /// <summary>
    /// Service for handling the entities auditory.
    /// </summary>
    public interface IAuditHandler
    {
        /// <summary>
        /// Gets a value indicating whether this instance has previous service history.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has previous service history; otherwise, <c>false</c>.
        /// </value>
        bool HasPreviousServiceHistory { get; }


        /// <summary>
        /// Adds the operation entities after saved.
        /// </summary>
        void AddOperationEntitiesAfterSaved();

        /// <summary>
        /// Adds the operation entities after saved asynchronous.
        /// </summary>
        /// <returns></returns>
        Task AddOperationEntitiesAfterSavedAsync();

        /// <summary>
        /// Adds the operation entities before saving.
        /// </summary>
        void AddOperationEntitiesBeforeSaving();

        /// <summary>
        /// Adds the operation entities before saving asynchronous.
        /// </summary>
        /// <returns></returns>
        Task AddOperationEntitiesBeforeSavingAsync();

        /// <summary>
        /// Clears the services history.
        /// </summary>
        void ClearServicesHistory();

        /// <summary>
        /// Refreshes the audited entries.
        /// </summary>
        /// <param name="changeTracker">The change tracker.</param>
        void RefreshAuditedEntries(ChangeTracker changeTracker);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZDatabase.Entries
{
    /// <summary>
    /// The audit entry in database.
    /// </summary>
    public class AuditEntry
    {
        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <value>
        /// The entry.
        /// </value>
        public EntityEntry Entry { get; }

        /// <summary>
        /// Gets the state of the original.
        /// </summary>
        /// <value>
        /// The state of the original.
        /// </value>
        public EntityState OriginalState { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AuditEntry"/> class.
        /// </summary>
        /// <param name="entry">The <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry"/> instance.</param>
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
            OriginalState = entry.State;
        }
    }
}
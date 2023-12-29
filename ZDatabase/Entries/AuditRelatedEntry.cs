using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZDatabase.Entries
{
    /// <summary>
    /// The audited related entry in database.
    /// </summary>
    /// <seealso cref="ZDatabase.Entries.AuditEntry" />
    public class AuditRelatedEntry
        : AuditEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRelatedEntry"/> class.
        /// </summary>
        /// <param name="entry">The <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry" /> instance.</param>
        public AuditRelatedEntry(EntityEntry entry)
            : base(entry)
        {
        }
    }
}
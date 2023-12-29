using Microsoft.EntityFrameworkCore;
using ZDatabase.Entries;

namespace ZDatabase.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="ZDatabase.Entries.AuditEntry"/>.
    /// </summary>
    internal static class AuditEntryExtensions
    {
        /// <summary>
        /// Gets the audited entry properties old values.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Dictionary with the changed properties and their values.</returns>
        internal static Dictionary<string, object?> GetOldValues(this AuditEntry entry)
        {
            return entry.OriginalState switch
            {
                EntityState.Deleted => entry.Entry.Properties.ToDictionary(k => k.Metadata.Name, v => v.OriginalValue),
                EntityState.Modified => entry.Entry.Properties.Where(x => x.HasValueModified()).ToDictionary(k => k.Metadata.Name, v => v.OriginalValue),
                _ => new Dictionary<string, object?>(),
            };
        }

        /// <summary>
        /// Gets the audited entry properties new values.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>Dictionary with the changed properties and their values.</returns>
        internal static Dictionary<string, object?> GetNewValues(this AuditEntry entry)
        {
            return entry.OriginalState switch
            {
                EntityState.Added => entry.Entry.Properties.ToDictionary(k => k.Metadata.Name, v => v.CurrentValue),
                EntityState.Modified => entry.Entry.Properties.Where(x => x.HasValueModified()).ToDictionary(k => k.Metadata.Name, v => v.CurrentValue),
                _ => new Dictionary<string, object?>(),
            };
        }
    }

    /// <summary>
    /// Extension methods for collections of <see cref="ZDatabase.Entries.AuditEntry"/>.
    /// </summary>
    internal static class AuditEntriesExtensions
    {
        internal static TAuditEntry[] EntriesWithoutTemporaryProperties<TAuditEntry>(this IEnumerable<TAuditEntry> entriesList)
            where TAuditEntry : AuditEntry
        {
            return entriesList.Where(x => !x.Entry.Properties.Any(x => x.IsTemporary)).ToArray();
        }
    }
}
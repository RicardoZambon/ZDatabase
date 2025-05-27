using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using ZDatabase.Attributes;
using ZDatabase.Entities;
using ZDatabase.Entities.Audit;

namespace ZDatabase.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry"/>.
    /// </summary>
    public static class EntityEntryExtensions
    {
        internal static IEnumerable<EntityEntry> GetManyToManyEntriesToBeAudited(this EntityEntry entry)
        {
            IEnumerable<IEnumerable?> relatedManyToManyEntries =
                entry.Metadata.GetReferencingForeignKeys()
                .SelectMany(x => x.GetReferencingSkipNavigations())
                .Where(x =>
                    (x.PropertyInfo?.CustomAttributes?.Any(a => a.AttributeType == typeof(AuditableRelationAttribute)) ?? false)
                    && x.TargetEntityType.ClrType.IsSubclassOf(typeof(AuditableEntityBase))
                )
                .Where(x => x.PropertyInfo != null && entry.Collection(x.PropertyInfo!.Name).CurrentValue != null)
                .Select(x => entry.Collection(x.PropertyInfo!.Name).CurrentValue)
            ;

            // Retrieve related entries
            List<EntityEntry> relatedEntries = [];
            foreach (IEnumerable? relationships in relatedManyToManyEntries)
            {
                if (relationships is null)
                {
                    continue;
                }

                foreach (object related in relationships)
                {
                    if (related is null)
                    {
                        continue;
                    }

                    EntityEntry relatedEntry = entry.Context.Entry(related);
                    if (relatedEntry is null)
                    {
                        continue;
                    }

                    relatedEntries.Add(relatedEntry);
                    relatedEntries.AddRange(relatedEntry.GetRelatedEntriesToBeAudited(entry));
                }
            }

            return relatedEntries;
        }

        /// <summary>
        /// Gets the related entries to be audited.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="originalEntry">The original entry.</param>"
        /// <returns></returns>
        internal static IEnumerable<EntityEntry> GetRelatedEntriesToBeAudited(this EntityEntry entry, EntityEntry? originalEntry = null)
        {
            List<EntityEntry> relatedEntries = [.. entry.Metadata.GetDeclaredForeignKeys()
                .Where(fk =>
                    fk.GetNavigation(false)?.PropertyInfo?.CustomAttributes?.Any(a => a.AttributeType == typeof(AuditableRelationAttribute)) ?? false
                    || (fk.GetReferencingSkipNavigations()
                        .Any(n => n.PropertyInfo?.CustomAttributes?.Any(a => a.AttributeType == typeof(AuditableRelationAttribute)) ?? false)
                    && fk.PrincipalEntityType.ClrType.IsSubclassOf(typeof(AuditableEntityBase)))
                )
                .Where(x => entry.CurrentValues[x.Properties[0].Name] != null)
                .Select(x =>
                    entry.Context.Entry(entry.Context.Find(x.PrincipalEntityType.ClrType, entry.CurrentValues[x.Properties[0].Name]) ?? new())
                )
                .Where(x => originalEntry is null || x != originalEntry)
            ];

            // Retrieve recursive related entries
            List<EntityEntry> additionalEntries = [];
            foreach (EntityEntry relatedEntry in relatedEntries)
            {
                additionalEntries.AddRange(relatedEntry.GetRelatedEntriesToBeAudited(originalEntry ?? entry));
            }
            relatedEntries.AddRange(additionalEntries);

            return relatedEntries;
        }

        /// <summary>
        /// Determines whether the entity has relations to be audited.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if the entity has relations to be audited; otherwise, <c>false</c>.
        /// </returns>
        internal static bool HasRelationsToBeAudited(this EntityEntry entry)
        {
            if (!entry.IsFromServicesHistoryBase() && !entry.IsFromOperationsHistoryBase())
            {
                return entry.State != EntityState.Detached
                    && entry.State != EntityState.Unchanged
                    && entry.Metadata.GetDeclaredForeignKeys()
                        .Any(fk =>
                            fk.GetReferencingSkipNavigations()
                                .Any(n => n.PropertyInfo?.CustomAttributes?.Any(a => a.AttributeType == typeof(AuditableRelationAttribute)) ?? false)
                            && fk.PrincipalEntityType.ClrType.IsSubclassOf(typeof(AuditableEntityBase)));
            }
            return false;
        }

        /// <summary>
        /// Determines whether the value was modified.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        ///   <c>true</c> if the value was modified; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasValueModified(this PropertyEntry property)
        {
            return property.IsModified
            && ((property.OriginalValue != null && property.CurrentValue == null)
                || (property.OriginalValue == null && property.CurrentValue != null)
                || (property.OriginalValue != null && property.CurrentValue != null && !property.OriginalValue.Equals(property.CurrentValue))
            );
        }

        /// <summary>
        /// Determines whether this instance is auditable.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if the specified entry is auditable; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsAuditable(this EntityEntry entry)
        {
            return entry.Entity.GetType().IsSubclassOf(typeof(AuditableEntityBase));
        }

        /// <summary>
        /// Determines whether this instance is from <see cref="ZDatabase.Entities.Audit.OperationsHistoryBase"/>.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if this instance is from <see cref="ZDatabase.Entities.Audit.OperationsHistoryBase"/>; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsFromOperationsHistoryBase(this EntityEntry entry)
        {
            return entry.Entity.GetType().IsSubclassOf(typeof(OperationsHistoryBase));
        }

        /// <summary>
        /// Determines whether this instance is from <see cref="ZDatabase.Entities.Audit.ServicesHistoryBase"/>.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if this instance is from <see cref="ZDatabase.Entities.Audit.ServicesHistoryBase"/>; otherwise, <c>false</c>.
        /// </returns>
        internal static bool IsFromServicesHistoryBase(this EntityEntry entry)
        {
            return entry.Entity.GetType().IsSubclassOf(typeof(ServicesHistoryBase));
        }

        /// <summary>
        /// Determines whether the entity should the be audited.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>
        ///   <c>true</c> if the entity should the be audited; otherwise, <c>false</c>.
        /// </returns>
        internal static bool ShouldBeAudited(this EntityEntry entry)
        {
            if (!entry.IsFromServicesHistoryBase() && !entry.IsFromOperationsHistoryBase())
            {
                return entry.State != EntityState.Detached
                    && entry.State != EntityState.Unchanged
                    && entry.IsAuditable();
            }
            return false;
        }

        internal static bool IsEnumerableTypeSubclassOf(this Type enumerableType, Type type)
        {
            if (enumerableType is null || !typeof(IEnumerable<>).IsAssignableFrom(enumerableType))
            {
                return false;
            }

            return enumerableType.GetGenericArguments().FirstOrDefault()?.IsAssignableFrom(type) ?? false;
        }
    }
}
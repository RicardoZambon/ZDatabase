using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ZDatabase.Attributes;
using ZDatabase.Entities;
using ZDatabase.Entities.Audit;

namespace ZDatabase.ExtensionMethods
{
    /// <summary>
    /// Extension methods for <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry"/>.
    /// </summary>
    internal static class EntityEntryExtensions
    {
        /// <summary>
        /// Gets the related entries to be audited.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        internal static IEnumerable<EntityEntry> GetRelatedEntriesToBeAudited(this EntityEntry entry)
        {
            return entry.Metadata.GetDeclaredForeignKeys()
                .Where(fk =>
                    fk.GetNavigation(false)?.PropertyInfo?.CustomAttributes?.Any(a => a.AttributeType == typeof(AuditableRelationAttribute)) ?? false
                    || (fk.GetReferencingSkipNavigations()
                        .Any(n => n.PropertyInfo?.CustomAttributes?.Any(a => a.AttributeType == typeof(AuditableRelationAttribute)) ?? false)
                    && fk.PrincipalEntityType.ClrType.IsSubclassOf(typeof(AuditableEntityBase)))
                )
                .Where(x => entry.CurrentValues[x.Properties[0].Name] != null)
                .Select(x =>
                    entry.Context.Entry(entry.Context.Find(x.PrincipalEntityType.ClrType, entry.CurrentValues[x.Properties[0].Name]) ?? new())
                );
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
        internal static bool HasValueModified(this PropertyEntry property)
        {
            return property.IsModified
            && (
                property.OriginalValue != null && property.CurrentValue == null
                || property.OriginalValue == null && property.CurrentValue != null
                || (
                    property.OriginalValue != null && property.CurrentValue != null
                    && !property.OriginalValue.Equals(property.CurrentValue)
                )
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
    }
}
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ZDatabase.EntityFrameworkCore.Audit.BusinessEntities.Audit;
using ZDatabase.EntityFrameworkCore.Audit.Entries;
using ZDatabase.EntityFrameworkCore.Common.BusinessEntities;

namespace ZDatabase.EntityFrameworkCore.Audit.ExtensionMethods
{
    internal static class OperationsHistoryExtensions
    {
        internal static void GenerateValues<TServiceHistory, TUsers, TUsersKey>(this OperationsHistory<TServiceHistory, TUsers, TUsersKey> operationsHistory, AuditEntry entry, TServiceHistory servicesHistory)
            where TServiceHistory : ServicesHistory<TUsers, TUsersKey>
            where TUsers : class
            where TUsersKey : struct
        {
            operationsHistory.EntityID = entry.Entry.Entity.GetType().IsSubclassOf(typeof(Entity)) ? (long)(entry.Entry.Property(nameof(Entity.ID)).CurrentValue ?? 0) : null;
            operationsHistory.EntityName = entry.Entry.Metadata.DisplayName();
            operationsHistory.NewValues = JsonSerializer.Serialize(entry.GetNewValues());
            operationsHistory.OldValues = JsonSerializer.Serialize(entry.GetOldValues());
            operationsHistory.OperationType = entry.OriginalState.ToString();
            operationsHistory.ServiceHistory = servicesHistory;
            operationsHistory.ServiceHistoryID = servicesHistory.ID;
            operationsHistory.TableName = entry.Entry.Metadata.GetTableName();
        }
    }
}
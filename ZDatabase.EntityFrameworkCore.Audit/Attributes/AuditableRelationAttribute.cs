namespace ZDatabase.EntityFrameworkCore.Audit.Attributes
{
    /// <summary>
    /// Attribute for auditable relationships.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public class AuditableRelationAttribute
        : Attribute
    {
    }
}
namespace ZDatabase.Attributes
{
    /// <summary>
    /// Attribute for auditable relationships.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AuditableRelationAttribute
        : Attribute
    {
    }
}
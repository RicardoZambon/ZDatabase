namespace ZDatabase.Services.Interfaces
{
    /// <summary>
    /// Service for providing the current user running the application.
    /// </summary>
    /// <typeparam name="TUserKey">The type of the user key.</typeparam>
    public interface ICurrentUserProvider<TUserKey>
        where TUserKey : struct
    {
        /// <summary>
        /// Gets the current user identifier.
        /// </summary>
        /// <value>
        /// The current user identifier.
        /// </value>
        TUserKey? CurrentUserID { get; }
    }
}
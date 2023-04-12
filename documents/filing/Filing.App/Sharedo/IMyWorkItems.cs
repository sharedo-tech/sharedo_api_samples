namespace Filing.App.Sharedo
{
    public interface IMyWorkItems
    {
        /// <summary>
        /// Get identities of most recent work items.
        /// </summary>
        /// <param name="count">The count of items to retrieve.</param>
        /// <returns>Collection of the identities.</returns>
        Task<IList<Guid>> GetMostRecent(int count);

        /// <summary>
        /// Get identities of most viewed work items.
        /// </summary>
        /// <param name="count">The count of items to retrieve.</param>
        /// <returns>Collection of the identities.</returns>
        Task<IList<Guid>> GetMostViewed(int count);

        /// <summary>
        /// Get identities of bookmarked work items.
        /// </summary>
        /// <param name="count">The count of items to retrieve.</param>
        /// <returns>Collection of the identities.</returns>
        Task<IList<Guid>> GetBookmarked(int count);
    }
}

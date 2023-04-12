using Filing.App.Sharedo.Contracts;

namespace Filing.App.Sharedo
{
    public interface IWorkItemSearches
    {
        /// <summary>
        /// Find matter by its id.
        /// </summary>
        /// <param name="workItemIds">The work item ids.</param>
        /// <returns>The work item if found, null if not found.</returns>
        Task<IList<WorkItem>> FindByIds(IList<Guid> workItemIds);

        /// <summary>
        /// Find matters by their title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByTitle(string title);

        /// <summary>
        /// Find matters by their reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByReference(string reference);

        /// <summary>
        /// Find matters by their client id.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByClientId(Guid clientId);

        /// <summary>
        /// Find matters by their client name.
        /// </summary>
        /// <param name="clientName">The client name.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByClientName(string clientName);

        /// <summary>
        /// Find matters by a roles which have a postcode, for example role 'client' with postcode 'SE11AB'.
        /// </summary>
        /// <param name="roles">The roles.</param>
        /// <param name="postCode">The post code.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByRolesWithPostCode(string[] roles, string postCode);

        /// <summary>
        /// Find matters by a roles entity having an email contact.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByEmail(string email);

        /// <summary>
        /// Find matters by an attribute value.
        /// </summary>
        /// <param name="key">The attribute key.</param>
        /// <param name="value">The attribute value to match exactly.</param>
        /// <returns>Collection of the work items</returns>
        Task<IList<WorkItem>> FindMattersByAttribute(string key, string value);

        /// <summary>
        /// Find document expectations for a work item
        /// </summary>
        /// <param name="workItemId">The work item id.</param>
        /// <returns>Collection of the work items.</returns>
        Task<IList<WorkItem>> FindDocumentExpectationForMatter(Guid workItemId);
    }
}

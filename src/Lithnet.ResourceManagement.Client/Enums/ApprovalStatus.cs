namespace Lithnet.ResourceManagement.Client
{
    /// <summary>
    /// The status of the approval request
    /// </summary>
    public enum ApprovalStatus
    {
        /// <summary>
        /// The status is unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// The request is pending approval
        /// </summary>
        Pending,

        /// <summary>
        /// The request has been approved
        /// </summary>
        Approved,

        /// <summary>
        /// The request has been rejected
        /// </summary>
        Rejected,

        /// <summary>
        /// The request has expired
        /// </summary>
        Expired,
    }
}

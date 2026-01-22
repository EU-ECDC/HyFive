using Reinforced.Typings.Attributes;

namespace HyFive.Models.V1.Session
{
    /// <summary>
    /// Defines the available transfer statuses used for filtering transfer sessions.
    /// </summary>
    [TsEnum(IncludeNamespace = false)]
    public enum TransferStatusType
    {
        /// <summary>
        /// Indicates that the session has been transferred to an administrator.
        /// </summary>
        TransferredToAdmin,

        /// <summary>
        /// Indicates that the session has been transferred to a coordinator.
        /// </summary>
        TransferredToCoordinator
    }
}

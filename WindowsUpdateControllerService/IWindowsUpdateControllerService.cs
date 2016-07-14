using System.ServiceModel;

namespace WindowsUpdateControllerService
{
    /// <summary>
    /// Contract for handling Windows Update service
    /// </summary>
    [ServiceContract]
    public interface IWindowsUpdateControllerService
    {
        /// <summary>
        /// Checks connection to the Windows Update service controller.
        /// </summary>
        /// <remarks>
        /// This actually need to be refactored, since semantically,
        /// the <see cref="Ping"/> function should only be used by the host service to check
        /// the connection to the host service.
        /// At the current implementation, it is deliberately 
        /// </remarks>
        /// <returns>true</returns>
        [OperationContract]
        bool Ping();

        /// <summary>
        /// Checks the status of host's Windows Update service.
        /// </summary>
        /// <returns>true if the Windows Update service is enabled.</returns>
        [OperationContract]
        bool IsWindowsUpdateEnabled();

        /// <summary>
        /// Enables/disables the host's Windows Update service.
        /// </summary>
        /// <param name="enabled">true means enable the Windows Update service, and vice versa.</param>
        [OperationContract]
        void SetWindowsUpdateService(bool enabled);

        /// <summary>
        /// Resets the host's Windows Update service, based on the setting defined in the persistent storage.
        /// </summary>
        [OperationContract]
        void ResetWindowsUpdate();
    }
}

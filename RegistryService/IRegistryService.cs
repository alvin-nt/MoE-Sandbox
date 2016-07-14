using System.Collections.Generic;
using System.ServiceModel;

namespace RegistryService
{
    /// <summary>
    /// Contains definition for the registry service
    /// </summary>
    [ServiceContract]
    public interface IRegistryService
    {
        [OperationContract]
        void SetRegistryRulesStates(Dictionary<RegistryIdentifier, object> rules, string username);

        /// <summary>
        /// Resets the registry state, using stored file.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool ResetRegistry();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Dictionary<RegistryIdentifier, object> GetCurrentRegistryRulesState();
    }
}

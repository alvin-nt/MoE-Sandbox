using System;
using System.Security.Principal;

namespace RegistryService
{
    /// <summary>
    /// Contains utilites related to the registry.
    /// </summary>
    public static class RegistryUtils
    {
        /// <summary>
        /// Gets the security identifier (SID) of the user.
        /// </summary>
        /// <param name="username">The username of the user, as defined in the Users folder.</param>
        /// <remarks>This method may throw an exception.</remarks>
        /// <returns>the SID of the username</returns>
        public static string GetSidFromUsername(string username)
        {
            var account = new NTAccount(username);
            var sid = (SecurityIdentifier)account.Translate(typeof(SecurityIdentifier));
            return sid.ToString();
        }
    }
}

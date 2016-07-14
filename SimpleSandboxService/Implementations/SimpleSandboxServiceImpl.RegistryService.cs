using System.Collections.Generic;
using RegistryService;
using System;

namespace SimpleSandboxService.Implementations
{
    public partial class SimpleSandboxServiceImpl : IRegistryService
    {
        public void SetRegistryRulesStates(Dictionary<RegistryIdentifier, object> rules, string username)
        {
            var userSid = RegistryUtils.GetSidFromUsername(username);
            foreach (var rule in rules)
            {
                try
                {
                    var regEntry = RegistryEntry.GetRegistryEntry(rule.Key, userSid);
                    regEntry.ValueData = rule.Value;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Logger.Warn(ex, "");
                }
            }
        }

        public bool ResetRegistry()
        {
            throw new NotImplementedException();
        }

        public Dictionary<RegistryIdentifier, object> GetCurrentRegistryRulesState()
        {
            throw new NotImplementedException();
        }
    }
}

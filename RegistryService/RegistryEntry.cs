using System;
using Microsoft.Win32;
using NLog;

namespace RegistryService
{
    /// <summary>
    /// Parent class registry entry for specific registry entries
    /// </summary>
    /// <remarks>
    /// Original source code example from 
    /// https://github.com/SafeExamBrowser/seb-win/blob/master/SebWindowsServiceWCF/RegistryHandler/RegistryEntry.cs
    /// </remarks>
    public abstract class RegistryEntry
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The complete path to the registry key starting with HKEY_[...]
        /// </summary>
        public string RegistryKey { get; protected set; }

        /// <summary>
        /// The name of the value
        /// </summary>
        public string ValueName { get; protected set; }

        /// <summary>
        /// The datatype of the value (e.g. REG_DWORD = Int32, REG_SZ = String)
        /// </summary>
        public Type ValueType { get; protected set; }

        /// <summary>
        /// The SID of the user for which the registry entries should be changed (SubKey of HK_USERS)
        /// </summary>
        protected string UserSid;

        protected RegistryEntry(string sid)
        {
            UserSid = sid;
        }

        /// <summary>
        /// The registry value data.
        /// </summary>
        /// <remarks>
        /// May be null, if the registry key does not exist.
        /// Setting this to null will cause the corresponding registry value to be deleted.
        /// </remarks>
        public object ValueData
        {
            get
            {
                try
                {
                    return Registry.GetValue(RegistryKey, ValueName, null);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Unable to get registry entry for key {RegistryKey} and value {ValueName}");
                    throw;
                }
            }

            set
            {
                try
                {
                    if (value == null)
                    {
                        if (ValueData == null) return; // do nothing if it is also null
                        try // null means delete.
                        {
                            var regKey = GetHiveFromKey(RegistryKey);

                            // Load the subkey
                            regKey = regKey.OpenSubKey(RegistryKey.Substring(RegistryKey.IndexOf('\\') + 1), true);

                            // If the subkey exists, delete the value
                            regKey?.DeleteValue(ValueName);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex, $"Cannot delete value {ValueName} from registry key {RegistryKey}.");
                            throw;
                        }
                    }
                    else
                    {
                        if (value.GetType() == ValueType)
                        {
                            Registry.SetValue(RegistryKey, ValueName, value);
                        }
                        else
                        {
                            Logger.Error($"Cannot set value {ValueName}:{value} for registry key {RegistryKey}. " +
                                         $"Expected type: {ValueType}, Actual type: {value.GetType()}");
                            throw new ArgumentException("The value is of the wrong type");
                        }
                    }
                }
                catch (ArgumentException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Cannot set value {ValueName}:{value} for registry key {RegistryKey}.");
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the hive (root registry key) object from a given full registry key path.
        /// </summary>
        /// <param name="fullKeyPath">The full key path to the registry key.</param>
        /// <remarks>Throws <see cref="ArgumentException"/> on </remarks>
        /// <returns>Registry Key object</returns>
        protected static RegistryKey GetHiveFromKey(string fullKeyPath)
        {
            if (fullKeyPath.StartsWith("HKEY_USERS"))
                return Registry.Users;
            if (fullKeyPath.StartsWith("HKEY_LOCAL_MACHINE"))
                return Registry.LocalMachine;

            throw new ArgumentException($"Unknown hive for path {fullKeyPath}");
        }

        /// <summary>
        /// Gets an entry for a registry rule.
        /// </summary>
        /// <param name="identifier">The identifier enumeration.</param>
        /// <param name="sid">The user's SID</param>
        /// <returns>The registry entry</returns>
        public static RegistryEntry GetRegistryEntry(RegistryIdentifier identifier, string sid)
        {
            switch (identifier)
            {
                case RegistryIdentifier.DisableLockWorkstation:
                    return new RegDisableWorkstation(sid);
                case RegistryIdentifier.DisableTaskManager:
                    return new RegDisableTaskManager(sid);
                case RegistryIdentifier.DisableChangePassword:
                    return new RegDisableChangePassword(sid);
                case RegistryIdentifier.DisableContextMenuView:
                    return new RegDisableContextMenuView(sid);
                case RegistryIdentifier.HideFastUserSwitching:
                    return new RegHideFastUserSwitching(sid);
                case RegistryIdentifier.DisableLogoff:
                    return new RegDisableLogoff(sid);
                case RegistryIdentifier.DisableClose:
                    return new RegDisableClose(sid);
                case RegistryIdentifier.EaseOfAccess:
                    return new RegEaseOfAccess(sid);
                case RegistryIdentifier.DontDisplayNetworkSelectionUi:
                    return new RegDontDisplayNetworkSelectionUi(sid);
                default:
                    throw new ArgumentOutOfRangeException(nameof(identifier), identifier,
                        $"Undefined registry rule for identifier {identifier}");
            }
        }
    }

    /// <summary>
    /// Registry entry for disabling lock workstation function.
    /// </summary>
    public class RegDisableWorkstation : RegistryEntry
    {
        public RegDisableWorkstation(string sid) : base(sid)
        {
            RegistryKey = $@"HKEY_USERS\{UserSid}\Software\Microsoft\Windows\CurrentVersion\Policies\System";
            ValueName = "DisableLockWorkstation";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for disabling task manager.
    /// </summary>
    public class RegDisableTaskManager : RegistryEntry
    {
        public RegDisableTaskManager(string sid) : base(sid)
        {
            RegistryKey = $@"HKEY_USERS\{UserSid}\Software\Microsoft\Windows\CurrentVersion\Policies\System";
            ValueName = "DisableTaskMgr";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for disabling change password function.
    /// </summary>
    public class RegDisableChangePassword : RegistryEntry
    {
        public RegDisableChangePassword(string sid) : base(sid)
        {
            RegistryKey = $@"HKEY_USERS\{UserSid}\Software\Microsoft\Windows\CurrentVersion\Policies\System";
            ValueName = "DisableChangePassword";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for disabling view to context menu. This basically disables right-click capability.
    /// </summary>
    public class RegDisableContextMenuView : RegistryEntry
    {
        public RegDisableContextMenuView(string sid) : base(sid)
        {
            RegistryKey = $@"HKEY_USERS\{UserSid}\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            ValueName = "NoViewContextMenu";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for hiding fast user switching.
    /// </summary>
    public class RegHideFastUserSwitching : RegistryEntry
    {
        public RegHideFastUserSwitching(string sid) : base(sid)
        {
            RegistryKey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\System";
            ValueName = "HideFastUserSwitching";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for disabling logoff function.
    /// </summary>
    public class RegDisableLogoff : RegistryEntry
    {
        public RegDisableLogoff(string sid) : base(sid)
        {
            RegistryKey = $@"HKEY_USERS\{UserSid}\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            ValueName = "NoLogoff";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for disabling close function.
    /// </summary>
    public class RegDisableClose : RegistryEntry
    {
        public RegDisableClose(string sid) : base(sid)
        {
            RegistryKey = $@"HKEY_USERS\{UserSid}\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
            ValueName = "NoClose";
            ValueType = typeof(int); // DWORD
        }
    }

    /// <summary>
    /// Registry entry for enabling accessability functions.
    /// </summary>
    public class RegEaseOfAccess : RegistryEntry
    {
        public RegEaseOfAccess(string sid) : base(sid)
        {
            RegistryKey =
                @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\Utilman.exe";
            ValueName = "Debugger";
            ValueType = typeof(string); // SZ
        }
    }

    /// <summary>
    /// Registry entry for not displaying network selection UI
    /// </summary>
    public class RegDontDisplayNetworkSelectionUi : RegistryEntry
    {
        public RegDontDisplayNetworkSelectionUi(string sid) : base(sid)
        {
            RegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\System";
            ValueName = "DontDisplayNetworkSelectionUI";
            ValueType = typeof(int); // DWORD
        }
    }
}
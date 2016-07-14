namespace RegistryService
{
    /// <summary>
    /// Rules related for the registry entries.
    /// These will be used for blocking certain Windows functions.
    /// </summary>
    public enum RegistryIdentifier
    {
        DisableLockWorkstation,
        DisableTaskManager,
        DisableChangePassword,
        DisableContextMenuView, // a.k.a. disables right click
        HideFastUserSwitching,
        DisableLogoff,
        DisableClose,
        EaseOfAccess,
        DontDisplayNetworkSelectionUi
    }
}
using System;

namespace MoE_Sandbox.Virtualization.Filesystem.Host
{
    /// <summary>
    /// Represents generic access mask for resource access
    /// Copied from http://www.pinvoke.net/default.aspx/Enums/ACCESS_MASK.html
    /// </summary>
    [Flags]
    public enum AccessMask : uint
    {
        Delete = 0x00010000,
        ReadControl = 0x00020000,
        WriteDac = 0x00040000,
        WriteOwner = 0x00080000,
        Synchronize = 0x00100000,

        StandardRightsRequired = 0x000F0000,

        StandardRightsRead = 0x00020000,
        StandardRightsWrite = 0x00020000,
        StandardRightsExecute = 0x00020000,

        StandardRightsAll = 0x001F0000,

        SpecificRightsAll = 0x0000FFFF,

        AccessSystemSecurity = 0x01000000,

        MaximumAllowed = 0x02000000,

        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000,

        DesktopReadObjects = 0x00000001,
        DesktopCreateWindow = 0x00000002,
        DesktopCreateMenu = 0x00000004,
        DesktopHookControl = 0x00000008,
        DesktopJournalRecord = 0x00000010,
        DesktopJournalPlayback = 0x00000020,
        DesktopEnumerate = 0x00000040,
        DesktopWriteObjects = 0x00000080,
        DesktopSwitchDesktop = 0x00000100,

        WinStaEnumDesktops = 0x00000001,
        WinStaReadAttributes = 0x00000002,
        WinStaAccessClipboard = 0x00000004,
        WinStaCreateDesktop = 0x00000008,
        WinStaWriteAttributes = 0x00000010,
        WinStaAccessGlobalAtoms = 0x00000020,
        WinStaExitWindows = 0x00000040,
        WinStaEnumerate = 0x00000100,
        WinStaReadScreen = 0x00000200,

        WinStaAllAccess = 0x0000037F
    }
}

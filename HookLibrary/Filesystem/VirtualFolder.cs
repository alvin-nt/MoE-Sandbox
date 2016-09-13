namespace HookLibrary.Filesystem
{
    public enum VirtualFolder
    {
        // System Data
        System,
        System32,
        ProgramFiles,
        ProgramFilesX86,
        StartMenu,

        // User Data
        ApplicationData,
        UserData,
        UserDocuments,
        UserPictures,
        UserMusic,
        UserVideos,
        Recent,
        Other,
        Temporary,

        // Internet-related stuff
        InternetCache,
        Cookies,
        InternetHistory,
        Favorites
    }

    public static class VirtualFolderExtensions
    {
        /// <summary>
        /// Gets the path to the virtual folder identified by the <see cref="VirtualFolder"/> specified.
        /// The returned path is relative to the file system's root directory.
        /// </summary>
        /// <remarks> The returned path is not guaranteed to exist.</remarks>
        /// <param name="virtualFolder">An enumerated constant that identifies a system virtual folder.</param>
        /// <returns>The path to the specified <see cref="VirtualFolder"/>.</returns>
        public static string ToPath(this VirtualFolder virtualFolder)
        {
            switch (virtualFolder)
            {
                case VirtualFolder.ProgramFiles:
                    return @"Program Files\";
                case VirtualFolder.ProgramFilesX86:
                    return @"Program Files (x86)\";
                case VirtualFolder.UserData:
                    return @"UserData\";
                case VirtualFolder.UserDocuments:
                    return @"UserData\Documents\";
                case VirtualFolder.UserMusic:
                    return @"UserData\Music\";
                case VirtualFolder.UserPictures:
                    return @"UserData\Pictures\";
                case VirtualFolder.UserVideos:
                    return @"UserData\Videos\";
                case VirtualFolder.Temporary:
                    return @"Temporary\";
                case VirtualFolder.Other:
                    return @"Other\";
                case VirtualFolder.System:
                    return @"System\";
                case VirtualFolder.System32:
                    return @"System\System32\";
                case VirtualFolder.StartMenu:
                    return @"StartMenu\";
                case VirtualFolder.ApplicationData:
                    return @"ApplicationData\";
                case VirtualFolder.InternetCache:
                    return @"INetCache\";
                case VirtualFolder.Cookies:
                    return @"INetCookies\";
                case VirtualFolder.InternetHistory:
                    return @"INetHistory\";
                default:
                    return null;
            }
        }
    }
}
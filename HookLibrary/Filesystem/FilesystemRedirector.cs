using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using NLog;

namespace HookLibrary.Filesystem
{
    public class FilesystemRedirector : MarshalByRefObject
    {
        /// <summary>
        /// Logger interface that is going to be used by the invoking hook functions.
        /// </summary>
        // TODO: not working, all output is redirected to console.
        public readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<string, VirtualFolder> _systemVariables;

        /// <summary>
        /// Path to the root of folder, in which all calls would be redirected to.
        /// </summary>
        protected readonly string _basePath;

        private readonly string _windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        private readonly string _system32Path = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);

        private readonly string _systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);

        public FilesystemRedirector()
        {
            // TODO: do the actual redirection stuff
            // TODO: path currently hardcoded; need to refactor
            _basePath = "D:\\test\\virtual\\";
            _systemVariables = InitializeSystemVariables();

            InitializeVirtualFolders();
        }

        private static IDictionary<string, VirtualFolder> InitializeSystemVariables()
        {
            IDictionary<string, VirtualFolder> systemVariables = new Dictionary<string, VirtualFolder>();

            // Always check if the dictionary doesn't already contain the same key.
            // The users might have configured the specialfolders to use the same folder.
            // BUG: Such configurations might lead to inconsistencies between different host systems.

            // Documents
            var tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserDocuments);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserDocuments);

            // Pictures
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserPictures);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserPictures);

            // Music
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserPictures);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserMusic);

            // Videos
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserVideos);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserVideos);

            // UserData
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserData);

            // Application Data
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.ApplicationData);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToLowerInvariant();
            if (!systemVariables.ContainsKey(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.ApplicationData);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.ApplicationData);

            // Temporary Folders
            var folders = GetTemporaryFolders();
            foreach (var folder in folders)
            {
                tmp = folder.ToLowerInvariant();
                if (!systemVariables.ContainsKey(tmp))
                    systemVariables.Add(tmp, VirtualFolder.Temporary);
            }

            // Internet cache
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache).ToLowerInvariant();
            if (!systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.InternetCache);
            
            // Internet cookies
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.Cookies).ToLowerInvariant();
            if (!systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.Cookies);

            // Internet history
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.History).ToLowerInvariant();
            if (!systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.InternetHistory);

            // Program Files
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.ProgramFiles);

            // Program files (x86)
            if (Environment.Is64BitOperatingSystem)
            {
                tmp = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).ToLowerInvariant();
                if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                    systemVariables.Add(tmp, VirtualFolder.ProgramFilesX86);
            }

            // System
            tmp = Environment.GetEnvironmentVariable("systemroot");
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.System);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.System).ToLowerInvariant();
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
                systemVariables.Add(tmp, VirtualFolder.System32);

            // Start Menu
            tmp = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.StartMenu);

            tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
                systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.StartMenu);

            return systemVariables;
        }

        private static IEnumerable<string> GetTemporaryFolders()
        {
            var folders = new List<string>(3) { Path.GetTempPath() };
            // Get user specific temporary folders.
            // Get system temporary folders.
            const string regSystemEnvironmentPath =
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment\";
            var value = Registry.GetValue(regSystemEnvironmentPath, "TEMP", null);
            if (value != null)
                folders.Add(Path.GetFullPath(value.ToString()));
            value = Registry.GetValue(regSystemEnvironmentPath, "TMP", null);
            if (value != null)
                folders.Add(Path.GetFullPath(value.ToString()));
            return folders;
        }

        private void InitializeVirtualFolders()
        {
            foreach (var virtualFolder in _systemVariables.Values)
            {
                var directoryPath = _basePath + virtualFolder.ToPath();

                // try to create directory
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
        }

        public bool Ping()
        {
            return true;
        }

        public void WriteToConsole(string s)
        {
            Console.WriteLine($"[{DateTime.Now}] {s}");
        }

        
        public string RedirectPath(string originalNtPath, string rootPath)
        {
            // check the root
            if (!string.IsNullOrEmpty(rootPath))
            {
                // assumption: just combine the root path with the originalNtPath
                // TODO: check
                originalNtPath = rootPath + originalNtPath;
            }

            // determine the existence of \?? at the beginning
            // this indicates a path to a mounted drive/folder, e.g. C:\
            if (originalNtPath.StartsWith(@"\??"))
            {
                // check the path
                var originalDosPath = originalNtPath.Substring(4);
                string redirectedDosPath = null;

                // skip if it is from the original source
                if (originalDosPath.StartsWith(_basePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    redirectedDosPath = originalDosPath;
                }

                // if referring to System32 or Windows, redirect them
                if (originalDosPath.StartsWith(_system32Path, StringComparison.InvariantCultureIgnoreCase) ||
                    originalDosPath.StartsWith(_systemPath, StringComparison.InvariantCultureIgnoreCase) ||
                    originalDosPath.StartsWith(_windowsPath, StringComparison.InvariantCultureIgnoreCase))
                {
                    redirectedDosPath = originalDosPath;
                }

                // TODO: refactor this to other function
                foreach (var systemPath in _systemVariables.Keys)
                {
                    if (!originalDosPath.StartsWith(systemPath)) continue;

                    VirtualFolder virtualFolder;
                    _systemVariables.TryGetValue(systemPath, out virtualFolder);
                    redirectedDosPath = _basePath + virtualFolder.ToPath() + originalDosPath.Substring(systemPath.Length);
                    break;
                }

                // redirect to 'Other' path instead.
                if (string.IsNullOrEmpty(redirectedDosPath))
                {
                    redirectedDosPath = _basePath + VirtualFolder.Other.ToPath() + originalDosPath.Substring(3);
                }

                // should not be null at this point
                return @"\??\" + redirectedDosPath;
            }

            // TODO: handle \Device\...
            return originalNtPath;
        }
    }
}

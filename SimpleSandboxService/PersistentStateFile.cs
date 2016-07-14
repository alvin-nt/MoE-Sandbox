using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NLog;
using RegistryService;

namespace SimpleSandboxService
{
    /// <summary>
    /// Wrapper for storing serializable system state.
    /// </summary>
    [Serializable]
    public struct SystemState
    {
        /// <summary>
        /// Stores all registry entry status.
        /// </summary>
        public Dictionary<RegistryIdentifier, object> RegistryValues
        { get; set; }

        /// <summary>
        /// Stores the current username.
        /// </summary>
        public string Username
        { get; set; }

        /// <summary>
        /// Stores the Windows Update service status.
        /// </summary>
        public bool WindowsUpdateIsEnabled
        { get; set; }

    }

    /// <summary>
    /// Wrapper for the file that stores the current system state.
    /// </summary>
    public class PersistentStateFile : IDisposable
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly string FilePath;

        /// <summary>
        /// The current system state
        /// </summary>
        protected SystemState SystemState;

        /// <summary>
        /// Create an in-memory instance of a persistent registry file.
        /// If a file is existing it gets automatically loaded into memory.
        /// </summary>
        /// <param name="username">The username of the currently logged in user - needed to identify the correct registry key path</param>
        public PersistentStateFile(string username = null)
        {
            SystemState = new SystemState
            {
                RegistryValues = new Dictionary<RegistryIdentifier, object>(),
                Username = username ?? ""
            };

            try
            {
                // The file is stored where the executable of the service is
                // TODO: move this to Configuration
                FilePath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\systemstate.srg";
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unable to build path for persistent service file");
                throw;
            }            

            if (File.Exists(FilePath))
                Load();
        }

        /// <summary>
        /// Loads the content of a saved file into memory
        /// Throws Exception if something goes wrong
        /// </summary>
        private void Load()
        {
            FileStream stream = null;

            try
            {
                using (stream = File.OpenRead(FilePath))
                {
                    var deserializer = new BinaryFormatter();
                    SystemState = (SystemState)deserializer.Deserialize(stream);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                stream?.Close();
                Logger.Error(ex, $"Unable to open persistent system state file at {FilePath}.");
                throw;
            }
        }

        /// <summary>
        /// Saves the currently stored registry information into a binary encoded file
        /// Throws Exception if something goes wrong
        /// </summary>
        public void Save()
        {
            FileStream stream = null;
            try
            {
                using (stream = File.Open(FilePath, FileMode.Create))
                {
                    var serializer = new BinaryFormatter();
                    serializer.Serialize(stream, SystemState);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                stream?.Close();
                Logger.Error(ex, $"Unable to save persistent system state file at {FilePath}.");
                throw;
            }
        }

        /// <summary>
        /// Delete the persistens registry file if it exists.
        /// Throws Exception if something goes wrong.
        /// </summary>
        public void Delete()
        {
            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Unable to delete persistent system state file at {FilePath}.");
                throw;
            }
        }

        public void Dispose()
        {

        }
    }
}
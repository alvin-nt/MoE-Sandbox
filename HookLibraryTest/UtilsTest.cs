using System;
using System.IO;
using System.Runtime.ExceptionServices;
using NUnit.Framework;
using HookLibrary.Filesystem.Host;
using HookLibrary.Filesystem.Host.NativeTypes;

namespace HookLibraryTest
{
    [TestFixture]
    public class UtilsTest
    {
        #region NtHandle Test

        [Test]
        public void Should_Reject_Invalid_Handles()
        {
            var nullHandle = IntPtr.Zero;
            string path;

            var retCode = Utils.GetNtPathFromHandle(nullHandle, out path);
            Assert.That(retCode == NtStatus.InvalidHandle);
            Assert.That(path.Equals(""));

            // simulates invalid handle value
            var invalidHandle = new IntPtr(0xFFFFFFFF);
            retCode = Utils.GetNtPathFromHandle(invalidHandle, out path);
            Assert.That(retCode == NtStatus.InvalidHandle);
            Assert.That(path.Equals(""));
        }

        [Test]
        public void Should_Convert_ConsolePath_From_NtHandle_To_NtPath_Correctly()
        {
            // simulation using
            var file = File.OpenRead(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\notepad.exe");
            var handle = file.SafeFileHandle;
            Assert.NotNull(handle);
            Assert.That(!handle.IsInvalid);

            string ntPath;
            var retCode = Utils.GetNtPathFromHandle(handle.DangerousGetHandle(), out ntPath);
            const string expectedString = @"\Device\HarddiskVolume3\Windows\system32\notepad.exe";

            Assert.That(retCode == NtStatus.Success,
                $"Error in invoking function! Expected status: {NtStatus.Success}, actual: {retCode}");
            Assert.That(ntPath.Equals(expectedString, StringComparison.CurrentCultureIgnoreCase),
                $"Error in getting filename! Expected: {expectedString}, actual: {ntPath}");

            // we're done here~
            file.Close();
        }

        #endregion

        #region NtPath Test

        [Test]
        public void Should_Convert_Win7MupNetworkPath_From_NtPath_To_DosPath_Correctly()
        {
            const string ntPath = @"\Device\Mup\1101";
            string dosPath;

            var retCode = Utils.GetDosPathFromNtPath(ntPath, out dosPath);
            Assert.That(dosPath.Equals(@"\\1101", StringComparison.CurrentCultureIgnoreCase));
            Assert.That(retCode == WinStatusCode.Success);
        }

        [Test]
        public void Should_Convert_WinXpLanmanNetworkPath_From_NtPath_To_DosPath_Correctly()
        {
            // testing Lanmanredirector (from Windows XP)
            const string ntPath = @"\Device\LanmanRedirector\1101";
            string dosPath;

            var retCode = Utils.GetDosPathFromNtPath(ntPath, out dosPath);
            var expectedString = @"\\1101";
            Assert.That(retCode == WinStatusCode.Success,
                $"Error in invoking function! Expected status: {WinStatusCode.Success}, actual: {retCode}");
            Assert.That(dosPath.Equals(expectedString, StringComparison.CurrentCultureIgnoreCase),
                $"Error in conversion! Expected: {expectedString}, actual: {dosPath}");
        }

        [Test]
        public void Should_Convert_Filename_From_NtPath_To_DosPath_Correctly()
        {
            // Note: this only tests the paths in C:
            // also assumes that notepad exists in the system.
            // Volume1 is A, Volume2 is B.
            const string ntPath = @"\Device\HarddiskVolume3\Windows\system32\notepad.exe";
            string dosPath;

            var retCode = Utils.GetDosPathFromNtPath(ntPath, out dosPath);

            // note that this function just converts stuff.. doesn't matter whether it exists or not.
            const string expectedString = @"C:\Windows\system32\notepad.exe";

            Assert.That(retCode == WinStatusCode.Success,
                $"Error in invoking function! Expected status: {WinStatusCode.Success}, actual: {retCode}");
            Assert.That(dosPath.Equals(expectedString, StringComparison.CurrentCultureIgnoreCase),
                $"Error in conversion! Expected: {expectedString}, actual: {dosPath}");
        }

        [Test]
        public void Should_Emit_UnknownPortError_On_Invalid_SerialDevices()
        {
            const string ntPath = @"\Device\Serial\42";
            string dosPath;

            var retCode = Utils.GetDosPathFromNtPath(ntPath, out dosPath);
            Assert.That(retCode == WinStatusCode.UnknownPort,
                $"Error in invoking function! Expected status: {WinStatusCode.Success}, actual: {retCode}");
            Assert.That(dosPath.Equals("", StringComparison.CurrentCultureIgnoreCase),
                $"Error in conversion! Expected: \"\", actual: {dosPath}");

        }

        [Test]
        public void Should_Emit_BadPathnameError_On_Invalid_Path()
        {
            const string ntPath = @"\Device\HugaHuga";
            string dosPath;

            var retCode = Utils.GetDosPathFromNtPath(ntPath, out dosPath);
            Assert.That(retCode == WinStatusCode.BadPathname,
                $"Error in invoking function! Expected status: {WinStatusCode.Success}, actual: {retCode}");
            Assert.That(dosPath.Equals("", StringComparison.CurrentCultureIgnoreCase),
                $"Error in conversion! Expected: \"\", actual: {dosPath}");
        }

        [Test, Ignore("Not implemented, requires mocking the Registry")]
        public void Should_Convert_SerialDevices_From_NtPath_To_DosPath_Correctly()
        {
            // TODO: requires mocking the registry; currently not implemented.
        }

        #endregion

        static void Main(string[] args)
        {
            
        }
    }
}
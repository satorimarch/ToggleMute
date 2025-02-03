using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ToggleMute.Services
{
    public interface IMuteService
    {
        public void ToggleMuteActiveWindow();

        public void MuteActiveWindow();

        public void UnmuteActiveWindow();

        public void UnmuteOtherWindows();

        public void MuteOtherWindows();
    }

    /// <summary>
    /// Functions for mute, unmute or toggle the mute status of a application.
    /// </summary>
    /// <remarks>
    /// Two process are regarded as same application by process Id and process name,
    /// to work correctly on multi-process application like web browser.
    /// </remarks>
    public class MuteService : IMuteService
    {
        private static Func<uint, bool> EqualByIdAndName(uint currentId)
        {
            var currentProcess = Process.GetProcessById((int)currentId);
            return (id) =>
                id == currentId || Process.GetProcessById((int)id).ProcessName == currentProcess.ProcessName;
        }

        private static Func<uint, bool> NotEqualByIdAndName(uint currentId)
        {
            var f = EqualByIdAndName(currentId);
            return (id) => f(id) == false;
        }

        public void ToggleMuteActiveWindow()
        {
            Debug.WriteLine("Try to toggle mute active window.");
            uint processId = GetActiveWindowProcessId();

            ToggleMuteApplication(EqualByIdAndName(processId));
        }

        public void MuteActiveWindow()
        {
            Debug.WriteLine("Try to mute active window.");
            uint processId = GetActiveWindowProcessId();
            MuteApplication(EqualByIdAndName(processId));
        }

        public void UnmuteActiveWindow()
        {
            Debug.WriteLine("Try to unmute active window.");
            uint processId = GetActiveWindowProcessId();
            UnmuteApplication(EqualByIdAndName(processId));
        }

        public void UnmuteOtherWindows()
        {
            Debug.WriteLine("Try to unmute other windows.");
            uint processId = GetActiveWindowProcessId();
            UnmuteApplication(NotEqualByIdAndName(processId));
        }

        public void MuteOtherWindows()
        {
            Debug.WriteLine("Try to mute other windows.");
            uint processId = GetActiveWindowProcessId();
            MuteApplication(NotEqualByIdAndName(processId));
        }

        private static void ToggleMuteApplication(Func<uint, bool> shouldToggle)
        {
            ChangeMuteStatus(shouldToggle, (prev) => !prev);
        }

        private static void MuteApplication(Func<uint, bool> shouldMute)
        {
            ChangeMuteStatus(shouldMute, (_) => true);
        }

        private static void UnmuteApplication(Func<uint, bool> shouldMute)
        {
            ChangeMuteStatus(shouldMute, (_) => false);
        }

        private static void ChangeMuteStatus(Func<uint, bool> shouldChange, Func<bool, bool> changeFromOld)
        {
            var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var sessions = device.AudioSessionManager.Sessions;

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                Debug.WriteLine(session.GetProcessID);
                if (shouldChange(session.GetProcessID))
                {
                    session.SimpleAudioVolume.Mute = changeFromOld(session.SimpleAudioVolume.Mute);
                }
            }
        }

        private static uint GetActiveWindowProcessId()
        {
            nint hWnd = GetForegroundWindow();
            GetWindowThreadProcessId(hWnd, out uint processId);
            return processId;
        }

        [DllImport("user32.dll")]
        private static extern nint GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);
    }
}

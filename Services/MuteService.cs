using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ToggleMute.Services
{
    /// <summary>
    /// Functions for mute, unmute or toggle the mute status of a application.
    /// </summary>
    /// <remarks>
    /// Two process are regarded as same application by process Id and process name,
    /// to work correctly on multi-process application like web browser.
    /// </remarks>
    public interface IMuteService
    {
        public void ToggleMuteActiveWindow();

        public void MuteActiveWindow();

        public void UnmuteActiveWindow();

        public void UnmuteOtherWindows();

        public void MuteOtherWindows();

        /// <summary>
        /// Process names of process to be ignored.
        /// </summary>
        public HashSet<string>? IgnoreProcesses { get; set; }
    }

    /// <inheritdoc cref="IMuteService"/>
    public class MuteService : IMuteService
    {
        public HashSet<string>? IgnoreProcesses { get; set; }

        private static Func<Process, bool> EqualByIdAndName(uint currentId)
        {
            var currentProcess = Process.GetProcessById((int)currentId);
            return (process) =>
                process.Id == currentId || process.ProcessName == currentProcess.ProcessName;
        }

        private static Func<Process, bool> NotEqualByIdAndName(uint currentId)
        {
            var f = EqualByIdAndName(currentId);
            return (process) => f(process) == false;
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

        private void ToggleMuteApplication(Func<Process, bool> shouldToggle)
        {
            ChangeMuteStatus(shouldToggle, (prev) => !prev);
        }

        private void MuteApplication(Func<Process, bool> shouldMute)
        {
            ChangeMuteStatus(shouldMute, (_) => true);
        }

        private void UnmuteApplication(Func<Process, bool> shouldMute)
        {
            ChangeMuteStatus(shouldMute, (_) => false);
        }

        private void ChangeMuteStatus(Func<Process, bool> shouldChange, Func<bool, bool> changeFromOld)
        {
            var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            var sessions = device.AudioSessionManager.Sessions;

            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                Process process = Process.GetProcessById((int)session.GetProcessID);
                if ((IgnoreProcesses?.Contains(process.ProcessName) ?? false) == false && shouldChange(process))
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

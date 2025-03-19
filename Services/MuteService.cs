using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using NAudio.CoreAudioApi;
using Serilog.Core;

namespace ToggleMute.Services;

/// <summary>
///     Provides functions to mute, unmute, or toggle the mute status of an application.
/// </summary>
/// <remarks>
///     Two processes are considered the same application if they share the same process ID and process name.
///     This ensures proper functionality for multi-process applications, such as web browsers.
/// </remarks>
public interface IMuteService
{
    public void ToggleMuteActiveWindow();

    public void MuteActiveWindow();

    public void UnmuteActiveWindow();

    public void UnmuteOtherWindows();

    public void MuteOtherWindows();

    /// <summary>
    ///     Names of processes to be ignored.
    /// </summary>
    public HashSet<string>? IgnoreProcesses { get; set; }
}

/// <inheritdoc cref="IMuteService" />
public class MuteService(ILogger<MuteService> logger) : IMuteService
{
    public HashSet<string>? IgnoreProcesses { get; set; }

    private static Func<Process, bool> EqualByIdAndName(uint currentId)
    {
        var currentProcess = Process.GetProcessById((int)currentId);
        return process =>
            process.Id == currentId || process.ProcessName == currentProcess.ProcessName;
    }

    private static Func<Process, bool> NotEqualByIdAndName(uint currentId)
    {
        var f = EqualByIdAndName(currentId);
        return process => f(process) == false;
    }

    public void ToggleMuteActiveWindow()
    {
        var processId = GetActiveWindowProcessId();
        logger.LogInformation("Toggling mute for active window with process ID: {ProcessId}", processId);
        ToggleMuteApplication(EqualByIdAndName(processId));
    }

    public void MuteActiveWindow()
    {
        var processId = GetActiveWindowProcessId();
        logger.LogInformation("Muting active window with process ID: {ProcessId}", processId);
        MuteApplication(EqualByIdAndName(processId));
    }

    public void UnmuteActiveWindow()
    {
        var processId = GetActiveWindowProcessId();
        logger.LogInformation("Unmuting active window with process ID: {ProcessId}", processId);
        UnmuteApplication(EqualByIdAndName(processId));
    }

    public void UnmuteOtherWindows()
    {
        var processId = GetActiveWindowProcessId();
        logger.LogInformation("Unmuting other windows excluding process ID: {ProcessId}", processId);
        UnmuteApplication(NotEqualByIdAndName(processId));
    }

    public void MuteOtherWindows()
    {
        var processId = GetActiveWindowProcessId();
        logger.LogInformation("Muting other windows excluding process ID: {ProcessId}", processId);
        MuteApplication(NotEqualByIdAndName(processId));
    }

    private void ToggleMuteApplication(Func<Process, bool> shouldToggle)
    {
        ChangeMuteStatus(shouldToggle, prev => !prev);
    }

    private void MuteApplication(Func<Process, bool> shouldMute)
    {
        ChangeMuteStatus(shouldMute, _ => true);
    }

    private void UnmuteApplication(Func<Process, bool> shouldMute)
    {
        ChangeMuteStatus(shouldMute, _ => false);
    }

    private void ChangeMuteStatus(Func<Process, bool> shouldChange, Func<bool, bool> changeFromOld)
    {
        var device = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        var sessions = device.AudioSessionManager.Sessions;

        for (var i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            var process = Process.GetProcessById((int)session.GetProcessID);
            if ((IgnoreProcesses?.Contains(process.ProcessName) ?? false) == false && shouldChange(process))
                session.SimpleAudioVolume.Mute = changeFromOld(session.SimpleAudioVolume.Mute);
        }
    }

    private static uint GetActiveWindowProcessId()
    {
        var hWnd = GetForegroundWindow();
        GetWindowThreadProcessId(hWnd, out var processId);
        return processId;
    }

    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);
}
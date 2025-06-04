using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Utility
{
    public class ProcessManager
    {

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        /// <summary>
        /// Terminates processes with the specified name.
        /// </summary>
        /// <param name="processName">The name of the process to terminate (without .exe extension, e.g., "notepad").</param>
        /// <param name="isCheckOnly">Is check only or not, find out but not kill the process.</param>
        /// <param name="isForceKill">Is force kill process or not.</param>
        /// <returns>Returns true if all matching processes are successfully terminated, false if no processes are found.</returns>
        public static bool KillProcessByName(string processName, bool isCheckOnly = false, bool isForceKill = true)
        {
            // Retrieve all processes matching the specified name
            Process[] processes = Process.GetProcessesByName(processName);

            // If no processes are found
            if (processes.Length == 0)
            {
                return false;
            }

            if (isCheckOnly) { return true; }

            // Iterate through and terminate each process
            foreach (Process process in processes)
            {
                if (!process.HasExited) // Ensure the process has not already exited
                {
                    if (isForceKill)
                    {
                        process.Kill(); // Forcefully terminate the process
                    }
                    else
                    {
                        process.CloseMainWindow(); // Attempt to close the process gracefully
                    }
                    process.WaitForExit(5000); // Wait for the process to exit, up to 5 seconds
                }
            }
            return true;
        }

        /// <summary>
        /// Check whether the specified process is a foreground process
        /// </summary>
        /// <param name="processName">The name of the process to terminate (without .exe extension, e.g., "notepad").</param>
        /// <returns>If the process is a foreground process, returns true; otherwise returns false</returns>
        public static bool IsProcessForeground(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                return false;
            }

            var process = processes[0]; // Get the first instance of the process

            if (process == null || process.HasExited)
            {
                return false;
            }

            IntPtr foregroundWindow = GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
            {
                return false;
            }

            Process currentProcess = Process.GetCurrentProcess();
            IntPtr processHandle = currentProcess.Handle;
            GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
            if (foregroundProcessId == (uint)currentProcess.Id) { return true; } //Check if the foreground process ID matches the current process ID, then true only for this program need.

            return foregroundProcessId == (uint)process.Id;
        }

    }

}

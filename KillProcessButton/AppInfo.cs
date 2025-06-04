using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace KillProcessButton
{
    public partial class AppInfo
    {
        public static string AppName
        {
            get { return "KillProcessButton"; }
        }
        public static string AppVersionString
        {
            get
            {
                return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion; //product version
                //return public static string AppCoreVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(); //assembly version 
                //return public static string AppCoreVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion; //file version  
            }
        }
        public static Version AppVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public static string AppPath { get { try { return Application.StartupPath + "\\"; } catch { return string.Empty; } } }
        public static int ProcessId { get { return Process.GetCurrentProcess().Id; } }
        public static int ThreadId { get { return Thread.CurrentThread.ManagedThreadId; } }
    }
}

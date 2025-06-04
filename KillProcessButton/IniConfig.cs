using System.IO;
using Utility;

namespace KillProcessButton
{
    internal class IniConfig
    {
        internal static string IniFile = AppInfo.AppName + ".config.ini";
        internal static IniPlus Ini = new IniPlus(Path.Combine(AppInfo.AppPath, IniFile));

        internal static IniPlusValue General_ProcessName;
        internal static IniPlusValue General_ForceKillProcess;
        internal static IniPlusValue General_ButtonWidth;
        internal static IniPlusValue General_ButtonHeight;
        internal static IniPlusValue General_ButtonBackColor;
        internal static IniPlusValue General_ButtonFrontColor;
        internal static IniPlusValue General_PopupConfirm;
        internal static IniPlusValue General_PopupConfirmTitle;
        internal static IniPlusValue General_PopupConfirmMessage;

        internal static void Init()
        {
            General_ProcessName = Ini.NewOrUpdateValue("General", "ProcessName", "notepad");
            General_ForceKillProcess = Ini.NewOrUpdateValue("General", "ForceKillProcess", "True");
            General_ButtonWidth = Ini.NewOrUpdateValue("General", "ButtonWidth", "32");
            General_ButtonHeight = Ini.NewOrUpdateValue("General", "ButtonHeight", "32");
            General_ButtonBackColor = Ini.NewOrUpdateValue("General", "ButtonBackColor", "#FF0000");
            General_ButtonFrontColor = Ini.NewOrUpdateValue("General", "ButtonFrontColor", "#FFFFFF");
            General_PopupConfirm = Ini.NewOrUpdateValue("General", "PopupConfirm", "True");
            General_PopupConfirmTitle = Ini.NewOrUpdateValue("General", "PopupConfirmTitle", "Confirmation");
            General_PopupConfirmMessage = Ini.NewOrUpdateValue("General", "PopupConfirmMessage", "Are you sure you want to close the application?");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace KillProcessButton
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;

            using (var mutex = new System.Threading.Mutex(true, AppInfo.AppName, out createdNew))
            {
                if (createdNew)
                {
                    IniConfig.Init();
                    try
                    {
                        IniConfig.Ini.ReadFromFile();
                        IniConfig.Ini.WriteToFile();
                    }
                    catch (Exception ex)
                    {
                        string msg = "MainForm.Exception: Error read or write config ini file." + Environment.NewLine;
                        MessageBox.Show(msg + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MiniButton());
                }
            }

        }
    }
}

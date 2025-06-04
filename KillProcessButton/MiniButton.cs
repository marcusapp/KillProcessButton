using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Utility;

namespace KillProcessButton
{
    public partial class MiniButton : Form
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int WS_EX_APPWINDOW = 0x00040000;

        private Timer _checkProcessTimer;
        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayMenu;

        private string _config_ProcessName;
        private bool _config_ForceKillProcess;
        private int _config_ButtonWidth;
        private int _config_ButtonHeight;
        private string _config_ButtonBackColor;
        private string _config_ButtonFrontColor;
        private bool _config_PopupConfirm;
        private string _config_PopupConfirmTitle;
        private string _config_PopupConfirmMessage;
        public MiniButton()
        {
            InitializeComponent();

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Name = "KillProcessButton";
            this.Text = "KillProcessButton";

            this.MouseClick += MiniButton_MouseClick;
            this.Load += MiniButton_Load;
            this.Paint += MiniButton_Paint;
            this.Resize += MiniButton_Resize;

            IntPtr hWnd = this.Handle;
            int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
            exStyle |= WS_EX_TOOLWINDOW;
            exStyle &= ~WS_EX_APPWINDOW;
            SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);

            try
            {
                this._config_ProcessName = IniConfig.General_ProcessName.Value;
                this._config_ForceKillProcess = IniConfig.General_ForceKillProcess.Value.IniValueToBoolean();
                this._config_ButtonWidth = int.Parse(IniConfig.General_ButtonWidth.Value);
                this._config_ButtonHeight = int.Parse(IniConfig.General_ButtonHeight.Value);
                this._config_ButtonBackColor = IniConfig.General_ButtonBackColor.Value;
                this._config_ButtonFrontColor = IniConfig.General_ButtonFrontColor.Value;
                this._config_PopupConfirm = IniConfig.General_PopupConfirm.Value.IniValueToBoolean();
                this._config_PopupConfirmTitle = IniConfig.General_PopupConfirmTitle.Value;
                this._config_PopupConfirmMessage = IniConfig.General_PopupConfirmMessage.Value;
            }
            catch (Exception ex)
            {
                string msg = "Error reading config.ini values." + Environment.NewLine;
                if (MessageBox.Show(msg + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop) == DialogResult.OK)
                {
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }
            }

            this._trayMenu = new ContextMenuStrip();
            this._trayMenu.Items.Add("Exit", null, OnExit);

            this._trayIcon = new NotifyIcon
            {
                Text = "Kill process button, monitoring on " + this._config_ProcessName + ".exe",
                Icon = Properties.Resources.Close,
                ContextMenuStrip = this._trayMenu,
                Visible = true
            };

            this.BackColor = ColorTranslator.FromHtml(this._config_ButtonBackColor);
            Size buttonSize = new System.Drawing.Size(this._config_ButtonWidth, this._config_ButtonHeight);
            this.ClientSize = buttonSize;
            this.MinimumSize = buttonSize;

            this.ResumeLayout(false);
            this.PerformLayout();

            this._checkProcessTimer = new Timer();
            this._checkProcessTimer.Interval = 500;
            this._checkProcessTimer.Tick += MiniButton_TimerTick;
            this._checkProcessTimer.Start();
        }

        private void MiniButton_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (this._config_PopupConfirm)
                    {
                        var title = string.IsNullOrEmpty(this._config_PopupConfirmTitle) ? "Confirmation" : this._config_PopupConfirmTitle;
                        var message = string.IsNullOrEmpty(this._config_PopupConfirmMessage) ? "Are you sure you want to close the application?" : this._config_PopupConfirmMessage;
                        if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (ProcessManager.KillProcessByName(this._config_ProcessName, isCheckOnly: false, isForceKill: this._config_ForceKillProcess))
                            {
                                this.Hide();
                            }
                        }
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    //Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MiniButton_Load(object sender, EventArgs e)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int x = workingArea.Width - this.Width;
            int y = workingArea.Top;
            this.Location = new Point(x, y);
        }

        private void MiniButton_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float xSize = this.ClientSize.Width * 0.8f;
            float ySize = this.ClientSize.Height * 0.8f;
            float size = Math.Min(xSize, ySize);

            float centerX = this.ClientSize.Width / 2f;
            float centerY = this.ClientSize.Height / 2f;

            PointF point1 = new PointF(centerX - size / 2, centerY - size / 2); 
            PointF point2 = new PointF(centerX + size / 2, centerY + size / 2); 
            PointF point3 = new PointF(centerX + size / 2, centerY - size / 2); 
            PointF point4 = new PointF(centerX - size / 2, centerY + size / 2); 

            using (Pen pen = new Pen(ColorTranslator.FromHtml(this._config_ButtonFrontColor), 3))
            {
                g.DrawLine(pen, point1, point2); 
                g.DrawLine(pen, point3, point4); 
            }
        }

        private void MiniButton_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void MiniButton_TimerTick(object sender, EventArgs e)
        {
            if (ProcessManager.KillProcessByName(this._config_ProcessName, isCheckOnly: true) && ProcessManager.IsProcessForeground(this._config_ProcessName))
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }
        private void OnExit(object sender, EventArgs e)
        {
            this._trayIcon.Visible = false;
            Application.Exit();
        }
    }
}

using System.IO;
using IWshRuntimeLibrary;
using HidLibrary;

namespace DS4Indicator
{
    internal static class Program
    {
        private static NotifyIcon _trayIcon;
        private static Icon _connectedIcon;
        private static Icon _disconnectedIcon;
        private const string AppName = "DS4Indicator";

        static bool IsStartupEnabled()
        {
            //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            //return registryKey?.GetValue(AppName) != null;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startUpFolderPath, AppName + ".lnk");
            return System.IO.File.Exists(shortcutPath);
        }

        static void SetStartup(bool enable)
        {
            //RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            //if (enable)
            //{
            //    registryKey.SetValue(AppName, Application.ExecutablePath);
            //}
            //else
            //{
            //    registryKey.DeleteValue(AppName, false);
            //}
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startUpFolderPath, AppName + ".lnk");

            if (enable)
            {
                CreateShortcut(shortcutPath);
            }
            else
            {
                if (System.IO.File.Exists(shortcutPath))
                {
                    System.IO.File.Delete(shortcutPath);
                }
            }
        }
        static void CreateShortcut(string shortcutPath)
        {
            WshShell wshShell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)wshShell.CreateShortcut(shortcutPath);

            shortcut.TargetPath = Application.ExecutablePath;
            shortcut.WorkingDirectory = Application.StartupPath;
            shortcut.Description = "Launch DS4 Indicator at startup";
            shortcut.Save();
        }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            DeviceMonitor deviceMonitor = new DeviceMonitor();
            deviceMonitor.DeviceChanged += OnDeviceChanged;

            _connectedIcon = new Icon("Resources\\ps4.ico");
            _disconnectedIcon = new Icon("Resources\\nops4.ico");

            _trayIcon = new NotifyIcon
            {
                Icon = _disconnectedIcon,
                Text = "DS4 Indicator",
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };

            ToolStripMenuItem statusMenuItem = new ToolStripMenuItem("Checking status...");
            _trayIcon.ContextMenuStrip.Items.Add(statusMenuItem);

            ToolStripMenuItem runtimeMenuItem = new ToolStripMenuItem("Runtime: 00:00:00");
            _trayIcon.ContextMenuStrip.Items.Add(runtimeMenuItem);

            ToolStripMenuItem startupMenuItem = new ToolStripMenuItem("Run at Startup")
            {
                Checked = IsStartupEnabled()
            };
            startupMenuItem.Click += (s, e) =>
            {
                bool isChecked = startupMenuItem.Checked;
                SetStartup(!isChecked);
                startupMenuItem.Checked = !isChecked;
            };
            _trayIcon.ContextMenuStrip.Items.Add(startupMenuItem);

            _trayIcon.ContextMenuStrip.Items.Add("Open", null, (s, e) => MessageBox.Show("DS4 Indicator created by Aneesh Sharma"));
            _trayIcon.ContextMenuStrip.Items.Add("Exit", null, (s, e) =>
            {
                deviceMonitor.Dispose();
                Application.Exit();
            });

            _statusMenuItem = statusMenuItem;
            _runtimeMenuItem = runtimeMenuItem;

            _runtimeTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _runtimeTimer.Tick += UpdateRuntime;

            OnDeviceChanged();
            Application.Run();
        }

        private static ToolStripMenuItem _statusMenuItem;
        private static ToolStripMenuItem _runtimeMenuItem;
        private static System.Windows.Forms.Timer _runtimeTimer;
        private static DateTime _connectionStartTime;
        private static DateTime lastDeviceChangeTime = DateTime.MinValue;
        private static bool _isControllerConnected = false;

        private static void OnDeviceChanged()
        {
            if ((DateTime.Now - lastDeviceChangeTime).TotalMilliseconds < 500)
            {
                return;
            }
            lastDeviceChangeTime = DateTime.Now;

            var controller = HidDevices.Enumerate(0x054C, 0x05C4).FirstOrDefault() ??
                             HidDevices.Enumerate(0x054C, 0x09CC).FirstOrDefault();

            if (controller != null)
            {
                UpdateStatusMenuItem("Controller Connected");

                if (!_isControllerConnected)
                {
                    _connectionStartTime = DateTime.Now;
                    _runtimeTimer.Start();
                    _isControllerConnected = true;
                    _trayIcon.Icon = _connectedIcon;

                    _trayIcon.BalloonTipTitle = "DS4 Indicator";
                    _trayIcon.BalloonTipText = "Controller Connected";
                    _trayIcon.ShowBalloonTip(1000);
                }
            }
            else
            {
                UpdateStatusMenuItem("No Controller Connected");

                if (_isControllerConnected)
                {
                    _runtimeTimer.Stop();
                    _isControllerConnected = false;
                    _trayIcon.Icon = _disconnectedIcon;

                    _trayIcon.BalloonTipTitle = "DS4 Indicator";
                    _trayIcon.BalloonTipText = "Controller Disconnected";
                    _trayIcon.ShowBalloonTip(1000);
                }
            }
        }

        private static void UpdateStatusMenuItem(string status)
        {
            if (_statusMenuItem != null)
            {
                _statusMenuItem.Text = status;
            }
        }

        private static void UpdateRuntime(object sender, EventArgs e)
        {
            if (_isControllerConnected)
            {
                TimeSpan runtime = DateTime.Now - _connectionStartTime;
                _runtimeMenuItem.Text = $"Runtime: {runtime.Hours:D2}:{runtime.Minutes:D2}:{runtime.Seconds:D2}";
            }
        }
    }

    public class DeviceMonitor : NativeWindow, IDisposable
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVNODES_CHANGED = 0x0007;

        public event Action DeviceChanged;

        public DeviceMonitor()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE && m.WParam.ToInt32() == DBT_DEVNODES_CHANGED)
            {
                DeviceChanged?.Invoke();
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            DestroyHandle();
        }
    }
}

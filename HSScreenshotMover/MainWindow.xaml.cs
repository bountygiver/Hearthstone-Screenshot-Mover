using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HSScreenshotMover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public FileSystemWatcher fileWatcher;
        public string pathAsString, path;

        public string DESKTOP = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        System.Windows.Forms.NotifyIcon ni;

        bool _isMonitoring = false;
        public bool isMonitoring
        {
            get
            {
                return _isMonitoring;
            }
            set
            {
                _isMonitoring = value;
                if (value)
                {
                    btnStartStop.Content = "Stop";
                }
                else
                {
                    btnStartStop.Content = "Start";
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            fileWatcher = new FileSystemWatcher();

            var Settings = Properties.Settings.Default;

            pathAsString = Settings.Path;

            lblPath.Content = pathAsString;

            chkAutoStart.IsChecked = Settings.StartAuto;

            fileWatcher.Path = DESKTOP;
            fileWatcher.NotifyFilter = NotifyFilters.FileName;

            fileWatcher.Filter = "Hearthstone_Screenshot*.png";

            fileWatcher.Created += ScreenShotMade;

            fileWatcher.EnableRaisingEvents = true;


            ni = new System.Windows.Forms.NotifyIcon();

            try
            {
                Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/icon.ico")).Stream;
                ni.Icon = new System.Drawing.Icon(iconStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            ni.Text = "Hearthstone screenshot auto mover";
            ni.Visible = true;
            ni.DoubleClick +=
                delegate(object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };


            try
            {
                path = System.IO.Path.GetFullPath(pathAsString);
                isMonitoring = true;
            }
            catch
            {

            }
        }

        private void ScreenShotMade(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Screenshot detected!");
            if (isMonitoring)
            {
                Console.WriteLine("Moving file... " + e.FullPath);
                try
                {
                    System.IO.File.Move(e.FullPath, System.IO.Path.Combine(path, e.Name));
                }
                catch
                {
                    MessageBox.Show("Failed to move file! " + e.Name);
                }
            }
        }

        private void chkAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            try {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                if (!rkApp.GetValueNames().Contains(curAssembly.GetName().Name)) {
                    rkApp.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                var Settings = Properties.Settings.Default;
                Settings.StartAuto = true;
            } catch {

            }
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                path = System.IO.Path.GetFullPath(pathAsString);
                if (!System.IO.Directory.Exists(path))
                {
                    isMonitoring = false;
                    path = pathAsString = "";
                    MessageBox.Show("Directory does not exist!");
                    return;
                }
                isMonitoring = !isMonitoring;
            }
            catch
            {
                isMonitoring = false;
                lblPath.Content = path = pathAsString = "";
                MessageBox.Show("Invalid path specified!");
            }
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            isMonitoring = false;
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            lblPath.Content = pathAsString = dialog.SelectedPath;

            try
            {
                if (System.IO.Path.GetFullPath(pathAsString) == System.IO.Path.GetFullPath(DESKTOP))
                {
                    MessageBox.Show("You must not choose your desktop as destination! Why else you are using this program in the first place?");
                    lblPath.Content = path = pathAsString = "";
                }
            }
            catch
            {
                lblPath.Content = path = pathAsString = "";
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {

            if (this.WindowState == WindowState.Minimized)
            {
                ni.ShowBalloonTip(500, "Hearthstone Screenshot mover", "The app is now running in the background... Double click to restore", System.Windows.Forms.ToolTipIcon.Info);
                this.Hide();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var Settings = Properties.Settings.Default;
            Settings.Path = pathAsString;
            Settings.Save();

            MessageBoxResult k = MessageBox.Show(this, "You can hide this window by minimizing it, it will continue running in the background and can be restored by double clicking the tray icon.", "Are you sure you want to quit?", MessageBoxButton.YesNo);
            if (k == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                ni.Visible = false;
            }
        }

        private void chkAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                rkApp.DeleteValue(curAssembly.GetName().Name, false);
                var Settings = Properties.Settings.Default;
                Settings.StartAuto = false;
            }
            catch
            {

            }
        }

    }
}

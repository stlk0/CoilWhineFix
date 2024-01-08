using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace CoilWhineFix;

public partial class App : Application
{
    private NotifyIcon _trayIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Set the current directory to the application's base directory
        System.IO.Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        
        InitializeTrayIcon();
    }

    private void InitializeTrayIcon()
    {
        _trayIcon = new NotifyIcon
        {
            Icon = new Icon("Resources\\icon_256x256.ico"),
            ContextMenuStrip = new ContextMenuStrip(),
            Visible = true
        };

        _trayIcon.ContextMenuStrip.Items.Add("Show", null, OnTrayIconOpenClicked);
        _trayIcon.ContextMenuStrip.Items.Add("Exit", null, OnTrayIconExitClicked);
        _trayIcon.DoubleClick += OnTrayIconOpenClicked;
    }

    private void OnTrayIconOpenClicked(object? sender, EventArgs e)
    {
        if (MainWindow == null) return;
        MainWindow.Show();
        MainWindow.WindowState = WindowState.Normal;
        MainWindow.Activate();
    }

    private void OnTrayIconExitClicked(object? sender, EventArgs e)
    {
        _trayIcon.Visible = false;
        Current.Shutdown();
    }

    public void MinimizeToSystemTray()
    {
        MainWindow?.Hide();
        _trayIcon.Visible = true;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon.Dispose();
        base.OnExit(e);
    }
}
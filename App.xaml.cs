using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace GlassFocusDesktop;

public partial class App : Application
{
    private static readonly string LogPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");

    public static void Log(string msg)
    {
        try { File.AppendAllText(LogPath, $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\n"); }
        catch { }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MessageBox.Show("App 启动成功！感谢您支持XR-win-tools！");
        Log("App.OnStartup begin");

        DispatcherUnhandledException += (_, args) =>
        {
            Log("UI Exception: " + args.Exception);
            try { MessageBox.Show(args.Exception.ToString(), "运行异常"); } catch { }
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            Log("Domain Exception: " + args.ExceptionObject);
        };

        base.OnStartup(e);
        Log("App.OnStartup end");
    }
}
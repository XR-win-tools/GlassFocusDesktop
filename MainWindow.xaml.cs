using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using GlassFocusDesktop.Models;
using GlassFocusDesktop.ViewModels;

namespace GlassFocusDesktop;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        App.Log("MainWindow ctor begin");
        InitializeComponent();
        App.Log("InitializeComponent done");

        ViewModel = new MainViewModel();
        DataContext = ViewModel;
        App.Log("DataContext set");

        ViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.PomodoroProgress))
                UpdateArc();
        };
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        App.Log("Window_Loaded begin");

        // Win11 系统圆角（安全 API，不会崩）
        try
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int pref = NativeMethods.DWMWCP_ROUND;
            NativeMethods.DwmSetWindowAttribute(
                hwnd, NativeMethods.DWMWA_WINDOW_CORNER_PREFERENCE, ref pref, sizeof(int));
            App.Log("Corner radius applied");
        }
        catch (Exception ex)
        {
            App.Log("Corner radius failed: " + ex.Message);
        }

        try { UpdateArc(); App.Log("UpdateArc ok"); }
        catch (Exception ex) { App.Log("UpdateArc failed: " + ex); }

        App.Log("Window_Loaded end");
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is DependencyObject d && FindParent<Button>(d) != null)
            return;
        DragMove();
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        var pos = e.GetPosition(MainCard);
        HighlightTransform.X = pos.X - LiquidHighlight.Width / 2;
        HighlightTransform.Y = pos.Y - LiquidHighlight.Height / 2;
    }

    private void PinButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsPinned = !ViewModel.IsPinned;
        Topmost = ViewModel.IsPinned;
        PinButton.Content = ViewModel.IsPinned ? "📍" : "📌";
    }

    private void CollapseButton_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.IsCollapsed = !ViewModel.IsCollapsed;
        if (ViewModel.IsCollapsed)
        {
            RootGrid.RowDefinitions[1].Height = new GridLength(0);
            RootGrid.RowDefinitions[2].Height = new GridLength(0);
            RootGrid.RowDefinitions[3].Height = new GridLength(0);
            RootGrid.RowDefinitions[4].Height = new GridLength(0);
            AnimateHeight(100);
        }
        else
        {
            RootGrid.RowDefinitions[1].Height = GridLength.Auto;
            RootGrid.RowDefinitions[2].Height = GridLength.Auto;
            RootGrid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Star);
            RootGrid.RowDefinitions[4].Height = GridLength.Auto;
            AnimateHeight(560);
        }
    }

    private void AnimateHeight(double target)
    {
        var anim = new DoubleAnimation(target, TimeSpan.FromMilliseconds(250))
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        BeginAnimation(HeightProperty, anim);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

    private void PomodoroStartPause_Click(object sender, RoutedEventArgs e)
        => ViewModel.StartPausePomodoro();

    private void PomodoroReset_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ResetPomodoro();
        UpdateArc();
    }

    private void AddTodo_Click(object sender, RoutedEventArgs e) => ViewModel.AddTodo();

    private void NewTodoTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ViewModel.AddTodo();
            e.Handled = true;
        }
    }

    private void RemoveTodo_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is TodoItem item)
            ViewModel.RemoveTodo(item);
    }

    private void UpdateArc()
    {
        var progress = Math.Min(ViewModel.PomodoroProgress, 0.999);
        const double radius = 46;
        var center = new Point(50, 50);
        var angle = progress * 360;
        const double startAngle = -90;
        var endAngle = startAngle + angle;

        var startPoint = new Point(
            center.X + radius * Math.Cos(startAngle * Math.PI / 180),
            center.Y + radius * Math.Sin(startAngle * Math.PI / 180));
        var endPoint = new Point(
            center.X + radius * Math.Cos(endAngle * Math.PI / 180),
            center.Y + radius * Math.Sin(endAngle * Math.PI / 180));

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(startPoint, false, false);
            ctx.ArcTo(endPoint, new Size(radius, radius), 0, angle > 180,
                SweepDirection.Clockwise, true, false);
        }
        geometry.Freeze();
        ArcPath.Data = geometry;
    }

    private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(child);
        while (parent != null)
        {
            if (parent is T t) return t;
            parent = VisualTreeHelper.GetParent(parent);
        }
        return null;
    }
}
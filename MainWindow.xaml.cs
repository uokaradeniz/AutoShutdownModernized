using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoShutdownModernized.ViewModels;
using Application = System.Windows.Application;

namespace AutoShutdownModernized;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private readonly NotifyIcon _trayIcon;
    private bool _isExitRequested;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        _trayIcon = CreateTrayIcon();
        StateChanged += MainWindow_StateChanged;
        Closing += MainWindow_Closing;
    }

    private static string GetLocalizedString(string key, string fallback)
        => Application.Current.TryFindResource(key)?.ToString() ?? fallback;

    private NotifyIcon CreateTrayIcon()
    {
        var showItem = new ToolStripMenuItem(GetLocalizedString("TrayShowText", "Show"), null, (_, _) => ShowWindowFromTray());
        var exitItem = new ToolStripMenuItem(GetLocalizedString("TrayExitText", "Exit"), null, (_, _) => ExitApplication());
        var menu = new ContextMenuStrip();
        menu.Items.Add(showItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        return new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = GetLocalizedString("AppTitle", "Auto Shutdown Modernized"),
            Visible = true,
            ContextMenuStrip = menu
        };
    }

    private void MainWindow_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            HideToTray();
        }
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_isExitRequested)
            return;

        if (DataContext is MainViewModel vm && vm.IsScheduled)
        {
            e.Cancel = true;
            HideToTray();
        }
    }

    private void HideToTray()
    {
        ShowInTaskbar = false;
        Hide();
        _trayIcon.ShowBalloonTip(
            1000,
            GetLocalizedString("TrayBalloonTitle", "Auto Shutdown Modernized"),
            GetLocalizedString("TrayBalloonMessage", "The app was minimized to the tray."),
            ToolTipIcon.Info);
    }

    private void ShowWindowFromTray()
    {
        Dispatcher.Invoke(() =>
        {
            ShowInTaskbar = true;
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            Show();
            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        });
    }

    private void ExitApplication()
    {
        _isExitRequested = true;
        Application.Current.Shutdown();
    }

    protected override void OnClosed(EventArgs e)
    {
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        base.OnClosed(e);
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _trayIcon.DoubleClick += (_, _) => ShowWindowFromTray();
    }

    private void TimeSelectionGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        var border = FindTimePartBorder(e.OriginalSource as DependencyObject);
        if (border is not null)
        {
            string part = border.Tag?.ToString() ?? "";
            string direction = e.Delta > 0 ? "+" : "-";
            if (!string.IsNullOrEmpty(part) && vm.AdjustTimeCommand.CanExecute(part + direction))
            {
                vm.AdjustTimeCommand.Execute(part + direction);
                e.Handled = true;
            }
        }
    }

    private static Border? FindTimePartBorder(DependencyObject? current)
    {
        while (current is not null)
        {
            if (current is Border border && border.Tag is string tag && !string.IsNullOrWhiteSpace(tag))
                return border;

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}
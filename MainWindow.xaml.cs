using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using AutoShutdownModernized.ViewModels;

namespace AutoShutdownModernized;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
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
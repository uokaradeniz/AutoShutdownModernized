using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

    private void TimePart_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (sender is Border border && DataContext is MainViewModel vm)
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
}
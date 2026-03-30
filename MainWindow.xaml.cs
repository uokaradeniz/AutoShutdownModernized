using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AutoShutdownModernized;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private DispatcherTimer _timer;
    private TimeSpan _timeRemaining;

    public MainWindow()
    {
        InitializeComponent();
        
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
    }

    private void TimeInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(TimeInput.Text, out int minutes) && minutes > 0)
        {
            _timeRemaining = TimeSpan.FromMinutes(minutes);
            
            // Start shutdown process
            int seconds = minutes * 60;
            Process.Start(new ProcessStartInfo("shutdown", $"-s -t {seconds}") { CreateNoWindow = true, UseShellExecute = false });

            UpdateCountdownDisplay();
            _timer.Start();
            
            StartButton.IsEnabled = false;
            TimeInput.IsEnabled = false;
            CancelButton.IsEnabled = true;
        }
        else
        {
            MessageBox.Show("Lütfen geçerli bir dakika giriniz.", "Geçersiz Girdi", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        // Cancel shutdown process
        Process.Start(new ProcessStartInfo("shutdown", "-a") { CreateNoWindow = true, UseShellExecute = false });

        _timer.Stop();
        CountdownDisplay.Text = "00:00:00";
        
        StartButton.IsEnabled = true;
        TimeInput.IsEnabled = true;
        TimeInput.Text = string.Empty;
        CancelButton.IsEnabled = false;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (_timeRemaining.TotalSeconds > 1)
        {
            _timeRemaining = _timeRemaining.Subtract(TimeSpan.FromSeconds(1));
            UpdateCountdownDisplay();
        }
        else
        {
            _timer.Stop();
            CountdownDisplay.Text = "00:00:00";
            StartButton.IsEnabled = true;
            TimeInput.IsEnabled = true;
            CancelButton.IsEnabled = false;
        }
    }

    private void UpdateCountdownDisplay()
    {
        CountdownDisplay.Text = _timeRemaining.ToString(@"hh\:mm\:ss");
    }
}
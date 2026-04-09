using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AutoShutdownModernized.Models;
using AutoShutdownModernized.Services;

namespace AutoShutdownModernized.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IShutdownService _shutdownService;
    private readonly IShutdownTrackerService _trackerService;
    private readonly Dispatcher _dispatcher;

    [ObservableProperty]
    private int _selectedHours;

    [ObservableProperty]
    private int _selectedMinutes;

    [ObservableProperty]
    private int _selectedSeconds;

    [ObservableProperty]
    private bool _isScheduled;

    [ObservableProperty]
    private TimeSpan _remainingTime;

    [ObservableProperty]
    private string _remainingTimeText = "00:00:00";

    public MainViewModel(IShutdownService shutdownService, IShutdownTrackerService trackerService)
    {
        _shutdownService = shutdownService;
        _trackerService = trackerService;
        
        // Using main dispatcher for UI updates
        _dispatcher = Dispatcher.CurrentDispatcher;
        
        SelectedHours = 1; // default to 1 hour
        SelectedMinutes = 0;
        SelectedSeconds = 0;
        
        _trackerService.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(object? sender, ShutdownState state)
    {
        _dispatcher.Invoke(() =>
        {
            IsScheduled = state.IsScheduled;
            RemainingTime = state.RemainingTime;
            RemainingTimeText = FormatRemainingTime(state.RemainingTime);
        });
    }

    private static string FormatRemainingTime(TimeSpan remainingTime)
    {
        if (remainingTime < TimeSpan.Zero)
            remainingTime = TimeSpan.Zero;

        return $"{(int)remainingTime.TotalHours:D2}:{remainingTime.Minutes:D2}:{remainingTime.Seconds:D2}";
    }

    [RelayCommand]
    private void StartShutdown()
    {
        var totalTime = new TimeSpan(SelectedHours, SelectedMinutes, SelectedSeconds);
        if (totalTime.TotalSeconds > 0)
        {
            _shutdownService.ScheduleShutdown(totalTime);
        }
    }

    [RelayCommand]
    private void CancelShutdown()
    {
        _shutdownService.CancelShutdown();
    }

    [RelayCommand]
    private void AdjustTime(string parameter)
    {
        switch (parameter)
        {
            case "H+": SelectedHours = (SelectedHours + 1) % 100; break;
            case "H-": SelectedHours = (SelectedHours - 1 + 100) % 100; break;
            case "M+": SelectedMinutes = (SelectedMinutes + 1) % 60; break;
            case "M-": SelectedMinutes = (SelectedMinutes - 1 + 60) % 60; break;
            case "S+": SelectedSeconds = (SelectedSeconds + 1) % 60; break;
            case "S-": SelectedSeconds = (SelectedSeconds - 1 + 60) % 60; break;
        }
    }
}

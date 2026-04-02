using System;
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
    private int _selectedTime;

    [ObservableProperty]
    private bool _isScheduled;

    [ObservableProperty]
    private TimeSpan _remainingTime;

    public MainViewModel(IShutdownService shutdownService, IShutdownTrackerService trackerService)
    {
        _shutdownService = shutdownService;
        _trackerService = trackerService;
        
        // Using main dispatcher for UI updates
        _dispatcher = Dispatcher.CurrentDispatcher;
        
        SelectedTime = 60; // default to 60 minutes
        
        _trackerService.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(object? sender, ShutdownState state)
    {
        _dispatcher.Invoke(() =>
        {
            IsScheduled = state.IsScheduled;
            RemainingTime = state.RemainingTime;
        });
    }

    [RelayCommand]
    private void StartShutdown()
    {
        if (SelectedTime > 0)
        {
            _shutdownService.ScheduleShutdown(TimeSpan.FromMinutes(SelectedTime));
        }
    }

    [RelayCommand]
    private void CancelShutdown()
    {
        _shutdownService.CancelShutdown();
    }
}


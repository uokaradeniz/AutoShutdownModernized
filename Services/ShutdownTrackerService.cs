using System;
using System.Threading;
using System.Threading.Tasks;
using AutoShutdownModernized.Models;
using Microsoft.Extensions.Hosting;

namespace AutoShutdownModernized.Services;

public class ShutdownTrackerService : BackgroundService, IShutdownTrackerService
{
    private DateTime? _shutdownTime;
    private bool _isScheduled;
    private readonly Lock _lock = new();

    public event EventHandler<ShutdownState>? StateChanged;

    public TimeSpan GetRemainingTime()
    {
        lock (_lock)
        {
            if (!_isScheduled || !_shutdownTime.HasValue)
                return TimeSpan.Zero;

            var remaining = _shutdownTime.Value - DateTime.Now;
            return remaining < TimeSpan.Zero ? TimeSpan.Zero : remaining;
        }
    }

    public bool IsShutdownScheduled()
    {
        lock (_lock)
        {
            return _isScheduled;
        }
    }

    public void NotifyShutdownScheduled(TimeSpan delay)
    {
        lock (_lock)
        {
            _shutdownTime = DateTime.Now.Add(delay);
            _isScheduled = true;
        }
        
        RaiseStateChanged();
    }

    public void NotifyShutdownCancelled()
    {
        lock (_lock)
        {
            _shutdownTime = null;
            _isScheduled = false;
        }
        
        RaiseStateChanged();
    }

    private void RaiseStateChanged()
    {
        StateChanged?.Invoke(this, new ShutdownState(IsShutdownScheduled(), GetRemainingTime()));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            bool shouldUpdate = false;
            
            lock (_lock)
            {
                if (_isScheduled)
                {
                    shouldUpdate = true;
                    
                    if (DateTime.Now >= _shutdownTime)
                    {
                        _isScheduled = false;
                        _shutdownTime = null;
                    }
                }
            }

            if (shouldUpdate)
            {
                RaiseStateChanged();
            }
        }
    }
}


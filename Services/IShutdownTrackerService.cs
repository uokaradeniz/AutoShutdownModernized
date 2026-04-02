using System;
using AutoShutdownModernized.Models;

namespace AutoShutdownModernized.Services;

public interface IShutdownTrackerService
{
    event EventHandler<ShutdownState>? StateChanged;

    TimeSpan GetRemainingTime();
    bool IsShutdownScheduled();

    void NotifyShutdownScheduled(TimeSpan delay);
    void NotifyShutdownCancelled();
}


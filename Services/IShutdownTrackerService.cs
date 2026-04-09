using AutoShutdownModernized.Models;

namespace AutoShutdownModernized.Services;

public interface IShutdownTrackerService
{
    event EventHandler<ShutdownState>? StateChanged;

    void NotifyShutdownScheduled(TimeSpan delay);
    void NotifyShutdownCancelled();
}


using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AutoShutdownModernized.Services;

public class ShutdownService(ILogger<ShutdownService> logger, IShutdownTrackerService trackerService) : IShutdownService
{
    public void ScheduleShutdown(TimeSpan delay)
    {
        int seconds = (int)Math.Max(0, delay.TotalSeconds);
        logger.LogInformation("Scheduling shutdown in {Seconds} seconds.", seconds);
        
        try
        {
            Process.Start(new ProcessStartInfo("shutdown", $"/s /t {seconds}") { CreateNoWindow = true, UseShellExecute = false });
            trackerService.NotifyShutdownScheduled(delay);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to schedule shutdown.");
        }
    }

    public void CancelShutdown()
    {
        logger.LogInformation("Cancelling shutdown.");
        
        try
        {
            Process.Start(new ProcessStartInfo("shutdown", "/a") { CreateNoWindow = true, UseShellExecute = false });
            trackerService.NotifyShutdownCancelled();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to cancel shutdown.");
        }
    }
}


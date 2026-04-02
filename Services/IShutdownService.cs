using System;

namespace AutoShutdownModernized.Services;

public interface IShutdownService
{
    void ScheduleShutdown(TimeSpan delay);
    void CancelShutdown();
}


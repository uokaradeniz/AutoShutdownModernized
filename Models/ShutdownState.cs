namespace AutoShutdownModernized.Models;

public record ShutdownState(bool IsScheduled, TimeSpan RemainingTime);


using System.Diagnostics;

namespace api;

public interface ITelemetryService
{
    ActivitySource ActivitySource { get; }
}

public class TelemetryService: ITelemetryService
{
    internal TelemetryService(string serviceName)
    {
        ActivitySource = new ActivitySource(serviceName);
    }

    public ActivitySource ActivitySource { get; }
}
namespace NCoreUtils.Google.Cloud.Monitoring;

public interface IMonitoringV3Api
{
    Task CreateTimeSeriesAsync(
        string projectId,
        IReadOnlyList<TimeSeries> timeSeries,
        CancellationToken cancellationToken = default
    );
}
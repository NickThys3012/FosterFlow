using FosterFlow.Application.Common.Interfaces;
using Prometheus;

namespace FosterFlow.Api.Observability;

/// <summary>
/// prometheus-net backed implementation of <see cref="IBusinessMetrics"/> (US-INF-4.1, #47).
/// All metrics are created (and therefore published at their zero value) in the constructor so
/// they appear on <c>/metrics</c> immediately, before any business event has occurred.
/// </summary>
public sealed class PrometheusBusinessMetrics : IBusinessMetrics
{
    private readonly Counter _catListingsCreated;
    private readonly Counter _matchesCreated;
    private readonly Counter _matchesAccepted;
    private readonly Histogram _careBriefingDuration;
    private readonly Gauge _activeFosters;
    private readonly Gauge _activeShelters;

    public PrometheusBusinessMetrics(IMetricFactory? factory = null)
    {
        factory ??= Metrics.DefaultFactory;

        _catListingsCreated = factory.CreateCounter(
            "fosterflow_cat_listings_created_total",
            "Total number of cat listings created.");

        _matchesCreated = factory.CreateCounter(
            "fosterflow_matches_created_total",
            "Total number of foster matches created.");

        _matchesAccepted = factory.CreateCounter(
            "fosterflow_matches_accepted_total",
            "Total number of foster matches accepted.");

        _careBriefingDuration = factory.CreateHistogram(
            "fosterflow_care_briefing_duration_seconds",
            "Duration of care briefing generation in seconds.",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.05, 2, 10)
            });

        _activeFosters = factory.CreateGauge(
            "fosterflow_active_fosters",
            "Current number of active foster carers.");
        
        _activeShelters = factory.CreateGauge(
            "fosterflow_active_shelters",
            "Current number of active shelters.");

        // Touch each metric so it is published at zero before the first observation.
        _catListingsCreated.IncTo(0);
        _matchesCreated.IncTo(0);
        _matchesAccepted.IncTo(0);
        _activeFosters.Set(0);
    }

    public void CatListingCreated() => _catListingsCreated.Inc();

    public void MatchCreated() => _matchesCreated.Inc();

    public void MatchAccepted() => _matchesAccepted.Inc();

    public void ObserveCareBriefingDuration(double seconds) => _careBriefingDuration.Observe(seconds);

    public void SetActiveFosters(int count) => _activeFosters.Set(count);

    public void IncrementActiveFosters() => _activeFosters.Inc();

    public void DecrementActiveFosters() => _activeFosters.Dec();
    public void IncrementActiveShelters() => _activeShelters.Inc();
    public void DecrementActiveShelter() => _activeShelters.Dec();
}

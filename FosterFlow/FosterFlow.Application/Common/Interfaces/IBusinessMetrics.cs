namespace FosterFlow.Application.Common.Interfaces;

/// <summary>
///     Abstraction over the application's custom business metrics (US-INF-4.1, #47).
///     Implemented in the API layer with prometheus-net so the Application layer stays
///     free of infrastructure concerns.
/// </summary>
public interface IBusinessMetrics
{
    /// <summary>Increments <c>fosterflow_cat_listings_created_total</c>.</summary>
    void CatListingCreated();

    /// <summary>Increments <c>fosterflow_matches_created_total</c>.</summary>
    void MatchCreated();

    /// <summary>Increments <c>fosterflow_matches_accepted_total</c>.</summary>
    void MatchAccepted();

    /// <summary>Observes <c>fosterflow_care_briefing_duration_seconds</c>.</summary>
    void ObserveCareBriefingDuration(double seconds);

    /// <summary>Sets the absolute value of <c>fosterflow_active_fosters</c>.</summary>
    void SetActiveFosters(int count);

    /// <summary>Increments <c>fosterflow_active_fosters</c> by one.</summary>
    void IncrementActiveFosters();

    /// <summary>Decrements <c>fosterflow_active_fosters</c> by one.</summary>
    void DecrementActiveFosters();
    /// <summary>Increments <c>fosterflow_active_shelters</c> by one.</summary>
    void IncrementActiveShelters();

    /// <summary>Decrements <c>fosterflow_active_shelters</c> by one.</summary>
    void DecrementActiveShelters();
}

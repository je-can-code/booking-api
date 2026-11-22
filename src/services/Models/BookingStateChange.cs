namespace services.suppliers.Models;

/// <summary>
/// The before and after of a supplier's state change.
/// </summary>
public record BookingStateChange
{
    public BookingStatus PreviousStatus { get; init; }
    public BookingStatus NewStatus { get; init; }
}
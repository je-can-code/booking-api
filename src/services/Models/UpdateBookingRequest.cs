namespace services.suppliers.Models;

/// <summary>
/// The shape of an update request for a booking.
/// </summary>
public sealed record UpdateBookingRequest
{
    public Guid BookingId { get; init; }
    public string SupplierReferenceId { get; init; } = string.Empty;
    public BookingStatus NewStatus { get; init; }
}
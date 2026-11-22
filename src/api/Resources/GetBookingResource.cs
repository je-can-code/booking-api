namespace api.Resources;

/// <summary>
/// The shape of a request for fetching the current state of a booking.
/// </summary>
public sealed record GetBookingResource
{
    public Guid BookingId { get; init; }
}
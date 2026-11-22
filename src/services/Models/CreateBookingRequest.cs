namespace services.suppliers.Models;

/// <summary>
/// The shape of a request for the service to create a new booking.
/// </summary>
public sealed record CreateBookingRequest
{
    public required Guid BookingId { get; init; }
    public required string SupplierReferenceId { get; init; }
    public required string DistributorId { get; init; }
    public required string IdempotencyKey { get; init; }
    public required BookingStatus Status { get; init; }
    public required StayInfo StayInfo { get; init; }
}
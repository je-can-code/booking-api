using services.suppliers.Models;

namespace api.Resources;

/// <summary>
/// The shape of a response for a booking.
/// </summary>
public sealed record BookingResponse
{
    public required Guid BookingId { get; init; }
    public required string SupplierReferenceId { get; init; }
    public required BookingStatus Status { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
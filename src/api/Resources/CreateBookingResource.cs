using services.suppliers.Models;

namespace api.Resources;

/// <summary>
/// The shape of the request for booking a new reservation.
/// </summary>
public sealed record CreateBookingResource
{
    public required string IdempotencyKey { get; init; }
    public required string DistributorId { get; init; }
    public required string PropertyId { get; init; }
    public required string RoomTypeId { get; init; }
    
    // TODO: validation for checkOut coming after checkIn?
    public required DateOnly CheckIn { get; init; }
    public required DateOnly CheckOut { get; init; }
    
    public required GuestData Guest { get; init; }
}
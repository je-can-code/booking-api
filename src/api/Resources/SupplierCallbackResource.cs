using services.suppliers.Models;

namespace api.Resources;

/// <summary>
/// The shape of a request for responding async to a <see cref="CreateBookingResource"/>.
/// </summary>
public sealed record SupplierCallbackResource
{
    public required Guid BookingId {get; init; }
    public required string SupplierReferenceId { get; init; }
    public required BookingStatus Status { get; init; }
}
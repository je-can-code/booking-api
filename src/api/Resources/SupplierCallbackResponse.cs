using services.suppliers.Models;

namespace api.Resources;

/// <summary>
/// The shape of the response for suppliers as acknowledgement of processing their update.
/// </summary>
public sealed record SupplierCallbackResponse
{
    public Guid BookingId {get; init; }
    public string SupplierReferenceId { get; init; } = string.Empty;
    public BookingStatus PreviousStatus { get; init; }
    public BookingStatus NewStatus { get; init; }
}
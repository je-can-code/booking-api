using api.Resources;
using services.suppliers.Models;

namespace api.Mappers;

/// <summary>
/// The implementation of the mapping between supplier models.
/// </summary>
internal sealed class SupplierMapper : ISupplierMapper
{
    /// <inheritdoc/>
    public UpdateBookingRequest ToRequest(Guid bookingId, string supplierReferenceId, BookingStatus newStatus)
    {
        return new()
        {
            BookingId = bookingId,
            SupplierReferenceId = supplierReferenceId,
            NewStatus = newStatus
        };
    }

    /// <inheritdoc/>
    public SupplierCallbackResponse ToResponse(SupplierCallbackResource resource, BookingStateChange bookingStateChange)
    {
        return new()
        {
            BookingId = resource.BookingId,
            SupplierReferenceId = resource.SupplierReferenceId,
            PreviousStatus = bookingStateChange.PreviousStatus,
            NewStatus = bookingStateChange.NewStatus
        };
    }
}
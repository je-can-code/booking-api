using api.Resources;
using services.suppliers.Models;

namespace api.Mappers;

/// <summary>
/// A contract for the shape of the mapper that handles supplier models.
/// </summary>
public interface ISupplierMapper
{
    /// <summary>
    /// Converts the data points into a <see cref="UpdateBookingRequest"/>.
    /// </summary>
    /// <param name="bookingId">The id of the booking to update.</param>
    /// <param name="supplierReferenceId">The reference id from the supplier.</param>
    /// <param name="newStatus">The new status to update to.</param>
    /// <returns>The combined data as one request.</returns>
    UpdateBookingRequest ToRequest(Guid bookingId, string supplierReferenceId, BookingStatus newStatus);
    
    /// <summary>
    /// Converts the <see cref="SupplierCallbackResource"/> and new status into a
    /// <see cref="SupplierCallbackResponse"/>.
    /// </summary>
    /// <param name="resource">The incoming resource.</param>
    /// <param name="bookingStateChange">The state change data for the booking.</param>
    /// <returns>The combined data as one response.</returns>
    SupplierCallbackResponse ToResponse(SupplierCallbackResource resource, BookingStateChange bookingStateChange);
}
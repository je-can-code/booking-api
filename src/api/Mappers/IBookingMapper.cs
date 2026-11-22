using api.Resources;
using services.suppliers.Models;

namespace api.Mappers;

/// <summary>
/// The contract shape for a mapper for translating between service layer and api layer models.
/// </summary>
public interface IBookingMapper
{
    /// <summary>
    /// Converts a <see cref="CreateBookingResource"/> into a <see cref="CreateBookingRequest"/>.
    /// </summary>
    /// <param name="resource">The incoming resource to map.</param>
    /// <returns>The mapped request.</returns>
    CreateBookingRequest ToCreateRequest(CreateBookingResource resource);
    
    /// <summary>
    /// Converts a <see cref="RawBookingResponse"/> to a <see cref="BookingResponse"/>.
    /// </summary>
    /// <param name="response">The incoming response to map.</param>
    /// <returns>The mapped response.</returns>
    BookingResponse ToResponse(RawBookingResponse response);
}
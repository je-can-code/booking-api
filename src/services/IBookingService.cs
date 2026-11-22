using services.suppliers.Models;

namespace services.suppliers;

/// <summary>
/// A contract for the shape of the service that handles booking logic.
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Creates a new booking in the database.
    /// </summary>
    /// <param name="request">The request data to build the booking from.</param>
    /// <returns>The created booking data.</returns>
    Task<RawBookingResponse> Create(CreateBookingRequest request);
    
    /// <summary>
    /// Gets an existing booking by its bookingId.
    /// </summary>
    /// <param name="bookingId">The id of the booking.</param>
    /// <returns>The found booking, or null if there is no booking with that id.</returns>
    Task<RawBookingResponse?> Get(Guid bookingId);

    /// <summary>
    /// Updates an existing booking with the supplier's provided status.
    /// </summary>
    /// <param name="request">The request data to update with.</param>
    /// <returns>The before and after state representing the update.</returns>
    Task<BookingStateChange?> Update(UpdateBookingRequest request);

}
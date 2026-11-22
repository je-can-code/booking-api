using data;
using services.suppliers.Models;

namespace services.suppliers.Mappers;

/// <summary>
/// A contract for the shape of the mapper that handles booking response models.
/// </summary>
public interface IBookingEntityMapper
{
    /// <summary>
    /// Converts the <see cref="CreateBookingRequest"/> into a database entity.
    /// </summary>
    /// <param name="request">The request to convert.</param>
    /// <returns>The converted entity.</returns>
    BookingEntity ToEntity(CreateBookingRequest request);
    
    /// <summary>
    /// Converts the <see cref="BookingEntity"/> into a <see cref="RawBookingResponse"/>.
    /// </summary>
    /// <param name="entity">The database entity to convert.</param>
    /// <returns>The converted response.</returns>
    RawBookingResponse ToRawResponse(BookingEntity entity);

    /// <summary>
    /// Converts the two states into a single state change model.
    /// </summary>
    /// <param name="entity">The current state of the booking entity.</param>
    /// <param name="previousStatus">The previous state prior to update.</param>
    /// <returns>The before and after snapshot.</returns>
    BookingStateChange ToStateChange(BookingEntity entity, BookingStatus previousStatus);
}
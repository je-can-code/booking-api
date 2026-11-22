namespace data.Repositories;

/// <summary>
/// A contract for the shape of a repository managing bookings.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Gets a single booking by its id.
    /// </summary>
    /// <param name="id">The key of the booking.</param>
    /// <returns>The found booking, or null if none was found.</returns>
    Task<BookingEntity?> GetById(Guid id);

    /// <summary>
    /// Gets a single booking by its unique index.
    /// </summary>
    /// <param name="distributorId">The id of the distributor.</param>
    /// <param name="idempotencyKey">The key ensuring idempotent results.</param>
    /// <returns>The found booking, or null if none was found.</returns>
    Task<BookingEntity?> GetByIndex(string distributorId, string idempotencyKey);
    
    /// <summary>
    /// Creates a new booking entity.
    /// </summary>
    /// <param name="bookingEntity">The entity to create.</param>
    /// <returns>The created booking.</returns>
    Task<BookingEntity> Create(BookingEntity bookingEntity);

    /// <summary>
    /// Updates the status of a booking.
    /// </summary>
    /// <param name="id">The id of the booking to update.</param>
    /// <param name="previousStatus">The previous status of the booking for lookup.</param>
    /// <param name="newStatus">The new status of the booking.</param>
    /// <param name="supplierReferenceId">The supplier reference code provided with the update.</param>
    /// <returns>The updated booking.</returns>
    Task<BookingEntity?> UpdateStatus(Guid id, string previousStatus, string newStatus, string supplierReferenceId);
}
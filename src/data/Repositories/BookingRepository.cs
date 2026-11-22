using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// The implementation of the repository that manages bookings.
/// </summary>
internal sealed class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _dbContext;

    public BookingRepository(BookingDbContext context)
    {
        _dbContext = context;
    }

    /// <inheritdoc/>
    public async Task<BookingEntity?> GetById(Guid id)
    {
        return await _dbContext
            .Bookings
            .AsNoTracking()
            .SingleOrDefaultAsync(entity => entity.BookingId == id, CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task<BookingEntity?> GetByIndex(string distributorId, string idempotencyKey)
    {
        return await _dbContext
            .Bookings
            .SingleOrDefaultAsync(entity =>
                    entity.DistributorId == distributorId &&
                    entity.IdempotencyKey == idempotencyKey,
                CancellationToken.None);
    }

    /// <inheritdoc/>
    public async Task<BookingEntity> Create(BookingEntity bookingEntity)
    {
        await _dbContext.AddAsync(bookingEntity);
        await _dbContext.SaveChangesAsync();
        return bookingEntity;
    }

    /// <inheritdoc/>
    public async Task<BookingEntity?> UpdateStatus(
        Guid id,
        string previousStatus,
        string newStatus,
        string supplierReferenceId)
    {
        var booking = await _dbContext
            .Bookings
            .SingleOrDefaultAsync(entity =>
                    entity.BookingId == id &&
                    // the check against previousStatus ensures very close calls don't clobber one another.
                    entity.Status == previousStatus,
                CancellationToken.None);

        // verify we were able to find the booking to update.
        if (booking is null)
        {
            // nothing matches those data points or someone beat us to the update.
            return null;
        }

        // update the data.
        booking.Status = newStatus;
        booking.SupplierReferenceId = supplierReferenceId;
        booking.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();
        return booking;
    }
}
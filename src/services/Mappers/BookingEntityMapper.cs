using data;
using services.suppliers.Models;

namespace services.suppliers.Mappers;

/// <summary>
/// The implementation of the conversion between <see cref="BookingEntity"/> and <see cref="RawBookingResponse"/>es.
/// </summary>
internal sealed class BookingEntityMapper : IBookingEntityMapper
{
    /// <inheritdoc/>
    public BookingEntity ToEntity(CreateBookingRequest request)
    {
        return new()
        {
            BookingId = request.BookingId,
            SupplierReferenceId = request.SupplierReferenceId,
            DistributorId = request.DistributorId,
            IdempotencyKey = request.IdempotencyKey,
            Status = request.Status.ToString().ToUpperInvariant(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
        };
    }

    /// <inheritdoc/>
    public RawBookingResponse ToRawResponse(BookingEntity entity)
    {
        return new()
        {
            BookingId = entity.BookingId,
            SupplierReferenceId = entity.SupplierReferenceId,
            DistributorId = entity.DistributorId,
            IdempotencyKey = entity.IdempotencyKey,
            Status = Enum.Parse<BookingStatus>(entity.Status),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    /// <inheritdoc/>
    public BookingStateChange ToStateChange(BookingEntity entity, BookingStatus previousStatus)
    {
        return new()
        {
            PreviousStatus = previousStatus,
            NewStatus = Enum.Parse<BookingStatus>(entity.Status),
        };
    }
}
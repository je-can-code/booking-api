using api.Resources;
using services.suppliers.Models;

namespace api.Mappers;

/// <summary>
/// The implementation of the mapper for converting to and from booking models and resources.
/// </summary>
internal sealed class BookingMapper : IBookingMapper
{
    /// <inheritdoc/>
    public CreateBookingRequest ToCreateRequest(CreateBookingResource resource)
    {
        return new()
        {
            BookingId = Guid.NewGuid(),
            SupplierReferenceId = string.Empty,
            DistributorId = resource.DistributorId,
            IdempotencyKey = resource.IdempotencyKey,
            Status = BookingStatus.RECEIVED,
            StayInfo = new()
            {
                CheckIn = resource.CheckIn,
                CheckOut = resource.CheckOut,
                Guest = resource.Guest,
                PropertyId = resource.PropertyId,
                RoomTypeId = resource.RoomTypeId,
            }
        };
    }

    /// <inheritdoc/>
    public BookingResponse ToResponse(RawBookingResponse response)
    {
        return new()
        {
            BookingId = response.BookingId,
            SupplierReferenceId = response.SupplierReferenceId,
            Status = response.Status,
            CreatedAt = response.CreatedAt,
            UpdatedAt = response.UpdatedAt,
        };
    }
}
using services.suppliers.Infrastructure.LuxurioSupremeSupplier;
using services.suppliers.Models;

namespace services.suppliers.Mappers;

internal sealed class LuxurioMapper : ILuxurioMapper
{
    public LuxurioSupremeRequest ToRequest(Guid bookingId, StayInfo stayInfo)
    {
        return new LuxurioSupremeRequest
        {
            BookingId = bookingId,
            StayInfo = stayInfo
        };
    }
}
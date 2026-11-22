using services.suppliers.Infrastructure.LuxurioSupremeSupplier;
using services.suppliers.Models;

namespace services.suppliers.Mappers;

public interface ILuxurioMapper
{
    public LuxurioSupremeRequest ToRequest(Guid bookingId, StayInfo stayInfo);
}
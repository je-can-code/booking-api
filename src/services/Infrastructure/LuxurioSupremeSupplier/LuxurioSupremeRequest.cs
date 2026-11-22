using services.suppliers.Models;

namespace services.suppliers.Infrastructure.LuxurioSupremeSupplier;

public sealed record LuxurioSupremeRequest
{
    public required Guid BookingId { get; init; }
    public required StayInfo StayInfo { get; init; }
}
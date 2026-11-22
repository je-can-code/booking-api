using services.suppliers.Models;

namespace services.suppliers.Infrastructure.LuxurioSupremeSupplier;

public sealed record LuxurioSupremeResponse
{
    public bool Completed { get; init; }
    public string SupplierReferenceId { get; init; } = string.Empty;
    public BookingStatus Status { get; init; }
}
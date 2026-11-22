using services.suppliers.Models;

namespace services.suppliers.Infrastructure.LuxurioSupremeSupplier;

/// <summary>
/// The implementation for communicating with the Luxurio Supreme external supplier.
/// </summary>
internal sealed class MockLuxurioSupremeApi : ILuxurioSupremeApi
{
    internal static string REJECTION = "reject";
    internal static string ACCEPTED = "accepted";

    public async Task<LuxurioSupremeResponse> PostAsync(LuxurioSupremeRequest request)
    {
        var roomType = request.StayInfo.RoomTypeId;
        
        // if the roomType property contains the magic rejection word, then reject it.
        if (roomType.Contains(REJECTION))
        {
            return new()
            {
                Completed = true,
                Status = BookingStatus.REJECTED,
                SupplierReferenceId = $"ls-rejected_{Guid.NewGuid()}",
            };
        }

        // if the roomType property contains the magic acceptance word, then confirm it.
        if (roomType.Contains(ACCEPTED))
        {
            return new()
            {
                Completed = true,
                Status = BookingStatus.CONFIRMED,
                SupplierReferenceId = $"ls-confirmed_{Guid.NewGuid()}",
            };
        }

        await Task.Delay(5000);

        // this response will never actually be used due to waitAsync timeout.
        // this expects them to ping us at the /Suppliers/callbacks endpoint.
        return new()
        {
            Completed = false,
            Status = BookingStatus.PENDING,
            SupplierReferenceId = string.Empty,
        };
    }
}
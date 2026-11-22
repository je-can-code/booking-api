using api.Mappers;

namespace api.Extensions;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all the various services from this layer to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void RegisterApiLayer(this IServiceCollection services)
    {
        services.AddTransient<ISupplierMapper, SupplierMapper>();
        services.AddTransient<IBookingMapper, BookingMapper>();
    }
}
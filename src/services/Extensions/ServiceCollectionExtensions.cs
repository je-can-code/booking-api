using Microsoft.Extensions.DependencyInjection;
using services.suppliers.Infrastructure;
using services.suppliers.Infrastructure.LuxurioSupremeSupplier;
using services.suppliers.Mappers;
using services.suppliers.Validators;

namespace services.suppliers.Extensions;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all the various services from this layer to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void RegisterServiceLayer(this IServiceCollection services)
    {
        services.AddTransient<IBookingService, BookingService>();
        services.AddTransient<IBookingEntityMapper, BookingEntityMapper>();
        services.AddSingleton<IStatusTransitionValidator, StatusTransitionValidator>();
        
        // normally, this would probably be a more complex setup including http client wiring.
        services.AddTransient<ILuxurioSupremeApi, MockLuxurioSupremeApi>();
        services.AddTransient<ILuxurioMapper,  LuxurioMapper>();
    }
}
using data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace data.Extensions;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register all the various services from this layer to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">The connection string.</param>
    public static void RegisterDataLayer(this IServiceCollection services, string connectionString)
    {
        services.RegisterRepositories();
        services.RegisterDatabase(connectionString);
    }

    /// <summary>
    /// Registers the repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection to register to.</param>
    private static void RegisterRepositories(this IServiceCollection services)
    {
        services.AddTransient<IBookingRepository, BookingRepository>();
    }

    /// <summary>
    /// Registers the database as SQLite with the given connection string.
    /// </summary>
    /// <param name="services">The service collection to register to.</param>
    /// <param name="connectionString">The database connection string.</param>
    private static void RegisterDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BookingDbContext>(optionsBuilder => optionsBuilder.UseSqlite(connectionString));
    }
}
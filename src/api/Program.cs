using System.Text.Json.Serialization;
using api.Extensions;
using data.Extensions;
using services.suppliers.Extensions;

namespace api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // register all our services.
        builder.Services.RegisterApiLayer();
        builder.Services.RegisterServiceLayer();
        builder.Services.RegisterDataLayer(builder.Configuration.GetConnectionString("DefaultConnection")!);

        // build the converter to read status numbers but block nonsense.
        var converter = new JsonStringEnumConverter(allowIntegerValues: false);
        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(converter);
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(converter));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        builder.Services.AddProblemDetails();

        var app = builder.Build();
        app.UseExceptionHandler();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Services.InitializeDatabase();

        await app.RunAsync();
    }
}
using api.Infrastructure;
using api.Mappers;
using api.Resources;
using Microsoft.AspNetCore.Mvc;
using services.suppliers;

namespace api.Controllers;

/// <summary>
/// A controller for managing bookings.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class BookingsController : ControllerBase
{
    private readonly ILogger<BookingsController> _logger;
    private readonly IBookingService _bookingService;
    private readonly IBookingMapper _bookingMapper;

    public BookingsController(
        ILogger<BookingsController> logger,
        IBookingService bookingService,
        IBookingMapper bookingMapper)
    {
        _logger = logger;
        _bookingService = bookingService;
        _bookingMapper = bookingMapper;
    }

    /// <summary>
    /// Creates a new booking record.
    /// </summary>
    /// <param name="resource">The incoming request to create a booking from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created booking.</returns>
    [HttpPost]
    public async Task<ActionResult> Create(CreateBookingResource resource, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "starting create request with idempotency key {IdempotencyKey} and distributor id {DistributorId}",
            resource.IdempotencyKey,
            resource.DistributorId);

        var mappedRequest = _bookingMapper.ToCreateRequest(resource);
        var response = await _bookingService.Create(mappedRequest);
        var mappedResponse = _bookingMapper.ToResponse(response);

        _logger.LogInformation(
            "create request issued for booking id {BookingId} in status {NewStatus}",
            mappedResponse.BookingId,
            mappedResponse.Status.ToString().ToUpperInvariant());
        
        // Could pivot on Status to return Accepted to indicate processing and Ok for confirmed/rejected.

        return Ok(ApiResponse<BookingResponse>.From(mappedResponse));
    }

    /// <summary>
    /// Gets a single booking by its id.
    /// </summary>
    /// <param name="bookingId">The id of the booking to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The found booking, or nothing if none was found.</returns>
    [HttpGet("{bookingId:guid}")]
    public async Task<ActionResult> Get(Guid bookingId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("attempting retrieval of booking id {BookingId}.", bookingId);

        var response = await _bookingService.Get(bookingId);

        // verify the bookingId yielded a record.
        if (response is null)
        {
            _logger.LogWarning("attempted to get booking id {BookingId}, but no booking by that id exists.", bookingId);
            return NotFound();
        }

        var mappedResponse = _bookingMapper.ToResponse(response);

        _logger.LogInformation("retrieved booking id {BookingId}.", mappedResponse.BookingId);

        return Ok(ApiResponse<BookingResponse>.From(mappedResponse));
    }
}
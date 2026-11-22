using api.Infrastructure;
using api.Mappers;
using api.Resources;
using Microsoft.AspNetCore.Mvc;
using services.suppliers;

namespace api.Controllers;

/// <summary>
/// A controller for managing interactions from external suppliers.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class SuppliersController : ControllerBase
{
    private readonly ILogger<SuppliersController> _logger;
    private readonly IBookingService _bookingService;
    private readonly ISupplierMapper _supplierMapper;

    public SuppliersController(
        ILogger<SuppliersController> logger,
        IBookingService bookingService,
        ISupplierMapper supplierMapper)
    {
        _logger = logger;
        _bookingService = bookingService;
        _supplierMapper = supplierMapper;
    }

    /// <summary>
    /// Handles a booking update from an external supplier. 
    /// </summary>
    /// <param name="resource">The update resource from the supplier.</param>
    /// <returns>The transition information for the supplier.</returns>
    [HttpPost("callbacks")]
    public async Task<ActionResult> Callbacks(SupplierCallbackResource resource)
    {
        _logger.LogInformation(
            "starting update request for booking id: {BookingId} and supplier reference id {SupplierReferenceId} updating status to {NewStatus}",
            resource.BookingId,
            resource.SupplierReferenceId,
            resource.Status.ToString().ToUpperInvariant());

        var mappedRequest = _supplierMapper.ToRequest(
            resource.BookingId,
            resource.SupplierReferenceId,
            resource.Status);
        var response = await _bookingService.Update(mappedRequest);

        if (response is null)
        {
            _logger.LogWarning(
                "attempted to update booking id {BookingId}, but no booking by that id exists.",
                resource.BookingId);
            return NotFound();
        }

        var mappedResponse = _supplierMapper.ToResponse(resource, response);

        _logger.LogInformation(
            "updated booking id {BookingId} status from {PreviousStatus} to {NewStatus}.",
            resource.BookingId,
            mappedResponse.PreviousStatus.ToString().ToUpperInvariant(),
            mappedResponse.NewStatus.ToString().ToUpperInvariant());

        return Ok(ApiResponse<SupplierCallbackResponse>.From(mappedResponse));
    }
}
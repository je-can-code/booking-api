using data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using services.suppliers.Infrastructure;
using services.suppliers.Infrastructure.LuxurioSupremeSupplier;
using services.suppliers.Mappers;
using services.suppliers.Models;
using services.suppliers.Validators;

namespace services.suppliers;

/// <summary>
/// The implementation that handles the logic for creating and retrieving bookings.
/// </summary>
internal sealed class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly IBookingRepository _repository;
    private readonly IBookingEntityMapper _mapper;
    private readonly ILuxurioSupremeApi _apiClient;
    private readonly ILuxurioMapper _luxurioMapper;
    private readonly IStatusTransitionValidator _validator;

    public BookingService(
        ILogger<BookingService> logger,
        IBookingRepository repository,
        IBookingEntityMapper mapper,
        ILuxurioSupremeApi apiClient,
        ILuxurioMapper luxurioMapper,
        IStatusTransitionValidator validator)
    {
        _logger = logger;
        _repository = repository;
        _mapper = mapper;
        _apiClient = apiClient;
        _luxurioMapper = luxurioMapper;
        _validator = validator;
    }

    /// <inheritdoc/>
    public async Task<RawBookingResponse> Create(CreateBookingRequest request)
    {
        // map and create the entity.
        var mappedRequest = _mapper.ToEntity(request);

        try
        {
            var response = await _repository.Create(mappedRequest);

            var luxurioRequest = _luxurioMapper.ToRequest(response.BookingId, request.StayInfo);
            var task = _apiClient.PostAsync(luxurioRequest);
            var apiResponse = await task.WaitAsync(TimeSpan.FromSeconds(3));

            // update the response with what the api returned, and map it back.
            var statusResponse = await _repository.UpdateStatus(
                mappedRequest.BookingId,
                nameof(BookingStatus.RECEIVED).ToUpperInvariant(),
                apiResponse.Status.ToString().ToUpperInvariant(),
                apiResponse.SupplierReferenceId);

            return _mapper.ToRawResponse(statusResponse!);

        }
        catch (DbUpdateException ex)
        {
            // specific to SQLite: unique constraint failed- the record already exists.
            if (ex.InnerException!.Message.Contains("SQLite Error 19"))
            {
                _logger.LogWarning(
                    ex,
                    "attempted to create a duplicate booking for distributor id {DistributorId} and idempotency key {IdempotencyKey}",
                    request.DistributorId,
                    request.IdempotencyKey);

                // instead of creating, just get the record and return it- it is a retry.
                var indexedBooking = (await _repository.GetByIndex(request.DistributorId, request.IdempotencyKey))!;
                return _mapper.ToRawResponse(indexedBooking);
            }

            // something else more unexpected happened!
            _logger.LogError(
                ex,
                "an unexpected error occured while creating a booking for id {BookingId}",
                mappedRequest.BookingId);
            throw;
        }
        catch (TimeoutException)
        {
            _logger.LogInformation(
                "the request to Luxurio Supreme timed out before replying for booking {BookingId}.",
                request.BookingId);
            
            var statusResponse = await _repository.UpdateStatus(
                mappedRequest.BookingId,
                nameof(BookingStatus.RECEIVED).ToUpperInvariant(),
                nameof(BookingStatus.PENDING).ToUpperInvariant(),
                string.Empty);
            
            return _mapper.ToRawResponse(statusResponse!);
        }
        catch (Exception ex)
        {
            // something else more unexpected happened!
            _logger.LogError(
                ex,
                "an unexpected error occured while creating a booking for id {BookingId}",
                mappedRequest.BookingId);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<RawBookingResponse?> Get(Guid bookingId)
    {
        var response = await _repository.GetById(bookingId);

        // validate something was found.
        if (response is null)
        {
            // return null for no matching bookingId.
            return null;
        }

        // map the found response.
        return _mapper.ToRawResponse(response);
    }

    /// <inheritdoc/>
    public async Task<BookingStateChange?> Update(UpdateBookingRequest request)
    {
        var response = await _repository.GetById(request.BookingId);

        // validate something was found.
        if (response is null)
        {
            // return null for no matching bookingId.
            return null;
        }

        // capture the previous status of the booking before update.
        var previousStatus = Enum.Parse<BookingStatus>(response.Status);

        // validate this status transition is permitted before executing update.
        if (_validator.CanTransitionTo(previousStatus, request.NewStatus) is false)
        {
            _logger.LogWarning(
                "attempted to transition from {BookingCurrentStatus} to {BookingNewStatus}, but that is not allowed.",
                response.Status,
                request.NewStatus);
            return _mapper.ToStateChange(response, previousStatus);
        }

        try
        {
            // execute the update of the status.
            var updatedResponse = await _repository.UpdateStatus(
                response.BookingId,
                previousStatus.ToString().ToUpperInvariant(),
                request.NewStatus.ToString().ToUpperInvariant(),
                request.SupplierReferenceId);
            
            // verify we weren't beaten by someone else that was just too fast for us.
            if (updatedResponse is null)
            {
                // get the booking that was actually updated and map it.
                var actualBooking = await _repository.GetById(request.BookingId);
                return _mapper.ToStateChange(actualBooking!, previousStatus);
            }

            // return the updated response.
            return _mapper.ToStateChange(updatedResponse!, previousStatus);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // formally log our concurrency complaint.
            _logger.LogWarning(
                ex,
                "attempted to update {BookingId} while it was already being updated.",
                response.BookingId);
            
            // get the booking that was actually updated and map it.
            var actualBooking = await _repository.GetById(request.BookingId);
            return _mapper.ToStateChange(actualBooking!, previousStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "an unexpected error occured while updating {BookingId}",
                response.BookingId);
            throw;
        }
    }
}
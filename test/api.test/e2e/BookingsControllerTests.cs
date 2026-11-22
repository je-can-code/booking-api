using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using api.Infrastructure;
using api.Resources;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using services.suppliers.Models;

namespace api.test.e2e;

public class BookingsControllerTests : BaseTests, IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _bookingsClient;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) },
        PropertyNameCaseInsensitive = true,
    };

    public BookingsControllerTests(WebApplicationFactory<Program> factory)
    {
        _bookingsClient = factory.CreateClient();
    }

    /// <summary>
    /// A full end-to-end flow test.
    /// 
    ///     Creates the booking ->
    ///         validates the booking exists and is pending ->
    ///             uses suppliers/callbacks to confirm booking ->
    ///                 proves the confirmed update stuck.
    /// </summary>
    [Fact]
    public async Task DelayedConfirmation()
    {
        // create the initial booking.
        var validRequest = _fixture.Create<CreateBookingResource>();
        var createdResponse = await _bookingsClient.PostAsJsonAsync("Bookings", validRequest, _serializerOptions);
        var createdResponseString = await createdResponse.Content.ReadAsStringAsync();
        var createdData = JsonSerializer.Deserialize<ApiResponse<BookingResponse>>(
            createdResponseString,
            _serializerOptions)!;
        createdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        createdData.Data.Status.Should().Be(BookingStatus.PENDING);

        // prove booking is presently pending.
        var pendingResponse = await _bookingsClient.GetAsync($"Bookings/{createdData.Data.BookingId}");
        var pendingContent = await pendingResponse.Content.ReadAsStringAsync();
        var pendingBody = JsonSerializer.Deserialize<ApiResponse<BookingResponse>>(pendingContent, _serializerOptions)!;
        pendingResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        pendingBody.Data.Status.Should().Be(BookingStatus.PENDING);

        // make the supplier call to update.
        var supplierRequest = _fixture.Build<SupplierCallbackResource>()
            .With(request => request.BookingId, createdData.Data.BookingId)
            .With(request => request.Status, BookingStatus.CONFIRMED)
            .Create();
        var supplierResponse = await _bookingsClient.PostAsJsonAsync(
            "Suppliers/callbacks",
            supplierRequest,
            _serializerOptions);
        var supplierContent = await supplierResponse.Content.ReadAsStringAsync();
        var supplierBody = JsonSerializer.Deserialize<ApiResponse<SupplierCallbackResponse>>(
            supplierContent,
            _serializerOptions)!;
        supplierResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        supplierBody.Data.PreviousStatus.Should().Be(BookingStatus.PENDING);
        supplierBody.Data.NewStatus.Should().Be(BookingStatus.CONFIRMED);

        // prove booking was updated to confirmed.
        var confirmedResponse = await _bookingsClient.GetAsync($"Bookings/{createdData.Data.BookingId}");
        var confirmedContent = await confirmedResponse.Content.ReadAsStringAsync();
        var confirmedBody = JsonSerializer.Deserialize<ApiResponse<BookingResponse>>(
            confirmedContent,
            _serializerOptions)!;
        confirmedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        confirmedBody.Data.Status.Should().Be(BookingStatus.CONFIRMED);
    }
}
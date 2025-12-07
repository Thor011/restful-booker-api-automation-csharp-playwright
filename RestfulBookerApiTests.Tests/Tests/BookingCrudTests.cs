using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RestfulBookerApiTests.Tests.Helpers;
using RestfulBookerApiTests.Tests.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestfulBookerApiTests.Tests.Tests
{
    [TestFixture]
    [Category("Booking")]
    [Category("CRUD")]
    public class BookingCrudTests
    {
        private ApiHelper _apiHelper;

        [SetUp]
        public void Setup()
        {
            _apiHelper = new ApiHelper();
        }

        [TearDown]
        public void TearDown()
        {
            _apiHelper?.Dispose();
        }

        [Test]
        [Description("TC-001: Should verify API health check")]
        public async Task TC001_VerifyApiHealthCheck()
        {
            // Act
            var response = await _apiHelper.GetAsync("/ping");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        [Description("TC-002: Should create a new booking successfully")]
        public async Task TC002_CreateNewBookingSuccessfully()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content);
            
            bookingResponse.Should().NotBeNull();
            bookingResponse.Bookingid.Should().BeGreaterThan(0);
            bookingResponse.Booking.Firstname.Should().Be(booking.Firstname);
            bookingResponse.Booking.Lastname.Should().Be(booking.Lastname);
            bookingResponse.Booking.Totalprice.Should().Be(booking.Totalprice);
        }

        [Test]
        [Description("TC-003: Should retrieve booking by ID")]
        public async Task TC003_RetrieveBookingById()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse.Bookingid;

            // Act
            var response = await _apiHelper.GetAsync($"/booking/{bookingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedBooking = _apiHelper.DeserializeResponse<Booking>(response.Content);
            
            retrievedBooking.Should().NotBeNull();
            retrievedBooking.Firstname.Should().Be(booking.Firstname);
            retrievedBooking.Lastname.Should().Be(booking.Lastname);
        }

        [Test]
        [Description("TC-004: Should retrieve all booking IDs")]
        public async Task TC004_RetrieveAllBookingIds()
        {
            // Act
            var response = await _apiHelper.GetAsync("/booking");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content);
            
            bookingIds.Should().NotBeNull();
            bookingIds.Should().NotBeEmpty();
        }

        [Test]
        [Description("TC-005: Should filter bookings by name")]
        public async Task TC005_FilterBookingsByName()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?firstname={booking.Firstname}&lastname={booking.Lastname}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content);
            
            bookingIds.Should().NotBeNull();
            bookingIds.Should().NotBeEmpty();
        }

        [Test]
        [Description("TC-006: Should update booking successfully")]
        public async Task TC006_UpdateBookingSuccessfully()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse.Bookingid;
            var token = await _apiHelper.CreateAuthToken();

            var updatedBooking = TestDataGenerator.GenerateValidBooking();

            // Act
            var response = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = _apiHelper.DeserializeResponse<Booking>(response.Content);
            
            updated.Firstname.Should().Be(updatedBooking.Firstname);
            updated.Lastname.Should().Be(updatedBooking.Lastname);
            updated.Totalprice.Should().Be(updatedBooking.Totalprice);
        }

        [Test]
        [Description("TC-007: Should partially update booking using PATCH")]
        public async Task TC007_PartiallyUpdateBooking()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse.Bookingid;
            var token = await _apiHelper.CreateAuthToken();

            var partialUpdate = new 
            {
                firstname = "UpdatedFirstName",
                lastname = "UpdatedLastName"
            };

            // Act
            var response = await _apiHelper.PatchAsync($"/booking/{bookingId}", partialUpdate, token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = _apiHelper.DeserializeResponse<Booking>(response.Content);
            
            updated.Firstname.Should().Be("UpdatedFirstName");
            updated.Lastname.Should().Be("UpdatedLastName");
        }

        [Test]
        [Description("TC-008: Should delete booking successfully")]
        public async Task TC008_DeleteBookingSuccessfully()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse.Bookingid;
            var token = await _apiHelper.CreateAuthToken();

            // Act
            var deleteResponse = await _apiHelper.DeleteAsync($"/booking/{bookingId}", token);

            // Assert
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Verify deletion
            var getResponse = await _apiHelper.GetAsync($"/booking/{bookingId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        [Description("TC-009: Should return 404 for non-existent booking")]
        public async Task TC009_Return404ForNonExistentBooking()
        {
            // Act
            var response = await _apiHelper.GetAsync("/booking/999999999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        [Description("TC-010: Should reject update without authentication")]
        public async Task TC010_RejectUpdateWithoutAuthentication()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse.Bookingid;

            var updatedBooking = TestDataGenerator.GenerateValidBooking();

            // Act
            var response = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}

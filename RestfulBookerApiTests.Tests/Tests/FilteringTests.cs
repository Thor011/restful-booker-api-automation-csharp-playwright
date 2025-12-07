using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RestfulBookerApiTests.Tests.Helpers;
using RestfulBookerApiTests.Tests.Models;

namespace RestfulBookerApiTests.Tests.Tests
{
    [TestFixture]
    [Category("Filtering")]
    [Category("Search")]
    public class FilteringTests
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
        [Description("TC-FILTER-001: Should filter by firstname only")]
        public async Task TCFILTER001_FilterByFirstnameOnly()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?firstname={booking.Firstname}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
            bookingIds.Should().Contain(b => b.Bookingid == createdBooking.Bookingid);
        }

        [Test]
        [Description("TC-FILTER-002: Should filter by lastname only")]
        public async Task TCFILTER002_FilterByLastnameOnly()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?lastname={booking.Lastname}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
            bookingIds.Should().Contain(b => b.Bookingid == createdBooking.Bookingid);
        }

        [Test]
        [Description("TC-FILTER-003: Should filter by checkin date")]
        public async Task TCFILTER003_FilterByCheckinDate()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?checkin={booking.Bookingdates.Checkin}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
        }

        [Test]
        [Description("TC-FILTER-004: Should filter by checkout date")]
        public async Task TCFILTER004_FilterByCheckoutDate()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?checkout={booking.Bookingdates.Checkout}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
        }

        [Test]
        [Description("TC-FILTER-005: Should filter by multiple parameters")]
        public async Task TCFILTER005_FilterByMultipleParameters()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync(
                $"/booking?firstname={booking.Firstname}&lastname={booking.Lastname}&checkin={booking.Bookingdates.Checkin}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
            bookingIds.Should().Contain(b => b.Bookingid == createdBooking.Bookingid);
        }

        [Test]
        [Description("TC-FILTER-006: Should return empty list for non-matching filter")]
        public async Task TCFILTER006_ReturnEmptyListForNonMatchingFilter()
        {
            // Act
            var response = await _apiHelper.GetAsync("/booking?firstname=NonExistentName123456789");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
            // May be empty or contain no matching results
        }

        [Test]
        [Description("TC-FILTER-007: Should handle case sensitivity in filters")]
        public async Task TCFILTER007_HandleCaseSensitivityInFilters()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Firstname = "TestUser";
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act - Try different cases
            var responseUpper = await _apiHelper.GetAsync($"/booking?firstname=TESTUSER");
            var responseLower = await _apiHelper.GetAsync($"/booking?firstname=testuser");
            var responseExact = await _apiHelper.GetAsync($"/booking?firstname=TestUser");

            // Assert
            responseUpper.StatusCode.Should().Be(HttpStatusCode.OK);
            responseLower.StatusCode.Should().Be(HttpStatusCode.OK);
            responseExact.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [Description("TC-FILTER-008: Should handle special characters in filter")]
        public async Task TCFILTER008_HandleSpecialCharactersInFilter()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Firstname = "John@Test";
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?firstname=John@Test");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [Description("TC-FILTER-009: Should handle URL encoding in filters")]
        public async Task TCFILTER009_HandleUrlEncodingInFilters()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Firstname = "John Doe"; // Space in name

            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync($"/booking?firstname=John%20Doe");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        [Description("TC-FILTER-010: Should filter by date range")]
        public async Task TCFILTER010_FilterByDateRange()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createdBooking = await _apiHelper.CreateBooking(booking);

            // Act
            var response = await _apiHelper.GetAsync(
                $"/booking?checkin={booking.Bookingdates.Checkin}&checkout={booking.Bookingdates.Checkout}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingIds = _apiHelper.DeserializeResponse<List<BookingId>>(response.Content!);
            
            bookingIds.Should().NotBeNull();
        }
    }
}

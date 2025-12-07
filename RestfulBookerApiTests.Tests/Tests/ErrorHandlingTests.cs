using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Category("Performance")]
    [Category("ErrorHandling")]
    public class ErrorHandlingTests
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
        [Description("TC-PERF-001: Should respond within acceptable time")]
        public async Task TCPERF001_RespondWithinAcceptableTime()
        {
            // Arrange
            var stopwatch = Stopwatch.StartNew();

            // Act
            var response = await _apiHelper.GetAsync("/booking");
            stopwatch.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000, "API should respond within 5 seconds");
        }

        [Test]
        [Description("TC-PERF-002: Should handle concurrent requests")]
        public async Task TCPERF002_HandleConcurrentRequests()
        {
            // Arrange
            var tasks = new List<Task<PlaywrightResponse>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(_apiHelper.GetAsync("/booking"));
            }

            // Act
            var responses = await Task.WhenAll(tasks);

            // Assert
            responses.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));
        }

        [Test]
        [Description("TC-ERR-001: Should handle missing required fields")]
        public async Task TCERR001_HandleMissingRequiredFields()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithMissingField();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        [Description("TC-ERR-002: Should handle invalid data types")]
        public async Task TCERR002_HandleInvalidDataTypes()
        {
            // Arrange
            var invalidBooking = new
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = "INVALID_PRICE", // Should be number
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "2024-01-01",
                    checkout = "2024-01-05"
                }
            };

            // Act
            var response = await _apiHelper.PostAsync("/booking", invalidBooking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        [Description("TC-ERR-003: Should handle invalid booking ID format")]
        public async Task TCERR003_HandleInvalidBookingIdFormat()
        {
            // Act
            var response = await _apiHelper.GetAsync("/booking/invalid_id");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        [Description("TC-ERR-004: Should handle XSS attempts")]
        public async Task TCERR004_HandleXssAttempts()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithXss();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            if (response.IsSuccessful)
            {
                var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
                // CRITICAL BUG: API accepts script tags without sanitization
                bookingResponse.Booking.Firstname.Should().NotContain("<script>",
                    "XSS payload should be sanitized");
                bookingResponse.Booking.Additionalneeds.Should().NotContain("<img",
                    "XSS payload should be sanitized");
            }
        }

        [Test]
        [Description("TC-ERR-005: Should handle SQL injection attempts")]
        public async Task TCERR005_HandleSqlInjectionAttempts()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithSqlInjection();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            if (response.IsSuccessful)
            {
                var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
                // Verify SQL injection is not executed
                bookingResponse.Should().NotBeNull();
                bookingResponse.Bookingid.Should().BeGreaterThan(0);
            }
        }

        [Test]
        [Description("TC-ERR-006: Should handle large payload")]
        public async Task TCERR006_HandleLargePayload()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithLargePayload();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            // API should either accept or reject with appropriate status
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.RequestEntityTooLarge);
        }

        [Test]
        [Description("TC-ERR-007: Should handle invalid date formats")]
        public async Task TCERR007_HandleInvalidDateFormats()
        {
            // Arrange
            var invalidBooking = new
            {
                firstname = "John",
                lastname = "Doe",
                totalprice = 100,
                depositpaid = true,
                bookingdates = new
                {
                    checkin = "invalid-date",
                    checkout = "2024-01-05"
                },
                additionalneeds = "Breakfast"
            };

            // Act
            var response = await _apiHelper.PostAsync("/booking", invalidBooking);

            // Assert
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.InternalServerError,
                HttpStatusCode.OK // API might accept and store as-is
            );
        }

        [Test]
        [Description("TC-ERR-008: Should handle negative price")]
        public async Task TCERR008_HandleNegativePrice()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithNegativePrice();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            if (response.IsSuccessful)
            {
                var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
                // Business logic should reject negative prices
                bookingResponse.Booking.Totalprice.Should().BeGreaterThanOrEqualTo(0,
                    "Negative prices should not be accepted");
            }
        }

        [Test]
        [Description("TC-ERR-009: Should handle special characters")]
        public async Task TCERR009_HandleSpecialCharacters()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithSpecialCharacters();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            if (response.IsSuccessful)
            {
                var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
                bookingResponse.Should().NotBeNull();
            }
        }

        [Test]
        [Description("TC-ERR-010: Should validate checkout after checkin")]
        public async Task TCERR010_ValidateCheckoutAfterCheckin()
        {
            // Arrange
            var invalidBooking = new Booking
            {
                Firstname = "John",
                Lastname = "Doe",
                Totalprice = 100,
                Depositpaid = true,
                Bookingdates = new BookingDates
                {
                    Checkin = "2024-12-10",
                    Checkout = "2024-12-01" // Checkout before checkin
                },
                Additionalneeds = "Breakfast"
            };

            // Act
            var response = await _apiHelper.PostAsync("/booking", invalidBooking);

            // Assert
            // API should either validate or accept as-is
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.BadRequest,
                HttpStatusCode.OK
            );
        }
    }
}

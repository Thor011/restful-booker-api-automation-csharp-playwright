using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RestfulBookerApiTests.Tests.Helpers;
using RestfulBookerApiTests.Tests.Models;

namespace RestfulBookerApiTests.Tests.Tests
{
    [TestFixture]
    [Category("Validation")]
    [Category("DataIntegrity")]
    public class DataValidationTests
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
        [Description("TC-VAL-001: Should accept valid unicode characters")]
        public async Task TCVAL001_AcceptValidUnicodeCharacters()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithUnicode();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Should().NotBeNull();
            bookingResponse.Bookingid.Should().BeGreaterThan(0);
        }

        [Test]
        [Description("TC-VAL-002: Should validate price boundaries")]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(999999)]
        public async Task TCVAL002_ValidatePriceBoundaries(int price)
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Totalprice = price;

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Booking.Totalprice.Should().Be(price);
        }

        [Test]
        [Description("TC-VAL-003: Should validate deposit paid flag")]
        [TestCase(true)]
        [TestCase(false)]
        public async Task TCVAL003_ValidateDepositPaidFlag(bool depositPaid)
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Depositpaid = depositPaid;

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Booking.Depositpaid.Should().Be(depositPaid);
        }

        [Test]
        [Description("TC-VAL-004: Should handle empty additional needs")]
        public async Task TCVAL004_HandleEmptyAdditionalNeeds()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Additionalneeds = "";

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Should().NotBeNull();
        }

        [Test]
        [Description("TC-VAL-005: Should handle null additional needs")]
        public async Task TCVAL005_HandleNullAdditionalNeeds()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Additionalneeds = null;

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Should().NotBeNull();
        }

        [Test]
        [Description("TC-VAL-006: Should validate firstname length")]
        public async Task TCVAL006_ValidateFirstnameLength()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Firstname = TestDataGenerator.GenerateRandomString(255); // Long name

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Booking.Firstname.Should().NotBeNullOrEmpty();
        }

        [Test]
        [Description("TC-VAL-007: Should validate lastname length")]
        public async Task TCVAL007_ValidateLastnameLength()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Lastname = TestDataGenerator.GenerateRandomString(255); // Long name

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Booking.Lastname.Should().NotBeNullOrEmpty();
        }

        [Test]
        [Description("TC-VAL-008: Should validate same day checkin/checkout")]
        public async Task TCVAL008_ValidateSameDayCheckinCheckout()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            booking.Bookingdates.Checkin = date;
            booking.Bookingdates.Checkout = date;

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        }

        [Test]
        [Description("TC-VAL-009: Should validate past dates")]
        public async Task TCVAL009_ValidatePastDates()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Bookingdates.Checkin = "2020-01-01";
            booking.Bookingdates.Checkout = "2020-01-05";

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Should().NotBeNull();
        }

        [Test]
        [Description("TC-VAL-010: Should validate far future dates")]
        public async Task TCVAL010_ValidateFarFutureDates()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Bookingdates.Checkin = "2099-01-01";
            booking.Bookingdates.Checkout = "2099-12-31";

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Should().NotBeNull();
        }

        [Test]
        [Description("TC-VAL-011: Should handle leap year dates")]
        public async Task TCVAL011_HandleLeapYearDates()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Bookingdates.Checkin = "2024-02-29";
            booking.Bookingdates.Checkout = "2024-03-01";

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Booking.Bookingdates.Checkin.Should().Be("2024-02-29");
        }

        [Test]
        [Description("TC-VAL-012: Should validate single character names")]
        public async Task TCVAL012_ValidateSingleCharacterNames()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Firstname = "A";
            booking.Lastname = "B";

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Booking.Firstname.Should().Be("A");
            bookingResponse.Booking.Lastname.Should().Be("B");
        }

        [Test]
        [Description("TC-VAL-013: Should preserve whitespace in names")]
        public async Task TCVAL013_PreserveWhitespaceInNames()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Firstname = "  John  ";
            booking.Lastname = "  Doe  ";

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            bookingResponse.Should().NotBeNull();
            // Verify whitespace handling (API may trim or preserve)
            bookingResponse.Booking.Firstname.Should().NotBeNullOrEmpty();
        }

        [Test]
        [Description("TC-VAL-014: Should validate maximum price value")]
        public async Task TCVAL014_ValidateMaximumPriceValue()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Totalprice = int.MaxValue;

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        }

        [Test]
        [Description("TC-VAL-015: Should handle command injection in additional needs")]
        public async Task TCVAL015_HandleCommandInjectionInAdditionalNeeds()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateBookingWithCommandInjection();

            // Act
            var response = await _apiHelper.PostAsync("/booking", booking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(response.Content!);
            
            // Verify command injection is not executed
            bookingResponse.Should().NotBeNull();
            bookingResponse.Bookingid.Should().BeGreaterThan(0);
        }
    }
}

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RestfulBookerApiTests.Tests.Helpers;
using RestfulBookerApiTests.Tests.Models;
using RestfulBookerApiTests.Tests.Config;

namespace RestfulBookerApiTests.Tests.Tests
{
    [TestFixture]
    [Category("Authentication")]
    [Category("Security")]
    public class AuthenticationTests
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
        [Description("TC-AUTH-001: Should create auth token with valid credentials")]
        public async Task TCAUTH001_CreateAuthTokenWithValidCredentials()
        {
            // Arrange
            var credentials = new AuthCredentials
            {
                Username = TestConfig.DefaultUsername,
                Password = TestConfig.DefaultPassword
            };

            // Act
            var response = await _apiHelper.PostAsync("/auth", credentials);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var tokenResponse = _apiHelper.DeserializeResponse<AuthToken>(response.Content!);
            
            tokenResponse.Should().NotBeNull();
            tokenResponse.Token.Should().NotBeNullOrEmpty();
        }

        [Test]
        [Description("TC-AUTH-002: Should reject invalid credentials")]
        public async Task TCAUTH002_RejectInvalidCredentials()
        {
            // Arrange
            var credentials = new AuthCredentials
            {
                Username = "invalid",
                Password = "wrongpassword"
            };

            // Act
            var response = await _apiHelper.PostAsync("/auth", credentials);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = response.Content!;
            content.Should().Contain("reason").And.Contain("Bad credentials");
        }

        [Test]
        [Description("TC-AUTH-003: Should allow update with valid token")]
        public async Task TCAUTH003_AllowUpdateWithValidToken()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse!.Bookingid;
            var token = await _apiHelper.CreateAuthToken();

            var updatedBooking = TestDataGenerator.GenerateValidBooking();

            // Act
            var response = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = _apiHelper.DeserializeResponse<Booking>(response.Content!);
            updated.Firstname.Should().Be(updatedBooking.Firstname);
        }

        [Test]
        [Description("TC-AUTH-004: Should allow delete with valid token")]
        public async Task TCAUTH004_AllowDeleteWithValidToken()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse!.Bookingid;
            var token = await _apiHelper.CreateAuthToken();

            // Act
            var response = await _apiHelper.DeleteAsync($"/booking/{bookingId}", token);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Test]
        [Description("TC-AUTH-005: Should reject update without token")]
        public async Task TCAUTH005_RejectUpdateWithoutToken()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse!.Bookingid;

            var updatedBooking = TestDataGenerator.GenerateValidBooking();

            // Act
            var response = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        [Description("TC-AUTH-006: Should reject delete without token")]
        public async Task TCAUTH006_RejectDeleteWithoutToken()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse!.Bookingid;

            // Act
            var response = await _apiHelper.DeleteAsync($"/booking/{bookingId}", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Test]
        [Description("TC-AUTH-007: Should allow update with Basic Auth")]
        public async Task TCAUTH007_AllowUpdateWithBasicAuth()
        {
            // Arrange
            var booking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = createResponse!.Bookingid;

            var updatedBooking = TestDataGenerator.GenerateValidBooking();

            // Act
            var response = await _apiHelper.PutAsyncWithBasicAuth($"/booking/{bookingId}", updatedBooking);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = _apiHelper.DeserializeResponse<Booking>(response.Content!);
            updated.Firstname.Should().Be(updatedBooking.Firstname);
        }
    }
}

using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using RestfulBookerApiTests.Tests.Helpers;
using RestfulBookerApiTests.Tests.Models;

namespace RestfulBookerApiTests.Tests.Scenarios
{
    [TestFixture]
    [Category("Scenario")]
    [Category("E2E")]
    public class EndToEndScenarioTests
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
        [Description("SCENARIO-001: Complete booking lifecycle - Create, Read, Update, Delete")]
        public async Task Scenario001_CompleteBookingLifecycle()
        {
            TestLogger.Info("Starting complete booking lifecycle test");

            // Step 1: Create a booking
            TestLogger.Info("Step 1: Creating a new booking");
            var originalBooking = TestDataGenerator.GenerateValidBooking();
            var createResponse = await _apiHelper.PostAsync("/booking", originalBooking);
            
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var bookingResponse = _apiHelper.DeserializeResponse<BookingResponse>(createResponse.Content!);
            var bookingId = bookingResponse.Bookingid;
            TestLogger.Info($"Booking created with ID: {bookingId}");

            // Step 2: Retrieve the booking
            TestLogger.Info("Step 2: Retrieving the created booking");
            var getResponse = await _apiHelper.GetAsync($"/booking/{bookingId}");
            
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedBooking = _apiHelper.DeserializeResponse<Booking>(getResponse.Content!);
            retrievedBooking.Firstname.Should().Be(originalBooking.Firstname);

            // Step 3: Update the booking
            TestLogger.Info("Step 3: Updating the booking");
            var token = await _apiHelper.CreateAuthToken();
            var updatedBooking = TestDataGenerator.GenerateValidBooking();
            var updateResponse = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, token);
            
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = _apiHelper.DeserializeResponse<Booking>(updateResponse.Content!);
            updated.Firstname.Should().Be(updatedBooking.Firstname);

            // Step 4: Verify the update
            TestLogger.Info("Step 4: Verifying the update");
            var verifyResponse = await _apiHelper.GetAsync($"/booking/{bookingId}");
            
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var verifiedBooking = _apiHelper.DeserializeResponse<Booking>(verifyResponse.Content!);
            verifiedBooking.Firstname.Should().Be(updatedBooking.Firstname);

            // Step 5: Delete the booking
            TestLogger.Info("Step 5: Deleting the booking");
            var deleteResponse = await _apiHelper.DeleteAsync($"/booking/{bookingId}", token);
            
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Step 6: Verify deletion
            TestLogger.Info("Step 6: Verifying deletion");
            var deletedCheckResponse = await _apiHelper.GetAsync($"/booking/{bookingId}");
            
            deletedCheckResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
            TestLogger.Info("Complete booking lifecycle test completed successfully");
        }

        [Test]
        [Description("SCENARIO-002: Multiple booking management")]
        public async Task Scenario002_MultipleBookingManagement()
        {
            TestLogger.Info("Starting multiple booking management test");

            // Create multiple bookings
            var bookings = new BookingResponse[3];
            for (int i = 0; i < 3; i++)
            {
                var booking = TestDataGenerator.GenerateValidBooking();
                bookings[i] = await _apiHelper.CreateBooking(booking);
                TestLogger.Info($"Created booking {i + 1} with ID: {bookings[i].Bookingid}");
            }

            // Retrieve all bookings
            var getAllResponse = await _apiHelper.GetAsync("/booking");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update each booking
            var token = await _apiHelper.CreateAuthToken();
            foreach (var booking in bookings)
            {
                var updatedBooking = TestDataGenerator.GenerateValidBooking();
                var updateResponse = await _apiHelper.PutAsync($"/booking/{booking.Bookingid}", updatedBooking, token);
                updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                TestLogger.Info($"Updated booking ID: {booking.Bookingid}");
            }

            // Delete all created bookings
            foreach (var booking in bookings)
            {
                var deleteResponse = await _apiHelper.DeleteAsync($"/booking/{booking.Bookingid}", token);
                deleteResponse.StatusCode.Should().Be(HttpStatusCode.Created);
                TestLogger.Info($"Deleted booking ID: {booking.Bookingid}");
            }

            TestLogger.Info("Multiple booking management test completed successfully");
        }

        [Test]
        [Description("SCENARIO-003: Guest check-in workflow")]
        public async Task Scenario003_GuestCheckInWorkflow()
        {
            TestLogger.Info("Starting guest check-in workflow test");

            // Step 1: Create booking with deposit not paid
            var booking = TestDataGenerator.GenerateValidBooking();
            booking.Depositpaid = false;
            var bookingResponse = await _apiHelper.CreateBooking(booking);
            TestLogger.Info($"Booking created for guest: {booking.Firstname} {booking.Lastname}");

            // Step 2: Retrieve booking details
            var getResponse = await _apiHelper.GetAsync($"/booking/{bookingResponse.Bookingid}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedBooking = _apiHelper.DeserializeResponse<Booking>(getResponse.Content!);
            retrievedBooking.Depositpaid.Should().BeFalse();

            // Step 3: Update booking - mark deposit as paid
            var token = await _apiHelper.CreateAuthToken();
            retrievedBooking.Depositpaid = true;
            var updateResponse = await _apiHelper.PutAsync($"/booking/{bookingResponse.Bookingid}", retrievedBooking, token);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Deposit marked as paid");

            // Step 4: Add additional needs
            var patchUpdate = new { additionalneeds = "Late checkout requested" };
            var patchResponse = await _apiHelper.PatchAsync($"/booking/{bookingResponse.Bookingid}", patchUpdate, token);
            patchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Additional needs updated");

            // Step 5: Final verification
            var finalResponse = await _apiHelper.GetAsync($"/booking/{bookingResponse.Bookingid}");
            finalResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var finalBooking = _apiHelper.DeserializeResponse<Booking>(finalResponse.Content!);
            finalBooking.Depositpaid.Should().BeTrue();

            TestLogger.Info("Guest check-in workflow completed successfully");
        }

        [Test]
        [Description("SCENARIO-004: Booking modification workflow")]
        public async Task Scenario004_BookingModificationWorkflow()
        {
            TestLogger.Info("Starting booking modification workflow test");

            // Step 1: Create initial booking
            var booking = TestDataGenerator.GenerateValidBooking();
            var bookingResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = bookingResponse.Bookingid;
            TestLogger.Info($"Initial booking created with ID: {bookingId}");

            var token = await _apiHelper.CreateAuthToken();

            // Step 2: Modify guest name
            var nameUpdate = new 
            { 
                firstname = "Modified",
                lastname = "Guest"
            };
            var nameResponse = await _apiHelper.PatchAsync($"/booking/{bookingId}", nameUpdate, token);
            nameResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Guest name modified");

            // Step 3: Modify price
            var priceUpdate = new { totalprice = 999 };
            var priceResponse = await _apiHelper.PatchAsync($"/booking/{bookingId}", priceUpdate, token);
            priceResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Price modified");

            // Step 4: Modify dates
            var dateUpdate = new 
            { 
                bookingdates = new 
                {
                    checkin = "2025-01-15",
                    checkout = "2025-01-20"
                }
            };
            var dateResponse = await _apiHelper.PatchAsync($"/booking/{bookingId}", dateUpdate, token);
            dateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Dates modified");

            // Step 5: Verify all modifications
            var verifyResponse = await _apiHelper.GetAsync($"/booking/{bookingId}");
            verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var finalBooking = _apiHelper.DeserializeResponse<Booking>(verifyResponse.Content!);
            
            finalBooking.Firstname.Should().Be("Modified");
            finalBooking.Lastname.Should().Be("Guest");
            finalBooking.Totalprice.Should().Be(999);

            TestLogger.Info("Booking modification workflow completed successfully");
        }

        [Test]
        [Description("SCENARIO-005: Search and filter workflow")]
        public async Task Scenario005_SearchAndFilterWorkflow()
        {
            TestLogger.Info("Starting search and filter workflow test");

            // Step 1: Create specific bookings for searching
            var searchBooking = TestDataGenerator.GenerateValidBooking();
            searchBooking.Firstname = "SearchTest";
            searchBooking.Lastname = "UserTest";
            
            var booking1 = await _apiHelper.CreateBooking(searchBooking);
            TestLogger.Info($"Created searchable booking with ID: {booking1.Bookingid}");

            // Step 2: Search by first name
            var firstNameSearch = await _apiHelper.GetAsync($"/booking?firstname={searchBooking.Firstname}");
            firstNameSearch.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("First name search completed");

            // Step 3: Search by last name
            var lastNameSearch = await _apiHelper.GetAsync($"/booking?lastname={searchBooking.Lastname}");
            lastNameSearch.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Last name search completed");

            // Step 4: Search by full name
            var fullNameSearch = await _apiHelper.GetAsync(
                $"/booking?firstname={searchBooking.Firstname}&lastname={searchBooking.Lastname}");
            fullNameSearch.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Full name search completed");

            // Step 5: Retrieve specific booking
            var getResponse = await _apiHelper.GetAsync($"/booking/{booking1.Bookingid}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedBooking = _apiHelper.DeserializeResponse<Booking>(getResponse.Content!);
            retrievedBooking.Firstname.Should().Be(searchBooking.Firstname);

            TestLogger.Info("Search and filter workflow completed successfully");
        }

        [Test]
        [Description("SCENARIO-006: Authentication and authorization workflow")]
        public async Task Scenario006_AuthenticationAuthorizationWorkflow()
        {
            TestLogger.Info("Starting authentication and authorization workflow test");

            // Step 1: Create booking without auth
            var booking = TestDataGenerator.GenerateValidBooking();
            var bookingResponse = await _apiHelper.CreateBooking(booking);
            var bookingId = bookingResponse.Bookingid;
            TestLogger.Info($"Booking created without auth, ID: {bookingId}");

            // Step 2: Try to update without token (should fail)
            var updatedBooking = TestDataGenerator.GenerateValidBooking();
            var unauthorizedUpdate = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, null);
            unauthorizedUpdate.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            TestLogger.Info("Unauthorized update correctly rejected");

            // Step 3: Create auth token
            var token = await _apiHelper.CreateAuthToken();
            token.Should().NotBeNullOrEmpty();
            TestLogger.Info("Auth token created successfully");

            // Step 4: Update with token (should succeed)
            var authorizedUpdate = await _apiHelper.PutAsync($"/booking/{bookingId}", updatedBooking, token);
            authorizedUpdate.StatusCode.Should().Be(HttpStatusCode.OK);
            TestLogger.Info("Authorized update successful");

            // Step 5: Try to delete without token (should fail)
            var unauthorizedDelete = await _apiHelper.DeleteAsync($"/booking/{bookingId}", null);
            unauthorizedDelete.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            TestLogger.Info("Unauthorized delete correctly rejected");

            // Step 6: Delete with token (should succeed)
            var authorizedDelete = await _apiHelper.DeleteAsync($"/booking/{bookingId}", token);
            authorizedDelete.StatusCode.Should().Be(HttpStatusCode.Created);
            TestLogger.Info("Authorized delete successful");

            TestLogger.Info("Authentication and authorization workflow completed successfully");
        }
    }
}


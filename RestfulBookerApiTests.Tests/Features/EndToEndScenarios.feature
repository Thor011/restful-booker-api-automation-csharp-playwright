Feature: End-to-End Booking Scenarios
  As a hotel booking system user
  I want to perform complete booking workflows
  So that I can manage the entire booking lifecycle

  Background:
    Given the RestfulBooker API is available
    And the system is ready to accept bookings
    # API Endpoints (End-to-End Workflows):
    # POST   /auth - Authenticate
    # POST   /booking - Create booking
    # GET    /booking/{id} - Retrieve booking
    # GET    /booking - List all bookings
    # PUT    /booking/{id} - Update booking
    # PATCH  /booking/{id} - Partial update
    # DELETE /booking/{id} - Delete booking
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Complete booking lifecycle from creation to deletion
    # Endpoints: POST /booking, GET /booking/{id}, POST /auth, PUT /booking/{id}, DELETE /booking/{id}
    Given I am a guest wanting to make a reservation
    When I create a new booking for "John Doe" from "2025-01-15" to "2025-01-20"
    Then the booking should be created with a unique ID
    
    When I retrieve the booking details
    Then I should see all my booking information is correct
    
    When I authenticate as an administrator
    And I update the booking to change the guest name to "Jane Smith"
    Then the booking should be updated successfully
    
    When I verify the updated booking
    Then the guest name should now be "Jane Smith"
    And other details should remain unchanged
    
    When I delete the booking as an administrator
    Then the booking should be deleted successfully
    
    When I try to retrieve the deleted booking
    Then I should receive a "not found" response

  Scenario: Manage multiple bookings simultaneously
    # Endpoints: POST /booking, GET /booking, POST /auth, PUT /booking/{id}, DELETE /booking/{id}
    Given I want to create multiple reservations
    When I create 3 different bookings for different guests
    Then all 3 bookings should be created successfully
    And each should have a unique booking ID
    
    When I retrieve the list of all bookings
    Then I should see all my created bookings in the system
    
    When I authenticate as an administrator
    And I update each booking with new information
    Then all updates should be successful
    
    When I delete all 3 bookings
    Then all bookings should be deleted successfully

  Scenario: Guest check-in workflow
    # Endpoints: POST /booking, GET /booking/{id}, POST /auth, PATCH /booking/{id}
    Given a guest "Alice Johnson" has made a reservation
    And the deposit has not been paid yet
    When I create the booking with deposit paid as "false"
    Then the booking should be created successfully
    
    When I retrieve the booking to verify the details
    Then the deposit paid flag should be "false"
    
    When the guest arrives and pays the deposit
    And I authenticate as staff
    And I update the deposit paid flag to "true"
    Then the booking should be updated successfully
    
    When the guest requests late checkout
    And I add "Late checkout requested" to additional needs
    Then the additional needs should be updated
    
    When I verify the final booking state
    Then deposit should be marked as paid
    And additional needs should include "Late checkout requested"

  Scenario: Modify existing booking dates and guest information
    # Endpoints: POST /booking, POST /auth, PATCH /booking/{id}, GET /booking/{id}
    Given I have created a booking for "Bob Smith"
    And the booking is from "2025-02-01" to "2025-02-05"
    When I authenticate as an administrator
    
    And I change the guest name to "Modified" and "Guest"
    Then the name change should be successful
    
    When I update the price to "999"
    Then the price should be updated to 999
    
    When I change the dates to "2025-01-15" to "2025-01-20"
    Then the dates should be updated successfully
    
    When I verify all modifications
    Then the first name should be "Modified"
    And the last name should be "Guest"
    And the total price should be 999
    And the dates should be updated correctly

  Scenario: Search for bookings and retrieve specific one
    # Endpoints: POST /booking, GET /booking?firstname={value}, GET /booking?lastname={value}, GET /booking/{id}
    Given I want to find a specific booking
    When I create a booking for guest "SearchTest UserTest"
    Then I should receive a booking ID
    
    When I search for bookings with firstname "SearchTest"
    Then the search should return results
    And my booking should be in the results
    
    When I search by lastname "UserTest"
    Then the search should return results
    And my booking should be in the results
    
    When I search using both firstname "SearchTest" and lastname "UserTest"
    Then I should find the exact booking
    
    When I retrieve the specific booking using its ID
    Then the guest name should be "SearchTest UserTest"

  Scenario: Authentication and authorization workflow
    # Endpoints: POST /booking, PUT /booking/{id}, POST /auth, DELETE /booking/{id}
    Given I want to modify a booking
    When I create a booking without authentication
    Then the booking creation should succeed
    And I should receive a booking ID
    
    When I try to update the booking without authentication
    Then the update should be rejected
    And I should receive a forbidden error
    
    When I authenticate with valid credentials
    Then I should receive a valid authentication token
    
    When I update the booking using the authentication token
    Then the update should be successful
    And the booking should reflect the changes
    
    When I try to delete the booking without authentication
    Then the deletion should be rejected
    And I should receive a forbidden error
    
    When I delete the booking using the authentication token
    Then the deletion should be successful
    And the booking should no longer exist

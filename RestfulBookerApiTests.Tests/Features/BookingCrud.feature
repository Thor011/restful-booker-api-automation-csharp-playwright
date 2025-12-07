Feature: Booking CRUD Operations
  As a hotel booking system user
  I want to create, read, update, and delete bookings
  So that I can manage hotel reservations effectively

  Background:
    Given the RestfulBooker API is available
    And I have valid booking details
    # API Endpoints:
    # GET    /ping - Health check
    # POST   /booking - Create new booking
    # GET    /booking/{id} - Get booking by ID
    # GET    /booking - Get all booking IDs
    # GET    /booking?firstname={value}&lastname={value} - Filter bookings
    # PUT    /booking/{id} - Update booking (requires auth)
    # PATCH  /booking/{id} - Partial update (requires auth)
    # DELETE /booking/{id} - Delete booking (requires auth)
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Verify API health check
    # Endpoint: GET /ping
    When I send a health check request to the API
    Then the API should respond with a healthy status
    And the response time should be acceptable

  Scenario: Create a new booking successfully
    # Endpoint: POST /booking
    Given I have valid guest information
    And I have valid check-in and check-out dates
    When I create a new booking
    Then the booking should be created successfully
    And I should receive a unique booking ID
    And the booking details should match my request

  Scenario: Retrieve booking by ID
    # Endpoint: GET /booking/{id}
    Given I have created a booking
    When I request the booking details using the booking ID
    Then I should receive the complete booking information
    And all booking fields should match the created booking

  Scenario: Retrieve all booking IDs
    # Endpoint: GET /booking
    Given there are existing bookings in the system
    When I request all booking IDs
    Then I should receive a list of booking IDs
    And the list should not be empty

  Scenario: Filter bookings by guest name
    # Endpoint: GET /booking?firstname={value}&lastname={value}
    Given I have created a booking for "John Doe"
    When I search for bookings with firstname "John" and lastname "Doe"
    Then I should find the booking in the search results
    And the booking ID should match the created booking

  Scenario: Update booking completely
    # Endpoint: PUT /booking/{id}
    Given I have created a booking
    And I have authentication credentials
    When I update all booking details
    Then the booking should be updated successfully
    And all fields should reflect the new values

  Scenario: Partially update booking details
    # Endpoint: PATCH /booking/{id}
    Given I have created a booking
    And I have authentication credentials
    When I update only the guest's first name and last name
    Then only the specified fields should be updated
    And other fields should remain unchanged

  Scenario: Delete a booking
    # Endpoint: DELETE /booking/{id}
    Given I have created a booking
    And I have authentication credentials
    When I delete the booking
    Then the booking should be deleted successfully
    And the booking should no longer exist in the system

  Scenario: Handle non-existent booking
    # Endpoint: GET /booking/{id}
    Given a booking ID that does not exist
    When I try to retrieve the booking
    Then I should receive a "not found" response
    And no booking data should be returned

  Scenario: Reject update without authentication
    # Endpoint: PUT /booking/{id}
    Given I have created a booking
    And I do not have authentication credentials
    When I try to update the booking
    Then the update should be rejected
    And I should receive an authorization error

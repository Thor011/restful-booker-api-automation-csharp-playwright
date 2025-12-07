Feature: Performance and Load Testing
  As a hotel booking system operator
  I want the system to perform well under load
  So that users have a responsive experience

  Background:
    Given the RestfulBooker API is available
    # API Endpoints (Performance Testing):
    # POST   /booking - Create booking (timed)
    # GET    /booking/{id} - Get booking (timed)
    # PUT    /booking/{id} - Update booking (timed)
    # DELETE /booking/{id} - Delete booking (timed)
    # GET    /booking - List bookings (concurrent tests)
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Create booking within acceptable time
    Given I have valid booking details
    When I create a new booking
    Then the response should be received within 3 seconds
    And the booking should be created successfully

  Scenario: Retrieve booking within acceptable time
    Given I have an existing booking
    When I retrieve the booking details
    Then the response should be received within 2 seconds
    And the correct booking data should be returned

  Scenario: Update booking within acceptable time
    Given I have an existing booking
    And I have authentication credentials
    When I update the booking
    Then the response should be received within 3 seconds
    And the update should be successful

  Scenario: Delete booking within acceptable time
    Given I have an existing booking
    And I have authentication credentials
    When I delete the booking
    Then the response should be received within 2 seconds
    And the deletion should be successful

  Scenario: Handle burst of GET requests
    When I send 10 concurrent GET requests to retrieve bookings
    Then all requests should complete within 10 seconds
    And all responses should be successful

  Scenario: Handle burst of POST requests
    When I send 5 concurrent POST requests to create bookings
    Then all requests should complete within 15 seconds
    And all bookings should be created successfully

  Scenario: Measure average response time
    When I send 10 consecutive GET requests
    Then the average response time should be under 3 seconds
    And I should track minimum and maximum response times

  Scenario: Complete CRUD cycle efficiently
    Given I have valid booking details
    When I perform a complete CRUD cycle (Create, Read, Update, Delete)
    Then the entire cycle should complete within 10 seconds
    And each operation should be successful

  Scenario: Maintain performance with large result set
    When I request all bookings from the system
    Then the response should be received within 5 seconds
    Even if there are many bookings in the system

  Scenario: Authenticate quickly
    Given I have valid credentials
    When I request an authentication token
    Then the token should be issued within 2 seconds
    And the token should be valid

  Scenario: Search and filter efficiently
    Given I have existing bookings in the system
    When I perform a filtered search by name
    Then the results should be returned within 3 seconds
    And the results should be accurate

  Scenario: Partial update efficiently
    Given I have an existing booking
    And I have authentication credentials
    When I perform a partial update (PATCH)
    Then the response should be received within 3 seconds
    And only the specified fields should be updated

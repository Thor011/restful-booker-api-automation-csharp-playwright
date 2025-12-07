Feature: Error Handling and Security
  As a hotel booking system
  I want to handle errors gracefully and prevent security vulnerabilities
  So that the system remains stable and secure

  Background:
    Given the RestfulBooker API is available
    # API Endpoints:
    # POST   /booking - Create booking (error handling)
    # GET    /booking/{id} - Get booking (404 handling)
    # PUT    /booking/{id} - Update booking (validation errors)
    # DELETE /booking/{id} - Delete booking (auth errors)
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Handle missing required fields
    Given I have booking data with missing required fields
    When I try to create the booking
    Then the system should reject the request
    And I should receive an appropriate error message

  Scenario: Handle invalid data types
    Given I have booking data with invalid data types
    And the price field contains text instead of a number
    When I try to create the booking
    Then the system should reject the request
    And an error should be returned

  Scenario: Handle invalid booking ID format
    When I try to retrieve a booking with ID "invalid_id"
    Then I should receive a "not found" response
    And no booking data should be returned

  Scenario: Prevent XSS attacks
    Given I have booking data with XSS payloads in name fields
    When I create the booking
    Then the booking should be created
    But the XSS payload should be sanitized or encoded
    And scripts should not be executed

  Scenario: Prevent SQL injection attacks
    Given I have booking data with SQL injection attempts
    When I create the booking
    Then the booking should be created safely
    And the SQL injection should not be executed
    And data integrity should be maintained

  Scenario: Handle large payload requests
    Given I have booking data with very large additional needs field
    When I try to create the booking
    Then the system should either accept or reject based on size limits
    And the response should be appropriate

  Scenario: Handle invalid date formats
    Given I have booking data with invalid date format "invalid-date"
    When I try to create the booking
    Then the system should handle the invalid date appropriately
    And return a meaningful error or accept based on validation

  Scenario: Reject negative prices
    Given I have a booking with negative price "-999999"
    When I create the booking
    Then the system should validate the price
    And negative prices should ideally be rejected

  Scenario: Handle special characters in all fields
    Given I have booking data with special characters in all text fields
    When I create the booking
    Then the booking should be created successfully
    And special characters should be stored correctly

  Scenario: Validate checkout date is after check-in date
    Given I have a booking where check-out date is before check-in date
    When I try to create the booking
    Then the system should validate the date logic
    And handle the invalid date range appropriately

  Scenario: Prevent command injection
    Given I have booking data with command injection attempts
    When I create the booking
    Then the commands should not be executed
    And the system should remain secure
    And the data should be stored safely

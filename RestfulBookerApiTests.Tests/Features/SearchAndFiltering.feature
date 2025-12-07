Feature: Search and Filtering
  As a hotel booking system user
  I want to search and filter bookings
  So that I can quickly find specific reservations

  Background:
    Given the RestfulBooker API is available
    # API Endpoints:
    # GET /booking - Get all booking IDs
    # GET /booking?firstname={value} - Filter by first name
    # GET /booking?lastname={value} - Filter by last name
    # GET /booking?checkin={YYYY-MM-DD} - Filter by check-in date
    # GET /booking?checkout={YYYY-MM-DD} - Filter by check-out date
    # GET /booking?firstname={value}&lastname={value}&checkin={date} - Multiple filters
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Filter bookings by first name only
    Given I have created a booking for guest "John Smith"
    When I search for bookings with firstname "John"
    Then I should find the booking in the results
    And the results should include the created booking ID

  Scenario: Filter bookings by last name only
    Given I have created a booking for guest "John Smith"
    When I search for bookings with lastname "Smith"
    Then I should find the booking in the results
    And the results should include the created booking ID

  Scenario: Filter bookings by check-in date
    Given I have created a booking with check-in date "2025-01-15"
    When I search for bookings with check-in date "2025-01-15"
    Then I should receive matching bookings
    And the results should not be empty

  Scenario: Filter bookings by check-out date
    Given I have created a booking with check-out date "2025-01-20"
    When I search for bookings with check-out date "2025-01-20"
    Then I should receive matching bookings
    And the results should not be empty

  Scenario: Filter bookings using multiple parameters
    Given I have created a booking for "John Smith" with dates "2025-01-15" to "2025-01-20"
    When I search using firstname "John", lastname "Smith", and check-in "2025-01-15"
    Then I should find the exact booking
    And the results should include the created booking ID

  Scenario: Return empty results for non-matching filter
    When I search for bookings with firstname "NonExistentName123456789"
    Then I should receive an empty result set or no matching bookings

  Scenario: Handle case sensitivity in name filters
    Given I have created a booking for guest "TestUser LastName"
    When I search with different case variations
      | SearchCase  |
      | TESTUSER    |
      | testuser    |
      | TestUser    |
    Then the search should handle case sensitivity appropriately

  Scenario: Handle special characters in filter values
    Given I have created a booking for guest "John@Test"
    When I search for bookings with firstname "John@Test"
    Then the search should complete successfully
    And special characters should be handled properly

  Scenario: Handle URL encoding in filter parameters
    Given I have created a booking for guest "John Doe" (with space)
    When I search with URL-encoded parameters "John%20Doe"
    Then the search should work correctly
    And I should find the matching booking

  Scenario: Filter by date range
    Given I have created a booking from "2025-01-15" to "2025-01-20"
    When I search for bookings within this date range
    Then I should receive matching bookings
    And the created booking should be included

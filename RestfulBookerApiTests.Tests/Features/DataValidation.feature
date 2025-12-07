Feature: Data Validation
  As a hotel booking system
  I want to validate all booking data properly
  So that data integrity is maintained and invalid data is rejected

  Background:
    Given the RestfulBooker API is available
    # API Endpoints:
    # POST /booking - Create booking with validation
    # PUT  /booking/{id} - Update booking with validation
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Accept valid unicode characters in names
    Given I have booking details with unicode characters in names
    When I create the booking
    Then the booking should be created successfully
    And the unicode characters should be preserved

  Scenario Outline: Validate price boundaries
    Given I have a booking with price of <price>
    When I create the booking
    Then the booking should be created successfully
    And the price should be stored as <price>

    Examples:
      | price   |
      | 0       |
      | 1       |
      | 999999  |

  Scenario Outline: Validate deposit paid flag
    Given I have a booking with deposit paid set to <depositPaid>
    When I create the booking
    Then the booking should be created successfully
    And the deposit paid flag should be <depositPaid>

    Examples:
      | depositPaid |
      | true        |
      | false       |

  Scenario: Handle empty additional needs
    Given I have a booking with empty additional needs
    When I create the booking
    Then the booking should be created successfully
    And the additional needs should be accepted

  Scenario: Handle null additional needs
    Given I have a booking with null additional needs
    When I create the booking
    Then the booking should be created successfully
    And the system should handle the null value gracefully

  Scenario: Accept very long guest names
    Given I have booking details with very long first and last names
    When I create the booking
    Then the booking should be created successfully
    And the long names should be stored correctly

  Scenario: Accept single character names
    Given I have a booking with single character first and last names
    When I create the booking
    Then the booking should be created successfully
    And the names should be "A" and "B"

  Scenario: Handle same day check-in and check-out
    Given I have a booking with check-in and check-out on the same day
    When I create the booking
    Then the system should either accept or reject based on business rules

  Scenario: Accept bookings with past dates
    Given I have a booking with check-in date of "2020-01-01"
    And check-out date of "2020-01-05"
    When I create the booking
    Then the booking should be created successfully
    And the historical dates should be stored

  Scenario: Accept bookings with far future dates
    Given I have a booking with check-in date of "2099-01-01"
    And check-out date of "2099-12-31"
    When I create the booking
    Then the booking should be created successfully
    And the future dates should be stored

  Scenario: Handle leap year dates correctly
    Given I have a booking with check-in date of "2024-02-29"
    When I create the booking
    Then the booking should be created successfully
    And the leap year date should be stored as "2024-02-29"

  Scenario: Preserve whitespace in names
    Given I have a booking with names containing leading and trailing spaces
    When I create the booking
    Then the booking should be created successfully
    And the system should handle whitespace appropriately

  Scenario: Validate maximum price value
    Given I have a booking with the maximum possible price
    When I create the booking
    Then the system should either accept or reject based on limits

  Scenario: Prevent command injection in additional needs
    Given I have a booking with command injection attempts in additional needs
    When I create the booking
    Then the booking should be created
    And the commands should not be executed
    And the system should remain secure

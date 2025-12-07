Feature: Boundary and Edge Case Testing
  As a hotel booking system
  I want to handle boundary values and edge cases properly
  So that the system behaves correctly under extreme conditions

  Background:
    Given the RestfulBooker API is available
    # API Endpoints (Boundary Testing):
    # GET  /booking/{id} - Test with boundary ID values
    # POST /booking - Test with extreme data values
    # GET  /invalidendpoint - Test invalid endpoints
    # Base URL: https://restful-booker.herokuapp.com

  Scenario Outline: Handle booking ID boundaries
    When I try to retrieve a booking with ID "<bookingId>"
    Then the system should respond appropriately
    And handle the edge case correctly

    Examples:
      | bookingId     | Description              |
      | 1             | Minimum valid ID         |
      | 2147483647    | Maximum integer ID       |
      | -1            | Negative ID              |
      | 0             | Zero ID                  |

  Scenario: Handle very long guest names
    Given I have booking details with names of 1000 characters
    When I try to create the booking
    Then the system should either accept or enforce length limits
    And respond appropriately

  Scenario: Handle zero price
    Given I have a booking with price exactly 0
    When I create the booking
    Then the booking should be created successfully
    And the price should be stored as 0

  Scenario: Handle invalid API endpoint
    When I send a request to "/invalidendpoint"
    Then I should receive a "not found" response

  Scenario: Handle malformed JSON
    Given I have malformed JSON data with missing closing braces
    When I try to create a booking
    Then the system should reject the request
    And return a bad request error

  Scenario: Handle empty JSON object
    Given I send an empty JSON object "{}"
    When I try to create a booking
    Then the system should validate required fields
    And reject the request

  Scenario: Handle null values in request
    Given I have booking data with null values for required fields
    When I try to create the booking
    Then the system should reject the request
    And indicate which fields are required

  Scenario: Handle date at year boundary
    Given I have a booking from "2024-12-31" to "2025-01-01"
    When I create the booking
    Then the booking should be created successfully
    And the dates should cross the year boundary correctly

  Scenario Outline: Handle invalid dates
    Given I have a booking with check-in date "<invalidDate>"
    When I try to create the booking
    Then the system should handle the invalid date appropriately

    Examples:
      | invalidDate   | Reason                          |
      | 2024-13-01    | Invalid month (13)              |
      | 2024-02-30    | February doesn't have 30 days   |
      | invalid-date  | Completely invalid format       |

  Scenario: Handle URL with trailing slash
    When I send a request to "/booking/"
    Then the system should handle the trailing slash
    And respond appropriately

  Scenario: Handle multiple slashes in URL
    When I send a request to "//booking//1"
    Then the system should normalize the URL
    Or return an appropriate error

  Scenario: Handle extremely long additional needs
    Given I have booking with additional needs of 5000 characters
    When I try to create the booking
    Then the system should enforce size limits if applicable
    And handle the large payload appropriately

  Scenario Outline: Handle invalid booking ID formats
    When I try to retrieve a booking with ID "<invalidId>"
    Then I should receive a "not found" or error response

    Examples:
      | invalidId      |
      | abc123         |
      | @#$%           |
      | 123abc         |

  Scenario: Handle extremely large price
    Given I have a booking with price "999999999"
    When I create the booking
    Then the booking should be created successfully
    And the large price should be stored correctly

  Scenario Outline: Handle extreme year dates
    Given I have a booking with check-in date "<date>"
    When I create the booking
    Then the booking should be created successfully

    Examples:
      | date       | Description    |
      | 1900-01-01 | Very old date  |
      | 9999-12-31 | Far future date|

  Scenario: Handle empty string for required fields
    Given I have a booking with empty string "" for firstname
    When I try to create the booking
    Then the system should validate the field
    And handle the empty string appropriately

  Scenario: Handle whitespace-only names
    Given I have a booking with names containing only spaces "   "
    When I try to create the booking
    Then the system should validate or trim the whitespace
    And handle it according to business rules

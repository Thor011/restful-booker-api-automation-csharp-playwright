Feature: Idempotency and Consistency
  As a hotel booking system
  I want operations to be consistent and idempotent where appropriate
  So that duplicate requests don't cause unintended side effects

  Background:
    Given the RestfulBooker API is available
    # API Endpoints (Idempotency Testing):
    # GET    /booking/{id} - Idempotent read operations
    # PUT    /booking/{id} - Idempotent updates
    # DELETE /booking/{id} - Delete (not idempotent after first call)
    # POST   /booking - Non-idempotent creates
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Retrieve same data on multiple GET requests
    Given I have created a booking
    When I retrieve the booking 3 times
    Then all three responses should be identical
    And the booking data should not change between requests

  Scenario: Handle duplicate PUT requests idempotently
    Given I have created a booking
    And I have authentication credentials
    When I send the same update request 3 times
    Then all three updates should succeed
    And the final state should be the same as after the first update

  Scenario: Handle multiple DELETE requests gracefully
    Given I have created a booking
    And I have authentication credentials
    When I send a delete request
    Then the first delete should succeed
    And subsequent delete attempts should be handled gracefully
    And the booking should remain deleted

  Scenario: Maintain consistency with rapid successive updates
    Given I have created a booking
    And I have authentication credentials
    When I rapidly update the booking three times with different values
    Then the final state should reflect the last update
    And no updates should be lost

  Scenario: Handle retry after timeout scenario
    Given I have valid booking details
    When I create a booking twice with the same data
    Then both requests should succeed
    And two separate bookings should be created with different IDs
    Because POST is not idempotent

  Scenario: Retrieve deleted resource consistently
    Given I have created and then deleted a booking
    When I try to retrieve the booking multiple times
    Then all retrieval attempts should consistently return "not found"
    And the behavior should be predictable

  Scenario: Handle concurrent GET and UPDATE requests
    Given I have created a booking
    And I have authentication credentials
    When I send an update request and a GET request simultaneously
    Then both requests should complete successfully
    And the data should remain consistent
    And no race conditions should occur

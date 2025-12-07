Feature: Authentication and Authorization
  As a hotel booking system administrator
  I want to secure booking modifications with authentication
  So that only authorized users can update or delete bookings

  Background:
    Given the RestfulBooker API is available
    # API Endpoints:
    # POST   /auth - Create authentication token
    # PUT    /booking/{id} - Update booking (requires auth)
    # PATCH  /booking/{id} - Partial update (requires auth)
    # DELETE /booking/{id} - Delete booking (requires auth)
    # Base URL: https://restful-booker.herokuapp.com

  Scenario: Create authentication token with valid credentials
    # Endpoint: POST /auth
    Given I have valid username and password
    When I request an authentication token
    Then I should receive a valid token
    And the token should not be empty

  Scenario: Reject authentication with invalid credentials
    # Endpoint: POST /auth
    Given I have invalid username or password
    When I request an authentication token
    Then the authentication should fail
    And I should receive an error message indicating bad credentials

  Scenario: Allow booking update with valid token
    # Endpoint: PUT /booking/{id}
    Given I have created a booking
    And I have a valid authentication token
    When I update the booking using the token
    Then the update should be successful
    And the booking should reflect the new values

  Scenario: Allow booking deletion with valid token
    # Endpoint: DELETE /booking/{id}
    Given I have created a booking
    And I have a valid authentication token
    When I delete the booking using the token
    Then the deletion should be successful
    And the booking should no longer exist

  Scenario: Reject booking update without token
    # Endpoint: PUT /booking/{id}
    Given I have created a booking
    And I do not provide an authentication token
    When I try to update the booking
    Then the update should be rejected
    And I should receive a forbidden error

  Scenario: Reject booking deletion without token
    # Endpoint: DELETE /booking/{id}
    Given I have created a booking
    And I do not provide an authentication token
    When I try to delete the booking
    Then the deletion should be rejected
    And I should receive a forbidden error

  Scenario: Allow booking update with Basic Authentication
    # Endpoint: PUT /booking/{id}
    Given I have created a booking
    And I have valid Basic Authentication credentials
    When I update the booking using Basic Auth
    Then the update should be successful
    And the booking should reflect the new values
